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
    [HideInInspector]
    public bool isSteppedOn;
    bool AtDestination = true;
    List<Rigidbody> rigidbodies = new List<Rigidbody>();
    Vector3 lastPos;
    Transform _transform;
    void Start()
    {
        _transform = transform;
        GoToFirstWayPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Move();

            if (rigidbodies.Count > 0)
            {
                for (int i = 0; i < rigidbodies.Count; i++)
                {
                    if (rigidbodies[i] != null)
                    {
                        Rigidbody rb = rigidbodies[i];
                        //Vector3 vel = Vector3.zero;
                        //if (MoveY)
                        //{
                        //    vel = new Vector3(rb.velocity.x * Time.deltaTime, _transform.position.y - lastPos.y, rb.velocity.z * Time.deltaTime);
                        //}
                        //if (MoveX)
                        //{
                        //    vel = new Vector3(_transform.position.x - lastPos.x, rb.velocity.y * Time.deltaTime, rb.velocity.z * Time.deltaTime);
                        //}
                        //if (MoveZ)
                        //{
                        //    vel = new Vector3(rb.velocity.x * Time.deltaTime, rb.velocity.y * Time.deltaTime, _transform.position.z - lastPos.z);
                        //}
                        Vector3 vel = new Vector3((_transform.position.x - lastPos.x) + ((rb.velocity.x * Time.deltaTime) / 2),
                                                  (_transform.position.y - lastPos.y) + ((rb.velocity.y * Time.deltaTime) / 2),
                                                  (_transform.position.z - lastPos.z) + ((rb.velocity.z * Time.deltaTime) / 2));
                        rb.transform.Translate(vel, transform);
                    }
                }
            }
            lastPos = _transform.position;

        }
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
    //private void MoveBack()
    //{
    //    if (!AtDestination)
    //    {
    //        if (Vector3.Distance(transform.position, NextPlatform.transform.position) > .1f)
    //        {
    //            float step = speed * Time.deltaTime;
    //            transform.position = Vector3.MoveTowards(transform.position, NextPlatform.transform.position, step);
    //        }
    //        else
    //        {
    //            AtDestination = true;
    //        }
    //    }
    //    else
    //    {
    //        if (ArrayIndex != 0)
    //        {
    //            NextPlatform = WaypointsToGoTo[ArrayIndex];
    //        }
    //        ArrayIndex--;
    //        AtDestination = false;
    //    }
    //}
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
            //isSteppedOn = true;
            active = true;
        }
        Rigidbody rb = col.collider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Add(rb);
        }
    }
    private void OnCollisionStay(Collision col)
    {
        Rigidbody rb = col.collider.GetComponent<Rigidbody>();
        if (rb != null && !rigidbodies.Contains(rb))
        {
            Add(rb);
        }
    }
    private void OnCollisionExit(Collision col)
    {
        Rigidbody rb = col.collider.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Remove(rb);
        }
    }
    void Add(Rigidbody rb)
    {
        if (!rigidbodies.Contains(rb))
            rigidbodies.Add(rb);
    }
    void Remove(Rigidbody rb)
    {
        if (rigidbodies.Contains(rb))
            rigidbodies.Remove(rb);
    }
    //private void OnCollisionExit(Collision col)
    //{
    //    if (col.gameObject.tag == "Player" && StepOnToActivate)
    //    {
    //        isSteppedOn = false;
    //    }
    //}
    #endregion
}
