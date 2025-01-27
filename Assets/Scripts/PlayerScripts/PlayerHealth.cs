using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    Slider healthSlider;

    public bool canDie = true;


    public void Start()
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        healthSlider.gameObject.SetActive(true);
    }

    public void Update() {
    }

    public void TakeDamage(float damage, GameObject source)
    {
        Debug.Log("took damage");
        health -= damage;
        healthSlider.value = health;
        if (health < 0)
        {
            if (canDie == true) {
                //Destroy(this.gameObject);
                SceneManager.LoadScene(0);
            } else {
                // [GLOBAL]
                /* GlobalSoundComposer gsc = GameObject.Find("GlobalSoundComposer").GetComponent<GlobalSoundComposer>(); */
                // [GLOBAL.TRACK]
                /*
                if (!gsc.TrackIsPlaying("music:generic/debug_track")) {
                    gsc.PlayTrack("music:generic/debug_track");
                }
                */
                // [GLOBAL.SFX]
                /* gsc.PlayFx("sfx:generic/debug"); */
                // [LOCAL]
                /* LocalSoundComposer lsc = this.GetComponent<LocalSoundComposer>(); */
                // [LOCAL.TRACK] FAILS
                /*
                if (!lsc.TrackIsPlaying("music:generic/debug_track")) {
                    lsc.PlayTrack("music:generic/debug_track");
                }
                */
                // [LOCAL.SFX]
                /* lsc.PlayFx("sfx:generic/debug"); */
            }
        }
    }
}
