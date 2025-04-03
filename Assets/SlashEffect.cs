using System.Collections;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    SpriteRenderer sr;

    [SerializeField]
    private float slashForce = 10f; // Speed of the slash effect

    [SerializeField]
    private float fadeTime = 0.5f; // Time to fade out the effect

    private void Start() {
        //sr.flipY = Random.value > 0.5f; ; // Flip the sprite vertically based on the random value

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * slashForce, ForceMode2D.Impulse); // Apply an impulse force in the direction of the slash
        
        StartCoroutine(fadeOut()); // Start the fade out coroutine
    }

    IEnumerator fadeOut() {
        while (sr.color.a > 0) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - Time.deltaTime / fadeTime); // Decrease the alpha value over time
            yield return null; // Wait for the next frame
        }

        Destroy(gameObject); // Destroy the game object after fading out
    }
}
