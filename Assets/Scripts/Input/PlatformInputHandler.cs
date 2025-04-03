using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlatformInputHandler : MonoBehaviour {
    private static PlatformInputHandler instance;
    public static PlatformInputHandler Instance { get { return instance; } }

    //References
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private UIControls_Handler uiControlsHandler;

    // Constants
    public const string PC = "pc";
    public const string PS4 = "ps4";
    public const string PS5 = "ps5";
    public const string XBOX = "xbox";

    public const string SCHEME_MnK = "MnK";
    public const string SCHEME_Gamepad = "Gamepad";


    // Current state variables
    [System.NonSerialized]
    public string currentInputScheme = SCHEME_MnK;
    [System.NonSerialized]
    public string currentPlatform = PC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        currentInputScheme = playerInput.currentControlScheme;

        // Updates the values for the uiControlsHandler as we're storing those value here and want to avoid double storage
        uiControlsHandler.PC = PC;
        uiControlsHandler.PS4 = PS4;
        uiControlsHandler.PS5 = PS5;
        uiControlsHandler.XBOX = XBOX;

        // Link listener for deviceChanges
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void Update() {
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
            uiControlsHandler.UpdateCurrentMaps(currentPlatform);
        }
    }
}
