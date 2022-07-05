using UnityEngine;
using UnityEngine.UI;

// VRUI Button Icon Toggle Component
namespace SpaceBear.VRUI
{
    [ExecuteAlways]
    public class VRUIButtonIconToggle : MonoBehaviour
    {
        public Texture offImage;
        public Texture onImage;
        public bool isOn = false;

        Button button;
        RawImage image;

        void Start()
        {
            button = gameObject.GetComponentInChildren<Button>();
            image = gameObject.GetComponentInChildren<RawImage>();

            button.onClick.AddListener(() => { isOn = !isOn; });
        }

        private void Update()
        {
            image.texture = isOn ? onImage : offImage;
        }

    }
}
