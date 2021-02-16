﻿using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("The max health the player would be able to get to")]
    [SerializeField]
    int maxHealth;

    [Tooltip("The current health of the player")]
    [SerializeField]
    int currentHealth;
    private void Start()
    {
        //this is for if you forget to set the current health 
        //in the inspector
        if (currentHealth == 0)
            currentHealth = maxHealth;
    }
    public void HurtPlayer(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            GetComponent<PlayerDeath>().isdead = true;
        }
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}