using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour {
    private static InputHandler instance;
    public static InputHandler Instance { get { return instance; } }

    //References
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private UIControls_Handler uiControlsHandler;

    [SerializeField]
    EventSystem eventSystem;

    [SerializeField]
    PlayerHand playerHand;

    [SerializeField]
    PlayerController playerController;

    // Constants
    private const string PC = "pc";
    private const string PS4 = "ps4";
    private const string PS5 = "ps5";
    private const string XBOX = "xbox";

    private const string SCHEME_MnK = "MnK";
    private const string SCHEME_Gamepad = "Gamepad";


    // Current state variables
    [System.NonSerialized]
    public string currentInputScheme = SCHEME_MnK;
    [System.NonSerialized]
    public string currentPlatform = PC;

    [Header("Input actions")]
    [SerializeField]
    private InputActionReference playerUse;

    [SerializeField]
    private InputActionReference playerSecondaryUse;

    [SerializeField]
    private InputActionReference playerMove;

    //Could be both the joystick and mouse
    [SerializeField]
    private InputActionReference playerCursorInput;

    [SerializeField]
    private InputActionReference playerMeleeSwitch;


    #region Private Variables
    private bool isHoveringButton = false;

    private bool playing = true;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        eventSystem.IsPointerOverGameObject();

        currentInputScheme = playerInput.currentControlScheme;

        // Updates the values for the uiControlsHandler as we're storing those value here and want to avoid double storage
        uiControlsHandler.PC = PC;
        uiControlsHandler.PS4 = PS4;
        uiControlsHandler.PS5 = PS5;
        uiControlsHandler.XBOX = XBOX;

        // Link listener for deviceChanges
        InputSystem.onDeviceChange += OnDeviceChange;

        playerUse.action.started += PlayerUseHandler;
        playerSecondaryUse.action.started += playerHand.removeBuilding;
        playerMeleeSwitch.action.started += SwitchMeleeModeHandler;
    }

    //Should be called after input is taken, hopefully (logic to avoid double inputs are here)
    private void Update() {
        // Check if the current input scheme has changed
        if (playerInput != null && playerInput.currentControlScheme != currentInputScheme) {
            UpdateCurrentPlatform();
            currentInputScheme = playerInput.currentControlScheme;
        }

        if (DebugConsole.Instance != null) {
            if (DebugConsole.Instance.inputIsFocused == true) {
                playerController.takeInput(Vector2.zero);
                playing = false;
                return;
            } // No bindings when Debug-Console is focused
        }

        playing = true;

        playerHand.takeInput(playerCursorInput.action.ReadValue<Vector2>(), currentInputScheme);
        playerController.takeInput(playerMove.action.ReadValue<Vector2>());

        isHoveringButton = eventSystem.IsPointerOverGameObject();
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
            uiControlsHandler.UpdateCurrentMaps(currentPlatform);
        }
    }

    private void PlayerUseHandler(InputAction.CallbackContext obj) {
        if (isHoveringButton && currentInputScheme == SCHEME_MnK && playing){
            return;
        }
        playerHand.playerUseFunc();
    }

    private void SwitchMeleeModeHandler(InputAction.CallbackContext obj) {
        if (DebugConsole.Instance != null) { if (DebugConsole.Instance.inputIsFocused == true) { return; } } // No bindings when Debug-Console is focused

        bool newIsMeleeMode = !playerHand.isMeleeMode;
        if (newIsMeleeMode) {
            playerSecondaryUse.action.started -= playerHand.removeBuilding;
        } else {
            playerSecondaryUse.action.started += playerHand.removeBuilding;
        }

        playerHand.switchMeleeMode(newIsMeleeMode);
    }

    //This section would be usefull if we decide to centralize the buttons through this sole script
    #region to be removed?
    /*
    public void buttonClick(string buttonInfo) {

        string[] strings = buttonInfo.Split(" ");
        if (strings.Length <= 0) {
            Debug.LogWarning($"Button info not formatted properly: {buttonInfo}");
            return;
        }


        Debug.Log(strings[0]);


        bool success = false;
        if (strings.Length > 1) {
            success = buttonWithMessage(strings[0], strings[1]);
        } else {
            success = buttonWithoutMessage(strings[0]);
        }

        Debug.Log($"{success} : {strings[1]}");

        if (!success) {
            Debug.LogError($"Failed to execute button logic: {strings}");
        }
    }
    private bool buttonWithoutMessage(string buttonAction) {
        return false;
    }

    private bool buttonWithMessage(string buttonAction, string buttonMessage) {
        switch (buttonAction) {
            case "SwitchSelection":
                int selection;
                try {
                    selection = int.Parse(buttonMessage);
                } catch {
                    return false;
                }
                PlayerHand.Instance.SwitchBuildSelection(selection);
                return true;
        }
        return false;
    }
    */
    #endregion
}
