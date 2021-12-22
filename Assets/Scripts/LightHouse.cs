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


    float progress;
    void Start()
    {
        //350
        progress = LightHouseTopY - LightHouseBottemY;

        progress /= 350;
        if (!AtTop)
        {
            transform.position = new Vector3(transform.position.x, LightHouseBottemY, transform.position.z);
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
        if (AtTop)
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
            if (Gear.eulerAngles.y >= 350)
            {
                AtTop = true;
            }
            else
            {
                //rotation
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Gear.eulerAngles.y * 2, transform.eulerAngles.z);
                //Going up
                progress = .00286f * Gear.eulerAngles.y;
                Mathf.Clamp(progress, 0, 1);
                 transform.position = Vector3.Lerp(new Vector3(transform.position.x, LightHouseBottemY, transform.position.z),
                    new Vector3(transform.position.x, LightHouseTopY, transform.position.z), progress);
            }
        }
    }
}
