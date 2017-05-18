Integrating VR Sensor Methods: Baseball Simulator
============ 
## David Johnson, Brian Fletcher, Nick Luce

__Overview:__ The aim of this project, is to combine a few existing utilities to form a more immersive VR Baseball simulator experience than any of these utilities could achieve alone. 

## Required Utilities:
[__FlaFla2/Unity Wiimote API__](https://github.com/Flafla2/Unity-Wiimote): Accesses basic user input from button presses and updates Wiimote motion data while ingame  
[__Vitruvius Kinect Utility__](https://github.com/LightBuzz/Vitruvius): Elaborates on the original Kinect utilities by providing more nuanced interaction between the user's motion and the Unity environment  
[__Trinus VR Unity__](https://www.assetstore.unity3d.com/en/#!/content/43781): Facilitates the connection between the PC and the HMD(Head Mounted Display) for video and sensor traffic  
[__Kinect for Windows SDK 2.0__](https://developer.microsoft.com/en-us/windows/kinect/develop): Gives the PC basic tools to read Kinect body tracking data  
[__Trinus VR App__](https://www.assetstore.unity3d.com/en/#!/content/43781): Trinus VR uses an app, availible on the Apple App Store as well as Google Play, to manage the phone's side of this project. 

__Cross Platform:__ The API is compatible with Windows and can communicate with some Android devices and iPhones (Check Trinus requirements as well any further requirements that could be needed for the user's phone to connect to the PC with the selected connection method).

## Requirements: ##
Any Requirements Here or in the Documents for the included utilities should be met for the program to function properly
* Kinect for Windows v2/Kinect for Xbox v2 and Adaptor for Windows
* Official Nintendo-Brand Wiimote 
#### PC #### 
* Windows 7/8/10
* CPU: Intel i3 2nd Gen or AMD FX-4000s Series CPU; 2.0+ GHz; USB 3.0/.1 Compatible
* GPU: Directx 10.0 Compatible; Results Might Vary
* Chipset: USB 3.0 & Bluetooth Availible
#### Mobile Device (Android Highly Reccomended) ####
* Android 4.0+/iOS 9.2+
* See Trinus VR documentation for device specifications required to meet the video projection, connetion method, and motion sensor needs  of the user

__Project Description:__
This project utilizes the skeleton modelling system made by the Vitruvius Utility in combination with FlaFla2's Wiimote Unity integration and Trinus VR for Unity API. This project uses these to attach the camera object, provided by the Trinus API, to the neck bone of the Vitruvius skeleton. This combination allows for the rotational tracking provided by the Trinus and Wiimote utility to coordinate with the remaining body tracking data of the Kinect.  

The Vitruvius skeleton maintains the rotation of the camera around the y-axis over extended periods of time in order to reduce drift. While the rapidly updating rotation data from the mobile device allows for better real-time tracking of the head's rotation. This produces a more immersive experience in terms of head rotation, reducing jitter, and prevents a greater degree of error from forming over time. This rotation is about the center of the gameobject's rotation and will not rotate around another point of reference (Except as mentioned in the next paragraph). The Wiimote's rotation will also not rotate around a point other than the center of the object.   

In terms of positional tracking, the Vitruvius skeleton controls the translation of the game objects connected to the user's body. This includes the use of the Trinus utility, to rotate the headset around a point of reference more closely associated to the neck than the center of the headset. This rotation will change the position of the headset. Other than this exception, all postional tranformation for the game's objects is handled by the Kinect.  

Merging the tracking methods of these sources is the puropse of this project, to attain a immersive VR experience comperable to a more expensive system such as an Oculus or Vive. Providing a higher degree of freedom for the motion of objects a user relies on, while refraining from using more complicated tracking methods used in the other systems. 

_See Further Technical Details in Documentation Directory

## Installation: ##

#### Main Methods ####
1. Install the Trinus VR app to a compatable mobile device  
2. Install the Kinect for Windows SDK v2.0 to the PC  
3. Don't connect to the Wiimote to the PC over bluetooth(Current Version doesn't use the wii mote) 
4. Download the repository and unpack the "BaseballSimPackage.unitypackage" located in the mainsrc folder  
5. Open the scene file "BaseballSimScene.unity" also located in the src folder   
6. Open the app on the mobile device and cofigure it for the desired function
7. Connect the Kinect to the PC(Test in the Kinect Studio)
8. Create Mobile Hotspot on mobile device from the Trinus App in "USB Tap to Activate"
9. Disable Mobile Data on phone
10. Start Client Server
9. Play the scence and follow the connection methods detailed in the documentation for each utilities' devices (Wi-Fi Connection used by default for much better performance and to easily reduce physical connectivity issues between the device and PC; USB connection is also not supported for iOS devices)

#### Extension ####
5) If the scene is not loaded in properly  
  a. Create a blank new scene and insert the prefab "BBSimPrefab" from the project src folder  
  b. Deconstruct this prefab and take all the immediate child objects from the prefab heirarchy and move them to be objects of the scene.  
  c. Return to Step 6  
8) If the phone or PC's network adapter won't support a wifi connect
  a. Deselect "Create Mobile Hotspot" in the Trinus Manager Object 

  
  
## Controls ##

#### Wiimote Controls: ####
A: Respawn Ball  
1: Callibrate Wiimote Gyroscope  
2: Callibrate Wiimote Accelerometer  

#### Keyboard Controls ####
R: Recallibrate Headset Rotation (Recallibration Required after the Kinect detects the skeleton and the mobile device's camera runtime object is created.)  
I: Ignore View 
P: Spawn Ball

__Potential Updates:__

