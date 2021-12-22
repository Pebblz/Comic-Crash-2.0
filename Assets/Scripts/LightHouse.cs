using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHouse : MonoBehaviour
{
    [SerializeField]
    Transform Gear;

    bool AtTop;

    bool DoneTurningOn;

    [SerializeField]
    GameObject[] ObjectToTurnOn;

    [SerializeField]
    float LightHouseBottemY = 39.5f;

    [SerializeField]
    float LightHouseTopY = 66.5f;

    float distanceToMovePerGearRot;
    void Start()
    {
        if (!AtTop)
        {
            transform.position =new Vector3(transform.position.x, LightHouseBottemY, transform.position.z);
            for (int i = 0; i < ObjectToTurnOn.Length; i++)
            {
                ObjectToTurnOn[i].SetActive(false);
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, LightHouseTopY, transform.position.z);
            for (int i = 0; i < ObjectToTurnOn.Length; i++)
            {
                if (ObjectToTurnOn[i] != null)
                    ObjectToTurnOn[i].SetActive(true);
                if (i == ObjectToTurnOn.Length)
                    DoneTurningOn = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(AtTop)
        {
            transform.position = new Vector3(transform.position.x, LightHouseTopY, transform.position.z);
            if (!DoneTurningOn)
            {
                for (int i = 0; i < ObjectToTurnOn.Length; i++)
                {
                    if (ObjectToTurnOn[i] != null)
                        ObjectToTurnOn[i].SetActive(true);
                    if (i == ObjectToTurnOn.Length)
                        DoneTurningOn = true;
                }
            }
        }
        else
        {
            if(Gear.eulerAngles.y >= 350)
            {
                AtTop = true;
            }
            else
            {
                //rotation

                //Going up
            }
        }
    }
}
