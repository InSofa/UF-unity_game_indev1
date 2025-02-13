using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class basicEnemy : MonoBehaviour
{
    Rigidbody2D rb;
    EnemyHandler enemyHandler;
    Transform bedPosition;
    Transform playerPosition;

    [SerializeField]
    float speed, speedVariance, acceleration, accelerationVariance, playerDetectionDistance, attackRange, attackCD;

    [SerializeField]
    int damage;

    Vector2 dir;
    float bedDistance, playerDistance, obstacleDistance,time;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        enemyHandler = GetComponent<EnemyHandler>();

        GameObject player = GameObject.Find("Player");

        playerPosition = player.transform;

        bedPosition = GameObject.Find("Bed").transform;

        time = attackCD;

        speed = Random.Range(speed -speedVariance, speed + speedVariance);
        acceleration = Random.Range(acceleration - accelerationVariance, acceleration + accelerationVariance);
    }

    // Update is called once per frame
    void Update()
    {
        bedDistance = Vector2.Distance(transform.position, bedPosition.position);
        playerDistance = Vector2.Distance(transform.position, playerPosition.position);

        time -= Time.deltaTime;
        if (time < 0) {
            enemyHandler.attack(damage);
            time = attackCD;
        }

        //Needs error handling, returns "index out of range. must be non negative and less than the size of the collection" (still works tho)
        if (playerDistance <= playerDetectionDistance && playerDistance < bedDistance) {
            dir = enemyHandler.moveDir(playerPosition.position);
        } else {
            dir = enemyHandler.moveDir(bedPosition.position);
        }

        //This was replaced by the above code, this would be more optimized if done properly
        //Instead of attacking everytime it can it will only attack if within the range of the target (but since its a bit buggy with target picking it ends up ignoring a bunch of buildings)
        /*
        if(playerDistance <= attackRange || bedDistance <= attackRange)
        {
            dir = Vector2.zero;
            time -= Time.deltaTime;
            if(time <= 0)
            {
                //enemyHandler.damagePlayer(damage, this.gameObject);
                time = attackCD;
            }
        }
        else if (playerDistance <= playerDetectionDistance && playerDistance < bedDistance)
        {
            dir = enemyHandler.moveDir(playerPosition.position);
        }
        else
        {
            dir = enemyHandler.moveDir(bedPosition.position);
        }
        print("Path count: " + enemyHandler.path.Count + "  Building:" + enemyHandler.path[0].building);
        if (enemyHandler.path.Count >= 1 && enemyHandler.path[0].building != null) {
            print("WALKING TO BUILDING");
            obstacleDistance = Vector2.Distance(transform.position, enemyHandler.path[0].worldPosition);
            if (obstacleDistance <= attackRange) {
                time -= Time.deltaTime;
                if (time <= 0) {
                    print("ATTACKING BUILDING");
                    enemyHandler.damageBuilding((int)damage);
                    time = attackCD;
                }
            }
        }
        */
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity = Vector2.Lerp(velocity, dir * speed, acceleration);
        rb.linearVelocity = velocity;
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionDistance);
    }
}
