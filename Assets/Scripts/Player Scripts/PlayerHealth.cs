using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
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

    PhotonView photonView;


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
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
        pause = FindObjectOfType<Pause>();
        movement = GetComponent<PlayerMovement>();
    }
    private void LateUpdate()
    {
        IFrameTimer -= Time.deltaTime;
        if (photonView.IsMine)
        {
            if (movement.InWater || movement.inWaterAndFloor)
            {

                if (movement.AtTheTopOfWater)
                {
                    WaterUI.ReFillAirBar = true;

                    if (WaterUI.airLeft == MaxAirTimer)
                    {
                        if (WaterUI.airBar.enabled)
                            WaterUI.DisableSlider();
                    }
                }
                else
                {
                    if (!pause.isPaused)
                    {
                        if (!WaterUI.airBar.enabled)
                            WaterUI.EnableSlider();

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
            if (currentHealth <= 0)
            {
                GetComponent<PlayerDeath>().isdead = true;
            }
        }

        currentAir = WaterUI.airLeft;
    }
    public void GainAir(int amount)
    {
        if (WaterUI.airLeft + amount > MaxAirTimer)
            WaterUI.airLeft = MaxAirTimer;
        else
            WaterUI.airLeft += amount;
    }
    public void HurtPlayer(int amount)
    {
        if (IFrameTimer <= 0)
        {
            currentHealth -= amount;
            FindObjectOfType<HealthBar>().FindNewHealthBar();
            if (currentHealth <= 0)
            {
                GetComponent<PlayerDeath>().isdead = true;
            }
            IFrameTimer = 1;
        }
    }
    //public void HurtPlayer(int amount, Vector3 knockBackDir)
    //{
    //    if (IFrameTimer <= 0)
    //    {
    //        currentHealth -= amount;

    //        if (currentHealth <= 0)
    //        {
    //            GetComponent<PlayerDeath>().isdead = true;
    //        }
    //        IFrameTimer = 1;
    //    }
    //}

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
