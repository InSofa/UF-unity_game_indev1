using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicEnemy : MonoBehaviour
{
    Rigidbody2D rb;
    EnemyHandler enemyHandler;
    Transform bedPosition;
    Transform playerPosition;

    [SerializeField]
    float speed, speedVariance, acceleration, accelerationVariance, playerDetectionDistance, damage, attackRange, attackCD;

    Vector2 dir;
    float distance, p_distance, time;

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
        distance = Vector2.Distance(transform.position, bedPosition.position);
        p_distance = Vector2.Distance(transform.position, playerPosition.position);

        if(p_distance <= attackRange || distance <= attackRange)
        {
            dir = Vector2.zero;
            time -= Time.deltaTime;
            if(time <= 0)
            {
                enemyHandler.damagePlayer(damage, this.gameObject);
                time = attackCD;
            }
        }
        else if (p_distance <= playerDetectionDistance && p_distance < distance)
        {
            dir = enemyHandler.moveDir(playerPosition.position);
        }
        else
        {
            dir = enemyHandler.moveDir(bedPosition.position);
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity = Vector2.Lerp(velocity, dir * speed, acceleration);
        rb.linearVelocity = velocity;
    }
}
