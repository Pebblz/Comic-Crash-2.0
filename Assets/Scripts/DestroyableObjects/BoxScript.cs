using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BoxScript : MonoBehaviour
{
    [SerializeField] GameObject BrokenBox;
    [SerializeField] List<WaysToBreak> Ways;
    //this will replace the unbroken box with the broken box
    void DestroyBox()
    {
        Instantiate(BrokenBox, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    //make sure the trigger is set the the side that you want to collide with 
    private void OnTriggerEnter(Collider col)
    {
        if (Ways.Contains(WaysToBreak.JumpOn) || Ways.Contains(WaysToBreak.JumpUnder))
        {
            if (col.tag == "Player")
            {
                DestroyBox();
            }
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        if (Ways.Contains(WaysToBreak.Shoot) || Ways.Contains(WaysToBreak.Punch))
        {
            //this will check if the thing that hit it is a bullet
            if (col.gameObject.GetComponent<Bullet>() != null)
            {
                DestroyBox();
            }
        }
    }
    /// <summary>
    /// These are all the different ways that a box can get destroyed. Some boxes can get destroyed only in certain ways 
    /// </summary>
    enum WaysToBreak
    {
        JumpOn,
        JumpUnder,
        Shoot,
        Punch
    }
}
