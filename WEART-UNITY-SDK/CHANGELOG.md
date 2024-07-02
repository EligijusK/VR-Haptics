
# Change Log
All notable changes to this project will be documented in this file.

## [1.3.0]

### Added
* With the same SDK we can now use it on both PC and Android Standalone directly on the headsets
* BleConnectionPanel is responsible for showing the touch divers available on Standalone and gives the ability to connect those to the application. The touch divers connected are responsible for changing the CalibrationManager hands positions that are available for the calibration
* ActuationPanel is able to display the effects that are applied on the haptic objects
* WristPanel presents the following functionalities from left  to right
    * Show/Hide BleConnectionPanel
    * Show/Hide MiddlewareStatusDisplay
    * Reset calibration process
    * Show/Hide the Actuation Panel
* Add WeArtTemperatureVolume that changes the temperature of the touchable objects that enter or exit the volume. This is present only when importing the sample scene
* Added new offset for tracking device:
    - HTC Wrist trackers
    - HTC XR Elite controllers
    - Pico 4 Enterprise and Pico Neo 4   

### Changed
* WeArtController now has both PC and Standalone functionality, having settings for both platforms in the inspector
* Sample scene updated with the newest features
* CalibrationManager automatically shows the corresponding hands on calibration based on the touch divers connected
* During Running state, only connected hand/s is/are showed
* Improved surface exploration
* Physical hands only have four haptic objects
* Improved the hands interation with trigger collider touchable objects, allowing to feel objects when the hand is inside a different effect trigger volume
* Scripts removed from ghost hands present in hand prefabs(WEARTRightHand/WEARTLeftHand) and replaced with WeArtGhostHandController
* Only one instance of WeArtHandController and WeArtDeviceTrackingObject are present for each hand prefab
* Improved hand grasping by adding capsule collider proximity checkers instead of box colliders
* WeArtTouchDIVER class renamed WeArtDevice
* Removed fake hands references
* All texts on MiddlewareStatus panel based on TextMeshPro format
* BLE Plugin .aar  file updated from version 2.4.0 to 2.4.3 (Working on Quest 2, HTC XR Elite, Pico 4 Enterprise and Pico 3 Neo)
* Preset offsets using OpenXR with Meta Quest and PicoXR Devices

In some particular conditions (noisy environment), we still have some performance issues using Quest 2 for Standalone applications

### Fixed
* Fixed Unity version 2019 and 2020 interaction with mesh colliders
* Fixed WeArtTouchableObject's OnDisable() not affecting WeArtHandController for non trigger colliders

## [1.2.0]
 
### Added

* Add new physic hand system
* Add middleware status display component and scripts
* Add Raw Thimble Sensor data tracking (WeArtThimbleSensorObject component)
* Added sample scenes
* Editing at run-time textures
* Add calibration procedure start/stop and listener 
* Add calibration UX prefab
* Add hand grasping events
* Add new default tracking message and values for closure and abduction
* Debug Actuations

## [1.1.0] 

### Added

* Added sample scenes
* Editing at run-time textures
* Add calibration procedure start/stop and listener
* Add hand grasping events
* Add new default tracking message and values for closure and abduction
