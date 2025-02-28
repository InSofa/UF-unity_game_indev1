using UnityEngine;

public class WaterProjectile : MonoBehaviour
{
    public float falloffStrenght = 1f;

    Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, falloffStrenght * Time.fixedDeltaTime);
        if(rb.linearVelocity.magnitude < 0.1f) {
            Destroy(gameObject);
        }
    }
}
