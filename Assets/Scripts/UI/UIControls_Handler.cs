using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    // Index Maps (Mapping of tokens to sprite-atlas-indexes)
    public Dictionary<string, string> PC_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> PS4_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> PS5_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> XBOX_UI_Controls_IndexMap = new Dictionary<string, string>();

    // Reference the sprite-atlas for each platform
    [SerializeField]
    public TMP_SpriteAsset pcUIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset ps4UIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset ps5UIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset xboxUIControlsAtlas;

    // Reference DictionarySerializers for sprite-atlas-index mappings per platform
    [SerializeField]
    public String_DictionarySerializer pcUIControlsIndexSerializer;
    [SerializeField]
    public String_DictionarySerializer ps4UIControlsIndexSerializer;
    [SerializeField]
    public String_DictionarySerializer ps5UIControlsIndexSerializer;
    [SerializeField]
    public String_DictionarySerializer xboxUIControlsIndexSerializer;

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

        // Link the dictionary serializers to the dictionaries
        pcUIControlsIndexSerializer.LinkDictionary(PC_UI_Controls_IndexMap);
        ps4UIControlsIndexSerializer.LinkDictionary(PS4_UI_Controls_IndexMap);
        ps5UIControlsIndexSerializer.LinkDictionary(PS5_UI_Controls_IndexMap);
        xboxUIControlsIndexSerializer.LinkDictionary(XBOX_UI_Controls_IndexMap);

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