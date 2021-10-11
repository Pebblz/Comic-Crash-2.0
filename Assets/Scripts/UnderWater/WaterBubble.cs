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

    private void Update()
    {
        //timer till bubble is destroyed;
        TimeTillBubbleIsDestroyed -= Time.deltaTime;
        //constently makes bubble go up
        transform.position = new Vector3(transform.position.x, transform.position.y + FloatUpSpeed, transform.position.z);
        //if bubble timer is less then zero destroy bubble 
        if (TimeTillBubbleIsDestroyed <= 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    //if player collects bubble, he gains air and destroys bubble
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerHealth>().GainAir(AmountOfAirGained);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
