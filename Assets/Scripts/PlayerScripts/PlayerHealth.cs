using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Sound Settings")]
    LocalSoundComposer lsc;
    [SerializeField]
    List<String> hurtSounds;

    [SerializeField]
    string deathSound;


    [Space]
    [Header("Health Settings")]


    private float health;
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    Slider healthSlider;
    [SerializeField]
    TMP_Text healthText;

    public bool canDie = true;


    public void Start()
    {
        lsc = GetComponent<LocalSoundComposer>();

        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        UpdateHpUI();
        healthSlider.gameObject.SetActive(true);
    }

    private void UpdateHpUI() {
        healthSlider.value = health;
        healthText.text = $"{Mathf.Clamp(health,0,Mathf.Infinity).ToString()}hp / {maxHealth.ToString()}hp";
    }

    IEnumerator Death() {
        lsc.PlayFx(deathSound);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }

    private void DeathCheck() {
        if (health < 0) {
            if (canDie == true) {
                //Destroy(this.gameObject);
                //StartCoroutine(Death());
                SceneManager.LoadScene(0);
            } else {
                // [GLOBAL.TRACK]
                /*
                GlobalSoundComposer gsc = GameObject.Find("GlobalSoundComposer").GetComponent<GlobalSoundComposer>();
                if (!gsc.TrackIsPlaying("music:generic/debug_track")) {
                    gsc.PlayTrack("music:generic/debug_track");
                }
                */

                // [GLOBAL.SFX]
                /*
                GlobalSoundComposer gsc = GameObject.Find("GlobalSoundComposer").GetComponent<GlobalSoundComposer>();
                gsc.PlayFx("sfx:generic/debug");
                */

                // [LOCAL.TRACK] FAILS
                /*
                LocalSoundComposer lsc = this.GetComponent<LocalSoundComposer>();
                if (!lsc.TrackIsPlaying("music:generic/debug_track")) {
                    lsc.PlayTrack("music:generic/debug_track");
                }
                */

                // [LOCAL.SFX]
                /*
                LocalSoundComposer lsc = this.GetComponent<LocalSoundComposer>();
                lsc.PlayFx("sfx:generic/debug");
                */

            }
            return;
        }
    }

    public void SetHealth(float newHealth) {

        if (newHealth < health) {
            lsc.PlayRandomFx(hurtSounds);
        }

        health = newHealth;
        UpdateHpUI();

        DeathCheck();
    }

    public float GetHealth() {
        return health;
    }

    public void TakeDamage(float damage, GameObject source) {
        health -= damage;
        UpdateHpUI();

        DeathCheck();

        lsc.PlayRandomFx(hurtSounds);
    }
}
