using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class EnemyDetection : MonoBehaviour
{
    [SerializeField]
    private float minimunDetectionAngle = -50, maximumDetectionAngle = 50, detectionRadius = 20;

    [SerializeField]
    private LayerMask detectionLayer;


    public bool IsPlayerInSight()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        for(int i = 0; i < colliders.Length; i++)
        {
            PlayerMovement movement = colliders[i].transform.GetComponent<PlayerMovement>();

            if(movement != null)
            {
                Vector3 targetDirection = movement.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if(viewableAngle > minimunDetectionAngle && viewableAngle < maximumDetectionAngle && !IsPlayerDead(movement.gameObject))
                {
                    return true;
                }
            }
        }
        return false;
    }

    //SinglePlayer
    public GameObject GetPlayerInSight()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            PlayerMovement movement = colliders[i].transform.GetComponent<PlayerMovement>();

            if (movement != null)
            {
                Vector3 targetDirection = movement.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > minimunDetectionAngle && viewableAngle < maximumDetectionAngle && !IsPlayerDead(movement.gameObject))
                {
                    return movement.gameObject;
                }
            }
        }
        return null;
    }

    //MultiPlayer
    public List<GameObject> GetPlayersInSight()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        List<GameObject> ReturnedList = new List<GameObject>();
        for (int i = 0; i < colliders.Length; i++)
        {
            PlayerMovement movement = colliders[i].transform.GetComponent<PlayerMovement>();

            if (movement != null)
            {
                Vector3 targetDirection = movement.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > minimunDetectionAngle && viewableAngle < maximumDetectionAngle && !IsPlayerDead(movement.gameObject))
                {
                    ReturnedList.Add(movement.gameObject);
                }
            }
            if (i == colliders.Length)
                return ReturnedList;
        }
        return ReturnedList;
    }

    public List<GameObject> FindAllPlayersInLevel()
    {
        List<GameObject> ReturnedList = new List<GameObject>();

        PlayerMovement[] temp = FindObjectsOfType<PlayerMovement>();

        foreach(PlayerMovement p in temp)
        {
            ReturnedList.Add(p.gameObject);
        }
        return ReturnedList;
    }

    public GameObject FindClosestPlayer(List<GameObject> detectedPlayers)
    {
        float closeDist = 0;
        GameObject closePlayer = null;
        foreach (GameObject g in detectedPlayers)
        {
            if (closeDist == 0 && !IsPlayerDead(g))
            {
                closeDist = Vector3.Distance(transform.position, g.transform.position);
                closePlayer = g;
            }
            else
            {
                if (closeDist > Vector3.Distance(transform.position, g.transform.position) && !IsPlayerDead(g))
                {
                    closeDist = Vector3.Distance(transform.position, g.transform.position);
                    closePlayer = g;
                }
            }
        }
        return closePlayer;
    }

    bool IsPlayerDead(GameObject player)
    {
        if (player.GetComponent<PlayerDeath>().isdead)
            return true;
        else
            return false;
    }
}
