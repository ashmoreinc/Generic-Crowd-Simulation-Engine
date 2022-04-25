using System;
using UnityEngine;

public struct Force {
    // Force data type to facilitate passing around the data of a force
    public Vector3 origin;
    public String forceType;
    public float rotationFactor;
    public float speedFactor;
    public float maxRotationEffect;
    public float maxSpeedEffect;

    public String name;
    public Force(String name, Vector3 forceOrigin, String forceType, float rotationMultiplier, 
            float speedMultiplier, float maxRotationEffect, float maxSpeedEffect) {
        this.name = name;
        this.origin = forceOrigin;
        this.forceType = forceType;
        this.rotationFactor = rotationMultiplier;
        this.speedFactor = speedMultiplier;
        this.maxRotationEffect = maxRotationEffect;
        this.maxSpeedEffect = maxSpeedEffect;
    }

    public Force(Vector3 forceOrigin, Behaviour behaviour) {
        this.name = behaviour.name;
        this.origin = forceOrigin;
        this.forceType = behaviour.behaviourType;
        this.rotationFactor = behaviour.rotationFactor;
        this.speedFactor = behaviour.speedFactor;
        this.maxRotationEffect = behaviour.maxRotationEffect;
        this.maxSpeedEffect = behaviour.maxSpeedEffect;
    }
    
    public void setOrigin(Vector3 newVect) {
        origin = newVect;
    }

    public void increaseSpeedFactorBy(float factor) {
        this.speedFactor += factor;
    }

    public float getFinalSpeed() {
        if (speedFactor > maxSpeedEffect)
            return maxSpeedEffect;
        if (speedFactor < 0)
            return 0;
        return speedFactor;
    }
}
