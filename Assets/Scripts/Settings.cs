using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public static float joystickLookSensitivity = 1.0f;
    public static bool rawJoystickInput = false;

    [SerializeField]
    Slider joystickSensitivitySlider;

    [SerializeField]
    TMP_InputField joystickSensitivityInput;

    [SerializeField]
    Toggle rawJoystickInputToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        joystickLookSensitivity = PlayerPrefs.GetFloat("joystickLookSensitivity", 3f);
        joystickSensitivitySlider.value = joystickLookSensitivity;

        if (PlayerPrefs.HasKey("rawJoystickInput")) {
            rawJoystickInput = PlayerPrefs.GetInt("rawJoystickInput") == 1;
            rawJoystickInputToggle.isOn = rawJoystickInput;
        } else {
            PlayerPrefs.SetInt("rawJoystickInput", rawJoystickInput ? 1 : 0);
        }
    }

    public void SetControllerSensitivity(bool isSlider) {
        float newSens;
        if (isSlider) {
            newSens = joystickSensitivitySlider.value;
        } else {
            newSens = float.Parse(joystickSensitivityInput.text);
        }
        newSens = Mathf.Round(newSens * 100) / 100;

        joystickLookSensitivity = newSens;
        joystickSensitivitySlider.value = newSens;
        joystickSensitivityInput.text = newSens.ToString();
        PlayerPrefs.SetFloat("joystickLookSensitivity", joystickLookSensitivity);
    }

    public void SetRawJoystickInput() {
        rawJoystickInput = rawJoystickInputToggle.isOn;
        PlayerPrefs.SetInt("rawJoystickInput", rawJoystickInputToggle.isOn ? 1 : 0);
    }

}
