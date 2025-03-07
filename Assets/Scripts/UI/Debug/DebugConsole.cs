using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour {
    public TMP_InputField inputField;
    public TMP_Text outputText;
    public ScrollRect scrollRect;
    public UIHandler uIHandler;

    public bool inputIsFocused = false;

    // Singleton pattern deffinitions
    private static DebugConsole _instance;
    public static DebugConsole Instance { get { return _instance; } }
    // Singelton pattern logic
    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        // Ensure the input field is assigned
        if (inputField != null) {
            inputField.onSubmit.AddListener(HandleInput);
        }
    }

    public void HandleInput(string input) {
        if (string.IsNullOrWhiteSpace(input)) return;

        // Append input to the output text
        AppendToConsole("> " + input);

        // Example: If the input is "https://example.com", make it clickable
        string processedInput = ConvertUrlsToClickable(input);
        AppendToConsole(processedInput);

        // Clear input field
        inputField.text = "";
        inputField.ActivateInputField();
    }

    void AppendToConsole(string message) {
        outputText.text += message + "\n";
        Canvas.ForceUpdateCanvases();
        if (scrollRect != null) {
            //scrollRect.scrollOffset = scrollRect.contentContainer.layout.max - scrollRect.contentViewport.layout.size;
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    string ConvertUrlsToClickable(string text) {
        string pattern = @"(http[s]?://\S+)";
        return Regex.Replace(text, pattern, "<link=\"$1\"><color=blue><u>$1</u></color></link>");
    }

    public void OnFocused() {
        inputIsFocused = true;
    }

    public void OnFocusLost() {
        inputIsFocused = false;
    }
}