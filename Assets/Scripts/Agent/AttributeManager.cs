using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ValueRange {
    // This class allows for the input of a range which facilitates randomisation of an attribute
    // Serialised for usage with JSON
    public float min;
    public float max;
    public ValueRange(float min, float max) {
        this.min = min;
        this.max = max;
    }
    
    public ValueRange(float val) {
        this.min = val;
        this.max = val;
    }
    
    public ValueRange(float[] vals) {
        try {
            this.min = vals[0];
        } catch (Exception ex) { // No values provided, this cannot work
            throw new ArgumentException(
                "No values declared for vals of type float[], vals must contain 1 or 2 values only.");
        }

        try {
            this.max = vals[1];
        } catch (Exception ex) {
            this.max = vals[0]; // Only one value provided, therefore both values must be the same.
        }

    }
    
    public bool isSingleValue() {
        return max == min;
    }

    public float getMin() {
        return min;
    }

    public float getMax() {
        return max;
    }
    
    public float getRandom() {
        if (isSingleValue()) {
            return min;
        }
        return Random.Range(min, max);
    }
}

[Serializable]
public class BehaviourPreset {
    // Ancillary class for deserialiing JSON
    public string name;
    public string[] affectors;
    public string behaviourType;
    public bool takesFocus;
    
    public ValueRange radius;
    public ValueRange speedFactor;
    public ValueRange rotationFactor;
    public ValueRange maxSpeedEffect;
    public ValueRange maxRotationEffect;
    public ValueRange fieldOfEffect;

    public Behaviour getBehaviour() {
        return new Behaviour(name, new List<String>(affectors), behaviourType, takesFocus,
            radius.getRandom(),
            speedFactor.getRandom(),
            rotationFactor.getRandom(),
            maxSpeedEffect.getRandom(),
            maxRotationEffect.getRandom(),
            fieldOfEffect.getRandom());
    }
    
    public BehaviourPreset(){}
}


// Ancillary classes for JSON read/write
[Serializable]
public class Behaviours {
    public BehaviourPreset[] behaviours;
}

[Serializable]
public class core_behaviour {
    public ValueRange max_speed;
    public ValueRange base_speed;
    public ValueRange acceleration_factor;
}

public static class AttributeManager {
    // The data manager, holds a redundancy measure (builtIn_{variable})
    
    private static Dictionary<String, ValueRange> attributes = new Dictionary<string, ValueRange>();
    private static Dictionary<String, BehaviourPreset> behaviours = new Dictionary<string, BehaviourPreset>();
    
    private static Dictionary<String, ValueRange> builtInAttributes = new Dictionary<string, ValueRange>(){
        {"max speed", new ValueRange(5f, 8f)},
        {"base speed", new ValueRange(2.5f, 4f)},
        {"acceleration factor", new ValueRange(1f, 3f)}
    };

    private static Dictionary<String, BehaviourPreset> builtInBehaviours = new Dictionary<string, BehaviourPreset>() {
        {"Preemptive Avoidance", new BehaviourPreset(){
                name="Preemptive Avoidance",
                affectors = new string[] {"Agent"},
                behaviourType="repulsive",
                takesFocus = false,
                radius = new ValueRange(3f, 7f),
                speedFactor = new ValueRange(0.05f, 0.1f),
                rotationFactor = new ValueRange(.1f, .75f),
                maxSpeedEffect = new ValueRange(0f),
                maxRotationEffect = new ValueRange(-1f),
                fieldOfEffect = new ValueRange(180f) 
            }
        },
        {"Obstacle Avoidance", new BehaviourPreset(){
            name="Obstacle Avoidance",
            affectors = new string[] {"Obstacle"},
            behaviourType="repulsive",
            takesFocus = false,
            radius = new ValueRange(1f, 2f),
            speedFactor = new ValueRange(1f),
            //rotationFactor = new ValueRange(0.05f, .5f),
            rotationFactor = new ValueRange(5f, 7f),
            maxSpeedEffect = new ValueRange(1f),
            maxRotationEffect = new ValueRange(1f),
            fieldOfEffect = new ValueRange(360f) 
        }},
        {"Immediate Avoidance", new BehaviourPreset(){
            name="Immediate Avoidance",
            affectors = new string[] {"Agent"},
            behaviourType="repulsive",
            takesFocus = false,
            radius = new ValueRange(1f, 2f),
            speedFactor = new ValueRange(1f),
            rotationFactor = new ValueRange(5f, 7f),
            maxSpeedEffect = new ValueRange(1f),
            maxRotationEffect = new ValueRange(1f),
            fieldOfEffect = new ValueRange(360f) 
        }},
    };

