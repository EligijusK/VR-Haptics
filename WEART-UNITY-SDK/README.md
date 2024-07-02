# Unity SDK

Welcome to the WEART Unity SDK documentation.

The WEART SDK allows the creation of haptic experiences in Unity by enabling interfacing with the TouchDIVER device for both PC and Android Standalone applications. The new SDK handles both platforms and allows development and testing from the Unity Editor, as well as the ability to build for Windows and standalone headsets:

* Start and Stop device execution
* Calibrate the finger tracking device
* Receive tracking data from the devices
* Retrive raw data from the device
* Send haptic effects to the devices (actuations)
* Read status information from the device

The minimum setup to use the weart SDK consists of:

* A TouchDIVER device
* An Unity project using the SDK package 
* Middleware running (only on the PC version)

### Unity version compatibility:

#### Android standalone:
* 2021
* 2022

####  PC windows:
* 2019
* 2020
* 2021
* 2022

@note It's possible to use the same WEART SDK on both platforms (Windows and Android) starting from Unity 2019, but exclusively when generating a build for Android Standalone, it's necessary to use Unity 2021 and 2022.

## Importing WEART SDK

Create a new project or open an existing one.

Go to "Window" and then to "Package Manager".

![](./packageManager.png)

Then press on the "+" and select "Add package from disk".

![](./packageFromDisk.png)

Find the location of the SDK, select <b><i>package.json</i></b> and press "Open".

![](./packageJson.png)




