using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterFog : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 4)
        {
            var obj = FindObjectOfType<ChangeCinemachineInput>();

            obj.Underwater();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            var obj = FindObjectOfType<ChangeCinemachineInput>();

            obj.NotUnderwater();
        }
    }
}
