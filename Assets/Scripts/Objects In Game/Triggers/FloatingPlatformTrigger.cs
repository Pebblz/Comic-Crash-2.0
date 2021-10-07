using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatformTrigger : MonoBehaviour
{
    FloatingPlatform ParentPlatform;
    void Start()
    {
        ParentPlatform = transform.GetComponentInParent<FloatingPlatform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            ParentPlatform.ChildTriggered = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
            ParentPlatform.ChildTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            ParentPlatform.ChildTriggered = false;
    }
}
