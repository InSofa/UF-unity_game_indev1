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
    float minSpawnDist = 25f;

    [SerializeField]
    float wallRadius = 30f;

    [SerializeField]
    LineRenderer wall;

    [SerializeField]
    EdgeCollider2D wallCollider;

    [SerializeField]
    PolygonCollider2D wallCameraCollider;

    [SerializeField]
    GameObject enemy;

    [SerializeField]
    int enemiesToSpawn = 10;

    [SerializeField]
    Transform pivot;

    /*
    [SerializeField]
    float timeBetweenWave = 15f;
    float time;
    */
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GenerateWall();
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

    public void SpawnWaveAdvanced() {
        int spawnPoints = 4;

        for (int i = 0; i < spawnPoints; i++) {
            float angle = Random.Range(0, 360);

            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * wallRadius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * wallRadius;
            //points.Add(new Vector2(x, y));
        }
    }
}
