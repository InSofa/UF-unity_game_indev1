using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pillow : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            collision.GetComponent<PlayerHand>().addPillow(1);
            Destroy(this.gameObject);
        }
    }
}
