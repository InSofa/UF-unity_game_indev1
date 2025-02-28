using NUnit.Framework.Constraints;
using UnityEngine;

public class MortarProjectile : MonoBehaviour
{
    public Vector2 targetPos;
    public Vector2 inaccuracyModifier;
    Vector2 startPos;

    public float damage;
    public float explosionRadius;
    public float travelTime;
    public LayerMask hitMask;

    public GameObject explosionEffect;

    float startDistance;
    float distanceTravelled;

    public Vector2 maxSize;
    public Vector2 minSize;
    Vector2 dir;

    [SerializeField]
    AnimationCurve sizeCurve;

    bool initiate = false;

    float timeSinceStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void init()
    {
        targetPos = new Vector2(targetPos.x + Random.Range(-inaccuracyModifier.x, inaccuracyModifier.x), targetPos.y + Random.Range(-inaccuracyModifier.y, inaccuracyModifier.y));

        startDistance = Vector2.Distance(transform.position, targetPos);
        maxSize = transform.localScale;

        startPos = transform.position;

        dir = targetPos - (Vector2)transform.position;
        dir.Normalize();
        initiate = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!initiate) return;

        timeSinceStart += Time.fixedDeltaTime;
        transform.position = startPos + (dir * (startDistance * (timeSinceStart / travelTime)));
        if(timeSinceStart >= travelTime) {
            Explode();
        }

        Vector2 newSize = (maxSize - minSize) * sizeCurve.Evaluate(timeSinceStart / travelTime) + minSize;
        transform.localScale = newSize;
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

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
