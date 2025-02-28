using System.Text.RegularExpressions;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class PauseText_TokenIconResolver : MonoBehaviour {
    // The UIControls_Handler prefab with the icon registry for each platform
    [SerializeField]
    private GameObject GlobalUIControls;

    // The player to grab the current input device from
    [SerializeField]
    private PlayerInput playerInput;

    // The text lines for this object
    [SerializeField]
    private string textLine;

    // Platform constants
    private const string PC = "pc";
    private const string PS4 = "ps4";
    private const string PS5 = "ps5";
    private const string XBOX = "xbox";

    // Current state
    private TMP_Text tmpText;
    private string currentInputScheme = "MnK";
    private string currentPlatform = PC;
    private Dictionary<string, string> currentTokenIndexmap;
    private TMP_SpriteAsset currentIconAtlas;

    // Awake is called when the script instance is being loaded
    void Awake() {
        InputSystem.onDeviceChange += OnDeviceChange; // Listen for device changes instead of polling every frame
    }

    // Start
    void Start() {
        UpdateCurrentsBasedOnPlatform();
        UpdateText();
    }

    // Update is called once per frame
    void Update() {
        // Check if the current input scheme has changed
        if (playerInput != null && playerInput.currentControlScheme != currentInputScheme) {
            UpdateBasedOnCurrentSchemeAndDevice();
            currentInputScheme = playerInput.currentControlScheme;
        }
    }

    private void UpdateCurrentsBasedOnPlatform() {
        if (GlobalUIControls != null) {
            switch (currentPlatform) {
                case (PC):
                    currentTokenIndexmap = GlobalUIControls.GetComponent<UIControls_Handler>().PC_UI_Controls_IndexMap;
                    currentIconAtlas = GlobalUIControls.GetComponent<UIControls_Handler>().pcUIControlsAtlas;
                    break;

                case (PS4):
                    currentTokenIndexmap = GlobalUIControls.GetComponent<UIControls_Handler>().PS4_UI_Controls_IndexMap;
                    currentIconAtlas = GlobalUIControls.GetComponent<UIControls_Handler>().ps4UIControlsAtlas;
                    break;

                case (PS5):
                    currentTokenIndexmap = GlobalUIControls.GetComponent<UIControls_Handler>().PS5_UI_Controls_IndexMap;
                    currentIconAtlas = GlobalUIControls.GetComponent<UIControls_Handler>().ps5UIControlsAtlas;
                    break;

                case (XBOX):
                    currentTokenIndexmap = GlobalUIControls.GetComponent<UIControls_Handler>().XBOX_UI_Controls_IndexMap;
                    currentIconAtlas = GlobalUIControls.GetComponent<UIControls_Handler>().xboxUIControlsAtlas;
                    break;
            }
        }
    }

    private void UpdateBasedOnCurrentSchemeAndDevice() {
        string newPlatform = currentPlatform;
        if (playerInput.currentControlScheme == "MnK") {
            newPlatform = PC;
        } else if (playerInput.currentControlScheme == "Gamepad") {
            string deviceName = Gamepad.current.name.ToLower();
            if (deviceName.Contains("dualsense")) {
                newPlatform = PS5;
            } else if (deviceName.Contains("xbox")) {
                newPlatform = XBOX;
            } else {
                newPlatform = PS4;
            }
        }
    
        if (newPlatform != currentPlatform) {
            currentPlatform = newPlatform;
            UpdateCurrentsBasedOnPlatform();
            UpdateText(); // Update text only if platform has changed
        }
    }

    //private void UpdatePlatformIcons() {
    //    UIControls_Handler controlsHandler = GlobalUIControls?.GetComponent<UIControls_Handler>();
    //    if (controlsHandler == null) {
    //        Debug.LogError("UIControls_Handler component not found on GlobalUIControls!");
    //        return;
    //    }
    //
    //    switch (currentPlatform) {
    //        case PC:
    //            currentPlatformIcons = controlsHandler.PC_UI_Controls;
    //            break;
    //        case PS4:
    //            currentPlatformIcons = controlsHandler.PS4_UI_Controls;
    //            break;
    //        case PS5:
    //            currentPlatformIcons = controlsHandler.PS5_UI_Controls;
    //            break;
    //        case XBOX:
    //            currentPlatformIcons = controlsHandler.XBOX_UI_Controls;
    //            break;
    //        default:
    //            currentPlatformIcons = null;
    //            Debug.LogWarning("Unsupported platform detected.");
    //            break;
    //    }
    //}

    void UpdateText() {
        // Start with the base text
        StringBuilder sb = new StringBuilder(textLine + "\n");

        string textToDisplay = sb.ToString();

        // If we have a valid controls dictionary, replace tokens with proper sprite tags
        string pattern = @"<[^>]+>"; // Regex pattern to find tokens
        MatchCollection matches = Regex.Matches(textToDisplay, pattern);

        // Foreach match, check if the token is in the currentTokenIndexmap, to get it's index, then get the name for the currentIconAtlas which we use as sprite.
        foreach (Match match in matches) {
            string token = match.Value;
            string tokenName = token.Trim('<', '>');
            if (currentTokenIndexmap.ContainsKey(token)) {
                string spriteId = currentTokenIndexmap[token];
                string imageTag = $"<sprite=\"{currentIconAtlas.name}\" index={spriteId}>";
                sb.Replace(token, imageTag);
            }
        }

        textToDisplay = sb.ToString();

        tmpText.text = textToDisplay;
        tmpText.ForceMeshUpdate();
    }

    //private string DetermineCurrentPlatform() {
    //
    //    // Identify gamepad or keyboard/mouse input based on playerInput
    //    if (playerInput != null) {
    //        string deviceType = playerInput.currentControlScheme.ToLower(); // "MnK" / "Gamepad"
    //        if (deviceType == "mnk") return PC;
    //        if (deviceType == "gamepad") {
    //            string deviceName = Gamepad.current.name.ToLower();
    //            if (deviceName.Contains("dualsense")) return PS5;
    //            if (deviceName.Contains("xbox")) return XBOX;
    //            return PS4; // Default fallback for unknown controllers
    //        }
    //    }
    //
    //    return PC; // Default to PC if no specific device is detected
    //}

    // OnDeviceChange is called when a new input device is added or removed
    private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed) {
            UpdateBasedOnCurrentSchemeAndDevice();
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Ensures text updates when values change in the Unity editor.
    /// Only runs in the editor to avoid unnecessary updates during gameplay.
    /// </summary>
    void OnValidate() {
        if (!Application.isPlaying) return;
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
        UpdateCurrentsBasedOnPlatform();
        UpdateText();
    }
#endif
}
