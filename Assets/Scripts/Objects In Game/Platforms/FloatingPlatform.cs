using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    public float HowFarDown;

    Vector3 startPos;

    [SerializeField]
    float speed = .7f;

    bool GoUp;

    [HideInInspector]
    public bool ChildTriggered;
    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        if(ChildTriggered)
        {
            goDown();
        }
        else
        {
            GoUp = true;
        }

        if (GoUp)
        {
            goUp();
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player" && ChildTriggered)
    //    {
    //        goDown();
    //    }
    //}
    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player" && ChildTriggered)
    //    {
    //        goDown();
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        GoUp = true;
    //    }
    //}
    void goDown()
    {
        if (GoUp)
            GoUp = false;
        if (transform.position.y > startPos.y - HowFarDown)
        {
            this.gameObject.transform.Translate(new Vector3(0, -1 * Time.deltaTime * speed, 0), Space.World);
        }
    }
    void goUp()
    {
        if (transform.position.y < startPos.y)
        {
            this.gameObject.transform.Translate(new Vector3(0, Time.deltaTime * speed, 0), Space.World);
        }
        else
        {
            GoUp = false;
        }
    }
}
