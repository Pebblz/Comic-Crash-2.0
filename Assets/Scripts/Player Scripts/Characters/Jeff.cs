using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jeff : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Roll();
        }
        if (Input.GetMouseButtonUp(1))
        {
            BackToNormal();
        }
    }

    void Roll()
    {
        GetComponent<PlayerMovement>().Roll = true;
    }
    void BackToNormal()
    {
        GetComponent<PlayerMovement>().Roll = false;
    }
}
