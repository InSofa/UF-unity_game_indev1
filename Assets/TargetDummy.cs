using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetDummy : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshPro textMesh;

    EnemyHealth enemyHealth;

    public List<float> damagePerSecond = new List<float>();
    public int dpsSamples = 10;

    public float averageDPS {
        get {
            float sum = 0;
            foreach (float dps in damagePerSecond) {
                sum += dps;
            }
            return Mathf.Round(sum / damagePerSecond.Count);
        }
    }

    float timeElapsed = 0f, lastDamage = 0f;

    [SerializeField]
    float dpsCycleTime;

    [SerializeField]
    bool logDPS;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= dpsCycleTime) {
            float damage = enemyHealth.damageTaken - lastDamage;
            damagePerSecond.Add((enemyHealth.damageTaken - lastDamage) / timeElapsed);

            if (damagePerSecond.Count >= dpsSamples) {
                damagePerSecond.RemoveAt(0);
            }

            lastDamage = enemyHealth.damageTaken;
            if (logDPS) {
                Debug.Log($"DPS on {this.gameObject.name}: {averageDPS}");
            }
            if (textMesh != null) {
                textMesh.text = $"DPS: {averageDPS}";
            }
            timeElapsed = 0;
        }
    }
}
