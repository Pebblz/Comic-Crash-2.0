using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    [SerializeField, Range(200, 1500), Tooltip("The speed at which the conveyer belt moves things")]
    float speed;

    [SerializeField, Tooltip("The direction the conveyer belt moves the number needs to be set to 1 or -1 to work correctly. 1 means it moves forward on the axis and -1 means it moves backwords on the axis")]
    public Vector3 direction;

    [SerializeField, Tooltip("The gameobjects on the belt currently")]
    List<GameObject> onBelt;

    [SerializeField, Tooltip("if this is set active the conveyer belt is working")]
    public bool active = true;

    void Update()
    {
        if (active)
        {
            //loops throught each of the gameobjects on the belt and moves them
            for (int i = 0; i <= onBelt.Count - 1; i++)
            {
                onBelt[i].GetComponent<Rigidbody>().velocity = speed * direction * Time.deltaTime;
            }
        }
    }
    #region Collsion methods
    private void OnCollisionEnter(Collision col)
    {
        //adds the objects on the conveyer belt to the belt list
        if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform")
        {
            onBelt.Add(col.gameObject);
        }
    }
    private void OnCollisionExit(Collision col)
    {
        //removes the objects on the conveyer belt list
        if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform")
        {
            onBelt.Remove(col.gameObject);
        }
    }
    #endregion
    //call this to make reverse the conveyer belts direction
    public void SwitchBeltDirection()
    {
        direction *= -1;
    }
}
