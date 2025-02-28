using UnityEngine;

public class MortarProjectile : MonoBehaviour
{
    public Vector2 targetPos;

    public float damage;
    public float explosionRadius;
    public float travelTime;
    public LayerMask hitMask;

    public GameObject explosionEffect;

    float startDistance;
    float distanceTravelled;

    Vector2 startSize;
    Vector2 dir;

    [SerializeField]
    AnimationCurve sizeCurve;

    bool initiate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void init()
    {
        startDistance = Vector2.Distance(transform.position, targetPos);
        startSize = transform.localScale;

        dir = targetPos - (Vector2)transform.position;
        dir.Normalize();
        initiate = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!initiate) return;

        distanceTravelled = startDistance - Vector2.Distance(transform.position, targetPos);

        Debug.Log(startDistance + "   " + distanceTravelled);

        transform.localScale = startSize * sizeCurve.Evaluate(distanceTravelled / startDistance);
        if (distanceTravelled >= startDistance) {
            Explode();
        }

        transform.position += (Vector3)dir * (startDistance / travelTime) * Time.fixedDeltaTime;
    }

    void Explode() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, hitMask);
        foreach (Collider2D hit in hits) {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null) {
                enemyHealth.TakeDamage(damage);
            }
        }
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        Destroy(gameObject);
    }
}
