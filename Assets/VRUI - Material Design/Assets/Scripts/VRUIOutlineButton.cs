using UnityEngine;
using UnityEngine.UI;

// VRUI Outline Button Component
namespace Spacebear.VRUI
{
	public class VRUIOutlineButton : UnityEngine.UI.Button
	{

		Text text;

		// Override base transition method to add the coloring of the text as part of the transition effect 

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
						ColorTween(color * this.colors.colorMultiplier, instant);
						break;
				}
			}
		}

		private void ColorTween(Color targetColor, bool instant)
		{
			if (this.targetGraphic == null)
			{
				this.targetGraphic = this.image;
			}
			if (text == null)
			{
				text = GetComponentInChildren<Text>();
			}

			base.image.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
			text.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);

		}
	}
}
