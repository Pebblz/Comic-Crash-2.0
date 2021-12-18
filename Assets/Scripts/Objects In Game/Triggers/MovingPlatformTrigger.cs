using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformTrigger : MonoBehaviour
{
    MovingPlatforms Mplatform;
    WayPointPlatform Wplatform;
    GameObject player;
    void Start()
    {
        if (transform.GetComponentInParent<MovingPlatforms>())
        {
            Mplatform = transform.GetComponentInParent<MovingPlatforms>();
        }
        else
        {
            if (transform.GetComponentInParent<WayPointPlatform>())
            {
                Wplatform = transform.GetComponentInParent<WayPointPlatform>();
            }
        }
    }
    private void Update()
    {
        if(player == null)
        {
            if(Mplatform != null)
            {
                Mplatform.isSteppedOn = false;
            }
            if (Wplatform != null)
            {
                Wplatform.isSteppedOn = false;
            }
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if(Mplatform != null && col.gameObject.tag == "Player")
        {
            player = col.gameObject;
            Mplatform.isSteppedOn = true;
        }
        if (Wplatform != null && col.gameObject.tag == "Player")
        {
            player = col.gameObject;
            Wplatform.isSteppedOn = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (Mplatform != null && col.gameObject.tag == "Player")
        {
            player = null;
            Mplatform.isSteppedOn = false;
        }
        if (Wplatform != null && col.gameObject.tag == "Player")
        {
            player = null;
            Wplatform.isSteppedOn = false;
        }
    }
}
