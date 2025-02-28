using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// TextDisplay is a Unity script that dynamically updates a TMP_Text component based on the current input platform.
/// It replaces placeholder tokens (e.g., <move_around>) with platform-specific control icons.
/// The script also conditionally displays extra text based on debug toggles.
/// 
/// ## Functionality:
/// - Detects input devices (keyboard, mouse, gamepads) and determines the platform.
/// - Uses predefined dictionaries to replace tokens with platform-specific icons.
/// - Efficiently updates the text when input devices change using InputSystem.onDeviceChange.
/// 
/// ## Reasoning Behind Design Choices:
/// - **Constants for Readability:** Platform names (e.g., "pc", "ps4") are stored as constants to prevent typos.
/// - **Efficient String Handling:** Uses `StringBuilder` instead of string concatenation to optimize text processing.
/// - **Optimized Input Detection:** Instead of checking every frame (`Update`), we use `InputSystem.onDeviceChange` for better performance.
/// - **`OnValidate` Runs Only in Editor:** Prevents unnecessary text updates outside of play mode.
/// 
/// ## Usage:
/// - Attach this script to a `TMP_Text` GameObject in Unity.
/// - Set the general text and conditional texts in the inspector.
/// - Add token placeholders like `<move_around>` in the text. The script will replace them based on the platform.
/// 
/// ## Potential Issues:
/// ### **Sprite Asset Requirement**
/// The current implementation attempts to insert images using `<sprite name="imagePath">`, but TextMeshPro does not support direct file paths.
/// Instead, it requires a **TMP Sprite Asset**, which must be created and assigned manually.
/// 
/// **How to Fix:**
/// 1. Convert your icons to a **TMP Sprite Asset**:
///    - Open **Window → TextMeshPro → Sprite Importer**.
///    - Create a new `TMP_SpriteAsset` with your control icons.
///    - Assign the sprite asset to the `TMP_Text` component.
/// 
/// 2. Modify the script to use sprite names instead of file paths:
///    - Update the dictionary to store **sprite names**, not file paths:
///      ```csharp
///      platformToTokens[PC] = new Dictionary<string, string>()
///      {
///          { "<move_around>", "move_around" },
///          { "<look_around>", "look_around" },
///      };
///      ```
/// 
///    - Change how the replacement is handled:
///      ```csharp
///      string imageTag = $"<sprite name=\"{platformToTokens[currentPlatform][token]}\">";
///      ```
/// 
/// 3. Ensure the `TMP_Text` component has the correct **Sprite Asset** assigned in the Unity Inspector.
/// 
/// Without these adjustments, the images will not render correctly in the TextMeshPro UI.
/// </summary>

public class PsudoExample_TokenIconResolver : MonoBehaviour {
    // Constants for platform names to avoid hardcoded strings and typos
    private const string PC = "pc";
    private const string PS4 = "ps4";
    private const string PS5 = "ps5";
    private const string XBOX = "xbox";

    [SerializeField] private string generalText = "General Text"; // Main text that is always displayed
    [SerializeField] private string conditionalText1 = "Conditional Text 1"; // Optional debug text 1
    [SerializeField] private string conditionalText2 = "Conditional Text 2"; // Optional debug text 2
    [SerializeField] private bool debug_condition1 = false; // Enable/disable conditionalText1
    [SerializeField] private bool debug_condition2 = false; // Enable/disable conditionalText2
    [SerializeField] private float tokenIconWidth = 20f; // Width of control icons
    [SerializeField] private float tokenIconHeight = 20f; // Height of control icons

    private TMP_Text tmpText; // Reference to the TextMeshPro component

    // Dictionary mapping platform names to another dictionary of control tokens
    private readonly Dictionary<string, Dictionary<string, string>> platformToTokens =
        new Dictionary<string, Dictionary<string, string>>();

    private string currentPlatform = PC; // Default to PC if no input is detected

    void Awake() {
        tmpText = GetComponent<TMP_Text>(); // Ensure TextMeshPro component is assigned early
        InitializePlatformTokens();
        InputSystem.onDeviceChange += OnDeviceChange; // Listen for device changes instead of polling every frame
    }

    void Start() {
        currentPlatform = DetermineCurrentPlatform(); // Detect platform at start
        UpdateText(); // Apply initial text update
    }

