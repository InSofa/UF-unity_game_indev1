using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurretProjectile : MonoBehaviour
{
    public float damage;

    public bool isPiercing;

    EnemyHealth[] enemiesHit;

    public LayerMask hitMask;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Insert hit logic
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            if (isPiercing) {
                EnemyHealth check = enemiesHit.FirstOrDefault(e => e == enemyHealth);
                if (check == null) {
                    enemyHealth.TakeDamage(damage);
                    enemiesHit.Append(enemyHealth);
                }
            } else {
                enemyHealth.TakeDamage(damage);
                Destroy(this.gameObject);
            }
        }

    }
}
