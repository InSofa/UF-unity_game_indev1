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

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMaterial = sr.material;

        health = maxHealth;

        EnemySpawner.instance.currentEnemies.Add(this.gameObject);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("took damage, i is enemy");
        health -= damage;

        StopAllCoroutines();
        StartCoroutine(flash());

        if (health < 0)
        {
            if(destroyParticle != null)
            {
                GameObject particle = Instantiate(destroyParticle, transform.position, Quaternion.identity);
                Destroy(particle, 5);
            }

            spawnPillows();

            EnemySpawner.instance.currentEnemies.Remove(this.gameObject);
            Destroy(this.gameObject);
            return;
        }
    }

    private void spawnPillows()
    {
        if (maxPillows == 0) {
            return;
        }

        if (pillowPrefab == null) {
            throw new System.Exception("Pillow Prefab not set on " + gameObject.name);
        }

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
        flashMaterial.color = flashColor;
        sr.material = flashMaterial;
        yield return new WaitForSeconds(flashTime);
        sr.material = originalMaterial;
    }
}
