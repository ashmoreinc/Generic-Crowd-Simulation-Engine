using System;
using System.Collections.Generic;

public interface IBehaviour {
    String name {
        get;
        set;
    } // The name of the behaviour
    List<String> affectors { get; set; } // What affects this behaviour (e.g. agents, obstacles etc.)
    String behaviourType { get; set; } // The type of behaviour this illicits from the agent (attractive or repulsive)
    bool takesFocus { get; set; }
    float radius {
        get;
        set;
    } // The radius of effect
    float speedFactor {
        get;
        set;
    } // The factor this behaviour has on the speed change
    float rotationFactor {
        get;
        set;
    } // The factor this behaviour has on rotational change
    float maxSpeedEffect {
        get;
        set;
    } // The maximum effect this behaviour can have on the speed
    float maxRotationEffect {
        get;
        set;
    } // The maximum effect this behaviour can have on the rotation
    float fieldOfEffect {
        get;
        set;
    } // The vision code of the behaviour (in degrees)
}

public class Behaviour : IBehaviour {
    // Behaviour class, contains various constructors for different formatted instantiations
    public String name { get; set; }
    public List<String> affectors { get; set; }
    public String behaviourType { get; set; }
    public bool takesFocus { get; set; }
    public float radius { get; set; }
    public float speedFactor { get; set; }
    public float rotationFactor { get; set; }
    public float maxSpeedEffect { get; set; }
    public float maxRotationEffect { get; set; }
    public float fieldOfEffect { get; set; }

    public Behaviour(String name, List<String> affectors, String behaviourType) {
        this.name = name;
        this.affectors = affectors;
        this.behaviourType = behaviourType;
        takesFocus = false;
        radius = 1f;
        speedFactor = 1f;
        rotationFactor = 1f;
        maxSpeedEffect = 5f;
        maxRotationEffect = 5f;
        fieldOfEffect = 360f;
    }
    
    public Behaviour(String name, List<String> affectors, String behaviourType, bool takesFocus) {
        this.name = name;
        this.affectors = affectors;
        this.behaviourType = behaviourType;
        this.takesFocus = takesFocus;
        radius = 1f;
        speedFactor = 1f;
        rotationFactor = 1f;
        maxSpeedEffect = 5f;
        maxRotationEffect = 5f;
        fieldOfEffect = 360f;
    }

    public Behaviour(String name, List<String> affectors, String behaviourType, bool takesFocus, float radius) {
        this.name = name;
        this.affectors = affectors;
        this.behaviourType = behaviourType;
        this.takesFocus = takesFocus;
        this.radius = radius;
        speedFactor = 1f;
        rotationFactor = 1f;
        maxSpeedEffect = 5f;
        maxRotationEffect = 5f;
        fieldOfEffect = 360f;
    }

    public Behaviour(String name, List<String> affectors, String behaviourType, bool takesFocus, float radius, float speedFactor, float rotationFactor) {
        this.name = name;
        this.affectors = affectors;
        this.behaviourType = behaviourType;
        this.takesFocus = takesFocus;
        this.radius = radius;
        this.speedFactor = speedFactor;
        this.rotationFactor = rotationFactor;
        maxSpeedEffect = 5f;
        maxRotationEffect = 5f;
        fieldOfEffect = 360f;
    }

    public Behaviour(String name, List<String> affectors, String behaviourType, bool takesFocus, float radius, float speedFactor, float rotationFactor, float maxSpeedEffect,
        float maxRotationEffect) {
        this.name = name;
        this.affectors = affectors;
        this.behaviourType = behaviourType;
        this.takesFocus = takesFocus;
        this.radius = radius;
        this.speedFactor = speedFactor;
        this.rotationFactor = rotationFactor;
        this.maxSpeedEffect = maxSpeedEffect;
        this.maxRotationEffect = maxRotationEffect;
        fieldOfEffect = 360f;
    }
    
    public Behaviour(String name, List<String> affectors, String behaviourType, bool takesFocus, float radius, float speedFactor, float rotationFactor, float maxSpeedEffect,
        float maxRotationEffect, float fieldOfEffect) {
        this.name = name;
        this.affectors = affectors;
        this.behaviourType = behaviourType;
        this.takesFocus = takesFocus;
        this.radius = radius;
        this.speedFactor = speedFactor;
        this.rotationFactor = rotationFactor;
        this.maxSpeedEffect = maxSpeedEffect;
        this.maxRotationEffect = maxRotationEffect;
        this.fieldOfEffect = fieldOfEffect;
    }
}