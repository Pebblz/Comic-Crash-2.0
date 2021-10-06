using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    public float HowFarDown;

    Vector3 startPos;

    bool GoUp;
    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        if (GoUp)
        {
            goUp();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.transform.position.y > transform.gameObject.transform.position.y)
        {
            goDown();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.transform.position.y > transform.gameObject.transform.position.y)
        {
            goDown();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GoUp = true;
        }
    }
    void goDown()
    {
        if (transform.position.y > startPos.y - HowFarDown)
        {
            this.gameObject.transform.Translate(new Vector3(0, -1 * Time.deltaTime * 1f, 0), Space.World);
        }
    }
    void goUp()
    {
        if (transform.position.y < startPos.y)
        {
            this.gameObject.transform.Translate(new Vector3(0, Time.deltaTime * 1f, 0), Space.World);
        }
        else
        {
            GoUp = false;
        }
    }
}
