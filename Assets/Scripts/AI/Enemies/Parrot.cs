using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrot : MonoBehaviour
{
    [SerializeField, Tooltip("Movement speed")] float moveSpeed = 3;
    [SerializeField, Tooltip("Flying up speed")] float UpSpeed = 5;
    [SerializeField, Tooltip("Rotation speed")] float rotSpeed = 3;
    [SerializeField, Tooltip("The Rigidbody")] Rigidbody rb;
    [SerializeField, Tooltip("Enemy detection")] EnemyDetection detection;

    List<GameObject> playersInSight;

    GameObject huntedPlayer;

    float beforeFallHeight;

    bool falling = false, goingUp = false;

    void Update()
    {
        if (!falling && !goingUp)
        {
            if (detection.IsPlayerInSphere() && playersInSight.Count == 0)
            {
                playersInSight = detection.GetPlayersInSphere();
                huntedPlayer = detection.FindClosestPlayer(playersInSight);
            }
            if (playersInSight.Count > 0 && huntedPlayer != null)
            {
                Movement();
            }
        }
        else
        {
            if(falling)
            {

            }
            if(goingUp)
            {
                if(transform.position.y < beforeFallHeight)
                {
                    rb.useGravity = false;
                    rb.velocity = (transform.up * UpSpeed);
                }
            }
        }
    }
    void Falling()
    {
        falling = true;
        rb.useGravity = true;
    }

    void Movement()
    {
        Vector3 playerPos = new Vector3(huntedPlayer.transform.position.x, transform.position.y, huntedPlayer.transform.position.z);

        if (!Dot(.99f, playerPos))
        {
            //Rotates slowly towards player
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(playerPos - transform.position),
                                                  rotSpeed * Time.deltaTime);

            //Moves enemy towards player
            rb.velocity = (transform.forward + new Vector3(0, rb.velocity.y, 0)) * moveSpeed;
        }
        else
        {
            beforeFallHeight = transform.position.y;
            Falling();
        }
    }

    bool Dot(float distAway, Vector3 OtherPos)
    {
        //the direction of the last seen pos 
        Vector3 dir = (OtherPos - transform.position).normalized;
        //this sees if the enemies looking at the last seen pos
        float dot = Vector3.Dot(dir, -transform.up);

        if (dot > distAway)
            return true;
        else
            return false;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerHealth>().HurtPlayer(1);
        }
        else if (!col.isTrigger)
        {
            falling = false;
            goingUp = true;
        }
    }
}
