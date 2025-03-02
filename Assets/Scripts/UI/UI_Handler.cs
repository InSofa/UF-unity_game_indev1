using Unity.VisualScripting;
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

    [Header("Menus")]

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private GameObject turretMenu;

    [SerializeField]
    private GameObject wallMenu;

    [Header("Interactable Objects")]
    [SerializeField]
    GameObject[] pauseMenuInteractableObjects;

    [SerializeField]
    GameObject[] optionsMenuInteractableObjects;

    [SerializeField]
    GameObject[] mainInteractableObjects;

    [SerializeField]
    GameObject[] turretSelectionInteractableObjects;

    [SerializeField]
    GameObject[] wallSelectionInteractableObjects;

    //0 = Gameplay/Main Menu, 1 = Pause Menu, 2 = Options Menu
    int currentMenu = 0;


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
        switchMenu(1);
    }

    //if newMenu = curentMenu, close menu and return to menu 0, else open newMenu
    public void switchMenu(int newMenu) {
        if(currentMenu == newMenu || newMenu == 0) {
            currentMenu = 0;
            updateMenu(null, mainInteractableObjects);
            return;
        }
        switch (newMenu) {
            case 1:
                if (currentScene == 1) {
                    updateMenu(pauseMenu, pauseMenuInteractableObjects);
                }
                break;
            case 2:
                updateMenu(optionsMenu, optionsMenuInteractableObjects);
                break;
        }
        currentMenu = newMenu;
    }

    private void updateMenu(GameObject menuToShow, GameObject[] interactiveList) {
        if(pauseMenu != null) pauseMenu.SetActive(false);
        if (optionsMenu != null) optionsMenu.SetActive(false);

        // Enable/Disable Interactable Objects to make sure they dont interfere with UI navigation
        for (int i = 0; i < pauseMenuInteractableObjects.Length; i++) {
            Selectable selectable = pauseMenuInteractableObjects[i].GetComponent<Selectable>();
            if (selectable != null) {
                selectable.interactable = false;
            }
        }

        for (int i = 0; i < optionsMenuInteractableObjects.Length; i++) {
            Selectable selectable = optionsMenuInteractableObjects[i].GetComponent<Selectable>();
            if (selectable != null) {
                selectable.interactable = false;
            }
        }

        for (int i = 0; i < mainInteractableObjects.Length; i++) {
            Selectable selectable = mainInteractableObjects[i].GetComponent<Selectable>();
            if (selectable != null) {
                selectable.interactable = false;
            }
        }

        if(menuToShow != null) menuToShow.SetActive(true);

        // Enable/Disable Interactable Objects to make sure they dont interfere with UI navigation
        for (int i = 0; i < interactiveList.Length; i++) {
            Selectable selectable = interactiveList[i].GetComponent<Selectable>();
            if (selectable != null) {
                selectable.interactable = true;
                if (i == 0) {
                    selectable.Select();
                }
            }
        }
    }
    public void expandMenu(int menu) {
        switch (menu) {
            case 1:
                expandMenu(turretMenu, turretSelectionInteractableObjects);
                break;
            case 2:
                expandMenu(wallMenu, wallSelectionInteractableObjects);
                break;
        }
    }

    public void expandMenu(GameObject menuToToggle, GameObject[] interactiveList) {
        menuToToggle.SetActive(!menuToToggle.activeSelf);
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

        /*
        Button button = interactiveList[i].GetComponent<Button>();
        if (button != null) {
        button.interactable = menuToToggle.activeSelf;
        continue;
        }

        Slider slider = interactiveList[i].GetComponent<Slider>();
        if (slider != null) {
        slider.interactable = menuToToggle.activeSelf;
        }
        */
    }
}
