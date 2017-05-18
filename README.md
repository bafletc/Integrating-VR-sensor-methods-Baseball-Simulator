Integrating VR Sensor Methods: Baseball Simulator
====== 
### David Johnson, Brian Fletcher, Nick Luce

__Overview:__ The aim of this project, is to combine a few existing utilities to form a more immersive VR Baseball simulator experience than any of these utilities
could achieve alone. 

### Required Utilities:
[__FlaFla2/Unity Wiimote API__](https://github.com/Flafla2/Unity-Wiimote): Accesses basic user input from button presses and updates Wiimote motion data while ingame
[__Vitruvius Kinect Utility__](https://github.com/LightBuzz/Vitruvius): Elaborates on the original Kinect utilities by providing more nuanced interaction between the user's motion and the Unity environment
[__Trinus VR Unity__](https://www.assetstore.unity3d.com/en/#!/content/43781): Facilitates the connection between the PC and the HMD(Head Mounted Display) for video 
and sensor traffic
[__Kinect for Windows SDK 2.0__](https://developer.microsoft.com/en-us/windows/kinect/develop): Gives the PC basic tools to read Kinect body tracking data
[__Trinus VR App__](https://www.assetstore.unity3d.com/en/#!/content/43781): Trinus VR uses an app availible on the Apple app store as well as google play to manage
the ph
__Cross Platform:__ The API is compatible with Windows and can communicate with some Android devices and iPhones(Check Trinus requirements as well any further 
requirements that could be needed for the user's phone to connect to the PC with the selected connection method.)

To install, open Unity-Wiimote.unitypackage or go to Assets->Import Package->Custom Package... in the Unity Editor and locate Unity-Wiimote.unitypackage.

__Project Description:__ This project utilizes the skeleton modelling system made by the Vitruvius Utility in combination with FlaFla2's Wiimote Unity integration and Trinus VR for Unity API. This project uses these to attach the camera object, provided by the Trinus API, to the neck bone of the Vitruvius skeleton. This combination allows for the rotational tracking provided by the Trinus and Wiimote utility to coordinate with the remaining body tracking data of the Kinect.
  The Vitruvius skeleton maintains the rotation of the camera around the y-axis over extended periods of time in order to reduce drift. While the rapidly updating rotation data from the mobile device allows for better real-time tracking of the head's rotation. This produces a more immersive experience in terms of head rotation, reducing jitter, and prevents a greater degree of error from forming over time. This rotation is about the center of the gameobject's rotation and will not rotate around another point of reference.(Except as mentioned in the next paragraph). The Wiimote's rotation will also not rotate around a point other than the center of the object. 
  In terms of positional tracking, the Vitruvius skeleton controls the translation of the game objects connected to the user's body. This includes the use of the Trinus utility, to rotate the headset around a point of reference more closely associated to the neck than the center of the headset. This rotation will change the position of the headset. Other than this exception, all postional tranformation for the game's objects is handled by the Kinect. 
  Merging the tracking methods of these sources is the puropse of this project, to attain a immersive VR experience comperable to a more expensive system such as an Oculus or Vive. Providing a higher degree of freedom for the motion of objects a user relies on, while refraining from using more complicated tracking methods used in the other systems. 
    
# Installation: #




__Potential Updates:__

While the API is very powerful already, I would still like to make changes to it to improve it even more. Namely I would like to:

Add support for all common extension controllers (Classic Controller Pro, etc.)
Add support for Nunchuck passthrough / Classic Controller passthrough mode on the Wii Motion Plus
Add speaker support (no small feat!)
If you would like to help implement any of these changes, feel free to submit a pull r
