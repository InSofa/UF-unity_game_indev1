using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    float health;
    [SerializeField]
    float maxHealth;

    [SerializeField]
    Slider healthSlider;


    private void Start()
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthSlider.value = health;
        if (health < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
