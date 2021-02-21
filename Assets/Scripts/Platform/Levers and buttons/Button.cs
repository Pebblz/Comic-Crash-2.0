using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Tooltip("If this is true you can only hit the button once and it wont be re-hittable")]
    [SerializeField]
    bool HitOnce = true;

    [Tooltip("The Object that'll be getting changed based on the Action enum")]
    [SerializeField]
    GameObject ImpactedGameobject;


    [Tooltip("The Object that'll be getting changed based on the Action enum")]
    [SerializeField]
    WaysToActivate waysToActivate;

    [Tooltip("Change this to decide what you want to change with the lever switch")]
    [SerializeField]
    Action action;

    [Tooltip("The gameobjects on the belt currently")]
    [SerializeField]
    List<GameObject> onButton;

    private bool active = false;


    // Update is called once per frame
    void Update()
    {
        if (waysToActivate == WaysToActivate.WalkOn)
        {
            if (onButton.Count > 0)
            {
                active = true;
            }
            else if(!HitOnce)
            {
                active = false;
            }
            DoAction();
        }
        else if (waysToActivate == WaysToActivate.Shoot)
        {
            if (active)
            {
                DoAction();
            } else
            {
                TurnOff();
            }
        }
    }

    void DoAction()
    {
        if (action == Action.ConveyerBelt)
        {
            if (active)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }
        else if (action == Action.movingPlatform)
        {
            if (active)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }
        else if (action == Action.rotationPlatform)
        {
            if (active)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }
    }

    void TurnOn()
    {
        if (action == Action.ConveyerBelt)
        {
            ImpactedGameobject.GetComponent<ConveyerBelt>().active = true;
        }
        else if (action == Action.movingPlatform)
        {

            ImpactedGameobject.GetComponent<MovingPlatforms>().active = true;

        }
        else if (action == Action.rotationPlatform)
        {

            ImpactedGameobject.GetComponent<RotatingPlatform>().active = true;

        }
    }
    void TurnOff()
    {
        if (action == Action.ConveyerBelt)
        {
            ImpactedGameobject.GetComponent<ConveyerBelt>().active = false;
        }
        else if (action == Action.movingPlatform)
        {
            ImpactedGameobject.GetComponent<MovingPlatforms>().active = false;
        }
        else if (action == Action.rotationPlatform)
        {
            ImpactedGameobject.GetComponent<RotatingPlatform>().active = false;
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        if (waysToActivate == WaysToActivate.WalkOn)
        {
            if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform")
            {
                onButton.Add(col.gameObject);
            }
        }
        if (waysToActivate == WaysToActivate.Shoot)
        {
            if (col.gameObject.tag == "Shot" && HitOnce)
            {
                active = true;
            }
            if (col.gameObject.tag == "Shot" && !HitOnce)
            {
                active = !active;
            }
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (waysToActivate == WaysToActivate.WalkOn)
        {
            if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform")
            {
                onButton.Remove(col.gameObject);
            }
        }
    }
    enum WaysToActivate
    {
        WalkOn,
        Shoot
    }
    enum Action
    {
        ConveyerBelt,
        movingPlatform,
        rotationPlatform
    }
}
