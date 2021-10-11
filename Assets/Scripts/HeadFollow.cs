using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HeadFollow : MonoBehaviour
{
    GameObject player;
    public GameObject head;
    Quaternion headStartingRot;
    bool lookAt;
    [SerializeField] float rotateSpeed;
    [SerializeField] float HeadYOffset;
    private void Start()
    {
        headStartingRot = head.transform.rotation;
    }
    void Update()
    {
        if (player == null)
        {
            //this needs to be here because the player can switch characters
            player = PhotonFindCurrentClient();
        }
        if (player.GetComponent<PhotonView>().IsMine)
        {
            //this checks if any of the rays hit an object with pickupables script
            if (lookAt)
            {
                head.transform.rotation = lookAtSlowly(head.transform, player.transform.position + new Vector3(0, HeadYOffset, 0), rotateSpeed);
            }
            else
            {
                head.transform.rotation = lookAtSlowly(head.transform, headStartingRot, rotateSpeed);
            }
        }

    }
    Quaternion lookAtSlowly(Transform t, Vector3 target, float speed)
    {
        Vector3 relativePos = target - t.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        return Quaternion.Lerp(t.rotation, toRotation, speed * Time.deltaTime);
    }
    Quaternion lookAtSlowly(Transform t, Quaternion targetRot, float speed)
    {
        //Vector3 relativePos = target - t.position;
        //Quaternion toRotation = Quaternion.LookRotation(relativePos);
        return Quaternion.Lerp(t.rotation, targetRot, speed * Time.deltaTime);
    }
    GameObject PhotonFindCurrentClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject g in players)
        {
            if (g.GetComponent<PhotonView>().IsMine)
                return g;
        }
        return null;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (player.GetComponent<PhotonView>().IsMine)
        {
            if (col.gameObject.tag == "Player")
            {
                lookAt = true;
            }
        }
    }
    private void OnTriggerStay(Collider col)
    {
        if (player.GetComponent<PhotonView>().IsMine)
        {
            if (col.gameObject.tag == "Player")
            {
                lookAt = true;
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (player.GetComponent<PhotonView>().IsMine)
        {
            if (col.gameObject.tag == "Player")
            {
                lookAt = false;
            }
        }
    }
}
