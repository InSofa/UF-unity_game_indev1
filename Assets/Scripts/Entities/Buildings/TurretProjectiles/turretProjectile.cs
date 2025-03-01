using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TurretProjectile : MonoBehaviour
{
    public float damage;

    public bool isPiercing;

    List<EnemyHealth> enemiesHit = new List<EnemyHealth>();
    EnemyHealth check = null;

    public LayerMask hitMask;

    public bool init = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Insert hit logic
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            if (isPiercing) {

                check = enemiesHit.FirstOrDefault(health => health == enemyHealth);
                //Debug.Log(enemiesHit.FirstOrDefault(health => health == enemyHealth));
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
