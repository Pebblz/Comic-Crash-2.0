using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<BlobBert>() )
        {
            col.GetComponent<PlayerHealth>().HurtPlayer(1);
        }
    }
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<BlobBert>() )
        {
            col.GetComponent<PlayerHealth>().HurtPlayer(1);
        }
    }
}
