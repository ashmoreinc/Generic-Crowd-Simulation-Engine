using UnityEngine;

public class AttrManagerUpdater : MonoBehaviour {
    // Facilitates the changing of the JSON data location for a specific scene.
    // Script is attached to an object in the scene, will run at the start of the scene and load the custom file
    // Then deletes itself from the scene
    
    public string resourcePath = "json/";
    public string resourceCoreName = "core_behaviours";
    public string resourceBhvName = "behaviours";

    private void Awake() {
        // Interface with Attribute manager
        AttributeManager.setFileLocation(resourcePath);
        AttributeManager.setFilenames(resourceCoreName, resourceBhvName);
        AttributeManager.LoadJSON();
        
        // No longer needed. Destroy.
        Destroy(gameObject);
    }
}
