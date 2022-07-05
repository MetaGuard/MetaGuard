using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// VRUI Pagination Component
namespace SpaceBear.VRUI
{
    public class VRUIPagination : MonoBehaviour
    {

        [SerializeField] GameObject leftButton;
        [SerializeField] GameObject rightButton;
        [SerializeField] GameObject content;
        [SerializeField] GameObject carousel;
        [SerializeField] GameObject pageIndicator;

        RectTransform contentRectTransform;
        ScrollRect scrollRect;
        List<VRUIRadio> radioBtns = new List<VRUIRadio>();
        HorizontalLayoutGroup contentLayoutGroup;
        ToggleGroup group;

        float carouselWidth;
        float contentWidth;
        float desiredPosition;

        int pages = 0;
        int currentIndex = 0;
        bool isPagingTo = false;
        bool updateRadio = true;

        void Start()
        {

            // Initialize values and create pagination objects

            group = gameObject.GetComponent<ToggleGroup>();

            carouselWidth = carousel.GetComponent<RectTransform>().rect.width;

            scrollRect = carousel.GetComponent<ScrollRect>();

            contentRectTransform = content.GetComponent<RectTransform>();

            contentLayoutGroup = content.GetComponent<HorizontalLayoutGroup>();

            scrollRect.onValueChanged.AddListener((Vector2 v) => UpdatePositionIndex(scrollRect.horizontalNormalizedPosition));

        }

        void Update()
        {

            //If pagination is activated scroll to the desired position

            if (contentWidth != contentRectTransform.rect.width)
            {
                contentWidth = contentRectTransform.rect.width;

                StartCoroutine(CreatePagination());
            }

            if (isPagingTo)
            {

                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, desiredPosition, Time.deltaTime * 5f);

                if (MyApproximately(scrollRect.horizontalNormalizedPosition, desiredPosition, 0.0005f))
                {

                    scrollRect.horizontalNormalizedPosition = desiredPosition;

                    isPagingTo = false;

                }
            }
        }

        // Radio button onClick event
        void PageTo(Transform radio)
        {
            updateRadio = false;
            ScrollToPage(radio.GetSiblingIndex());

        }

        void UpdatePositionIndex(float pos)
        {

            // Update the highlighted radio button

            EnableButton(leftButton, pos >= 0.01f);

            EnableButton(rightButton, pos <= 0.99f);

            currentIndex = Mathf.FloorToInt(pos * contentRectTransform.rect.width / carouselWidth);

            int index = Mathf.Max(Mathf.Min(currentIndex, pages - 1), 0);

            if (updateRadio && radioBtns[index])
            {
               radioBtns[index].isOn = true;
            }

        }

        private bool MyApproximately(float a, float b, float c)
        {
            return Mathf.Abs(a - b) < c;
        }

        /* Calculate the size of the carousel and the number of pages, 
         * and create a radio button for each page.
         * */
        IEnumerator CreatePagination()
        {
            content.GetComponent<ContentSizeFitter>().enabled = false;
            content.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
            content.GetComponent<ContentSizeFitter>().enabled = true;

            yield return null;

            pages = Mathf.CeilToInt(contentWidth / carouselWidth);

            radioBtns.Clear();

            if (pages > 1)
            {

                foreach (VRUIRadio btn in gameObject.GetComponentsInChildren<VRUIRadio>())
                {
                    Destroy(btn.gameObject);
                }

                for (int i = 0; i < pages; i++)
                {

                    VRUIRadio radio = Instantiate(pageIndicator, transform).GetComponent<VRUIRadio>();

                    radio.onPointerClick.AddListener((b) => PageTo(radio.transform));

                    radio.group = group;

                    radioBtns.Add(radio);

                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());

                group.SetAllTogglesOff();

                gameObject.SetActive(true);

                UpdatePositionIndex(scrollRect.horizontalNormalizedPosition);
            }
            else
            {
                gameObject.SetActive(false);
                leftButton.transform.parent.gameObject.SetActive(false);
                rightButton.transform.parent.gameObject.SetActive(false);
            }

            VRUIColorPalette.Instance.UpdateColors();

        }

        void EnableButton(GameObject btn, bool enable)
        {
            // Enable/disable left/right buttons
            if (enable)
            {
                btn.GetComponent<Button>().interactable = true;
                btn.GetComponent<CanvasGroup>().alpha = 1f;

            }
            else
            {
                btn.GetComponent<Button>().interactable = false;
                btn.GetComponent<CanvasGroup>().alpha = 0.3f;
            }
        }

        // Left and right buttons onClick events
        public void PageLeft()
        {
            updateRadio = true;
            ScrollToPage(--currentIndex);
        }

        public void PageRight()
        {
            updateRadio = true;
            ScrollToPage(++currentIndex);
        }

        // Calculate the desired pagination position and enable scrolling to the desired position
        void ScrollToPage(int index)
        {
            currentIndex = Mathf.Max(0, Mathf.Min(pages - 1, index));

            float pagePosition = currentIndex * (carouselWidth - (contentLayoutGroup.spacing - contentLayoutGroup.padding.left));

            float pagePositionNormalized = pagePosition / (contentRectTransform.rect.width - carouselWidth);

            desiredPosition = Mathf.Min(pagePositionNormalized, 1f);

            isPagingTo = true;
        }

        // Stop pagination if user drag on the carousel
        public void OnCarouselDrag()
        {
            updateRadio = true;
            isPagingTo = false;
        }
    }
}