    private static bool jsonLoaded = false;

    private static string jsonLocation = "json/";
    private static string jsonCoreBhvFilename = "core_behaviours";
    private static string jsonBhvFilename = "behaviours";

    // JSON Functions
    public static void LoadJSON() {
        // Get the core behaviours
        if (jsonLoaded) return;
        
        // Empty the current behaviours
        attributes = new Dictionary<string, ValueRange>();
        behaviours = new Dictionary<string, BehaviourPreset>();
        
        TextAsset coreJson = Resources.Load<TextAsset>(jsonLocation + jsonCoreBhvFilename);
        if (coreJson != null) {
            core_behaviour core = JsonUtility.FromJson<core_behaviour>(coreJson.text);
            
            attributes.Add("max speed", core.max_speed);
            attributes.Add("base speed", core.base_speed);
            attributes.Add("acceleration factor", core.acceleration_factor);
            Debug.Log("JSON Loaded: Core Attribute (Max Speed)");
            Debug.Log("JSON Loaded: Behaviour (Base Speed)");
            Debug.Log("JSON Loaded: Behaviour (Acceleration Factor)");
        }

        TextAsset bhvJson = Resources.Load<TextAsset>(jsonLocation + jsonBhvFilename);
        if (bhvJson != null) {
            Behaviours bhvs = JsonUtility.FromJson<Behaviours>(bhvJson.text);
            foreach(BehaviourPreset bhv in bhvs.behaviours){
                AttributeManager.behaviours.Add(bhv.name, bhv);
                Debug.Log("JSON Loaded: Behaviour (" + bhv.name + ")");
            }
        }

        jsonLoaded = true;
    }

    //      JSON Setters
    public static void setFilenames(string core, string bhvs) {
        if (core != null) jsonCoreBhvFilename = core;
        if (bhvs != null) jsonBhvFilename = bhvs;
        
        // Set the loaded status to false as the location is likely different and a new data needs to be reloaded.
        jsonLoaded = false;
    }

    public static void setFileLocation(string location) {
        if (location != null) jsonLocation = location;

        // Set the loaded status to false as the location is likely different and a new data needs to be reloaded.
        jsonLoaded = false;
    }
    
    // Getters for attributes
    public static float getMaxSpeed() {
        return getAttribute("max speed").getRandom();
    }

    public static float getBaseSpeed() {
        return getAttribute("base speed").getRandom();
    }
    
    public static float getAccelerationFactor() {
        return getAttribute("acceleration factor").getRandom();
    }

    public static Behaviour getBehaviourPreemptiveAvoidance() {
        return getBehaviour("Preemptive Avoidance");
    }
    
    public static Behaviour getBehaviourObstacleAvoidance() {
        return getBehaviour("Obstacle Avoidance");
    }
    
    public static Behaviour getBehaviourImmediateAvoidance() {
        return getBehaviour("Immediate Avoidance");
    }

    public static Behaviour getBehaviourPreemptiveSpeedChange() {
        return getBehaviour("Preemptive Speed change");
    }

    private static Behaviour getBehaviour(String name) {
        LoadJSON();

        if (behaviours.ContainsKey(name)) {
            return behaviours[name].getBehaviour();
        }

        if (builtInBehaviours.ContainsKey(name)) {
            return builtInBehaviours[name].getBehaviour();
        }
        
        
        throw new ArgumentException("No behaviour found with name " + name);
    }
    private static ValueRange getAttribute(String name) {
        LoadJSON();
        
        if (attributes.ContainsKey(name)) {
            return attributes[name];
        }
        if(builtInAttributes.ContainsKey(name)) {
            return builtInAttributes[name];
        }
        throw new ArgumentException("No attribute found with name " + name);
    }
}