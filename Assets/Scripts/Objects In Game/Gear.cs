using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gear : MonoBehaviour
{
    public bool RotateOnX;
    public bool RotateOnY;
    Rigidbody body;
    void Start()
    {
        body = GetComponent<Rigidbody>();

        body.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (!col.gameObject.GetComponent<HandMan>())
                body.constraints = RigidbodyConstraints.FreezeAll;
            else
            {
                if (RotateOnX)
                {
                    body.constraints = RigidbodyConstraints.None;

                    body.constraints = RigidbodyConstraints.FreezePosition | 
                        RigidbodyConstraints.FreezeRotationY | 
                        RigidbodyConstraints.FreezeRotationZ;

                    col.gameObject.GetComponent<PlayerMovement>().PlayAnimation("Pushing");
                }
                if (RotateOnY)
                {
                    body.constraints = RigidbodyConstraints.None;

                    body.constraints = RigidbodyConstraints.FreezePosition | 
                        RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationZ;

                    col.gameObject.GetComponent<PlayerMovement>().PlayAnimation("Pushing");
                }
            }
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            body.constraints = RigidbodyConstraints.FreezeAll;

            if(col.gameObject.GetComponent<HandMan>())
                col.gameObject.GetComponent<PlayerMovement>().StopAnimation("Pushing");
        }
    }
}
