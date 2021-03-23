using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelePorter : MonoBehaviour
{
    [SerializeField, Tooltip("The TelePorter that this teleporter will send you to")]
    GameObject OtherTelePorter;

    [SerializeField, Tooltip("If this is tagged true on one of the teleporters, it'll send you to the teleporter tagged true but wont send you back")]
    bool OneWay = false;

    private float TeleporterTimer;

    void Update()
    {
        TeleporterTimer -= Time.deltaTime;
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            if (TeleporterTimer < 0 && !OneWay)
            {
                OtherTelePorter.GetComponent<TelePorter>().TeleporterTimer = 2;
                col.transform.position = OtherTelePorter.transform.position;
            }
        }
    }
}
