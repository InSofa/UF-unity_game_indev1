using UnityEngine;

public class WaterProjectile : MonoBehaviour
{
    public float falloffStrenght = 1f;
    public float killVelocity = 0.5f;

    public float startVel;
    public float startDamage;
    public float minDamagePercent = 0.5f;

    Rigidbody2D rb;
    TurretProjectile tp;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        tp = GetComponent<TurretProjectile>();

        startDamage = 0;
    }

    private void Update() {
        if(tp.init) {
            startVel = rb.linearVelocity.magnitude;
            startDamage = tp.damage;
            tp.init = false;
        }
    }

    private void FixedUpdate() {
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, falloffStrenght * Time.fixedDeltaTime);

        if(startDamage != 0) {
            tp.damage = Mathf.Max(startDamage * (rb.linearVelocity.magnitude/ startVel), startDamage * minDamagePercent);
        }

        if (rb.linearVelocity.magnitude < killVelocity) {
            Destroy(gameObject);
        }
    }
}
