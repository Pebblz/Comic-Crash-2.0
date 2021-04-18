using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Vector3 respawnPoint;

    Quaternion rotation;
    GameObject Camera;
    Rigidbody rigidbody;
    [SerializeField] float MaxVelocity;
    #region MonoBehaviours
    void Start()
    {
        rotation = transform.rotation;
        if (respawnPoint == new Vector3(0, 0, 0))
            respawnPoint = gameObject.transform.position;
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, MaxVelocity);
        if (transform.parent != null)
        {
            transform.rotation = new Quaternion(rotation.x, transform.rotation.y, rotation.z, transform.rotation.w);
        }
        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
    }
    #endregion

    public void RepoPlayer()
    {
        Camera.GetComponent<Camera>().ResetCamera();
        transform.position = respawnPoint;
    }
    public void PushPlayer(Vector3 direction, float power)
    {
        if(!GetComponent<PlayerMovement>().IsGrounded())
        {
            GetComponent<Rigidbody>().velocity = direction * (power / 2);
        }
        else
        {
            GetComponent<Rigidbody>().velocity = direction * power;
        }
    }
}
