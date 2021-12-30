using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gear : MonoBehaviour
{ 
    [Header("Random Booleans")]
    public bool StopRotationAtMax;

    public bool AtMaxRotation;

    public bool AtMinRotation;

    public bool SpawnedItem;
    [Space(3)]

    [Header("X rotation")]
    public bool RotateOnX;
    [SerializeField]
    float maxRotationX, minRotationX;

    [Space(3)]

    [Header("X rotation")]
    public bool RotateOnY;
    [SerializeField]
    float maxRotationY, minRotationY;

    Rigidbody body;

    void Start()
    {
        if (GetComponent<MeshCollider>())
            GetComponent<MeshCollider>().convex = true;

        body = GetComponent<Rigidbody>();

        body.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void Update()
    {
        #region Y Bounds
        if (RotateOnY)
        {
            if(transform.rotation.y < minRotationY)
            {
                transform.rotation = new Quaternion(transform.rotation.x, minRotationY, transform.rotation.z, transform.rotation.w);
                AtMinRotation = true;
            }
            else
            {
                AtMinRotation = false;
            }
            if(transform.rotation.y > maxRotationX)
            {
                transform.rotation = new Quaternion(transform.rotation.x, maxRotationY, transform.rotation.z, transform.rotation.w);
                AtMaxRotation = true;
            }
            else
            {
                AtMaxRotation = false;
            }
        }
        #endregion
        #region X Bounds
        if (RotateOnX)
        {
            if (transform.rotation.x < minRotationX)
            {
                transform.rotation = new Quaternion(minRotationX,transform.rotation.y, transform.rotation.z, transform.rotation.w);
                AtMinRotation = true;
            }
            else
            {
                AtMinRotation = false;
            }
            if (transform.rotation.x > maxRotationX)
            {
                transform.rotation = new Quaternion(maxRotationX, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                AtMaxRotation = true;
            }
            else
            {
                AtMaxRotation = false;
            }
        }
        #endregion
    }
    #region Collision
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
    #endregion
}
