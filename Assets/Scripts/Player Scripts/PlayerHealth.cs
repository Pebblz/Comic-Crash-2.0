using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Range(1f, 10f), Tooltip("The max health the player would be able to get to")]
    public int maxHealth;

    [SerializeField, Tooltip("The current health of the player")]
    public int currentHealth;
    private void Awake()
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
