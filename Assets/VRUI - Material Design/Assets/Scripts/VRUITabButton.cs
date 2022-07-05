using UnityEngine;
using UnityEngine.UI;

// VRUI Tab Component
namespace SpaceBear.VRUI
{
	public class VRUITabButton : UnityEngine.UI.Button
	{

		Image underline;

		Color accentColor = Color.white;

		public void setColors(Color a)
		{

			// Set accent color for the underline based select states 

			accentColor = a;

			Color underlineColor;

			if (this.currentSelectionState == Selectable.SelectionState.Highlighted || this.currentSelectionState == Selectable.SelectionState.Pressed)
			{

				underlineColor = accentColor;

			}
			else
			{

				underlineColor = new Color(0f, 0f, 0f, 0f);

			}

			if (underline == null)
			{

				underline = transform.Find("Underline").GetComponent<Image>();
			}

			underline.CrossFadeColor(underlineColor, 0f, true, true);
		}

		// Override base transition method to implement color change for the underline

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

						Color underlineColor = state == Selectable.SelectionState.Highlighted || state == Selectable.SelectionState.Pressed ? accentColor : targetColor;

						ColorTween(targetColor, instant, underlineColor);

						break;
				}
			}
		}

		private void ColorTween(Color targetColor, bool instant, Color underlineColor)
		{
			if (this.targetGraphic == null)
			{
				this.targetGraphic = this.image;
			}
			if (underline == null)
			{
				underline = transform.Find("Underline").GetComponent<Image>();
			}

			base.image.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);

			underline.CrossFadeColor(underlineColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
		}
	}
}
