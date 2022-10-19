# Generic-Crowd-Simulation-Engine
The development of a Generic Cloud Simulatiion Engine. Built using Unity.

This was developed along side [Ricardo Goncalves](https://github.com/ricardogoncalves-cds) using PlasticSCM, this is the final port from that VC.

## Usage
To get started using this system, clone this repo and open it via Unity Hub.
This project runs on Unity Version 2020.3.30f1, and is confirmed to work in this version.
This project _should_ work in later versions of Unity, but has not been confirmed.

## View previous development
Some scenes are already present in the project, to view these, navigate (from within Unity) to Scenes and the sub-directories hold the scenes.
Some scenes of note are (all within the Asset directory)
* Scenes/Feature Testing/\*, these are the scenes which were used to test features
* Scenes/Corridor Test, this is a usage of an implementation using the generic system
* Scenes/Evacuation, this is another usage of an implentation using the generic system

## Project design

### Environment
Environments are made up of static GameObjects with colliders (and a render if you want it to be visbile).
Obstacles in the environment must be on the Obstacle Layer (Layer 6), this allows the agent's to see them as obstacles.

Once the scene is set up as you desire, you must bake the scene with NavMesh.

### Agent
The core of the system is the agent(s) which make it up.
There is a Prefab inside Prefabs\NewMovement\Agent
This prefab contains the following:

Script | Function
----------|----------
Movement | Controls the Agent's movement
Core Behaviour | Controls the core behaviours of the agent (Move To, Follow, Wander)
Behaviour Manager | Controls the More complex behaviours of the agent
SensorManager | Controls the deployment of sensors for the agent

### Change the loaded JSON file
The attributes for the agents are pulled via JSON using two files (core_behaviours.json and behaviours.json),
The default location for these files are inside the Resources folder under the JSON directory.

To change the directory used in a scene, add a GameObject with the AttrManagerUpdater script attached to it.
Change the location of the AttrManagerUpdate to point to the file.
The root directory for the JSON files **must** be the Resources folder, so to navigate to a direcory called JSON within the Resources, set the path to be JSON/.

A worked example of changing the JSON file location is done inside the Evacuation scene.
