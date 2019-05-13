# Motion Sensing Game with Kinect Sensor - Move Your Body

中文版的 Readme [請選我](../master/Chinese.md)

# Summary
“Motion Sensing Game with Kinect Sensor - Move Your Body” is my one-year Graduation Project of University. Adopt the Kinect sensor that just launched at that time to develop the 3D Game.

**<p align="center">Playing Game and Kinect Signal View</p>**
<p align="center">
  <img src="../master/PlayingGameResult.png?raw=true">
</p>

For motion sensing, because the KinectSDK hadn’t released from Microsoft, we used the OpeNI/NITE which made from Kinect producer company called PrimeSight to receive Kinect image and depth source.

Then analyzed it using Image Processing/Computer Vision, designed and developed gesture detection algorithm. At the Same time, designed the Game Content, developed Game with OOP and Design Pattern and XNA 4.0 Game Framework to built it up.


The game operate through Kinect sensor and gesture detection when you enter the menu in the beginning：

**<p align="center">Menu - Skelection detection and Pose calibration</p>**
<p align="center">
  <img src="../master/MenuOne.png?raw=true">
</p>

**<p align="center">Ooperate your menu</p>**
<p align="center">
  <img src="../master/MenuTwo.png?raw=true">
</p>

There are two stage in the game, one is "Run Away from Monster" and the other is "Pose Imitation"

**<p align="center">Run Away from Monster</p>**
<p align="center">
  <img src="../master/GameOne.png?raw=true">
</p>
 
**<p align="center">Pose Imitation</p>**
<p align="center">
  <img src="../master/GameTwo.png?raw=true">
</p>

# Development Environment
- Hardware : Kinect + AUX To USB Adapter cable
- Kinect Driver : SensorKinect-Win-OpenSource32-5.0.3.4
- Programming Language: C#
- Development Library for Game : XNA 4.0 Game Framework, JibLix Physics Engine
- Development Library for Kinect : OpenNI-Win32-1.3.2.3-Dev.msi, NITE-Win32-1.4.1.2-Dev.msi
- IDE : Visual Studio 2010
- OS : Windows 7
- GPU Supported : GT130 

# Team Members
- Eason Kuo : Team Leading, Project Management, Speech, Handling Kinect Image Source and Gesture Algorithm Development.
- Cheng-Yi Huang : Game Design, Game Development, Integrate Gesture Signal Subsystem. 


# Slide Link
You could read the “Motion Sensing Game with Kinect Sensor - Move Your Body” game slide from the [Slide Link](https://www.slideshare.net/YiChengKuo1/20111027-graduation-project-motion-sensing-game-with-kinect-sensor-move-your-body)

# Video Link
You could watch the “Motion Sensing Game with Kinect Sensor - Move Your Body” game demo from the [Video Link](https://youtu.be/Np3yjK-OHMM)