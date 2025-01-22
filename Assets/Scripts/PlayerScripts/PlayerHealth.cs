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
            }
        }
    }
}
