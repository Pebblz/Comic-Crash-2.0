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
        if (Ways.Contains(WaysToBreak.JumpOn))
        {
            if (col.gameObject.tag == "Player")
            {
                if (!col.gameObject.GetComponent<PlayerMovement>().IsGrounded() &&
                    col.gameObject.transform.position.y > gameObject.transform.position.y)
                {
                    DestroyBox();
                }
            }
        }
        if (Ways.Contains(WaysToBreak.JumpUnder))
        {
            if (!col.gameObject.GetComponent<PlayerMovement>().IsGrounded() &&
                col.gameObject.transform.position.y < gameObject.transform.position.y)
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
