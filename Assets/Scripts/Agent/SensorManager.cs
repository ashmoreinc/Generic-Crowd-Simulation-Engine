using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour {
    public List<GameObject> sensors;

    public void createSensor(String name, float radius) {
        // Create a base object for the Sensor
        GameObject sensor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        sensor.transform.SetParent(this.transform);
        sensor.layer = 2;

        sensor.name = name;
        // Set location
        sensor.transform.position = this.transform.position;
        
        // Change the scale of the cylinder to match the given data
        var diameter = radius * 2;
        sensor.transform.localScale = new Vector3(diameter, 0.01f, diameter);
        
        // Update the base parameters of the cylinder
        Destroy(sensor.GetComponent<MeshFilter>());
        Destroy(sensor.GetComponent<MeshRenderer>());
        
        sensor.GetComponent<CapsuleCollider>().isTrigger = true;
        
        // Add scripts needed
        sensor.AddComponent<Sensor>();
        /*
        var rigid = sensor.AddComponent<Rigidbody>();
        rigid.isKinematic = true;
        */
        
        // Add to list
        sensors.Add(sensor);
    }


    public GameObject getSensorWithName(String name) {
        // Find the sensor and return it if found
        foreach (GameObject sensor in sensors) {
            if (sensor.name == name) {
                return sensor;
            }
        }

        return null;
    }
}
