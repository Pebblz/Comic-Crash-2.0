using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWalker : Enemy, IRespawnable
{
    [SerializeField]
    [Tooltip("The gameobject the mann is walking on")]
    GameObject walking_on;
    Vector3 starting_pos;
    public void reset_data()
    {
        throw new System.NotImplementedException();
    }

    protected override void Awake()
    {
        base.Awake();
        this.starting_pos = this.transform.position;

    }

    private void idle()
    {
        //RaycastHit hit;
       // Physics.Raycast(this.transform.position, walking_on.transform.position,);
    }
}
