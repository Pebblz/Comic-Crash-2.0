using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMaterialStandingOn : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 3f)]
    [Tooltip("Distance to raycast for foot step detection")]
    float footStepCheckDistance;
    private GameObject player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {   
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

        }
        print("Am I grounded? " + player.GetComponent<PlayerMovement>().OnGround);
        if (player.GetComponent<PlayerMovement>().OnGround)
        {
            RaycastHit hit;
            if(Physics.Raycast(player.transform.position, 
                               player.transform.TransformDirection(Vector3.down),
                               out hit,
                               footStepCheckDistance)){
                Debug.Log("AAAAA it hit");
                Debug.DrawRay(player.transform.position + new Vector3(0, 2f,0),
                              player.transform.TransformDirection(Vector3.down) * hit.distance *10,
                              Color.yellow, 3f);

                Material mat = hit.collider.gameObject.GetComponent<Renderer>().material;
    
                Debug.Log($"Material Standing on: {mat.name}");
                

            
            }
        }
    }
}
