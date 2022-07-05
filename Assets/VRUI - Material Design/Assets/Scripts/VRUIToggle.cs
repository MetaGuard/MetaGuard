using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// VRUI Toggle Component
namespace SpaceBear.VRUI
{
	public class VRUIToggle : UnityEngine.UI.Toggle
	{

		Image handleImg;
		RectTransform handleTransform;

		protected override void Awake()
		{

			// Override base awake to implement color assignment on wakeup and value change

			base.Awake();

			this.onValueChanged.AddListener(delegate
			{

				SetHandleColor();

			});

			SetHandleColor();
		}

		void Update()
		{

			// Move toggle's handle position base on isOn state

			if (handleTransform == null)
			{
				handleTransform = transform.Find("Background/Handle").GetComponent<RectTransform>();
			}

			Vector3 handlePosition = new Vector3(this.isOn ? 16f : 8f, 0f, 0f);

			handleTransform.localPosition = Vector3.Lerp(handleTransform.localPosition, handlePosition, 0.2f);
		}

		// Set color of toggle's handle based on isOn state

		void SetHandleColor()
		{

			if (handleImg == null)
			{
				handleImg = transform.Find("Background/Handle").GetComponent<Image>();
			}

			handleImg.CrossFadeColor(this.isOn ? this.colors.normalColor : this.colors.disabledColor, 0f, true, true);

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{

				if (handleTransform == null)
				{
					handleTransform = transform.Find("Background/Handle").GetComponent<RectTransform>();
				}

				handleTransform.localPosition = new Vector3(this.isOn ? 16f : 8f, 0f, 0f);

			}
#endif
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			// Override base transition method as it doesn't apply, and assign state color manually

			SetHandleColor();

		}
	}
}
