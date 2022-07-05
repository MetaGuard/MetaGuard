using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class InterfaceManager : MonoBehaviour {
    public GameObject XRRig;
    public GameObject MainCamera;
    public GameObject VRUIFloorObject;
    public GameObject VRUIHatObject;
    public GameObject VRUIMenuObject;

    public GameObject LeftLaser;
    public GameObject RightLaser;

    public Text StatusText;
    public GameObject PhotoMGOn;
    public GameObject PhotoMGOff;
    public SpaceBear.VRUI.VRUIColorPalette ColorPalette;
    public SpaceBear.VRUI.VRUISlider VRUISlider;

    public bool masterToggle = false;
    public bool heightToggle = false;
    public bool wingspanToggle = false;
    public bool roomSizeToggle = false;
    public bool ipdToggle = false;
    public bool geolocationToggle = false;
    public bool squatDepthToggle = false;
    public bool armLengthsToggle = false;
    public bool handednessToggle = false;
    public bool voiceToggle = false;
    public bool trackingRateToggle = false;
    public int privacyLevel = 2;

    // Start is called before the first frame update
    void Start() {
        // Place hat above player
        VRUIHatObject.transform.SetParent(XRRig.transform);

        // Place floor below player
        VRUIFloorObject.transform.SetParent(XRRig.transform);
    }

    // Update is called once per frame
    void Update() {
        // Keep hat above head
        VRUIHatObject.transform.position = MainCamera.transform.position;

        // Keep floor below head
        VRUIFloorObject.transform.position = new Vector3(MainCamera.transform.position.x, XRRig.transform.position.y, MainCamera.transform.position.z);

        // Toggle menu (left hand)
        if (SteamVR_Actions._default.PrimaryButton.GetStateDown(SteamVR_Input_Sources.LeftHand)) {
            if (VRUIMenuObject.active) {
                CloseMenu();
            } else {
                PositionMenu();
                LeftLaser.SetActive(true);
            }
        }

        // Toggle menu (right hand)
        if (SteamVR_Actions._default.PrimaryButton.GetStateDown(SteamVR_Input_Sources.RightHand)) {
            if (VRUIMenuObject.active) {
                CloseMenu();
            } else {
                PositionMenu();
                RightLaser.SetActive(true);
            }
        }
    }

    // Place menu in front of player
    void PositionMenu() {
        VRUIMenuObject.transform.position = MainCamera.transform.position + MainCamera.transform.forward * 2.5f;
        VRUIMenuObject.transform.forward = MainCamera.transform.forward;
        VRUIMenuObject.transform.eulerAngles = new Vector3(0, VRUIMenuObject.transform.eulerAngles.y, VRUIMenuObject.transform.eulerAngles.z);
        VRUIMenuObject.SetActive(true);
    }

    // Hide UI elements
    public void CloseMenu() {
        VRUIMenuObject.SetActive(false);
        LeftLaser.SetActive(false);
        RightLaser.SetActive(false);
    }

    // Toggle switches
    public void MasterToggle(bool b) {
        masterToggle = b;
        if (b) {
            StatusText.text = "MetaGuard On";
            PhotoMGOn.SetActive(true);
            PhotoMGOff.SetActive(false);
            ColorPalette.accentColor = new Color(0.1607843f, 0.7137255f, 0.9647059f);
        } else {
            StatusText.text = "MetaGuard Off";
            PhotoMGOn.SetActive(false);
            PhotoMGOff.SetActive(true);
            ColorPalette.accentColor = new Color(0.7411765f, 0.7411765f, 0.7411765f);
        }
        ColorPalette.UpdateColors();
    }
    public void HeightToggle(bool b) {
        heightToggle = b;
    }
    public void WingspanToggle(bool b) {
        wingspanToggle = b;
    }
    public void RoomSizeToggle(bool b) {
        roomSizeToggle = b;
    }
    public void IPDToggle(bool b) {
        ipdToggle = b;
    }
    public void GeolocationToggle(bool b) {
        geolocationToggle = b;
    }
    public void SquatDepthToggle(bool b) {
        squatDepthToggle = b;
    }
    public void ArmLengthsToggle(bool b) {
        armLengthsToggle = b;
    }
    public void HandednessToggle(bool b) {
        handednessToggle = b;
    }
    public void VoiceToggle(bool b) {
        voiceToggle = b;
    }
    public void TrackingRateToggle(bool b) {
        trackingRateToggle = b;
    }
    public void SetLowPrivacy() {
        privacyLevel = 1;
        VRUISlider.value = 1;
    }
    public void SetMedPrivacy() {
        privacyLevel = 2;
        VRUISlider.value = 2;
    }
    public void SetHighPrivacy() {
        privacyLevel = 3;
        VRUISlider.value = 3;
    }
}
