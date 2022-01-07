using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class WaterBubble : MonoBehaviour
{
    [SerializeField, Tooltip("Amount of air gain when collecting bubble")]
    int AmountOfAirGained;

    [SerializeField, Tooltip("The speed of the bubble floating up")]
    float FloatUpSpeed;

    [SerializeField, Tooltip("Timer for the bubble till it gets destroyed")]
    float TimeTillBubbleIsDestroyed;

    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        //timer till bubble is destroyed;
        TimeTillBubbleIsDestroyed -= Time.deltaTime;
        //constently makes bubble go up
        transform.position = new Vector3(transform.position.x, transform.position.y + FloatUpSpeed, transform.position.z);
        //if bubble timer is less then zero destroy bubble 
        if (TimeTillBubbleIsDestroyed <= 0)
        {
            photonView.RPC("DestroyGameObject", RpcTarget.All);
        }
    }
    //if player collects bubble, he gains air and destroys bubble
    private void OnTriggerEnter(Collider col)
    {
        if(col.TryGetComponent<PlayerHealth>(out var player))
        {
            player.GainAir(AmountOfAirGained);
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
