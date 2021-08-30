using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Range(1f, 10f), Tooltip("The max health the player would be able to get to")]
    public int maxHealth;

    [SerializeField, Tooltip("The current health of the player")]
    public int currentHealth;

    [SerializeField, Tooltip("The current IFrames of the player")]
    public float IFrameTimer;
    private void Awake()
    {
        //this is for if you forget to set the current health 
        //in the inspector
        if (currentHealth == 0)
            currentHealth = maxHealth;
    }
    private void Update()
    {
        IFrameTimer -= Time.deltaTime;
    }
    public void HurtPlayer(int amount)
    {
        if (IFrameTimer <= 0)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                GetComponent<PlayerDeath>().isdead = true;
            }
            IFrameTimer = 2;
        }
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
