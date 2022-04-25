using UnityEngine;

public class EvacTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
         Destroy(other.gameObject);
    }
}
