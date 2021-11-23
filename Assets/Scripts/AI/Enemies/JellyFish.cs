using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class JellyFish : MonoBehaviour
{
    [SerializeField, Range(0f, 40f), Tooltip("The amount the JellyFish will move up and down")]
    float DistanceToMoveY;

    [SerializeField, Range(0f, 10f), Tooltip("The speed at which the JellyFish moves")]
    float MaxSpeed = 2;

    float speed;

    public float acceleration;
    [SerializeField, Range(1f, 10f), Tooltip("The amount of damage done to the player")]
    float damage = 2;
    [SerializeField]
    float TimeToWait;
    private bool GoBackY;

    private Vector3 StartPoint;

    private float EndPointY;

    private float Timer;

    PhotonView photonView;
    private void Start()
    {
        StartPoint = transform.position;

        EndPointY = StartPoint.y + DistanceToMoveY;

        photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (speed < MaxSpeed && Timer <= 0)
        {
            speed += acceleration * Time.deltaTime;
        }

        UpAndDown();
        if(Timer > 0)
            Timer -= Time.deltaTime;
    }
    void UpAndDown()
    {
        if (Timer <= 0)
        {
            if (GoBackY)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
                if (this.gameObject.transform.position.y <= StartPoint.y)
                {
                    GoBackY = false;
                    Timer = TimeToWait;
                    speed = 0;
                }
            }
            else
            {

                transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
                if (this.gameObject.transform.position.y >= EndPointY)
                {
                    GoBackY = true;
                    Timer = TimeToWait;
                    speed = 0;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
            BlowUp(col.gameObject);
    }
    void BlowUp(GameObject player)
    {
        player.GetComponent<PlayerHealth>().HurtPlayer((int)damage);

        //play particle effect
        if(photonView.IsMine)
            photonView.RPC("DestroyJelly", RpcTarget.All);
    }
    [PunRPC]
    void DestroyJelly()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
