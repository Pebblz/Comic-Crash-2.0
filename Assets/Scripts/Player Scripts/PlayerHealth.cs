using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    int maxHealth;
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
