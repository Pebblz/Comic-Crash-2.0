using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobBert : MonoBehaviour
{
    //[SerializeField,Tooltip("This is number the timer will get set to when you crouch. This is here so the player doesn't get stuck in the grate")] 
    //float CrouchTimer;

    //private float currentTimer;

    void Update()
    {
        ////if you crouch you start your crouch timer;
        //if(Input.GetKeyDown(KeyCode.C))
        //{
        //    currentTimer = CrouchTimer;
        //}
        //currentTimer -= Time.deltaTime;
    }
    private void OnCollisionEnter(Collision col)
    {
        //if blobert crouches he can pass through the grates 
        if (col.gameObject.tag == "Grate")
        {
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), col.gameObject.GetComponent<Collider>());
        }
    }
    
}
