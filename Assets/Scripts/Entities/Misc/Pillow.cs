using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillow : MonoBehaviour
{
    [SerializeField]
    string pillowSFX;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            collision.GetComponent<PlayerHand>().addPillow(1);
            GlobalSoundComposer.Instance.PlayFx(pillowSFX);
            Destroy(this.gameObject);
        }
    }
}
