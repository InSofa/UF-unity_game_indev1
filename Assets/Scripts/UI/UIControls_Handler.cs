using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIControls_Handler : MonoBehaviour {

    // Constants
    private const string PC = "pc";
    private const string PS4 = "ps4";
    private const string PS5 = "ps5";
    private const string XBOX = "xbox";
    private const string SCHEME_MnK = "MnK";
    private const string SCHEME_Gamepad = "Gamepad";

    // The player to grab the current input device from
    [SerializeField]
    private PlayerInput playerInput;

    /*
    // Index Maps (Mapping of tokens to sprite-atlas-indexes)
    public Dictionary<string, string> PC_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> PS4_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> PS5_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> XBOX_UI_Controls_IndexMap = new Dictionary<string, string>();
    */
    // Index Maps (Mapping of tokens to sprite-atlas-indexes)
    public Dictionary<string, string> PC_UI_Controls_IndexMap = new Dictionary<string, string> {
        { "<look_around>", "61" },    // Mouse
        { "<move_around>", "92" },    // WASD
        { "<options_button>", "82" }, // ESC
        { "<select_button>", "15" },  // P
        { "<west_button>", "5" },     // F
        { "<north_button>", "" },
        { "<east_button>", "17" },    // R
        { "<south_button>", "" },
        { "<right_button>", "95" },   // Arrow Right
        { "<up_button>", "88" },      // Arrow Up
        { "<left_button>", "94" },    // Arrow Left
        { "<down_button>", "89" },    // Arrow Down
        { "<west_trigger>", "16" },   // Q
        { "<east_trigger>", "4" },    // E

        { "<left_click>", "66" },     // Left Click
        { "<right_click>", "65" },    // Right Click
        { "<mouse>", "61" },          // Mouse
        { "<ctrl>", "81" },           // Control
        { "<shift>", "71" },          // Shift
        { "<dpad>", "93" }            // Arrow Keys
    };
    public Dictionary<string, string> PS4_UI_Controls_IndexMap = new Dictionary<string, string> {
        { "<look_around>", "37" },    // Right Joystick
        { "<move_around>", "29" },    // Left Joystick
        { "<options_button>", "60" }, // Options
        { "<select_button>", "61" },  // Share
        { "<west_button>", "3" },     // Square
        { "<north_button>", "0" },    // Triangle
        { "<east_button>", "1" },     // Circle
        { "<south_button>", "2" },    // Cross
        { "<right_button>", "47" },   // Dpad Right
        { "<up_button>", "44" },      // Dpad Up
        { "<left_button>", "46" },    // Dpad Left
        { "<down_button>", "45" },    // Dpad Up
        { "<west_trigger>", "10" },   // Left Trigger
        { "<east_trigger>", "11" },   // Right Trigger

        { "<dpad>", "48" },           // Arrow Keys
        { "<left_joystick>", "37" },  // Left Joystick
        { "<right_joystick>", "29" }, // Right Joystick

        { "<left_click>", "8" },      // Press Left Boulder (L1)
        { "<right_click>", "9" },     // Press Right Boulder (R1)
        { "<ctrl>", "59" },           // Press Right Joystick (R3)
        { "<shift>", "58" },          // Press Left Joystick (L3)
        { "<dpad_y>", "49" },         // Dpad Y
        { "<dpad_x>", "50" }          // Dpad X
    };
    public Dictionary<string, string> PS5_UI_Controls_IndexMap = new Dictionary<string, string> {
        { "<look_around>", "37" },    // Right Joystick
        { "<move_around>", "29" },    // Left Joystick
        { "<options_button>", "60" }, // Options
        { "<select_button>", "61" },  // Share
        { "<west_button>", "3" },     // Square
        { "<north_button>", "0" },    // Triangle
        { "<east_button>", "1" },     // Circle
        { "<south_button>", "2" },    // Cross
        { "<right_button>", "47" },   // Dpad Right
        { "<up_button>", "44" },      // Dpad Up
        { "<left_button>", "46" },    // Dpad Left
        { "<down_button>", "45" },    // Dpad Up
        { "<west_trigger>", "10" },   // Left Trigger
        { "<east_trigger>", "11" },   // Right Trigger

        { "<dpad>", "48" },           // Arrow Keys
        { "<left_joystick>", "37" },  // Left Joystick
        { "<right_joystick>", "29" }, // Right Joystick
        
        { "<left_click>", "8" },      // Press Left Boulder (L1)
        { "<right_click>", "9" },     // Press Right Boulder (R1)
        { "<ctrl>", "59" },           // Press Right Joystick (R3)
        { "<shift>", "58" },          // Press Left Joystick (L3)
        { "<dpad_y>", "49" },         // Dpad Y
        { "<dpad_x>", "50" }          // Dpad X
    };
    public Dictionary<string, string> XBOX_UI_Controls_IndexMap = new Dictionary<string, string> {
        { "<look_around>", "26" },    // Right JoyStick
        { "<move_around>", "18" },    // Left JoyStick
        { "<options_button>", "44" }, // Menu
        { "<select_button>", "45" },  // View
        { "<west_button>", "3" },     // X
        { "<north_button>", "0" },    // Y
        { "<east_button>", "1" },     // B
        { "<south_button>", "2" },    // A
        { "<right_button>", "12" },   // Dpad Right
        { "<up_button>", "14" },      // Dpad Up
        { "<left_button>", "13" },    // Dpad Left
        { "<down_button>", "15" },    // Dpad Down
        { "<west_trigger>", "10" },   // Left Trigger
        { "<east_trigger>", "11" },   // Right Trigger

        { "<dpad>", "34" },           // Arrow Keys
        { "<left_joystick>", "26" },  // Left Joystick
        { "<right_joystick>", "18" }, // Right Joystick

        { "<left_click>", "8" },      // Press Left Boulder (L1)
        { "<right_click>", "9" },     // Press Right Boulder (R1)
        { "<ctrl>", "24" },           // Press Right Joystick (R3)
        { "<shift>", "16" },          // Press Left Joystick (L3)
        { "<dpad_y>", "35" },         // Dpad Y
        { "<dpad_x>", "33" }          // Dpad X
    };

    // Reference the sprite-atlas for each platform
    [SerializeField]
    public TMP_SpriteAsset pcUIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset ps4UIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset ps5UIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset xboxUIControlsAtlas;

    [Space]
    [SerializeField]
    EventSystem eventSystem;

    // Current state variables
    [System.NonSerialized]
    public string currentInputScheme = SCHEME_MnK;
    [System.NonSerialized]
    public string currentPlatform = PC;
    [System.NonSerialized]
    public Dictionary<string, string> currentTokenIndexmap;
    [System.NonSerialized]
    public TMP_SpriteAsset currentIconAtlas;

    // Singleton pattern deffinitions
    private static UIControls_Handler _instance;
    public static UIControls_Handler Instance { get { return _instance; } }
    // Singelton pattern logic
    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        // Update the currents
        currentTokenIndexmap = PC_UI_Controls_IndexMap;
        currentIconAtlas = pcUIControlsAtlas;

        // Link listener for deviceChanges
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    // Start is called before the first frame update
    void Start() {
        UpdateCurrentMaps();
    }

    // Update is called once per frame
    void Update() {
        // Check if the current input scheme has changed
        if (playerInput != null && playerInput.currentControlScheme != currentInputScheme) {
            UpdateCurrentPlatform();
            currentInputScheme = playerInput.currentControlScheme;
        }
    }

    // Listener for device changes
    private void OnDeviceChange(InputDevice device, InputDeviceChange change) {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed) {
            UpdateCurrentPlatform();
        }
    }

    // Function to update current platform
    private void UpdateCurrentPlatform() {
        string newPlatform = currentPlatform;
        if (playerInput.currentControlScheme == SCHEME_MnK) {
            newPlatform = PC;
        } else if (playerInput.currentControlScheme == SCHEME_Gamepad) {
            //Resets the selected object to the first selected object in the event system
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);

            string deviceName = Gamepad.current.name.ToLower();
            Debug.Log("GamePad DeviceName: " + deviceName);
            if (deviceName.Contains("dualsense")) {
                newPlatform = PS5;
            } else if (deviceName.Contains("xbox") || deviceName.Contains("xinputcontrollerwindows")) {
                newPlatform = XBOX;
            } else {
                newPlatform = PS4;
            }
        }

        if (newPlatform != currentPlatform) {
            currentPlatform = newPlatform;
            UpdateCurrentMaps();
        }
    }

    // Function to update current sprite-atlas and indexmap based on platform
    private void UpdateCurrentMaps() {
        switch (currentPlatform) {
            case (PC):
                currentTokenIndexmap = PC_UI_Controls_IndexMap;
                currentIconAtlas = pcUIControlsAtlas;
                break;
            case (PS4):
                currentTokenIndexmap = PS4_UI_Controls_IndexMap;
                currentIconAtlas = ps4UIControlsAtlas;
                break;
            case (PS5):
                currentTokenIndexmap = PS5_UI_Controls_IndexMap;
                currentIconAtlas = ps5UIControlsAtlas;
                break;
            case (XBOX):
                currentTokenIndexmap = XBOX_UI_Controls_IndexMap;
                currentIconAtlas = xboxUIControlsAtlas;
                break;
        }
    }
}