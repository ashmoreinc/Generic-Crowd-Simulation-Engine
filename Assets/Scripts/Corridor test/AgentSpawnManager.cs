using System.Collections.Generic;
using UnityEngine;

public class AgentSpawnManager : MonoBehaviour {
    public GameObject agentPrefab;
    
    public GameObject spawnPoint1;
    public GameObject spawnPoint2;
    public GameObject despawnPoint1;
    public GameObject despawnPoint2;

    public float secsPerSpawn;

    public Material dir1Mat;
    public Material dir2Mat;
    
    private List<GameObject> dir1Agents;
    private List<GameObject> dir2Agents;

    private float dir1lastSpawnTime;
    private float dir2lastSpawnTime;
    
    public int maxAgentsPerDir;
    
    void Start() {
        // Set the last spawn times and current tracked lists to default
        dir1lastSpawnTime = Time.time;
        dir2lastSpawnTime = Time.time;
        dir1Agents = new List<GameObject>();
        dir2Agents = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        // Spawn agent's each update
        SpawnAgents();
    }

    void SpawnAgents() {
        // First direction
        // Check the last spawn time and see if it is ready to spawn a new agent
        if (dir1lastSpawnTime < Time.time - secsPerSpawn) {
            if (dir1Agents.Count < maxAgentsPerDir) { // Spawn a new agent if there are less than the cap
                // Create agent and set up attributes
                GameObject agent = Instantiate(agentPrefab);
                agent.transform.position = getRandomPointInMesh(spawnPoint1);
                agent.GetComponent<CoreBehaviour>().destination = getRandomPointInMesh(despawnPoint1);
                agent.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                agent.GetComponent<CoreBehaviour>().behaviourStatus = 1;
                agent.GetComponent<Renderer>().material = dir1Mat;

                // Add the agent to the list and update the last spawn time
                dir1Agents.Add(agent);
                dir1lastSpawnTime = Time.time;
            }
        }
        // Second direction
        // Check the last spawn time and see if it is ready to spawn a new agent
        if (dir2lastSpawnTime < Time.time - secsPerSpawn) {
            if (dir2Agents.Count < maxAgentsPerDir) { // Spawn a new agent if there are less than the cap
                GameObject agent = Instantiate(agentPrefab);
                // Create agent and set up attributes
                agent.transform.position = getRandomPointInMesh(spawnPoint2);
                agent.GetComponent<CoreBehaviour>().destination = getRandomPointInMesh(despawnPoint2);
                agent.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                agent.GetComponent<CoreBehaviour>().behaviourStatus = 1;
                agent.GetComponent<Renderer>().material = dir2Mat;

                // Add the agent to the list and update the last spawn time
                dir2Agents.Add(agent);
                dir2lastSpawnTime = Time.time;
            }
        }
    }

    Vector3 getRandomPointInMesh(GameObject obj) {
        // Return a random point within a given mesh
        var collBounds = obj.GetComponent<Collider>().bounds;

        return new Vector3(
            UnityEngine.Random.Range(collBounds.min.x, collBounds.max.x),
            1,
            UnityEngine.Random.Range(collBounds.min.z, collBounds.max.z)
        );
    }

    void DespawnAgent(GameObject agent) {
        // Delete an agent from the scene
        if (dir1Agents.Contains(agent)) {
            dir1Agents.Remove(agent);
        } else if (dir2Agents.Contains(agent)) {
            dir2Agents.Remove(agent);
        }

        Destroy(agent);
    }

    public void despawnerEntry(GameObject agent, GameObject despawner) {
        // Check if the despawner triggered is connected to the given agent
        if (despawner == despawnPoint1) {
            if (dir1Agents.Contains(agent)) {
                DespawnAgent(agent);
            }

            return;
        }

        // Check if the despawner triggered is connected to the given agent
        if (despawner == despawnPoint2) {
            if (dir2Agents.Contains(agent)) {
                DespawnAgent(agent);
            }
        }
    }
}
