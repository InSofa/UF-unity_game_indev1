using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowPull : MonoBehaviour
{
    [SerializeField]
    float pullForce, pullRange;

    [SerializeField]
    LayerMask pillowLayer;

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRange, pillowLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody2D pRb = colliders[i].GetComponent<Rigidbody2D>();
            Vector2 dir = transform.position - pRb.transform.position;

            pRb.linearVelocity = dir * pullForce;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullRange);
    }
}
