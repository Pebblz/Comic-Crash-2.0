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
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponent<BoxScript>())
        {
            print("Nugget");
            col.gameObject.GetComponent<BoxScript>().CheckPunchToSeeIfItShouldBreak();
        }
    }
}
