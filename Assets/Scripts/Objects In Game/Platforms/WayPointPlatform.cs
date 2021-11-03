using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPlatform : MonoBehaviour
{
    [SerializeField, Tooltip("The waypoints that the platform fallows")] 
    GameObject[] WayPoints;

    [SerializeField,Tooltip("The different ways the platform loops")] 
    WayToLoop wayToLoop;

    [SerializeField, Tooltip("The Speed of the platform")] 
    float speed;

    [Tooltip("If this is true the moving platforms active and moving")]
    public bool active = true;

    [SerializeField, Tooltip("If you want it to activate when step on")]
    bool StepOnToActivate;

    GameObject[] WaypointsToGoTo;
    GameObject NextPlatform;
    int ArrayIndex = 0;
    private bool isSteppedOn;
    bool AtDestination = true;
    void Start()
    {
        GoToFirstWayPoint();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void Move()
    {
        if (!AtDestination)
        {
            if (Vector3.Distance(transform.position, NextPlatform.transform.position) > .1f)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, NextPlatform.transform.position, step);
            }
            else
            {
                AtDestination = true;
            }
        }
        else
        {
            if (ArrayIndex != WaypointsToGoTo.Length)
            {
                NextPlatform = WaypointsToGoTo[ArrayIndex];
            }
            else
            {
                if (wayToLoop == WayToLoop.GoBackward)
                {
                    GoBackward();
                }
                if (wayToLoop == WayToLoop.GoToFirstWaypoint)
                {
                    GoToFirstWayPoint();
                }
                NextPlatform = WaypointsToGoTo[0];
                ArrayIndex = 0;
            }
            ArrayIndex++;
            AtDestination = false;
        }
    }
    private void MoveBack()
    {
        if (!AtDestination)
        {
            if (Vector3.Distance(transform.position, NextPlatform.transform.position) > .1f)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, NextPlatform.transform.position, step);
            }
            else
            {
                AtDestination = true;
            }
        }
        else
        {
            if (ArrayIndex != 0)
            {
                NextPlatform = WaypointsToGoTo[ArrayIndex];
            }
            ArrayIndex--;
            AtDestination = false;
        }
    }
    void GoBackward()
    {
        System.Array.Reverse(WaypointsToGoTo);
    }
    void GoToFirstWayPoint()
    {
        WaypointsToGoTo = WayPoints;
    }
    enum WayToLoop
    {
        GoBackward,
        GoToFirstWaypoint
    }
    #region collision functions
    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Player" && StepOnToActivate
            && col.transform.position.y > transform.position.y)
        {
            isSteppedOn = true;
        }

    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player" && StepOnToActivate)
        {
            isSteppedOn = false;
        }
    }
    #endregion
}
