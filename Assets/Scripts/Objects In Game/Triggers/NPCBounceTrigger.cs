using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBounceTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GetComponentInParent<NPCBounce>().collision = other.gameObject;
            GetComponentInParent<NPCBounce>().Triggered = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<NPCBounce>().collision = other.gameObject;
            GetComponentInParent<NPCBounce>().Triggered = true;
        }
    }
}
