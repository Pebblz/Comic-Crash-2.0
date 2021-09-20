using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFish : MonoBehaviour
{
    [SerializeField, Range(0f, 40f), Tooltip("The amount the JellyFish will move up and down")]
    float DistanceToMoveY;

    [SerializeField, Range(1f, 10f), Tooltip("The speed at which the JellyFish moves")]
    float speed = 2;

    [SerializeField, Range(1f, 10f), Tooltip("The amount of damage done to the player")]
    float damage = 2;

    private bool GoBackY;

    private Vector3 StartPoint;

    private float EndPointY;
    private void Start()
    {
        StartPoint = transform.position;

        EndPointY = StartPoint.y + DistanceToMoveY;
    }
    private void Update()
    {
        UpAndDown();
    }
    void UpAndDown()
    {
        if (GoBackY)
        {
            this.gameObject.transform.Translate(new Vector3(0, -1 * Time.deltaTime * speed, 0), Space.World);
            if (this.gameObject.transform.position.y <= StartPoint.y)
            {
                GoBackY = false;
            }
        }
        else
        {

            this.gameObject.transform.Translate(new Vector3(0, 1 * Time.deltaTime * speed, 0), Space.World);
            if (this.gameObject.transform.position.y >= EndPointY)
            {
                GoBackY = true;
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

        Destroy(gameObject);
    }
}
