using UnityEngine;

public class SpawnerEnemy : MonoBehaviour
{
    Rigidbody2D rb;
    EnemyHandler enemyHandler;
    Transform bedPosition;

    [Header("Basic Enemy Stats")]

    [SerializeField]
    float speed;

    [SerializeField]
    float speedVariance;

    [SerializeField]
    float acceleration;

    [SerializeField]
    float accelerationVariance;

    [SerializeField]
    float attackRange;

    [SerializeField]
    float attackCD;

    [SerializeField]
    int damage;

    [Header("Spawner Enemy variables")]
    [SerializeField]
    float spawnCD = 5f;

    private float useSpawnCD;

    [SerializeField]
    float spawnVariance = 1f;

    [SerializeField]
    int minSpawns, maxSpawns;

    [SerializeField]
    GameObject spawnPrefab;

    [SerializeField]
    float spawnRange = 2f;

    [SerializeField]
    float spawnStopCD = 1f;

    Vector2 dir;
    float bedDistance, obstacleDistance, attackTime, spawnTime, spawnStopTime;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();

        enemyHandler = GetComponent<EnemyHandler>();

        GameObject player = GameObject.Find("Player");

        bedPosition = GameObject.Find("Bed").transform;

        useSpawnCD = Random.Range(spawnCD - spawnVariance, spawnCD + spawnVariance);

        attackTime = attackCD;
        spawnTime = useSpawnCD;
        spawnStopCD = spawnStopTime;

        speed = Random.Range(speed - speedVariance, speed + speedVariance);
        acceleration = Random.Range(acceleration - accelerationVariance, acceleration + accelerationVariance);
    }

    // Update is called once per frame
    void Update()
    {
        bedDistance = Vector2.Distance(transform.position, bedPosition.position);

        attackTime -= Time.deltaTime;
        if (attackTime < 0) {
            enemyHandler.attack(damage);
            attackTime = attackCD;
        }

        if (spawnTime < 0) {
            SpawnEnemy();

            if(spawnStopTime < 0) {
                useSpawnCD = Random.Range(spawnCD - spawnVariance, spawnCD + spawnVariance);
                spawnTime = useSpawnCD;
                spawnStopTime = spawnStopCD;
            } else {
                spawnStopTime -= Time.deltaTime;

                dir = Vector2.zero;
            }
        } else {
            spawnTime -= Time.deltaTime;

            //Needs error handling, returns "index out of range. must be non negative and less than the size of the collection" (still works tho)
            dir = enemyHandler.moveDir(bedPosition.position);
        }
    }

    private void FixedUpdate() {
        Vector2 velocity = rb.linearVelocity;
        velocity = Vector2.Lerp(velocity, dir * speed, acceleration);
        rb.linearVelocity = velocity;
    }

    void SpawnEnemy() {
        if (spawnPrefab == null) {
            throw new System.Exception("Spawn Prefab not set on " + gameObject.name);
        }
        int spawnAmount = Random.Range(minSpawns, maxSpawns);

        for (int i = 0; i < spawnAmount; i++) {
            Vector2 spawnLocation = new Vector2(Random.value -.5f, Random.value - .5f);
            spawnLocation = spawnLocation.normalized * spawnRange;

            GameObject enemy = Instantiate(spawnPrefab, (Vector2)transform.position + spawnLocation, Quaternion.identity);
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
