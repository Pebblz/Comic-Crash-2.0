using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanScript : MonoBehaviour
{
    [SerializeField]
    Transform Gear;

    [SerializeField]
    float OceanBottemY = 39.5f;

    //bool GoingUp;

    //float lastProgress;

    [SerializeField]
    float OceanTopY = 66.5f;


    float progress;
    void Start()
    {
        //350
        progress = OceanTopY - OceanBottemY;

        //lastProgress = progress;

        progress /= 350;
    }

    // Update is called once per frame
    void Update()
    {
        //if(lastProgress > progress)
        //{
        //    GoingUp = false;
        //    lastProgress = progress;
        //}
        //if(lastProgress < progress)
        //{
        //    GoingUp = true;
        //    lastProgress = progress;
        //}
        progress = .00286f * Gear.eulerAngles.y;
                Mathf.Clamp(progress, -1, 1);
        //if (!GoingUp)
        //{
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, OceanBottemY, transform.position.z),
               new Vector3(transform.position.x, OceanTopY, transform.position.z), progress);
        //}
        //else
        //{
        //    transform.position = Vector3.Lerp(new Vector3(transform.position.x, OceanTopY , transform.position.z),
        //    new Vector3(transform.position.x, OceanBottemY, transform.position.z), progress);
        //}
    }
}
