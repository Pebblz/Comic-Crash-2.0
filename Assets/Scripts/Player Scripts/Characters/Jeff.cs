using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class Jeff : MonoBehaviour
{

    void Update()
    {
        if (InputManager.GetButtonDown("Right Mouse"))
        {
            Roll();
        }
        if (InputManager.GetButtonDown("Right Mouse"))
        {
            BackToNormal();
        }
    }

    void Roll()
    {
       // GetComponent<PlayerMovement>().Roll = true;
    }
    void BackToNormal()
    {
        //GetComponent<PlayerMovement>().Roll = false;
    }
}
