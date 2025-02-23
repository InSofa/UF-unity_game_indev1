using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIHandler : MonoBehaviour
{
    public enum Menus {
        PAUSE,
        OPTIONS
    }

    [SerializeField]
    public GameObject debugOverlay;

    [SerializeField]
    public InputActionReference debugButton;

    [SerializeField]
    public InputActionReference menuButton;

    private int currentScene;

    [SerializeField]
    private GameObject pauseMenu, optionsMenu;

    [SerializeField]
    GameObject[] pauseMenuInteractableObjects, optionsMenuInteractableObjects;

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
        toggleMenuHandler(Menus.PAUSE);
    }
    
    //Incase we want to toggle pause menu via buttons or similar
    public void toggleMenuHandler(Menus menuToToggle) {
        if (currentScene == 1) {
            toggleMenu(pauseMenu, pauseMenuInteractableObjects);
        }
    }

    private void toggleMenu(GameObject menuToToggle, GameObject[] interactiveList) {
        menuToToggle.SetActive(!menuToToggle.activeSelf);

        // Enable/Disable Interactable Objects to make sure they dont interfere with UI navigation
        for (int i = 0; i < interactiveList.Length; i++) {
            Button button = interactiveList[i].GetComponent<Button>();
            if (button != null) {
                button.interactable = menuToToggle.activeSelf;
                continue;
            }

            Slider slider = interactiveList[i].GetComponent<Slider>();
            if (slider != null) {
                slider.interactable = menuToToggle.activeSelf;
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
