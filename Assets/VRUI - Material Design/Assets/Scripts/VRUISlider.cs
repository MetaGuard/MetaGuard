using UnityEngine;
using UnityEngine.UI;

// VRUI Slider Component
namespace SpaceBear.VRUI
{
	public class VRUISlider : UnityEngine.UI.Slider
	{

		Image fill;
		Image handle;

		public void setColors(Color a)
		{

			// Set accent color for the fill and handle of the slider

			if (fill == null)
			{
				fill = transform.Find("Fill Area/Fill").GetComponent<Image>();
			}

			if (handle == null)
			{
				handle = transform.Find("Handle Slide Area/Handle").GetComponent<Image>();

			}

			fill.color = a;

			handle.color = a;
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			// Override base transition state because it is not needed
		}

	}
}
