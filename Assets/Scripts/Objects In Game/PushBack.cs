using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBack : MonoBehaviour
{
    [SerializeField] float pushHeight;
    [SerializeField] float pushDistince;
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            Vector3 _velocity = collision.contacts[0].point - collision.gameObject.transform.position;

            _velocity = -_velocity.normalized;

            _velocity.y = pushHeight;

            collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(_velocity.x * pushDistince,
                _velocity.y, _velocity.z * pushDistince);

            //faces direction on jump
            collision.gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(_velocity.x * pushDistince,
                _velocity.y, _velocity.z *  pushDistince));

            //collision.gameObject.GetComponent<PlayerMovement>().PreventSnapToGround();
        }

    }
}
