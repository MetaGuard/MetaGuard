using UnityEngine;
using UnityEditor;

namespace SpaceBear.VRUI
{

	// Custom Editor for assigning colors and themes to the UI
	[InitializeOnLoad]
	[ExecuteAlways]
	[CustomEditor(typeof(VRUIColorPalette))]
	[CanEditMultipleObjects]
	public class VRUIColorPaletteEditor : Editor
	{
		SerializedProperty isDarkTheme;
		SerializedProperty accentColor;
		SerializedProperty hoverColor;
		SerializedProperty pressedColor;
		SerializedProperty disabledColor;

		void OnEnable()
		{
			// Find all the relevant properties from the GUI

			isDarkTheme = serializedObject.FindProperty("isDarkTheme");
			accentColor = serializedObject.FindProperty("accentColor");
			hoverColor = serializedObject.FindProperty("hoverColor");
			pressedColor = serializedObject.FindProperty("pressedColor");
			disabledColor = serializedObject.FindProperty("disabledColor");

			// Add all the necessary tags

			TagsAndLayers.AddTag("VRUIBackground");
			TagsAndLayers.AddTag("VRUIText");
			TagsAndLayers.AddTag("VRUIIcon");
			TagsAndLayers.AddTag("VRUIOutline");
			TagsAndLayers.AddTag("VRUIButton");
			TagsAndLayers.AddTag("VRUIButtonTab");
			TagsAndLayers.AddTag("VRUIButtonControlBar");
			TagsAndLayers.AddTag("VRUIButtonList");
			TagsAndLayers.AddTag("VRUICheckbox");
			TagsAndLayers.AddTag("VRUIToggle");
			TagsAndLayers.AddTag("VRUIRadio");
			TagsAndLayers.AddTag("VRUISlider");
			TagsAndLayers.AddTag("VRUIButtonIcon");
			TagsAndLayers.AddTag("VRUIAccent");
		}

		public override void OnInspectorGUI()
		{

			// Create labels and fields for inputing of properties

			EditorGUILayout.PropertyField(isDarkTheme);

			EditorGUILayout.LabelField("Color Palette");

			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(accentColor);

			EditorGUILayout.LabelField("Button State Colors");

			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(hoverColor);

			EditorGUILayout.PropertyField(pressedColor);

			EditorGUILayout.PropertyField(disabledColor);

			// Apply input properties and call script to update colors

			serializedObject.ApplyModifiedProperties();

			VRUIColorPalette colorPalette = (VRUIColorPalette)target;

			colorPalette.UpdateColors();

		}

	}
}