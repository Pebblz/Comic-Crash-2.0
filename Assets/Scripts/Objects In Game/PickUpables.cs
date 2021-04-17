using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpables : MonoBehaviour
{
    [HideInInspector] public bool IsPickedUp;
    [HideInInspector] public Transform PlayerTransform;
    [SerializeField] float OffsetFromPlayerHead;

    // Update is called once per frame
    void Update()
    {
        if(IsPickedUp)
        {
            SetOnTopOfPlayer();
        }

    }
    public void PickedUp(Transform Player)
    {
        PlayerTransform = Player;
        transform.parent = PlayerTransform;
        IsPickedUp = true;
    }
    void SetOnTopOfPlayer()
    {
        transform.position = PlayerTransform.position + new Vector3(0, 1 + OffsetFromPlayerHead, 0);
    } 
    public void DropInFront()
    {
        transform.position = PlayerTransform.position + new Vector3(0, .5f, 0) + PlayerTransform.forward * 1.5f;
        transform.rotation = new Quaternion(0, PlayerTransform.rotation.y, 0, PlayerTransform.rotation.w);

        IsPickedUp = false;
        transform.parent = null;
    }
    public void Drop()
    {
        if (PlayerTransform != null)
        {
            PlayerTransform.GetComponent<HandMan>().PickUp = null;
            PlayerTransform.GetComponent<HandMan>().isHoldingOBJ = false;
        }
        IsPickedUp = false;
        transform.parent = null;

    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag != "Player" && 
            col.gameObject.transform.position.y > transform.position.y)
        {
            Drop();
        }    
    }
}
