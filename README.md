# *Lines of Play* - an AR Experiment with Dominoes


**_Lines of Play_**  is a domino AR Android application built with Unity highlighting recent advancements in [ARCore](https://github.com/google-ar/arcore-unity-sdk).

Unlike previous mobile AR applications limited to horizontal and vertical object placement, AR Core's [__Depth API__](https://developers.googleblog.com/2019/12/blending-realities-with-arcore-depth-api.html) allows game objects to interact with real objects in the player’s environment with near surface-level resolution. 

<img alt="LinesofPlay" src="https://github.com/googlecreativelab/lines-of-play/blob/master/Images/LOP_HERO_IMG_1920x1080.jpg?raw=true" />

Features of the player’s environment provide  constraints (*on __collision__*) and serve as visual obstructions (*__occlusion__*). 

Players can use *Lines of Play* to rapidly create and modify lines, rings, paths, and turns of domino tiles.

[<img alt="Get it on Google Play" height="50px" src="https://play.google.com/intl/en_us/badges/images/apps/en-play-badge-border.png" />](https://play.google.com/store/apps/details?id=com.arexperiments.dominoes)

This app was developed in the Unity Editor and written in C# using the MonoDevelop framework.  

For a list of supported Android devices, check [here](https://developers.google.com/ar/discover/supported-devices). 


## Contents

- [What is the Depth API?](#intro)
- [Building the Project in Unity](#project)
- [Using the Placement Tools](#tools)
- [General Notes](#general-notes)
- [Disclaimer](#disclaimer)
- [Contributors](#contributors)


## What is the Depth API?
<a name="intro"></a>

The ARCore Depth API enables depth map creation using a single camera without the use of peripheral hardware. The source gets different references to keep the depth textures up to date, retrieves focal length in pixels and transforms a camera-space vertex into world space.

A depth map is created by taking multiple images from different angles and comparing them as phone is moved and estimating the distance to every pixel. This depth-map determines which objects are closer to the player and thus whether a virtual object placed in the environment should appear overlapped by a real-world object.



## Building the Project in Unity
<a name="project"></a>

1. Before building and running the app, ensure [AR is enabled](https://play.google.com/store/apps/details?id=com.google.ar.core&hl=en) on your device.
2. [Download the Unity Editor](https://unity3d.com/get-unity/download) (this application was developed using 2018.4 (LTS)\
**_Note:_**  If this is your first time using the Unity Editor, install the Unity Hub
3. During download, install `Android Build Support`.
4. Install `Android SDK & NDK` Tools and `Open JDK`.
5. Download or clone this repository
6. Add project in Unity Hubs and open project
7. In Files ▸ Build Settings, Switch build target from `PC, Mac and Linux Standalone` to `Android`
8. Navigate to Edit ▸ Project Settings ▸ Player ▸ Other Settings
* Turn off `Multithreaded Rendering`
* Change color space from `Gamma` to `Linear`
* Turn off `Autographics API` 
* Enable `Open GLES3`
* Set to minimum API level to `Android 9.0 (Pie)` API level 24 or higher 
* Set unique names for *Product Name* and *Package Name*
9. In Player ▸ XR Settings check `ARCore supported`
10. Connect device and allow for USB debugging
11. Navigate to File ▸ Build Settings, make sure `Scene/MainScene` is checked in *Scenes in Build* queue
12. `Build and Run` application.

### Android troubleshooting for Unity
[Click here for troubleshooting help](https://developers.google.com/ar/develop/unity/quickstart-android). Also refer to the [Unity developer documentation](https://docs.unity3d.com/2018.4/Documentation/Manual/android-BuildProcess.html)


## Using the Placement Tools
<a name="tools"></a>

After application start, once scene has been scanned, a phantom domino tile is raycast onto the ground plane to provide a preview of where the domino will appear when the user taps the screen. Players can place multiple domino tiles using this method or can toggle between pre- defined path placement modes in the application’s interface. 

There are five different modes of domino path placement: 

- **Single placement** - Players can tap to place individual domino tiles *(as previously mentioned)*
- **Line placement** - Players can draw a path by specifying two points A and B and generate a line of dominoes
- **Circle placement** - Players can generate a ring of dominoes by specifying a center point of the circle and a point on its circumference
- **Square placement** - Player can specify two points A and B, that comprise the diagonal of a square. This is referred to as a 90-degree turn
- **Draw to place** - Players can freeform draw on surfaces and a path of equidistant dominos will generate 


## General Notes
<a name="general-notes"></a>


* For the Depth API to work, the *user needs to move their phone for a few seconds* before occlusion starts working reliably

* *Optimal range is 1-3 meters* (near and far depth estimates are less accurate)

* *Dynamic objects do not immediately occlude*. This adjusts when the scene becomes static



## Disclaimer
<a name="disclaimer"></a>
This is not an official Google product, but an [AR Experiment](https://experiments.withgoogle.com/collection/ar) developed at the Google Creative Lab. We’ll do our best to support and maintain this experiment but your mileage may vary.

We encourage open sourcing projects as a way of learning from each other. Please respect our and other creators’ rights, including copyright and trademark rights when present, when sharing these works and creating derivative work. If you want more info on Google's policy, you can find that [here](https://www.google.com/permissions/).




## Contributors
<a name="contributors"></a>
Built by [Jasmine Roberts](https://www.github.com/jasmineroberts) at the Google Creative Lab. 

## License
[Apache 2.0 License](https://www.apache.org/licenses/LICENSE-2.0)
