using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class CoreBehaviour : MonoBehaviour {
    private Move movement; // Reference to the movement system

    public int behaviourStatus = 1;
    /*
     * 0: Wandering
     * 1: moveTo
     * 2: follow
     * 3: idle
     * 4: Move To Zone
     */
    
    // Wandering variables
    public float xLim, yLim = 24;

    // How far away from the destination an agent can be before they are classed as having reached it
    [Range(0, 5)] public float distanceForConfirmation = 1.5f;
    
    // moveTo variables
    public Vector3 destination = new Vector3(9999, 9999, 9999); // An extreme value which identifies that there is no destination
    private NavMeshPath path;
    private bool hasPath = false;

    // follow variables
    public GameObject following;
    
    // Zone Variable
    public GameObject destinationZone;
    void Awake() {
        // Initialise components
        movement = this.GetComponent<Move>();
        path = new NavMeshPath();
    }

    private void Start() {
        // Randomise destination for wandering mode
        if(destination == new Vector3(9999, 9999, 9999)) 
            randomDestination();
    }

    void FixedUpdate() {
        // Call the correct behaviour based on behaviourStatus.
        // Default to idle if isn't correct
        switch (behaviourStatus) {
            case 0: 
                wandering();
                break;
            case 1: 
                moveTo();
                break;
            case 2:
                follow();
                break;
            case 3:
                idle();
                break;
            case 4:
                randomDestinationInZone();
                behaviourStatus = 1;
                break;
            default:
                behaviourStatus = 3;
                break;
        }
    }

    // Behaviours
    void idle() {
        // Tell the movement to stop moving
        movement.stopMoving();
    }

    void moveTo() {
        // Check if the behaviour is only Move To
        if (behaviourStatus == 1) {
            // If so stop moving when the destination been reached and swicth the behaviour to idle
            float distance = Vector3.Distance(transform.position, destination);
            
            if (distance <= distanceForConfirmation) {
                behaviourStatus = 3;
                return;
            }
        }
        
        // Otherwise calculate a path to the destination, if one can't be found, stop moving this update cycle
        calculatePath();
        if (!hasPath) {
            movement.stopMoving();
            if(movement.showLines)
                Debug.Log("Dont have path");
            return;
        }
        
        // Ensure the agent is moving at this point
        movement.startMoving();

        // Set the rotation to the next step in the destination path as an attractive force
        Force force = new Force("Destination", path.corners[1], "attractive", 1f, 1f, -1f, -1f);
        
        movement.addForce(force);
    }

    void wandering() {
        // Calculate path
        calculatePath();
        
        // Try to find a new destination if one could not be calculated
        if (!hasPath){
            for(int i=0; i<10; i++) { // 10 Attempts
                randomDestination();
                calculatePath();

                if (hasPath)
                    break;
            }
        }

        // If a path could not be calculated, stop the moving and stop the script for this update cycle
        if (!hasPath) {
            Debug.Log("Cannot calculate path");
            movement.stopMoving();
            return;
        }
        
        // Make sure the agent is moving for after this point
        movement.startMoving();
        
        // Check if reached destination and set a new destination
        float distance = Vector3.Distance(transform.position, destination);

        if (distance < distanceForConfirmation) {
            randomDestination();
        }
        
        // Destination has been set, now trigger the move to function
        moveTo();
    }

    void follow() {
        // Set the destination
        destination = following.transform.position;
        
        // Calculate a path and move to that path
        calculatePath();
        if (hasPath) {
            moveTo();
            movement.startMoving();
        } else {
            movement.stopMoving();
        }
    }
    
    // Ancillary functions
    void calculatePath() {
        // Use's navmesh to calculate a path towards a given destination
        hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        
        // Draw line
        if(movement.showLines)
            for(int i=0; i<path.corners.Length - 1; i++)
               Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

    }
    
    void randomDestination() {
        // Sets a random destination
        destination = new Vector3(Random.Range(xLim * -1, xLim), 0, Random.Range(yLim * -1, yLim));
    }

    void randomDestinationInZone() {
        // Sets a random destination with another gameObject
        var collBounds = destinationZone.GetComponent<Collider>().bounds;

        destination = new Vector3(
            UnityEngine.Random.Range(collBounds.min.x, collBounds.max.x),
            1,
            UnityEngine.Random.Range(collBounds.min.z, collBounds.max.z)
        );
    }
} 
