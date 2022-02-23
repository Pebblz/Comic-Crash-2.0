using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class JunkProjectile : MonoBehaviour
{
    public Transform TargetObject;
    public float TargetRadius = 30f;
    public Rigidbody rb;
    public PhotonView photonView;
    bool launching;
    private Quaternion initialRotation;
    private void Start()
    {

        initialRotation = transform.rotation;
    }
    private void Update()
    {
        if(!launching)
            Launch();

        transform.rotation = Quaternion.LookRotation(rb.velocity) * initialRotation;
    }
    // launches the object towards the TargetObject with a given LaunchAngle
    void Launch()
    {
        photonView = GetComponent<PhotonView>();
        launching = true;
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetXZPos = new Vector3(TargetObject.position.x, 0.0f, TargetObject.position.z);

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float randomAngle = Random.Range(55, 65);
        float tanAlpha = Mathf.Tan(randomAngle * Mathf.Deg2Rad);
        float H = TargetObject.position.y - transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rb.velocity = globalVelocity;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerHealth>().HurtPlayer(1);
            photonView.RPC("DestroyGameObject", RpcTarget.All);
        }
        else if (col.gameObject.tag != "Enemy")
        {
            photonView.RPC("DestroyGameObject", RpcTarget.All);
        }
    }

    [PunRPC]
    void DestroyGameObject()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
