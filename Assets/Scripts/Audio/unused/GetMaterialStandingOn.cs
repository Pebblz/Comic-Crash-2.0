using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class GetMaterialStandingOn : MonoBehaviour
{

    [SerializeField]
    [Tooltip("It's time to take a piss")]
    bool amIPissing = false;
    [SerializeField]
    [Range(0.5f, 1.5f)]
    [Tooltip("Distance to raycast for foot step detection")]
    float footStepCheckDistance = 0.5f;


    public Material current_mat;
    private GameObject player;
    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

        }

        RaycastHit hit;
        if (Physics.Raycast(player.transform.position,
                           player.transform.TransformDirection(Vector3.down),
                           out hit,
                           footStepCheckDistance))
        {

            if (amIPissing)
            {
                Debug.DrawRay(player.transform.position + new Vector3(0, 2f, 0),
                              player.transform.TransformDirection(Vector3.down) * hit.distance * 10,
                              Color.yellow, 3f);
            }
            var filter = hit.collider.gameObject.GetComponent<MeshFilter>();
            Material[] mats = hit.collider.gameObject.GetComponent<MeshRenderer>().materials;
            if(mats.Length == 1)

                current_mat = mats[0];




        }
    }

}
