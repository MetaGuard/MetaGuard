using UnityEngine;

// A simple manager for the SampleScene
namespace SpaceBear.VRUI
{
	public class SampleSceneManager : MonoBehaviour
	{

		public VRUIColorPalette pallete;

		public Color color1a;
		public Color color1b;
		public Color color1c;

		public Color color2a;
		public Color color2b;
		public Color color2c;

		public Color color3a;
		public Color color3b;
		public Color color3c;

		public Color color4a;
		public Color color4b;
		public Color color4c;

		// Use this to goggle Darktheme and Lighttheme

		public void ToggleDarkMode(bool b)
		{

			pallete.isDarkTheme = b;
			pallete.UpdateColors();

		}

		// Use this to change to color scheme 1

		public void ChangeColorTheme1(bool b)
		{
			pallete.accentColor = color1a;
			pallete.hoverColor = color1b;
			pallete.pressedColor = color1c;
			pallete.UpdateColors();
		}

		// Use this to change to color scheme 2

		public void ChangeColorTheme2(bool b)
		{
			pallete.accentColor = color2a;
			pallete.hoverColor = color2b;
			pallete.pressedColor = color2c;
			pallete.UpdateColors();
		}

		// Use this to change to color scheme 3

		public void ChangeColorTheme3(bool b)
		{
			pallete.accentColor = color3a;
			pallete.hoverColor = color3b;
			pallete.pressedColor = color3c;
			pallete.UpdateColors();
		}

		// Use this to change to color scheme 4

		public void ChangeColorTheme4(bool b)
		{
			pallete.accentColor = color4a;
			pallete.hoverColor = color4b;
			pallete.pressedColor = color4c;
			pallete.UpdateColors();
		}

	}
}
