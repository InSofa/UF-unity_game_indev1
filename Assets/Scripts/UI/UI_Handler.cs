using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    public GameObject debugOverlay;
    [SerializeField]
    public InputActionReference debugButton;

    [SerializeField]
    public InputActionReference menuButton;

    private int currentScene;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    GameObject[] pauseMenuInteractableObjects;

    private void Start() {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {   
        /*
        if (Input.GetKeyUp(KeyCode.Q))
        {
            Debug.Log("switched");
            healthSlider.gameObject.SetActive(!healthSlider.gameObject.activeSelf);
            healthSlider2.gameObject.SetActive(!healthSlider2.gameObject.activeSelf);
        }
        */

        if (menuButton != null) {
            menuButton.action.started += togglePauseMenu;
        }

        if (debugButton != null)
        {
            debugButton.action.started += toggleDebugOverlay;
        }
    }

    public void loadScene(int value) {
        SceneManager.LoadScene(value);
    }

    public void togglePauseMenu(InputAction.CallbackContext obj) {
        if (currentScene == 1) {
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            // Enable/Disable Interactable Objects to make sure they dont interfere with UI navigation
            for (int i = 0; i < pauseMenuInteractableObjects.Length; i++) {
                Button button = pauseMenuInteractableObjects[i].GetComponent<Button>();
                if (button != null) {
                    button.interactable = pauseMenu.activeSelf;
                    continue;
                }

                Slider slider = pauseMenuInteractableObjects[i].GetComponent<Slider>();
                if (slider != null) {
                    slider.interactable = pauseMenu.activeSelf;
                }
            }
        }
    }

    public void toggleDebugOverlay(InputAction.CallbackContext obj)
    {
        if (currentScene == 1)
        {
            debugOverlay.SetActive(!debugOverlay.activeSelf);
        }
    }
}
