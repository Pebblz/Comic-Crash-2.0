using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPlatform : MonoBehaviour
{
    [SerializeField] GameObject[] WayPoints;
    [SerializeField] WayToLoop wayToLoop;
    [SerializeField] float speed;
    GameObject[] WaypointsToGoTo;
    GameObject NextPlatform;
    int ArrayIndex = 0;

    bool AtDestination = true;
    void Start()
    {
        GoToFirstWayPoint();
    }

    // Update is called once per frame
    void Update()
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
}
