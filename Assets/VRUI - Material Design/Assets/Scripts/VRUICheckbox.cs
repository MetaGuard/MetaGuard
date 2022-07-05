using UnityEngine;
using UnityEngine.UI;

// VRUI Checkbox Component
namespace SpaceBear.VRUI
{
	public class VRUICheckbox : UnityEngine.UI.Toggle
	{

		Image checkedBG;
		Image checkmark;

		Color accentColor;

		protected override void Awake()
		{

			// Override base awake to implement color assignment on wakeup and value change

			base.Awake();

			this.onValueChanged.AddListener(delegate
			{

				SetCheckmarkColor();

			});
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SetCheckmarkColor();
		}

		public void setColors(Color a)
		{

			// Initialize accent color from GUI

			accentColor = a;

			SetCheckmarkColor();
		}

		void SetCheckmarkColor()
		{
			// Assign color to the checkmark and show/hide based on isOn state

			if (checkmark == null)
			{
				checkmark = transform.Find("Background/Checked/Checkmark").GetComponent<Image>();
			}

			if (checkedBG == null)
			{
				checkedBG = transform.Find("Background/Checked").GetComponent<Image>();

			}

			checkedBG.color = accentColor;

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{

				checkmark.canvasRenderer.SetAlpha(this.isOn ? 1f : 0f);

				checkedBG.canvasRenderer.SetAlpha(this.isOn ? 1f : 0f);

			}
			else
#endif
				checkmark.CrossFadeAlpha(this.isOn ? 1f : 0f, 0f, true);

		}

	}
}
