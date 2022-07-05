using UnityEngine;
using UnityEngine.UI;

namespace SpaceBear.VRUI
{
	public class VRUIControlBarButton : UnityEngine.UI.Button
	{

		RawImage icon;
		Text text;

		Color accentColor = Color.white;
		Color textColor = Color.white;

		public void setColors(Color a, Color t)
		{

			// Initialize colors from the GUI

			accentColor = a;

			textColor = t;

			Color contentColor = this.currentSelectionState == Selectable.SelectionState.Highlighted || this.currentSelectionState == Selectable.SelectionState.Pressed ? accentColor : textColor;

			if (icon == null)
			{
				icon = transform.Find("Icon").GetComponent<RawImage>();
			}

			if (text == null)
			{
				text = GetComponentInChildren<Text>();
			}

			icon.CrossFadeColor(contentColor, 0f, true, true);

			if (text)
				text.CrossFadeColor(contentColor, 0f, true, true);
		}

		// Override base transition method to add the coloring of the text and icon as part of the transition effect

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			Color color;
			switch (state)
			{
				case Selectable.SelectionState.Normal:
					color = this.colors.normalColor;
					break;
				case Selectable.SelectionState.Highlighted:
					color = this.colors.highlightedColor;
					break;
				case Selectable.SelectionState.Pressed:
					color = this.colors.pressedColor;
					break;
				case Selectable.SelectionState.Disabled:
					color = this.colors.disabledColor;
					break;
				default:
					color = Color.black;
					break;
			}
			if (base.gameObject.activeInHierarchy)
			{

				switch (this.transition)
				{
					case Selectable.Transition.ColorTint:

						Color targetColor = color * this.colors.colorMultiplier;

						Color contentColor = state == Selectable.SelectionState.Highlighted || state == Selectable.SelectionState.Pressed ? accentColor : textColor;

						ColorTween(targetColor, instant, contentColor);

						break;
				}
			}
		}

		private void ColorTween(Color targetColor, bool instant, Color contentColor)
		{
			if (this.targetGraphic == null)
			{
				this.targetGraphic = this.image;
			}

			if (icon == null)
			{
				icon = transform.Find("Icon").GetComponent<RawImage>();
			}

			if (text == null)
			{
				text = GetComponentInChildren<Text>();
			}

			base.image.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
			icon.CrossFadeColor(contentColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);

			if (text)
				text.CrossFadeColor(contentColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);

		}
	}
}
