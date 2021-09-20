using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnderwaterAirUI : MonoBehaviour
{
    public Slider airBar;
    [HideInInspector]
    public float airLeft;
    [HideInInspector]
    public float MaxAir;
    public float airRecoverySpeed;
    public bool ReFillAirBar;
    GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        airBar = GetComponent<Slider>();
        airLeft = airBar.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            //this needs to be here because the player can switch characters
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if(airBar.maxValue != MaxAir)
        {
            airBar.maxValue = MaxAir;
        }
        if (ReFillAirBar)
        {
            if (airLeft < airBar.maxValue)
            {
                airLeft += airRecoverySpeed;
            }
            else
            {
                ReFillAirBar = false;
            }
        }
        airBar.value = airLeft;
    }

    public void AirChange(float Air, float maxAir)
    {
        airBar.maxValue = maxAir;
        airLeft = Air;
    }

}
