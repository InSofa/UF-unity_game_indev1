using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    //To keep track of all enemies currently alive
    [HideInInspector]
    public List<GameObject> currentEnemies;

    [SerializeField]
    Wave[] waves;

    public int currentWave = 0;

    [SerializeField]
    float minSpawnDist = 25f;

    [SerializeField]
    float maxSpawnDist = 30f;

    [SerializeField]
    Vector2 wallClamp;

    [Space]
    [SerializeField]
    GameObject spawnEffect;


    /*
    [SerializeField]
    float timeBetweenWave = 15f;
    float time;
    */
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

    /*
    public void GenerateWall()
    {
        //Debug.Log($"Segment count: {segmentCount}, wallRadius: {wallRadius} SegmentSize: {segmentSize} Div: {segmentAngle}");
        int segmentCount = 361;

        wall.positionCount = segmentCount;

        List<Vector3> points3D = new List<Vector3>();
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < segmentCount; i++) {
            float x = Mathf.Sin(Mathf.Deg2Rad * i) * wallRadius;
            float y = Mathf.Cos(Mathf.Deg2Rad * i) * wallRadius;
            points.Add(new Vector2(x, y));
        }

        wallCollider.points = points.ToArray();
        //wallCameraCollider.SetPath(segmentCount, points.ToArray());
        wall.SetPositions(points3D.ToArray());
    }*/

    public void SpawnWave()
    {
        if (currentWave >= waves.Length) {
            Debug.Log("No more waves to spawn, resetting count");
            currentWave = 0;
        }

        Pyle[] pylesToSpawn = waves[currentWave].pyles;

        for (int i = 0; i < pylesToSpawn.Length; i++) {
            float angle = Random.Range(0, 360);
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * Random.Range(minSpawnDist, maxSpawnDist);
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * Random.Range(minSpawnDist, maxSpawnDist);
            Vector2 spawnPos = new Vector2(Mathf.Clamp(x, -wallClamp.x, wallClamp.x), Mathf.Clamp(y, -wallClamp.y, wallClamp.y));

            Pyle pyle = pylesToSpawn[i];

            if (spawnEffect != null) {
                GameObject effect = Instantiate(spawnEffect, spawnPos, Quaternion.identity);
                Destroy(effect, 5f);
            }

            for (int j = 0; j < pyle.enemiesToSpawn.Length; j++) {
                GameObject enemy = Instantiate(pyle.enemiesToSpawn[j], spawnPos, Quaternion.identity);
                currentEnemies.Add(enemy);
            }
        }

        currentWave++;
    }
}
