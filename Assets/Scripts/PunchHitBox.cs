using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchHitBox : MonoBehaviour
{
    float DestroyTimer = .4f;
    void Update()
    {
        DestroyTimer -= Time.deltaTime;
        if (DestroyTimer <= 0)
            Destroy(gameObject);
    }       
}
