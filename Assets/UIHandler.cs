using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    public InputActionReference menuButton;

    private int currentScene;

    [SerializeField]
    private GameObject pauseMenu;

    private void Start() {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update() {
        if (menuButton != null) {
            menuButton.action.started += togglePauseMenu;
        }
    }

    public void loadScene(int value) {
        SceneManager.LoadScene(value);
    }

    public void togglePauseMenu(InputAction.CallbackContext obj) {
        if (currentScene == 1) {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }
}
