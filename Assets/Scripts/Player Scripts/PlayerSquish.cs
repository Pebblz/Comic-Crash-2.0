using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquish : MonoBehaviour
{
    [SerializeField, Range(.1f, 5f)]
    float SquishWidth;
    [SerializeField, Range(.1f, 5f)]
    float SquishHeight;
    PlayerMovement pm;
    [SerializeField, Range(.1f, 6f)]
    float timeToSquish;
    [SerializeField, Range(.01f, 1f)]
    float TimeInAirToSquish;
    private Vector3 origanalScale;
    private bool squishTime;
    private bool doneSquishing;
    PlayerGroundPound gp;
    bool DoingSquish;
    float TimeInAir;
    // Start is called before the first frame update
    void Start()
    {
        origanalScale = transform.localScale;
        pm = GetComponent<PlayerMovement>();

        if(GetComponent<PlayerGroundPound>())
            gp = GetComponent<PlayerGroundPound>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PlayerGroundPound>())
        {
            if (!gp.GroundPounding && !pm.OnGround)
            {
                //this is so if jeff falls 1 inch he shouldn't squish
                //but if he falls a few feet he will
                TimeInAir += Time.deltaTime;
                if (TimeInAir > TimeInAirToSquish)
                    squishTime = true;
            }
            if (gp.GroundPounding)
            {
                squishTime = false;
            }
        }
        if(!pm.OnGround)
        {
            //this is so if jeff falls 1 inch he shouldn't squish
            //but if he falls a few feet he will
            TimeInAir += Time.deltaTime;
            if (TimeInAir > TimeInAirToSquish)
                squishTime = true;
        }
        if (pm.OnGround)
            TimeInAir = 0f;


        if (squishTime && pm.OnGround)
        {
            Squish();
        }
        //this is here to make sure he unSquishes
        //because if he jumps when he is unSquishing
        //he wont fully unSquish until he gets grounded again
        if(DoingSquish && !pm.OnGround)
        {
            Squish();
        }
    }
    void Squish()
    {
        DoingSquish = true;
        if (!doneSquishing)
        {
            if (transform.localScale.y > origanalScale.y / SquishHeight)
            {
                transform.localScale -= new Vector3(-SquishWidth, timeToSquish, -SquishWidth) * Time.deltaTime;
            }
            else
            {
                doneSquishing = true;
            }
        }
        else
        {
            if (transform.localScale.y < origanalScale.y)
            {
                transform.localScale += new Vector3(-SquishWidth, timeToSquish, -SquishWidth) * Time.deltaTime;
            }
            else
            {
                DoingSquish = false;
                doneSquishing = false;
                squishTime = false;
            }
        }
    }
}
