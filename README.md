KinectHelper
======
A set of utilities which support Kinect sensor control, hand interaction control, and gesture recognition. The goal of this project is to simplify and enhance WPF Kinect for Windows v1 development by wrapping most useful code snippets together for better usability and maintainability. 

Part of the library are integrated and/or modified from:

[Fizbin.Kinect.Gesture](https://github.com/EvilClosetMonkey/Fizbin.Kinect.Gestures)

[Vitruvius](https://github.com/LightBuzz/Vitruvius)

You will need Microsoft for Kinect for Windows SDK v1.8 to use this library. 

Componenets
======

Kinect.Helper.Controller
------
Libraries extends Microsoft Kinect for Windows SDK and Kinect for Windows SDK Toolkit, handles Kinect Sensor and InteractionStream.

**KinectManager**

- Return the instance of KinectSensor
- Start/Stop Kinect sensor
- Retrieve KinectSensor Status
- Initialize/Uninitialize ColorStream, DepthStream, SkeletonStream
- SensorChooser to allow user plug/un-plug Kinect sensor 
- WPFColorViewer, WPFSkeletonViewer
- Singleton pattern to allow user access the sensor from different UI elements. 
- Observable object, can be used as ViewModel

**KinectInteractionHelper**

- Retrieve Hand object (more data model for other joints might be added later)
- Retrieve Hand state (Grip, GripRelease)
- Gesture dectectors.
- Gesture detector for standard gesture
- Gesture detector for gesture with Hand Grip or Release. 

**KinectUtil**

- Mapping ColorStream or DepthStream coordinates into screen by giving the container UIElement. 
- Get Primary skeleton from all avaliable skeletons. 

Kinect.Helper.Gesture
------

> **Note:**
>
> Kinect.Helper.Gesture component is integrated into KinectInteractionHelper, it's recommended to call it through KinectInteractionHelper to give you the possibility to get hand interaction state, but it's not mandatory. 

**GestureController**

- Dectect gestures without hand interaction invovled.
- Dectect gestures with hand interaction (Grip or Grip released);


KinectControllerTest
------
Sample app to test the library using standard WPF event driven way. 

KinectGestureExplorer
------
Console to display Hand positions, hand interactions and user gestures using MVVMLight framework. 


#### Known Issue

1. SkeletonViewer does not scale correctly after size changing
2. Right hand pointer does not match (or not correctly scaled) to joint.
3. Enable to restart the kinect sensor when connect a new sensor.
4. SkeletonViewer freezes after running for a while sometimes.
