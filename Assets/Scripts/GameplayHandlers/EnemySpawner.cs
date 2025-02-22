using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    //To keep track of all enemies currently alive
    public List<GameObject> currentEnemies;

    [SerializeField]
    float minSpawnDist = 25f;

    [SerializeField]
    GameObject enemy;

    [SerializeField]
    int enemiesToSpawn = 10;

    [SerializeField]
    Transform pivot;

    [SerializeField]
    float timeBetweenWave = 15f;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //SpawnWave();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        time += Time.deltaTime;
        if (time >= timeBetweenWave) {
            SpawnWave();
            time = 0;
        }
        */
    }

    public void SpawnWave()
    {
        //Additional measures to make sure no new wave is started when a current one is still active
        if(currentEnemies.Count > 0) {
            Debug.Log("Enemies still alive");
            return;
        }

        Vector2 spawnPos = transform.position + (pivot.up * minSpawnDist);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject spawnObject = Instantiate(enemy, spawnPos, Quaternion.identity);
        }
    }
}
