using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class HandMan : MonoBehaviour
{
    [HideInInspector] public GameObject PickUp;
    [HideInInspector] public bool isHoldingOBJ = false;
    [SerializeField] int ThrowForce;
    private Transform Camera;
    float pickUpTimer;
    PlayerSquish squish;
    PlayerMovement movement;
    PlayerAttack pAttack;
    PhotonView photonView;
    [SerializeField]
    float HeavyObjectPushSpeed = .5f;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        squish = GetComponent<PlayerSquish>();
        pAttack = GetComponent<PlayerAttack>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        pickUpTimer -= Time.deltaTime;
        if (photonView.IsMine)
        {
            if (InputManager.GetButtonDown("Right Mouse") && pickUpTimer < 0)
            {
                //this checks to see if the players picking up an obj
                if (PickUp == null)
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
                                    pickUpTimer = .7f;
                                }
                            }
                        }
                    }
                }

            }
            if (!movement.OnGround)
                movement.StopAnimation("Pushing");

            if (PickUp == null)
            {
                squish.enabled = true;
                pAttack.CanAttack = true;
                movement.CanWallJump = true;
                isHoldingOBJ = false;
            }
            else
            {
                squish.enabled = false;
                pAttack.CanAttack = false;
                movement.CanWallJump = false;
                //this is for the throwing / droping logic
                if (InputManager.GetButtonDown("Left Mouse") && pickUpTimer < 0)
                {
                    if (GetComponent<Rigidbody>().velocity.x == 0 && GetComponent<Rigidbody>().velocity.z == 0)
                    {
                        PickUp.GetComponent<PickUpables>().DropInFront();
                        PickUp = null;
                        isHoldingOBJ = false;
                    }
                }
                if (InputManager.GetButtonDown("Right Mouse") && pickUpTimer <= 0)
                {
                    if (GetComponent<Rigidbody>().velocity.x == 0 && GetComponent<Rigidbody>().velocity.z == 0)
                    {
                        ThrowGameObject();
                    }
                }
            }
        }
        if (InputManager.GetAxis("Horizontal") != 0 && movement.OnGround ||
            InputManager.GetAxis("Vertical") != 0 && movement.OnGround)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.gameObject.transform.position + new Vector3(0, .3f, 0), transform.TransformDirection(Vector3.forward) + new Vector3(0, .3f, 0), out hit, .7f))
            {
                if (hit.collider.gameObject.tag == "HeavyGearRight" && hit.collider.transform.parent.eulerAngles.y + .5f <= 350)
                {
                    movement.PlayAnimation("Pushing");
                    Transform hitTransform = hit.collider.transform.parent;
                    hitTransform.Rotate(hitTransform.rotation.x,
                        hitTransform.rotation.y + .5f, hitTransform.rotation.z);
                }
                if (hit.collider.gameObject.tag == "HeavyGearLeft" && hit.collider.transform.parent.eulerAngles.y - .5f > 0)
                {
                    movement.PlayAnimation("Pushing");
                    Transform hitTransform = hit.collider.transform.parent;
                    hitTransform.Rotate(hitTransform.rotation.x,
                        hitTransform.rotation.y - .5f, hitTransform.rotation.z);
                }
            }
            else
            {
                movement.StopAnimation("Pushing");
            }
        }

    }
    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (InputManager.GetAxis("Horizontal") != 0 && movement.OnGround ||
                InputManager.GetAxis("Vertical") != 0 && movement.OnGround)
            {
                RaycastHit hit;
                if (Physics.Raycast(this.gameObject.transform.position + new Vector3(0, .3f, 0), transform.TransformDirection(Vector3.forward) + new Vector3(0, .3f, 0), out hit, .7f))
                {
                    if (hit.collider.gameObject.tag == "HeavyGearRight" && hit.collider.transform.parent.eulerAngles.y + .5f <= 350)
                    {
                        movement.PlayAnimation("Pushing");
                        Transform hitTransform = hit.collider.transform.parent;
                        hitTransform.Rotate(hitTransform.rotation.x,
                            hitTransform.rotation.y + .5f, hitTransform.rotation.z);
                    }
                    if (hit.collider.gameObject.tag == "HeavyGearLeft" && hit.collider.transform.parent.eulerAngles.y - .5f > 0)
                    {
                        movement.PlayAnimation("Pushing");
                        Transform hitTransform = hit.collider.transform.parent;
                        hitTransform.Rotate(hitTransform.rotation.x,
                            hitTransform.rotation.y - 1f, hitTransform.rotation.z);
                    }
                }
                else
                {
                    movement.StopAnimation("Pushing");
                }
            }
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
    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "HeavyObject" && movement.OnGround)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.gameObject.transform.position + new Vector3(0, .3f, 0), transform.TransformDirection(Vector3.forward) + new Vector3(0, .3f, 0), out hit, 1f))
            {
                if (hit.collider.gameObject.tag == "HeavyObject")
                {
                    movement.PlayAnimation("Pushing");
                    var pushDir = transform.forward;
                    hit.collider.attachedRigidbody.velocity = pushDir * HeavyObjectPushSpeed;
                }
            }
            else
            {
                movement.StopAnimation("Pushing");
            }
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "HeavyObject")
        {
            movement.StopAnimation("Pushing");
        }
    }
}
