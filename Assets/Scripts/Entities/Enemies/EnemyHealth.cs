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

    [Header("Damage/Death Effects")]
    [SerializeField]
    GameObject destroyParticle;

    SpriteRenderer sr;

    private Material originalMaterial;

    [SerializeField]
    Material flashMaterial;

    [SerializeField]
    float flashTime;

    [SerializeField]
    Color flashColor = Color.white;

    [Header("Pillow Spawning")]
    [SerializeField]
    GameObject pillowPrefab;

    [SerializeField]
    float minPillows, maxPillows, spawnRange;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;
        flashMaterial.color = flashColor;

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

        StopAllCoroutines();
        StartCoroutine(flash());
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

            Instantiate(pillowPrefab, transform.position + spawnPosition, Quaternion.identity);
        }
    }

    IEnumerator flash() {
        if (sr == null || flashMaterial == null) {
            yield break;
        }
        sr.material = flashMaterial;
        yield return new WaitForSeconds(flashTime);
        sr.material = originalMaterial;
    }
}
