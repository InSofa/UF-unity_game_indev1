using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillow : MonoBehaviour
{
    [SerializeField]
    string pillowSFX;

    [SerializeField]
    public int value = 1;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            PlayerHand playerHand = collision.GetComponent<PlayerHand>();
            playerHand.addPillow(value * playerHand.GlobalPickupInflationMultiplier);
            GlobalSoundComposer.Instance.PlayFx(pillowSFX);
            Destroy(this.gameObject);
        }
    }
}
