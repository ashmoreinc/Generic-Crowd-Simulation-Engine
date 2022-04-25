using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BehaviourManager : MonoBehaviour {
    // Manages the behaviours for the agent it is attached too.
    private Dictionary<String, Behaviour> behaviours;
    public SensorManager sensing;
    
    private Move movement;

    public bool isFocussing = false;

    public bool usePreemptive, useImmediate, useObstacle = true;
    
    void Start() {
        // Get the reference objects
        sensing = gameObject.GetComponent<SensorManager>();
        movement = gameObject.GetComponent<Move>();

        // Set up the behaviours
        behaviours = new Dictionary<String, Behaviour>();

        var preemptAvoidance = AttributeManager.getBehaviourPreemptiveAvoidance();
        var immediateAvoidance = AttributeManager.getBehaviourImmediateAvoidance();
        var obstacleAvoidance = AttributeManager.getBehaviourObstacleAvoidance();
        
        behaviours.Add(preemptAvoidance.name, preemptAvoidance);
        behaviours.Add(immediateAvoidance.name, immediateAvoidance);
        behaviours.Add(obstacleAvoidance.name, obstacleAvoidance);
        
        // Create the sensors from the behaviours
        createSensors();
    }

    void createSensors() {
        // Loop through all the behaviours and create a sensor based on them
        foreach (var keyVal in behaviours) {
            var bhv = behaviours[keyVal.Key];
            sensing.createSensor(bhv.name, bhv.radius);
        }
    }

    // Update is called once per frame
    void Update() {
        // Reset the focus flag
        isFocussing = false;
        
        // Run the behaviours
        if(useImmediate) immediateAvoidance();
        if(useObstacle) obstacleAvoidance();
        if (usePreemptive) {
            preemptiveAvoidance();
            preemptiveSpeedchange();
        }
    }

    void preemptiveAvoidance() {
        // Stop if the script is already focused on a different behaviour
        if (isFocussing)
            return;
        
        // Get the sensor object
        var sensorObj = sensing.getSensorWithName("Preemptive Avoidance");
        if (sensorObj == null) {
            throw new NullReferenceException("Could not retrieve sensor with the name 'Preemptive Avoidance'");
        }

        var sensor = sensorObj.GetComponent<Sensor>();
        var bhv = behaviours["Preemptive Avoidance"];
        
        foreach (GameObject detection in sensor.agents) { // Loop through the agents that have been sensed
            if (detection == null) continue;
            
            // Check for field of effect
            var angleDeg = Vector3.Angle(transform.forward, detection.transform.position - transform.position);

            if (angleDeg <= bhv.fieldOfEffect / 2) { 
                if (hasLineOfSight(detection)) { // Check if a line of sight to the other agent is present
                    
                    // Create a force from the closest point of the other agent based on the behaviour information
                    Collider coll = detection.GetComponent<Collider>();
                    Vector3 closestPoint = coll.ClosestPointOnBounds(transform.position);
                    
                    Force force = new Force(bhv.name, closestPoint, bhv.behaviourType, bhv.rotationFactor, 
                        bhv.speedFactor, bhv.maxRotationEffect, bhv.maxSpeedEffect);
                    
                    // Add the force
                    movement.addForce(force);
                }
            }
        }
    }
    
    void obstacleAvoidance() {
        // Don't run if the system is currently focused on another behaviour
        if (isFocussing)
            return;
        
        // Get the corresponding sensor
        var sensorObj = sensing.getSensorWithName("Obstacle Avoidance");
        if (sensorObj == null) {
            throw new NullReferenceException("Could not retrieve sensor with the name 'Obstacle Avoidance'");
        }

        var sensor = sensorObj.GetComponent<Sensor>();
        var bhv = behaviours["Obstacle Avoidance"];
        
        // Loop through all detected elements
        foreach (GameObject detection in sensor.obstacles) {
            if (detection == null) continue; // Skip if detected element may have been deleted
            
            // Ensure the objects is on the correct layer
            var layerName = LayerMask.LayerToName(detection.layer);
            if (!bhv.affectors.Contains(layerName)) { 
                // If the behaviour isn't affected by an object in this layer then skip to the next detection
                continue;
            }

            // Check for field of effect
            var angleDeg = Vector3.Angle(transform.forward, detection.transform.position - transform.position);
            
            if (angleDeg <= bhv.fieldOfEffect / 2) {
                if (hasLineOfSight(detection)) {
                    // Add the force from the closest point on the detected object
                    Collider coll = detection.GetComponent<Collider>();
                    Vector3 closestPoint = coll.ClosestPointOnBounds(transform.position);
                    if(movement.showLines)
                        Debug.DrawLine(transform.position, closestPoint, Color.magenta);
                    
                    var force = new Force(bhv.name, closestPoint, "repulsive", bhv.rotationFactor, 
                        1f, -1f, -1f);
                    movement.addForce(force);
                }
            }
        }
    }

    void immediateAvoidance() {
        // Get the sensor object
        var sensorObj = sensing.getSensorWithName("Immediate Avoidance");
        if (sensorObj == null) {
            throw new NullReferenceException("Could not retrieve sensor with the name 'Immediate Avoidance'");
        }
        
        // Clear forces
        if(isFocussing)
            movement.clearForces();
        
        var sensor = sensorObj.GetComponent<Sensor>();
        var bhv = behaviours["Immediate Avoidance"];

        // Set focus if there are agent's in the sensor
        if (sensor.agents.Count > 0) {
            isFocussing = true;
        }

        // Add force for each detected agent
        foreach (GameObject detection in sensor.agents) {
            if (detection == null) continue;
            if (hasLineOfSight(detection)) {
                Collider coll = detection.GetComponent<Collider>();
                Vector3 closestPoint = coll.ClosestPointOnBounds(transform.position);
                
                Force force = new Force(bhv.name, closestPoint, bhv.behaviourType, 
                    bhv.rotationFactor, bhv.speedFactor, 
                    bhv.maxRotationEffect, bhv.maxSpeedEffect);
                    
                movement.addForce(force);
            }
        }
    }

    void preemptiveSpeedchange() {
        if (isFocussing)
            return;
        
        var sensorObj = sensing.getSensorWithName("Preemptive Avoidance");
        if (sensorObj == null) {
            throw new NullReferenceException("Could not retrieve sensor with the name 'Preemptive Avoidance'");
        }

        var sensor = sensorObj.GetComponent<Sensor>();
        var bhv = behaviours["Preemptive Avoidance"];
        
        Force finalForce = new Force("Preemptive Speed", transform.position, "repulsive", -1f, 0.0f, -1f, bhv.maxSpeedEffect);

        foreach (GameObject detection in sensor.agents) {
            if (detection == null) continue;
            
            var angleDeg = Vector3.Angle(transform.forward, detection.transform.position - transform.position);

            if (angleDeg <= bhv.fieldOfEffect / 2) {
                if (hasLineOfSight(detection)) {
                    finalForce.increaseSpeedFactorBy(bhv.speedFactor);
                }
            }
        }
        movement.addSpeedForce(finalForce);
    }
    
    bool hasLineOfSight(GameObject other) {
        // Checks if the attached GameObject has a LineOfSight with another GameObject
        RaycastHit hit;
        var dir = other.transform.position - transform.position;
        
        if (Physics.Raycast (transform.position, dir, out hit)) {
            return hit.transform.gameObject == other;
        }

        return false;
    }
    
}
