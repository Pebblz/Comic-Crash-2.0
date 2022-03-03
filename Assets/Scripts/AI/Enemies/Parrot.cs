using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrot : MonoBehaviour
{
    [SerializeField,Tooltip("Movement speed")] float moveSpeed = 3;
    [SerializeField,Tooltip("Rotation speed")] float rotSpeed = 3;
    [SerializeField,Tooltip("The Rigidbody")] Rigidbody rb;

    bool falling = false, goingUp = false;

    void Update()
    {
        
    }
    void Falling()
    {
        rb.useGravity = true;
    }
    void GoingUp()
    {
        rb.useGravity = false;
    }
    bool Dot(float distAway, Vector3 OtherPos)
    {
        //the direction of the last seen pos 
        Vector3 dir = (OtherPos - transform.position).normalized;
        //this sees if the enemies looking at the last seen pos
        float dot = Vector3.Dot(dir, -transform.up);

        if (dot > distAway)
            return true;
        else
            return false;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerHealth>().HurtPlayer(1);
        }
        else if(!col.isTrigger)
        {
            falling = false;
            goingUp = true;
        }
    }
}
