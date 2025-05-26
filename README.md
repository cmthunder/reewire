# ReeWire
Utility to turn 3D models into wireframe ReeSaber objects.

## Requisites
* Unity 6 (Older versions might work)

## Usage
* Open the project in Unity.
* Add all your objects to the scene. **All objects must be childs of the object named Wireframe!** Any object without a Mesh Renderer will be skipped.
* Configure the wireframe. Settings can be found inside the Wireframe object.
* Press Play. Generation will start (Unity may freeze). The .json file will be generated inside the project folder.