    void OnDestroy() {
        InputSystem.onDeviceChange -= OnDeviceChange; // Cleanup event listener on destroy
    }

    /// <summary>
    /// Initializes the dictionary that maps platforms to their specific control icon tokens.
    /// These mappings can be changed in the future to reflect different button icons per platform.
    /// </summary>
    private void InitializePlatformTokens() {
        platformToTokens[PC] = new Dictionary<string, string>()
        {
            { "<move_around>", "/assets/Visuals/UIControls/JulioCackoIcons/default/move_around.png" },
            { "<look_around>", "/assets/Visuals/UIControls/JulioCackoIcons/default/look_around.png" },
        };

        platformToTokens[PS4] = new Dictionary<string, string>()
        {
            { "<move_around>", "/assets/Visuals/UIControls/JulioCackoIcons/default/move_around.png" },
            { "<look_around>", "/assets/Visuals/UIControls/JulioCackoIcons/default/look_around.png" },
        };

        platformToTokens[PS5] = new Dictionary<string, string>()
        {
            { "<move_around>", "/assets/Visuals/UIControls/JulioCackoIcons/default/move_around.png" },
            { "<look_around>", "/assets/Visuals/UIControls/JulioCackoIcons/default/look_around.png" },
        };

        platformToTokens[XBOX] = new Dictionary<string, string>()
        {
            { "<move_around>", "/assets/Visuals/UIControls/JulioCackoIcons/default/move_around.png" },
            { "<look_around>", "/assets/Visuals/UIControls/JulioCackoIcons/default/look_around.png" },
        };
    }

    /// <summary>
    /// Event handler that updates the platform when a new input device is added or removed.
    /// </summary>
    private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed) {
            string newPlatform = DetermineCurrentPlatform();
            if (newPlatform != currentPlatform) {
                currentPlatform = newPlatform;
                UpdateText(); // Update text only if platform has changed
            }
        }
    }

    /// <summary>
    /// Determines the current platform based on connected input devices.
    /// Gamepad detection uses name-based matching for "dualshock", "dualsense", and "xbox".
    /// </summary>
    private string DetermineCurrentPlatform() {
        if (Keyboard.current != null || Mouse.current != null) {
            return PC; // Assume PC if keyboard or mouse is detected
        }

        if (Gamepad.current != null) {
            string deviceName = Gamepad.current.name.ToLower();
            if (deviceName.Contains("dualshock")) return PS4;
            if (deviceName.Contains("dualsense")) return PS5;
            if (deviceName.Contains("xbox")) return XBOX;
            return XBOX; // Default fallback for unknown controllers
        }

        return PC; // Default to PC if no specific device is detected
    }

    /// <summary>
    /// Updates the displayed text, replacing tokens with platform-specific icons.
    /// Uses StringBuilder for efficient string modification.
    /// </summary>
    void UpdateText() {
        StringBuilder sb = new StringBuilder(generalText + "\n");

        if (debug_condition1) sb.Append(conditionalText1 + "\n");
        if (debug_condition2) sb.Append(conditionalText2 + "\n");

        string textToDisplay = sb.ToString();

        if (platformToTokens.ContainsKey(currentPlatform)) {
            string pattern = @"<[^>]+>"; // Regex pattern to find tokens
            MatchCollection matches = Regex.Matches(textToDisplay, pattern);

            foreach (Match match in matches) {
                string token = match.Value;
                if (platformToTokens[currentPlatform].ContainsKey(token)) {
                    string imagePath = platformToTokens[currentPlatform][token];
                    string imageTag = $"<sprite name=\"{imagePath}\" width={tokenIconWidth} height={tokenIconHeight} align=middle>";
                    sb.Replace(token, imageTag);
                }
            }
            textToDisplay = sb.ToString();
        } else {
            textToDisplay += "Platform not supported.\n";
        }

        tmpText.text = textToDisplay;
        tmpText.ForceMeshUpdate();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Ensures text updates when values change in the Unity editor.
    /// Only runs in the editor to avoid unnecessary updates during gameplay.
    /// </summary>
    void OnValidate() {
        if (!Application.isPlaying) return;
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
        UpdateText();
    }
#endif
}
