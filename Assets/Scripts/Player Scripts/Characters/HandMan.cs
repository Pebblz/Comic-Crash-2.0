using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandMan : MonoBehaviour
{
    [HideInInspector] public GameObject PickUp;
    [HideInInspector] public bool isHoldingOBJ = false;
    [SerializeField] int ThrowForce;
    private Transform Camera;
    Animator Anim;
    float pickUpTimer;
    void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        pickUpTimer -= Time.deltaTime;
    }
    public void Attack(InputAction.CallbackContext context)
    {

        if (PickUp == null)
        {
            GetComponent<PlayerAttack>().CanAttack = true;
            GetComponent<PlayerMovement>().canJump = true;
            isHoldingOBJ = false;
        }
        else
        {
            GetComponent<PlayerAttack>().CanAttack = false;
            GetComponent<PlayerMovement>().canJump = false;
            //this is for the throwing / droping logic
            if (pickUpTimer < 0)
            {
                PickUp.GetComponent<PickUpables>().DropInFront();
                PickUp = null;
                isHoldingOBJ = false;
            }
        }
    }
    public void PickUP(InputAction.CallbackContext context)
    {
        //this checks to see if the players picking up an obj
        if (PickUp == null && pickUpTimer < 0)
        {
            isHoldingOBJ = true;
            Vector3 start = this.gameObject.transform.position + new Vector3(0, .5f, 0);
            RaycastHit hit;
            for (float x = -.5f; x <= .5f; x += .5f)
            {
                for (float y = -4; y <= 4; y += .6f)
                {

                    if (Physics.Raycast(start, transform.TransformDirection(Vector3.forward) + new Vector3(x, y / 8, 0), out hit, 3.5f) && PickUp == null)
                    {
                        //this checks if any of the rays hit an object with pickupables script
                        if (hit.collider.gameObject.GetComponent<PickUpables>() != null)
                        {
                            isHoldingOBJ = true;
                            hit.collider.gameObject.GetComponent<PickUpables>().PickedUp(transform);
                            PickUp = hit.collider.gameObject;
                            pickUpTimer = 1;
                        }
                    }
                }
            }
        }
        else
        {
            ThrowGameObject();
        }
    }
    void ThrowGameObject()
    {
        if (Camera == null)
        {
            Camera = FindObjectOfType<Camera>().gameObject.transform;
        }
        if (PickUp != null)
        {
            transform.rotation = new Quaternion(transform.rotation.x, Camera.rotation.y, transform.rotation.z, transform.rotation.w);
            PickUp.GetComponent<Rigidbody>().AddForce(Camera.forward * ThrowForce, ForceMode.Impulse);
            PickUp.GetComponent<PickUpables>().Drop();
            isHoldingOBJ = false;
            PickUp = null;
        }
    }
}
