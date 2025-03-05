using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public static float joystickLookSensitivity = 1.0f;

    [SerializeField]
    Slider joystickSensitivitySlider;

    [SerializeField]
    TMP_InputField joystickSensitivityInput;

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
        joystickSensitivityInput.text = joystickLookSensitivity.ToString();

        joystickSensitivitySlider.onValueChanged.AddListener(delegate { SetControllerSensitivity(true); });
        joystickSensitivityInput.onEndEdit.AddListener(delegate { SetControllerSensitivity(false); });
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
}
