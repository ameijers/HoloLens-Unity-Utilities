This utility is called ObjectController and it allows you to control the size, rotation and position of a hologram as gameobject in your HoloLens application. A wired frame is created around the object which is controlled. In each corner a controller is placed for sizing the object. In the middle of the frame controllers are placed for rotating the object. At the top of the frame a sphere controller is placed. That controller allows you to move the object.

![](https://github.com/ameijers/HoloLens-Unity-Utilities/blob/master/WikiImages/ObjectController.PNG)

Resizing the object is done by tap and hold fingers together at one of the specified controllers. Moving the hand in x direction will resize the object. The sizing stops when fingers are released.

Rotating the object is done in the same way as resizing the object. Tap and hold the fingers together at one of the specified controllers. Move the hand in x direction to rotate left or right. The rotating stops when fingers are released.

Positioning the object somewhere else is achieved by tap fingers at the controller for positioning. At that moment the object will be dragged at PositionDistance from the HoloLens in the gaze direction. When a layer is specified for positioning and the object hits that layer, the object is moved in front of the layer.

## Adding the ObjectController via Editor
Make sure you have added all files in a folder of your assets in your Unity project. Than drag the ObjectController prefab onto a GameObject in the Hierarchy and make sure it becomes a child of that GameObject. Start configuring the settings and you are ready to go!

![](https://github.com/ameijers/HoloLens-Unity-Utilities/blob/master/WikiImages/ObjectController-Hierarchy.PNG)

## Adding the ObjectController via code
Create an instance of the ObjectController as a GameObject. Than set the parent via transform.SetParent() to the GameObject you wish to control. Configure the settings of the component script ObjectController in your GameObject.

## Settings
The following settings are present

![](https://github.com/ameijers/HoloLens-Unity-Utilities/blob/master/WikiImages/ObjectController-settings.PNG)

********ObjectToControl********  
This is the GameObject whic is controlled by the ObjectController

********LineWidth********  
Width of the line drawn between the controllers

********LineColor********  
Color of the line drawn between the controllers. Make sure that the lineShader is set. If not the lines will not be colored

********ControllerSize********  
The size of the controllers as a Vector3

********lineShader********  
A shader is needed to allow the lines being drawn in the specified color. The ObjectController has a Custom/ParticleShader in the project folder which can be used

********ControllerLayer********  
This specifies the Unity layer which can be used for the controllers to be hit. You can use the same layer to create a cursor which can be used to visualize the selection. When a controller is hit, the size of the controller gets larger.

**OutboundOfObject**  
This value specifies the space between the object which is controlled and the controllers placed around it

**RotationSensitivity**  
This value specifies the sensitivity of rotating the object

**ResizeSensitivity**  
this value specifies the sensitivity of resizing the object

**MinimalSize**  
The minimal size allowed during resizing

**MaxiumSize**  
the maximum size allowed during resizing

**PositionLayer**  
This specified the Unity layer which is used to position the object. This allows you to have an object moves along a layer in your scene during positioning

**PositionHitdistance**  
The distance between the layer and the object when the layer is hit

**PositionDistance**  
The distance between the HoloLens and the position of the object in the direction of the gaze during positioning


