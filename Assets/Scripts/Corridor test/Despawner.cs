using UnityEngine;

public class Despawner : MonoBehaviour {
    private AgentSpawnManager spawner;

    private void Start() { // Set Spawner manager 
        spawner = transform.parent.GetComponent<AgentSpawnManager>();
    }

    public void OnTriggerEnter(Collider other) { // Trigger the  despawn event
        spawner.despawnerEntry(other.gameObject, this.gameObject);
    }
}
