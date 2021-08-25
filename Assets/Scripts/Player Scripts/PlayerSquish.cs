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
    private Vector3 origanalScale;
    private bool squishTime;
    private bool doneSquishing;
    PlayerGroundPound gp;
    // Start is called before the first frame update
    void Start()
    {
        origanalScale = transform.localScale;
        pm = GetComponent<PlayerMovement>();
        gp = GetComponent<PlayerGroundPound>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gp.GroundPounding && !pm.OnGround)
        {
            squishTime = true;
        }
        if(gp.GroundPounding)
        {
            squishTime = false;
        }
        if (squishTime && pm.OnGround)
        {
            Squish();
        }
    }
    void Squish()
    {

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
                doneSquishing = false;
                squishTime = false;
            }
        }
    }
}
