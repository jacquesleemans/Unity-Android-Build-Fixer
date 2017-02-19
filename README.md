# Unity-Android-Build-Fixer
Simple Editor script which checks for common android build issues.

The idea with this "tool" is to make Android development less painful, especially when using multiple plugins, eg:

* Facebook SDK
* AdMob
* VoxelBusters Native plugins (Ultra pack)
* Chartboost
* other native plugins...

## TODO ##
1. Automatically change the minSDKVersion to the highest version in the xml files
2. Set the build target version to the above version
3. Mark the conflicting files
4. Add buttons to open the files in the external editor (Mono, VS, etc..)
5. Check for duplicate activities in the XML files. (This might already be done by the Unity merger)
6. Check the SDK Manager if there are any updates.
7. Check if all the SDK (Java, NDK, Android, etc..) paths are setup correctly

## Manual ##
1. Open the window (Menu bar->XGameDev->Android Fixer)
2. Click 'Check android'
3. Enable/Disable the checkboxes to show common android files

## Common Android Build Issues ##

### CommandInvokationFailure: Unable to merge android manifests. ###
Solution: minSDKVersion are the same in the AndroidManifest.xml files
Check the XML files, make sure the minSDKVersion

### Unable to convert classes into dex format ###
Solution:
1. Check the AAR and JAR files. looking for any files which looks like they might be the same, eg:
  * android-support-v4 and support-v4
  * support-annotations-24.0.0 and annotations

### WebGL - Build Folder is Empty ###
Not related to android, but general get this issue with cross-platform projects.
Solution:
in the link.xml file:

<assembly fullname="Facebook.Unity.Android" ignoreIfMissing="1">
...
</assembly>
