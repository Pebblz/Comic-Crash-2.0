using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDamage : MonoBehaviour
{
    [SerializeField]
    int damage;

    [SerializeField]
    bool damageOnHit;

    float timer = 3;

    bool startTimer;

    GameObject Player;

    private void Update()
    {
        if (startTimer)
            timer -= Time.deltaTime;
        if(timer < 0)
        {
            Player.GetComponent<PlayerHealth>().HurtPlayer(damage);
            startTimer = false;
            timer = 3;
        }

    }
    private void OnTriggerEnter(Collider col)
    {
        if (damageOnHit)
        {
            if (col.gameObject.GetComponent<BlobBert>())
            {
                col.GetComponent<PlayerHealth>().HurtPlayer(damage);
            }
        }
        else
        {
            Player = col.gameObject;
            startTimer = true;
        }
    }
    private void OnTriggerStay(Collider col)
    {
        if (damageOnHit)
        {
            if (col.gameObject.GetComponent<BlobBert>())
            {
                col.GetComponent<PlayerHealth>().HurtPlayer(damage);
            }
        }
        else
        {
            Player = col.gameObject;
            startTimer = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<BlobBert>())
        {
            startTimer = false;
            timer = 3;
            Player = null;
        }
    }
}
