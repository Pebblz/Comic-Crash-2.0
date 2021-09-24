using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Range(1f, 10f), Tooltip("The max health the player would be able to get to")]
    public int maxHealth;

    [SerializeField, Tooltip("The current health of the player")]
    public int currentHealth;

    [SerializeField, Tooltip("The current IFrames of the player")]
    public float IFrameTimer;

    [SerializeField, Tooltip("The max amount of time the player can be underwater")]
    public float MaxAirTimer;

    [SerializeField, Tooltip("The script UnderwaterAirUI ")]
    UnderwaterAirUI WaterUI;

    private PlayerMovement movement;

    public float currentAir;

    private Pause pause;
    private void Awake()
    {
        //this is for if you forget to set the current health 
        //in the inspector
        if (currentHealth == 0)
            currentHealth = maxHealth;

        if (WaterUI == null)
        {
            WaterUI = FindObjectOfType<UnderwaterAirUI>();
            WaterUI.MaxAir = MaxAirTimer;
        }
        else
        {
            WaterUI.MaxAir = MaxAirTimer;
        }
        //WaterUI.airLeft = currentAir;
        pause = FindObjectOfType<Pause>();
        movement = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        IFrameTimer -= Time.deltaTime;

        if (movement.InWater || movement.inWaterAndFloor )
        {
            if (!WaterUI.airBar.enabled)
                WaterUI.EnableSlider();
            if (movement.AtTheTopOfWater)
            {
                WaterUI.ReFillAirBar = true;
            }
            else
            {
                if (!pause.isPaused)
                {
                    WaterUI.airLeft -= Time.deltaTime;
                }
            }
            if (WaterUI.airLeft <= 0)
            {
                GetComponent<PlayerDeath>().isdead = true;
            }
        }
        else
        {
            if (WaterUI.airLeft < MaxAirTimer)
                WaterUI.ReFillAirBar = true;
            else
                WaterUI.ReFillAirBar = false;

            if (WaterUI.airBar.enabled)
                WaterUI.DisableSlider();
        }
        currentAir = WaterUI.airLeft;
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
