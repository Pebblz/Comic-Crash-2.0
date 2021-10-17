using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletScript : MonoBehaviour
{
    public GameObject target;

    [SerializeField]
    [Tooltip("Speed of the bullet")]
    float speed = 2f;

    [SerializeField]
    [Tooltip("The damage the bullet does")]
    int damage = 1;

    [SerializeField]
    [Tooltip("Time out until bullet gets destroyed")]
    float timeout = 1f;
    float init_timeout;

    bool aimed = false;
    PhotonView photonView;

    private void Awake()
    {
        init_timeout = timeout;
        photonView = GetComponent<PhotonView>();
       
    }

    private void Update()
    {
        if (target != null)
        {
            if (aimed == false)
            {
                this.transform.LookAt(target.transform);
                aimed = true;
            }

            this.GetComponent<Rigidbody>().AddForce(transform.forward * speed, ForceMode.Impulse);
            timeout -= Time.deltaTime;
            if (timeout <= 0f)
            {
                photonView.RPC("DestroyGameObject", RpcTarget.All);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().HurtPlayer(damage);
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
