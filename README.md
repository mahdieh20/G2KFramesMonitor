Gen. II Kinect 4 Windows – Frames Monitor
===========
Simple tool that visualizes the amount of frames/second in a certain environment when using a 
**MultiSourceFrameReader** where all the selected streams receive a new frame.

This tool is only supported with the **Gen. II Kinect for Windows** and is created with **C#** & **WPF**.
*(MVVM-pattern)*

You can read my blog post on the *MultiSourceFrameReader* [here](http://www.kinectingforwindows.com/2014/05/19/second-gen-kinect-comparing-multisourceframereader-and-xsourceframereader/) & this tool [here](http://www.kinectingforwindows.com/2014/05/25/gen-ii-kinect-for-windows-introducing-frames-monitor/).

![K4W logo](http://www.kinectingforwindows.com/wp-content/themes/twentyten/images/headers/logo.jpg)

## Disclaimer
This tutorial is based on the **November (2013)** version of the **alpha SDK** for Kinect for Windows Gen II and not the final sdk.

> “This is preliminary software and/or hardware and APIs are preliminary and subject to change”.

## Screenshots
In this monitor run you can see that there was a **lot of light** in the room so all the frames come in at **30 FPS**.

![Screenshot](http://www.kinectingforwindows.com/images/github/frames_monitor_light_monitor.png)

In this monitor run you can see that there was a **not much light** in the room so all the frames come in at **15 FPS**.

![Screenshot](http://www.kinectingforwindows.com/images/github/frames_monitor_dark_monitor.png)