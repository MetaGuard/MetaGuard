using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using System;
using rand = UnityEngine.Random;
using Valve.VR;

public class MetaGuard : MonoBehaviour {
    public InterfaceManager UI;
    public GameObject CameraOffset;
    public GameObject MainCamera;
    public GameObject LeftController;
    public GameObject RightController;
    public GameObject RightControllerOffset;
    public GameObject LeftControllerOffset;
    // public GameObject CameraOffsetRightEye;
    // public GameObject CameraOffsetLeftEye;
    // public Camera XRCamera;
    // public Camera RightXRCamera;
    // public Camera LeftXRCamera;

    private Vector3 leftControllerPos;
    private Vector3 rightControllerPos;
    private Valve.VR.SteamVR_PlayArea.Size size;

    // Controls initiating and toling off and on the protections
    private bool one_time = false;
    private bool toggle_all = false;

    // average device offset (across HTC Vive, Vive Pro 2, and Oculus Quest 2)
    private float y_offset = 0.107f;

    // Conversion ratios
    private float height_to_wingspan_ratio = 1.04f;
    private float fitness_threshold = 0.5f;

    // Attributes to protect
    private float height;
    private float arm_length_ratio;
    private float wingspan;
    private float squat_depth;
    private float ipd;
    private float room_width;
    private float room_length;

    // Required for protection logic
    private float room_center_x;
    private float room_center_z;

    // Noise values for each attribute depending on privacy level
    private List<float> height_noises = new List<float>();
    private List<float> right_arm_noises = new List<float>();
    private List<float> left_arm_noises = new List<float>();
    private List<float> wingspan_right_arm_noises = new List<float>();
    private List<float> wingspan_left_arm_noises = new List<float>();
    private List<float> wingspan_noises = new List<float>();
    private List<float> squat_depth_noises = new List<float>();
    private List<float> room_width_noises = new List<float>();
    private List<float> room_length_noises = new List<float>();
    private List<float> ipd_noises = new List<float>();

    // DP bounds for each attribute
    private float room_lower = 0f;
    private float room_upper = 5f;
    private float room_sensitivity;
    private float height_lower = 1.496f;
    private float height_upper = 1.826f;
    private float height_sensitivity;
    private float squat_depth_lower = 0f;
    private float squat_depth_upper;
    private float squat_depth_sensitivity;
    private float arm_ratio_lower = 0.95f;
    private float arm_ratio_upper = 1.05f;
    private float arm_ratio_sensitivity;
    private float wingspan_lower;
    private float wingspan_upper;
    private float wingspan_sensitivity;
    private float arm_length_lower = 0.663f;
    private float arm_length_upper = 0.853f;
    private float arm_length_sensitivity;
    private float ipd_lower = 0.0557f;
    private float ipd_upper = 0.0710f;
    private float ipd_sensitivity;

    // Toggle values for each attribute, true = off for some reason
    private bool room_toggle = true;
    private bool height_toggle = true;
    private bool ipd_toggle = true;
    private bool squat_depth_toggle = true;
    private bool arm_toggle = true;
    private bool master_toggle = true;
    private bool wingspan_toggle = true;

    // Epsilon values for each attribute
    private float[] height_epsilons = { 1f, 3f, 5f };
    private float[] room_epsilons = { 0.1f, 1f, 3f };
    private float[] ipd_epsilons = { 1f, 3f, 5f };
    private float[] squat_depth_epsilons = { 1f, 3f, 5f };
    private float[] arm_epsilons = { 0.5f, 1f, 3f };

    // The privacy level chosen by the user. It acts as an index for the lists of epilons and noises
    private int privacy_level = 2;
    private int privacy_levels = 3;

    //// Differential privacy function implmentation
    //// Adapted from laplace bounded domain of IBM's diffprivlib (for more details on the implementation): 
    //// https://github.com/IBM/differential-privacy-library/blob/main/diffprivlib/mechanisms/laplace.py
    // Clamps values so that we do not have extreme outputs 
    float Clamp(float value, float lower, float upper) {
        return Math.Max(lower, Math.Min(value, upper));
    }

    float _delta_c(float shape, float delta_q, float diam) {
        if (shape == 0) {
            return 2.0f;
        }
        return (2 - (float)Math.Exp(-delta_q / shape) - (float)Math.Exp(-(diam - delta_q) / shape)) / (1 - (float)Math.Exp(-diam / shape));
    }

    float _f(float shape, float delta_q, float eps, float delta, float diam) {
        return delta_q / (eps - (float)Math.Log(_delta_c(shape, delta_q, diam)) - (float)Math.Log(1 - delta));
    }

