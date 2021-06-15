using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : MonoBehaviour
{
    [SerializeField, Tooltip("If this is true you can only hit the button once and it wont be re-hittable")]
    bool HitOnce = true;

    [SerializeField, Tooltip("The Object that'll be getting changed based on the Action enum")]
    GameObject ImpactedGameobject;

    [SerializeField, Tooltip("The Object that'll be getting changed based on the Action enum")]
    WaysToActivate waysToActivate;

    [SerializeField, Tooltip("Change this to decide what you want to change with the lever switch")]
    Action action;

    [SerializeField, Tooltip("The gameobjects on the belt currently")]
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
            else if (!HitOnce)
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
            }
            else
            {
                TurnOff();
            }
        }
    }
    #region Main Functions
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
    #endregion
    #region Collision functions
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
    #endregion
    #region enums
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
    #endregion
}
