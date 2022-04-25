using System;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {
    private GameObject parent;
    
    // Set the lists
    public List<GameObject> agents = new List<GameObject>();
    public List<GameObject> obstacles = new List<GameObject>();
    private void Start() {
        // Initialise the parent reference
        parent = transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other) {
        // Check the layer matches agent or Obstacle
        if (other.gameObject.layer == 3) { // Is an agent
            if (other.gameObject != parent) { // Is not the parent
                try {
                    agents.Add(other.gameObject);
                } catch (Exception) {} // Surpress errors
            }
        } else if (other.gameObject.layer == 6) { // Is an obstacle
            try {
                obstacles.Add(other.gameObject);
            }
            catch (Exception ex) {
                Debug.LogError(ex.Message);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        // Check which list the exitting object is in and remove them if present
        if (agents.Contains(other.gameObject)) {
            agents.Remove(other.gameObject);
        } else if (obstacles.Contains(other.gameObject)) {
            obstacles.Remove(other.gameObject);
        }
    }
}
