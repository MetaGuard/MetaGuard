## VR UI Kit: Material Design System

**Installation:**

-   Download Oculus Integration from the Asset Store and import it into your project
    
-   Download and import this package
    
-   Open the SampleScene. That's it!
    

**How to use:**
SampleScene in the Demo folder is a good place to start. It will show you the various VRUI prefabs that are available, along with a VRUIManager game object that control the color schemes.

To use the VRUI prefabs in your scene, first, add the VRUIManager game object, then add the VRUI prefabs from the Assets/Prefabs folder that you want to use. It is as simple as that.

Note: if you want to use this UI for non-VR apps, all you need to do is use the Standalone Input Module in the Event System instead of Oculus' OVR Input Module.

**Contact:**
For further information or inquiries, email: [spacebearinc@gmail.com](mailto:spacebearinc@gmail.com)

**How to use with SteamVR**
The following instructions are from user Leon Müller:

1) Add the SteamVR_LaserPointer to the left or right controller in the [CameraRig] or Player prefab
2) Also add the SteamVRWrapper (http://answers.unity.com/answers/1622254/view.html) to the same controller
3) Add a BoxCollider and size it appropriately to every UI element (where the Button component is attached)
4) If you're just using the [CameraRig] and not the Player prefab, you also need to add an EventSystem and the SteamVR InputModule. You can take a look at the Player prefab, where it's already added.

Sadly, the SteamVR Laser Pointer only supports OnPointerEnter, OnPointerExit and OnPointerClicked, but no OnPointerDragged, so not everything will work.

Big thanks to Leon to write the above tutorial!