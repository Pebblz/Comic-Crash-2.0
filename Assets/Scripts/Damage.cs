using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Tooltip("How much damage the player would recive if hit")]
    [Range(1f,10f)]
    [SerializeField]
    int DamageTotal;

    [Tooltip("If player gets hit by this object would he need to get repositioned to a checkpoint")]
    [SerializeField]
    bool RepoPlayerWhenHit;
    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            col.GetComponent<PlayerHealth>().HurtPlayer(DamageTotal);
            //this here for if the player needs to get reset when he gets hit
            //for example this could be used if he falls out of bounds
            if (RepoPlayerWhenHit)
            {
                col.GetComponent<Player>().RepoPlayer();
            }
        }
    }
}
