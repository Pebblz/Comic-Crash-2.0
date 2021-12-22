using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterPokey : MonoBehaviour
{
    [SerializeField] float pushHeight;
    [SerializeField] float pushDistince;
    [SerializeField] int DamageToPlayer = 1;
    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Player")
        {
            col.collider.gameObject.GetComponent<PlayerHealth>().HurtPlayer(DamageToPlayer);

            Vector3 _velocity = col.contacts[0].point - col.gameObject.transform.position;

            _velocity = -_velocity.normalized;

            _velocity.y = pushHeight;

            col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(_velocity.x * pushDistince,
                _velocity.y, _velocity.z * pushDistince);

            //faces direction on jump
            col.gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(_velocity.x * pushDistince,
                _velocity.y, _velocity.z * pushDistince));

        }

    }
}
