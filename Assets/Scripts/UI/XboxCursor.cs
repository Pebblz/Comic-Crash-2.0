using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class XboxCursor : MonoBehaviour
{
    Vector2 Position;

    public GameObject Cursor;

    bool Clicked, enabled;

    
    void Start()
    {
        Position = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        Clicked = Input.GetButtonDown("Fire1");


        if(Clicked)
        {
            Vector3 start = this.gameObject.transform.position + new Vector3(0, .5f, 0);
            RaycastHit hit;

            if (Physics.Raycast(start, transform.TransformDirection(Vector3.forward), out hit, 0))
            {
                //this checks if any of the rays hit an object with pickupables script
                if (hit.collider.gameObject.GetComponent<PickUpables>() != null)
                {

                }
            }
        }

    }
    void UpdateMousePosition()
    {

    }
}
