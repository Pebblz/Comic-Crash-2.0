using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SpiderGoop : Enemy
{

    [SerializeField]
    [Tooltip("Distance Enemy has to be from a point to start targeting the next")]
    float point_dist = 0.3f;
    [SerializeField]
    PatrolPath path;
    private NavMeshAgent agent;
    Transform patrol_point;

    private void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        patrol_point = path.getNextPoint();
    }


    // Update is called once per frame
    void Update()
    {
        idle();
    }

    void idle()
    {
        if (patrol_point == null)
            patrol_point = path.getNextPoint();
        if (Vector3.Distance(this.transform.position, patrol_point.position) <= point_dist)
            patrol_point = path.getNextPoint();

        Quaternion lookRot = Quaternion.LookRotation(patrol_point.position - transform.position);
        Quaternion rot = Quaternion.Lerp(this.transform.rotation, lookRot, Time.deltaTime);
        this.transform.rotation = rot;
        agent.SetDestination(patrol_point.position);
    }
}