    // Samples from a Laplace distribution in a way that avoids floating point vulnerability
    float _laplace_sampler(float unif1, float unif2, float unif3, float unif4) {

        return (float)Math.Log(1 - unif1) * (float)Math.Cos((float)Math.PI * unif2) + (float)Math.Log(1 - unif3) * (float)Math.Cos((float)Math.PI * unif4);
    }

    // Main function to generate a differentially private noisy value
    float LDPNoise(float[] epsilons, float sensitivity, float value, float upper, float lower) {
        // Calculates the new scale of the noise distribution, as we onlzy consider an interval
        float eps = epsilons[privacy_level];
        float delta = 0;
        float diam = Math.Abs(upper - lower);
        float delta_q = sensitivity;
        float left = delta_q / (eps - (float)Math.Log(1 - delta));
        float right = _f(left, delta_q, eps, delta, diam);
        float old_interval_size = (right - left) * 2;

        while (old_interval_size > right - left) {

            old_interval_size = right - left;
            float middle = (right + left) / 2;

            if (_f(middle, delta_q, eps, delta, diam) >= middle) {
                left = middle;
            }
            if (_f(middle, delta_q, eps, delta, diam) <= middle) {
                right = middle;
            }
        }

        float scale = (right + left) / 2;

        // Clamping the input value
        value = Clamp(value, lower, upper);

        // Randomization
        int samples = 1;
        while (true) {
            bool res = false;
            float max_noise = int.MinValue;
            float temp_noise;
            // Loop until we get the noise value that is within the interval and, when more than one sample is produced, pick the max noise 
            for (int i = 0; i < samples; i++) {
                temp_noise = value + scale * _laplace_sampler(rand.value, rand.value, rand.value, rand.value);
                if (temp_noise >= max_noise && temp_noise >= lower && temp_noise <= upper) {
                    max_noise = temp_noise;
                    res = true;
                }
            }
            if (res) {
                return max_noise;
            }
            // This is done so that we do not need to fix the number of iterations and it can adapt when the interval is small, i.e., it needs more samples to succeed
            samples = (int)Math.Min(100000, samples * 2);
        }
    }

    void GetVRRoomSize() {
        var rect = new Valve.VR.HmdQuad_t();
        Valve.VR.SteamVR_PlayArea.GetBounds(size, ref rect);
        // Guide: x=v0, y=v1, z=2. Corner0=bottom right, Corner1=bottom left, Corner2=top left, Corner3=top right
        room_width = Math.Abs(rect.vCorners1.v0 - rect.vCorners0.v0);
        room_length = Math.Abs(rect.vCorners2.v2 - rect.vCorners1.v2);
        // center = left + width/2
        room_center_x = rect.vCorners1.v0 + room_width / 2f;
        // center = left + length/2
        room_center_x = rect.vCorners1.v2 + room_length / 2f;
    }

    void Start() {
        // offset the y coor attributes
        GetVRRoomSize();
        height_lower -= y_offset;
        height_upper -= y_offset;
        // Initialize remaining bounds 
        squat_depth_upper = height_upper * (1 - fitness_threshold);
        wingspan_lower = height_to_wingspan_ratio * height_lower;
        wingspan_upper = height_to_wingspan_ratio * height_upper;
        // Initialize sensitivities
        height_sensitivity = Math.Abs(height_upper - height_lower);
        arm_ratio_sensitivity = Math.Abs(arm_ratio_upper - arm_ratio_lower);
        squat_depth_sensitivity = Math.Abs(squat_depth_upper - squat_depth_lower);
        ipd_sensitivity = Math.Abs(ipd_upper - ipd_lower);
        room_sensitivity = Math.Abs(room_upper - room_lower);
        wingspan_sensitivity = Math.Abs(wingspan_upper - wingspan_lower);
        arm_length_sensitivity = Math.Abs(arm_length_upper - arm_length_lower);
    }

