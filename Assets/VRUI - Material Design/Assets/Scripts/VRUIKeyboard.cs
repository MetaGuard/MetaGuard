using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// VRUI Keyboard Component
namespace SpaceBear.VRUI
{
    [ExecuteInEditMode]
    public class VRUIKeyboard : MonoBehaviour
    {
        [SerializeField] GameObject lowercaseKeyboard;
        [SerializeField] GameObject uppercaseKeyboard;
        [SerializeField] GameObject specialKeyboard;
        [SerializeField] bool isUppercase = false;
        [SerializeField] bool isSymbols = false;
        [SerializeField] InputField inputField;

        int inputCaratPos;
        int inputSelectStartPos;
        int inputSelectEndPos;
        bool watchForUnfocus = false;
        Color highlightColor;
        Color clearColor = new Color(0, 0, 0, 0);

        public const string ABC = "abc";
        public const string SYM = "?!#";
        public const string BACK = "BACK";
        public const string DEL = "DEL";
        public const string ENTER = "ENTER";
        public const string UP = "UP";
        public const string LOW = "LOW";

        void Start()
        {
            InitiateKeyboard();
        }
        
        private void Update()
        {
            updateKeyboards();

            // Keep track of the carat position in the inputfield
            if (inputField && inputField.isFocused)
            {
                inputCaratPos = inputField.caretPosition;
                inputSelectStartPos = inputField.selectionAnchorPosition;
                inputSelectEndPos = inputField.selectionFocusPosition;
                watchForUnfocus = true;
            }

            // Reset the position of the carat when focus is lost
            if (watchForUnfocus && inputField && !inputField.isFocused)
            {
                GameObject focusedObject = EventSystem.current.currentSelectedGameObject;

                if (!focusedObject || !focusedObject.transform.IsChildOf(transform))
                {
                    inputCaratPos = inputField.text.Length;
                    inputSelectStartPos = inputCaratPos;
                    inputSelectEndPos = inputCaratPos;
                    watchForUnfocus = false;
                }
            }
        }

        // Update the position of the carat after keypress
        IEnumerator UpdateCarat()
        {

            highlightColor = inputField.selectionColor;
            inputField.selectionColor = clearColor;

            inputField.ActivateInputField();

            yield return new WaitForEndOfFrame();

            inputField.caretPosition = inputCaratPos;
            inputField.selectionAnchorPosition = inputCaratPos;
            inputField.selectionFocusPosition = inputCaratPos;
            inputField.selectionColor = highlightColor;
        }

        // Keypress event handler
        void OnKeyPress (string value)
        {

            if (!inputField) { return; }

            string val = value;

            if (val == "?!#")
            {
                isSymbols = true;
                return;
            }

            if (val == "abc")
            {
                isSymbols = false;
                return;
            }

            if (val == "UP")
            {
                isUppercase = true;
                return;
            }

            if (val == "LOW")
            {
                isUppercase = false;
                return;
            }

            int deleteChars = Mathf.Abs(inputSelectEndPos - inputSelectStartPos);

            if (deleteChars > 0)
            {
                inputCaratPos = Mathf.Min(inputSelectEndPos, inputSelectStartPos);
                inputField.text = inputField.text.Remove(inputCaratPos, deleteChars);
            }

            if (val == "ENTER")
            {
                val = "\n";
            }
            
            if (val == "BACK")
            {
                if (inputField.text.Length > 0 && deleteChars == 0)
                {
                    int removeIndex = Mathf.Max(inputCaratPos - 1, 0);
                    inputField.text = inputField.text.Remove(removeIndex, 1);
                    inputCaratPos = removeIndex;
                }
            } 
            else
            {
                inputField.text = inputField.text.Insert(inputCaratPos, val);
                inputCaratPos = inputCaratPos + val.Length;
            }

            StartCoroutine(UpdateCarat());
        }

        void InitiateKeyboard ()
        {
            foreach (Button btn in gameObject.GetComponentsInChildren<Button>(true))
            {
                btn.onClick.AddListener(delegate { OnKeyPress(btn.GetComponentInChildren<Text>(true).text); });
            }
        }

        // Show the right keyboard based on inputs
        void updateKeyboards ()
        {

            bool shouldUpdate = false;

            if (isSymbols)
            {
                shouldUpdate = !specialKeyboard.activeSelf;
            }
            else if (isUppercase)
            {
                shouldUpdate = !uppercaseKeyboard.activeSelf;
            }
            else
            {
                shouldUpdate = !lowercaseKeyboard.activeSelf;
            } 

            if (shouldUpdate)
            {
                specialKeyboard.SetActive(isSymbols);
                uppercaseKeyboard.SetActive(!isSymbols && isUppercase);
                lowercaseKeyboard.SetActive(!isSymbols && !isUppercase);
                VRUIColorPalette.Instance.UpdateColors();
            }
        }
    }
}