using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject explosionEffect;

    public float explosionRadius = 1f;
    public float explosionForce = 1f;
    public int explosionDamage = 1;

    public GameObject parent;

    public LayerMask damageLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Explode();
    }

    public void Explode() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayer);
        foreach (Collider2D collider in colliders) {
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null) {
                Vector2 direction = (rb.position - (Vector2)transform.position).normalized;
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
            }

            PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                if (parent != null) {
                    playerHealth.TakeDamage(explosionDamage, parent);
                } else {
                    playerHealth.TakeDamage(explosionDamage, this.gameObject);
                }
                continue;
            }

            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null) {
                enemyHealth.TakeDamage(explosionDamage);
                continue;
            }

            BuildingHealth buildingHealth = collider.GetComponent<BuildingHealth>();
            if (buildingHealth != null) {
                buildingHealth.TakeDamage(explosionDamage);
            }
        }
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        Destroy(gameObject);
    }
}
