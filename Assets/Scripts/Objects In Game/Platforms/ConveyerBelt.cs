using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    [SerializeField, Range(.01f, 1f)]
    float collsionOffset;

    [SerializeField, Range(200, 1500), Tooltip("The speed at which the conveyer belt moves things")]
    float speed;

    [SerializeField, Tooltip("The direction the conveyer belt moves the number needs to be set to 1 or -1 to work correctly. 1 means it moves forward on the axis and -1 means it moves backwords on the axis")]
    public Vector3 direction;

    [SerializeField, Tooltip("The gameobjects on the belt currently")]
    List<GameObject> onBelt;

    [Tooltip("if this is set active the conveyer belt is working")]
    public bool active = true;

    Pause pause;

    private void Start()
    {
        pause = FindObjectOfType<Pause>();
    }

    void Update()
    {
        if (active && !pause.isPaused)
        {
            //loops throught each of the gameobjects on the belt and moves them
            for (int i = 0; i < onBelt.Count; i++)
            {
                if(onBelt[i] == null)
                {
                    onBelt.Remove(onBelt[i]);
                    if (onBelt[i].GetComponent<PlayerMovement>())
                        onBelt[i].GetComponent<PlayerMovement>().onBelt = false;
                    return;
                }
                //i had to add all this extra stuff so you can jump off of the belts
                if (direction == new Vector3(1, 0, 0) || direction == new Vector3(-1, 0, 0))
                {
                    onBelt[i].GetComponent<Rigidbody>().velocity = new Vector3 (speed * direction.x * Time.deltaTime + onBelt[i].GetComponent<Rigidbody>().velocity.x,
                        onBelt[i].GetComponent<Rigidbody>().velocity.y, onBelt[i].GetComponent<Rigidbody>().velocity.z);
                }
                if(direction == new Vector3(0, 0, 1) || direction == new Vector3(0, 0, -1))
                {
                    onBelt[i].GetComponent<Rigidbody>().velocity = new Vector3(onBelt[i].GetComponent<Rigidbody>().velocity.x,
                        onBelt[i].GetComponent<Rigidbody>().velocity.y, speed * direction.z * Time.deltaTime + onBelt[i].GetComponent<Rigidbody>().velocity.z);
                }
            }
        }
    }
    #region Collsion methods
    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.position.y > transform.position.y + collsionOffset)
        {
            //adds the objects on the conveyer belt to the belt list
            if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform" && !onBelt.Contains(col.gameObject))
            {
                onBelt.Add(col.gameObject);
                if (col.gameObject.GetComponent<PlayerMovement>())
                    col.gameObject.GetComponent<PlayerMovement>().onBelt = true;
            }
        }
    }
    private void OnCollisionStay(Collision col)
    {
        if (col.transform.position.y > transform.position.y + collsionOffset)
        {
            //adds the objects on the conveyer belt to the belt list
            if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform" && !onBelt.Contains(col.gameObject))
            {
                onBelt.Add(col.gameObject);
                if (col.gameObject.GetComponent<PlayerMovement>())
                    col.gameObject.GetComponent<PlayerMovement>().onBelt = true;
            }
        }
    }
    private void OnCollisionExit(Collision col)
    {
        //removes the objects on the conveyer belt list
        if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform")
        {
            onBelt.Remove(col.gameObject);
            if (col.gameObject.GetComponent<PlayerMovement>())
                col.gameObject.GetComponent<PlayerMovement>().onBelt = false;
        }
    }
    #endregion
    //call this to make reverse the conveyer belts direction
    public void SwitchBeltDirection()
    {
        direction *= -1;
    }
}
