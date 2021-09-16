using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterSurfaces : MonoBehaviour
{
    public bool walkingOn;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            walkingOn = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            walkingOn = false;
    }
}
