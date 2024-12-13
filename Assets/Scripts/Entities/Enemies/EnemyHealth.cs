using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyHealth : MonoBehaviour
{

    //Health Values
    float health;

    [SerializeField]
    float maxHealth;

    [SerializeField]
    GameObject destroyParticle, pillow;

    [SerializeField]
    float minPillows, maxPillows, spawnRange;

    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health < 0)
        {
            if(destroyParticle != null)
            {
                GameObject particle = Instantiate(destroyParticle, transform.position, Quaternion.identity);
                Destroy(particle, 5);
            }

            spawnPillows();

            Destroy(this.gameObject);
            return;
        }
    }

    private void spawnPillows()
    {
        float amount = Mathf.RoundToInt(Random.Range(minPillows, maxPillows));

        for (int i = 0; i < amount; i++)
        {
            Vector2 directionPos = new Vector2(Random.Range(-1,1), Random.Range(-1,1));
            directionPos.Normalize();

            Vector3 spawnPosition = directionPos * spawnRange;
            spawnPosition.z = 0;

            Instantiate(pillow, transform.position + spawnPosition, Quaternion.identity);
        }
    }
}
