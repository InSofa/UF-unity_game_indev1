using UnityEngine;
using UnityEngine.UI;

public class DebugPlayerStatsSliders : MonoBehaviour {
    // Slider references to be assigned in the Inspector
    [SerializeField]
    private Slider speedSlider;
    [SerializeField]
    private Slider velocityXSlider;
    [SerializeField]
    private Slider velocityYSlider;
    [SerializeField]
    private Slider inputMagnitudeSlider;

    [SerializeField]
    private GameObject player;

    private Rigidbody2D rb;
    private PlayerController playerController;

    void Start() {
        // Get the Rigidbody2D component
        rb = player.GetComponent<Rigidbody2D>();
        playerController = player.GetComponent<PlayerController>();

        // Set the sliders' max values based on expected ranges
        if (speedSlider != null) {
            speedSlider.maxValue = 10f;
        }

        if (velocityXSlider != null) {
            velocityXSlider.maxValue = 10f;
        }

        if (velocityYSlider != null) {
            velocityYSlider.maxValue = 10f;
        }

        if (inputMagnitudeSlider != null) {
            inputMagnitudeSlider.maxValue = 1f;
        }
    }

    void Update() {
        // Update the sliders with the corresponding stats
        if (speedSlider != null)
            speedSlider.value = playerController.speed;

        if (velocityXSlider != null)
            velocityXSlider.value = Mathf.Abs(rb.linearVelocityX);

        if (velocityYSlider != null)
            velocityYSlider.value = Mathf.Abs(rb.linearVelocityY);

        if (inputMagnitudeSlider != null)
            inputMagnitudeSlider.value = playerController.movementInput.magnitude;
    }
}