    void Update() {
        height_toggle = !(UI.masterToggle && UI.heightToggle);
        arm_toggle = !(UI.masterToggle && UI.armLengthsToggle);
        room_toggle = !(UI.masterToggle && UI.roomSizeToggle);
        wingspan_toggle = !(UI.masterToggle && UI.wingspanToggle);
        squat_depth_toggle = !(UI.masterToggle && UI.squatDepthToggle);
        ipd_toggle = !(UI.masterToggle && UI.ipdToggle);
        master_toggle = !UI.masterToggle;
        privacy_level = UI.privacyLevel - 1;

        // It is only executed once. It calculates all the noisy values at all privacy levels for all attributes 
        if (!master_toggle && !one_time) {
            height = MainCamera.transform.localPosition.y;
            squat_depth = height * (1 - fitness_threshold);
            float right_arm_length = Vector3.Distance(MainCamera.transform.localPosition, RightController.transform.position);
            float left_arm_length = Vector3.Distance(MainCamera.transform.localPosition, LeftController.transform.position);
            arm_length_ratio = right_arm_length / left_arm_length;
            wingspan = height_to_wingspan_ratio * height;
            Vector3 leftEye = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye);
            Vector3 rightEye = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightEye);
            ipd = Vector3.Distance(leftEye, rightEye);
            // The loop calculates all the differentially private noisy measurements for all sensitive attributes
            for (int i = 0; i < privacy_levels; i++) {
                // This lobal variable will change and with it the epsilon of the noise distributions
                privacy_level = i;
                height_noises.Add(LDPNoise(height_epsilons, height_sensitivity, height, height_upper, height_lower) - height);
                float noisy_wingspan = LDPNoise(arm_epsilons, wingspan_sensitivity, wingspan, wingspan_upper, wingspan_lower);
                float noisy_arm_length_ratio = LDPNoise(arm_epsilons, arm_ratio_sensitivity, arm_length_ratio, arm_ratio_upper, arm_ratio_lower);
                right_arm_noises.Add(LDPNoise(arm_epsilons, arm_length_sensitivity, right_arm_length, arm_length_upper, arm_length_lower) / right_arm_length);
                left_arm_noises.Add(LDPNoise(arm_epsilons, arm_length_sensitivity, left_arm_length, arm_length_upper, arm_length_lower) / left_arm_length);
                wingspan_right_arm_noises.Add((noisy_wingspan / 2) * noisy_arm_length_ratio / right_arm_length);
                wingspan_left_arm_noises.Add((noisy_wingspan / 2) * (1 / noisy_arm_length_ratio) / left_arm_length);
                wingspan_noises.Add((noisy_wingspan / wingspan));
                squat_depth_noises.Add(LDPNoise(squat_depth_epsilons, squat_depth_sensitivity, squat_depth, squat_depth_upper, squat_depth_lower) / squat_depth);
                ipd_noises.Add(LDPNoise(ipd_epsilons, ipd_sensitivity, ipd, ipd_upper, ipd_lower));
                room_width_noises.Add((LDPNoise(room_epsilons, room_sensitivity, room_width / 2f, room_upper, room_lower)) / (room_width / 2.0f));
                room_length_noises.Add((LDPNoise(room_epsilons, room_sensitivity, room_length / 2f, room_upper, room_lower)) / (room_length / 2.0f));
            }
            privacy_level = 1;
            one_time = true;
        } else if (master_toggle && one_time) {
            one_time = false;
        }

        // Protections on the X and Z axis 
        if (one_time && !arm_toggle) {
            float half_distance_between_controllers = Vector3.Distance(RightController.transform.position, LeftController.transform.position) / 2.0f;
            // Right controller
            Vector3 direction_of_a_controller = RightController.transform.position - LeftController.transform.position;
            float angle_between_controllers = Mathf.Atan2(direction_of_a_controller.z, direction_of_a_controller.x);
            float coord_offset = half_distance_between_controllers * (right_arm_noises[privacy_level] - 1);
            float right_controller_offset_x = coord_offset * (float)Math.Cos(angle_between_controllers);
            float right_controller_offset_z = coord_offset * (float)Math.Sin(angle_between_controllers);
            // Left controller
            direction_of_a_controller = LeftController.transform.position - RightController.transform.position;
            angle_between_controllers = Mathf.Atan2(direction_of_a_controller.z, direction_of_a_controller.x);
            coord_offset = half_distance_between_controllers * (left_arm_noises[privacy_level] - 1);
            float left_controller_offset_x = coord_offset * (float)Math.Cos(angle_between_controllers);
            float left_controller_offset_z = coord_offset * (float)Math.Sin(angle_between_controllers);
            if (!room_toggle) {
                float headset_room_offset_x = MainCamera.transform.localPosition.x - (room_center_x + room_width_noises[privacy_level] * (MainCamera.transform.localPosition.x - room_center_x));
                float headset_room_offset_z = MainCamera.transform.localPosition.z - (room_center_z + room_width_noises[privacy_level] * (MainCamera.transform.localPosition.z - room_center_z));
                CameraOffset.transform.localPosition = new Vector3(headset_room_offset_x, CameraOffset.transform.localPosition.y, headset_room_offset_z);
                RightControllerOffset.transform.localPosition = new Vector3(headset_room_offset_x + right_controller_offset_x, RightControllerOffset.transform.localPosition.y, headset_room_offset_z + right_controller_offset_z);
                LeftControllerOffset.transform.localPosition = new Vector3(headset_room_offset_x + left_controller_offset_x, LeftControllerOffset.transform.localPosition.y, headset_room_offset_z + left_controller_offset_x);
            } else {
                CameraOffset.transform.localPosition = new Vector3(0f, CameraOffset.transform.localPosition.y, 0f);
                RightControllerOffset.transform.localPosition = new Vector3(right_controller_offset_x, RightControllerOffset.transform.localPosition.y, right_controller_offset_z);
                LeftControllerOffset.transform.localPosition = new Vector3(left_controller_offset_x, LeftControllerOffset.transform.localPosition.y, left_controller_offset_z);
            }
        }
        if (one_time && arm_toggle) {
            if (!room_toggle) {
                float headset_room_offset_x = (room_center_x + room_width_noises[privacy_level] * (MainCamera.transform.localPosition.x - room_center_x)) - MainCamera.transform.localPosition.x;
                float headset_room_offset_z = (room_center_z + room_width_noises[privacy_level] * (MainCamera.transform.localPosition.z - room_center_z)) - MainCamera.transform.localPosition.z;
                CameraOffset.transform.localPosition = new Vector3(headset_room_offset_x, CameraOffset.transform.localPosition.y, headset_room_offset_z);
                RightControllerOffset.transform.localPosition = new Vector3(headset_room_offset_x, RightControllerOffset.transform.localPosition.y, headset_room_offset_z);
                LeftControllerOffset.transform.localPosition = new Vector3(headset_room_offset_x, LeftControllerOffset.transform.localPosition.y, headset_room_offset_z);
            } else {
                CameraOffset.transform.localPosition = new Vector3(0f, CameraOffset.transform.localPosition.y, 0f);
                RightControllerOffset.transform.localPosition = new Vector3(0, RightControllerOffset.transform.localPosition.y, 0);
                LeftControllerOffset.transform.localPosition = new Vector3(0, LeftControllerOffset.transform.localPosition.y, 0);
            }
        }

        // Protections on the Y axis
        if (one_time && !squat_depth_toggle) {
            if (!height_toggle) {
                float temp_y_offset = MainCamera.transform.localPosition.y - ((height + height_noises[privacy_level]) - (height - MainCamera.transform.localPosition.y) * squat_depth_noises[privacy_level]);
                CameraOffset.transform.localPosition = new Vector3(CameraOffset.transform.localPosition.x, temp_y_offset, CameraOffset.transform.localPosition.z);
                RightControllerOffset.transform.localPosition = new Vector3(RightControllerOffset.transform.localPosition.x, temp_y_offset, RightControllerOffset.transform.localPosition.z);
                LeftControllerOffset.transform.localPosition = new Vector3(LeftControllerOffset.transform.localPosition.x, temp_y_offset, LeftControllerOffset.transform.localPosition.z);
            } else {
                float temp_y_offset = MainCamera.transform.localPosition.y - ((height) - (height - MainCamera.transform.localPosition.y) * squat_depth_noises[privacy_level]);
                CameraOffset.transform.localPosition = new Vector3(CameraOffset.transform.localPosition.x, temp_y_offset, CameraOffset.transform.localPosition.z);
                RightControllerOffset.transform.localPosition = new Vector3(RightControllerOffset.transform.localPosition.x, temp_y_offset, RightControllerOffset.transform.localPosition.z);
                LeftControllerOffset.transform.localPosition = new Vector3(LeftControllerOffset.transform.localPosition.x, temp_y_offset, LeftControllerOffset.transform.localPosition.z);
            }
        }
        if (one_time && squat_depth_toggle) {
            if (!height_toggle) {
                CameraOffset.transform.localPosition = new Vector3(CameraOffset.transform.localPosition.x, height_noises[privacy_level], CameraOffset.transform.localPosition.z);
                RightControllerOffset.transform.localPosition = new Vector3(RightControllerOffset.transform.localPosition.x, height_noises[privacy_level], RightControllerOffset.transform.localPosition.z);
                LeftControllerOffset.transform.localPosition = new Vector3(LeftControllerOffset.transform.localPosition.x, height_noises[privacy_level], LeftControllerOffset.transform.localPosition.z);
            } else {
                CameraOffset.transform.localPosition = new Vector3(CameraOffset.transform.localPosition.x, 0f, CameraOffset.transform.localPosition.z);
                RightControllerOffset.transform.localPosition = new Vector3(RightControllerOffset.transform.localPosition.x, 0f, RightControllerOffset.transform.localPosition.z);
                LeftControllerOffset.transform.localPosition = new Vector3(LeftControllerOffset.transform.localPosition.x, 0f, LeftControllerOffset.transform.localPosition.z);
            }
        }

        // IPD Protections
        if (one_time && !ipd_toggle) {
            MainCamera.transform.localScale = new Vector3(ipd_noises[privacy_level] / ipd, 0f, 0f);
        } else {
            MainCamera.transform.localScale = new Vector3(0f, 0f, 0f);
        }
    }
}
