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

# Installation: #



__Potential Updates:__

While the API is very powerful already, I would still like to make changes to it to improve it even more. Namely I would like to:

Add support for all common extension controllers (Classic Controller Pro, etc.)
Add support for Nunchuck passthrough / Classic Controller passthrough mode on the Wii Motion Plus
Add speaker support (no small feat!)
If you would like to help implement any of these changes, feel free to submit a pull r
