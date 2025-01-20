using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    Slider healthSlider, healthSlider2;

    public void Start()
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        healthSlider.gameObject.SetActive(true);

        healthSlider2.maxValue = maxHealth;
        healthSlider2.value = health;
        healthSlider2.gameObject.SetActive(false);
    }

    public void Update() {
        if (Input.GetKeyUp(KeyCode.Q)) {
            Debug.Log("switched");
            healthSlider.gameObject.SetActive(!healthSlider.gameObject.activeSelf);
            healthSlider2.gameObject.SetActive(!healthSlider2.gameObject.activeSelf);
        }


    }

    public void TakeDamage(float damage, GameObject source)
    {
        Debug.Log("took damage");
        health -= damage;
        healthSlider.value = health;
        healthSlider2.value = health;
        if (health < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
