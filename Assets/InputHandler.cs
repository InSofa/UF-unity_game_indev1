using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class InputHandler : MonoBehaviour
{
    private static InputHandler instance;
    public static InputHandler Instance {  get { return instance; } }

    //References
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private UIControls_Handler uiControlsHandler;

    [SerializeField]
    EventSystem eventSystem;

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

    [Header("Settings")]

    //The delay after a button is pressed and the player can use the left mouse for other actions
    [SerializeField]
    private float UIInteractionDelay = .1f;

    [Header("Input actions")]
    [SerializeField]
    private InputActionReference playerUse;

    [SerializeField]
    private InputActionReference playerMove;

    //Could be both the joystick and mouse
    [SerializeField]
    private InputActionReference playerCursorInput;

    [SerializeField]
    private InputActionReference playerMeleeSwitch;


    #region Private Variables
    private bool buttonPressed = false;

    private bool playerUseRequested = false;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instance == null) {
            instance = this;
        }
        else {
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

    //Should be called after input is taken, hopefully (logic to avoid double inputs are here)
    private void LateUpdate() {
        // Check if the current input scheme has changed
        if (playerInput != null && playerInput.currentControlScheme != currentInputScheme) {
            UpdateCurrentPlatform();
            currentInputScheme = playerInput.currentControlScheme;
        }

        //Logic handling PlayerHandInput
        if (currentInputScheme == SCHEME_MnK) {
            //Making sure that the player isn't just pushing a button
            Debug.Log("Making sure that the player isn't just pushing a button");
            bool playerUse = playerUseRequested && !buttonPressed;
            PlayerHand.Instance.takeInput(playerCursorInput.action.ReadValue<Vector2>(), currentInputScheme, playerUse);

            //Reset the variables
            playerUseRequested = false;
            buttonPressed = false;
        } else {
            PlayerHand.Instance.takeInput(playerCursorInput.action.ReadValue<Vector2>(), currentInputScheme, playerUseRequested);
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
            uiControlsHandler.UpdateCurrentMaps(currentPlatform);
        }
    }

    public void buttonClick(string buttonInfo) {
        buttonPressed = true;

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
}
