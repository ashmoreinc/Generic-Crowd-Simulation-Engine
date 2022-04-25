using System;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour  {
    // Base attributes
    public float maxSpeed = 10f;
    public float desiredSpeed = 4f;
    public float baseSpeed = 4f;
    public float accelerationFactor = 1f;
    public float rotationAccelerationFactor = 1f;

    private float currentSpeed = 0f;

    private bool idle = false;

    // Force data
    public List<Force> speedForces;
    public List<Force> forces;
    public List<float> speedChanges;
    public bool showLines = false;
    
    private void Start() {
        // Clean lists on start
        forces = new List<Force>();
        speedForces = new List<Force>();
        
        // Get the attribute data
        maxSpeed = AttributeManager.getMaxSpeed();
        baseSpeed = AttributeManager.getBaseSpeed();
        accelerationFactor = AttributeManager.getAccelerationFactor();
    }

    void FixedUpdate() {
        // Show direction
        if(showLines)
            Debug.DrawLine(transform.position, transform.forward * 5f + transform.position, Color.green);

        // Run the updates
        updateSpeed();
        updateRotation();
        // Clear forces ready for next update
        clearForces();
    }
    
    void updateSpeed() {
        bool isWithinRange(float check, float of, float range) {
            return check <= of + range && check >= of - range;
        }
        
        // Calculate Speed desire
        float speedChange = getSpeedByFactors();
        
        var calculatedSpeed = baseSpeed + speedChange;
        if (calculatedSpeed > maxSpeed) {
            calculatedSpeed = maxSpeed;
        } else if (calculatedSpeed < 0) {
            calculatedSpeed = 0;
        }
        
        // Output the calculated speed
        if(showLines)
            Debug.Log("Calculated Speed: " + calculatedSpeed);
        
        desiredSpeed = calculatedSpeed;

        if (idle) {
            desiredSpeed = 0;
        }
        
        // Attempt to match current speed to desired speed
        if (isWithinRange(currentSpeed, desiredSpeed, 0.1f)) {
            currentSpeed = desiredSpeed;
        } else {
            if (currentSpeed > desiredSpeed)  { // Decrease current speed to match desire
                currentSpeed += Time.deltaTime * accelerationFactor * -1;
            } else if(currentSpeed < desiredSpeed){ // Increase current speed to match desire
                currentSpeed += Time.deltaTime * accelerationFactor;
            }
        }

        // Update the agents position based on speed
        transform.Translate(new Vector3(0f, 0f, currentSpeed * Time.deltaTime));
        
        // Reset the affects altering the speed for the next run
        speedChanges = new List<float>();
    }

    void updateRotation() {
        Vector3 finalDirection = new Vector3();

        // Loop through forces to calculate the change to direction
        foreach (Force force in forces) {
            force.setOrigin(new Vector3(force.origin.x, transform.position.y, force.origin.z));

            var dist = Vector3.Distance(force.origin, transform.position);

            Vector3 direction;
            
            // Check if the effect is repulsive or attractive and calculate the direction accordingly
            if (force.forceType == "repulsive") {
                direction = (transform.position - force.origin).normalized * force.rotationFactor;
            } else if (force.forceType == "attractive") {
                direction = (force.origin - transform.position).normalized * force.rotationFactor;
            } else {
                throw new ArgumentException("Force type: " + force.forceType + " not recognised");
            }
            
            // Show the direction of effect
            if(showLines)
                Debug.DrawLine(transform.position, transform.position + direction * dist, Color.white);
            
            // Add the force to the final force
            finalDirection += direction;
        }
        
        // Set new rotation and update rotation
        var targetRotation = Quaternion.LookRotation(finalDirection, Vector3.up);
    
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationAccelerationFactor);
    }
    
    public void addForce(Force force) {
        // Add's a force if one isnt already present
        if(!forces.Contains(force))
            forces.Add(force);
    }

    public void stopMoving() {
        // Set idle variable to true to stop the movement
        idle = true;
    }
    
    public void startMoving() {
        // set the idle variable to false to start movement
        idle = false;
    }

    public void addSpeedForce(Force force) {
        // Add's a force which will affect speed
        speedForces.Add(force);
    }

    public float getSpeedByFactors() {
        // Get the current change in speed based on the forces
        Dictionary<String, float> identifiedForces = new Dictionary<String, float>();
        
        var changes = 0f;
        foreach (Force force in speedForces) { // Get each value of change from a percentage of the base speed which each force alters speed by
            float change;
            if (force.forceType == "repulsive") { // Reverse the value if repulsed
                change = baseSpeed * force.getFinalSpeed() * -1;
            } else {
                change = baseSpeed * force.getFinalSpeed();
            }

            changes += change;
        }

        return changes;
    }
    
    public void clearForces() { // Reset the force list
        forces = new List<Force>();
        speedForces = new List<Force>();
    }
}
