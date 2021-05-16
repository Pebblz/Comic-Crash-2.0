using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).length > anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            anim.SetBool("Spin", false);
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            anim.SetBool("Spin", true);
            col.GetComponent<Player>().respawnPoint = transform.position + new Vector3(0,2,0);
        }
    }
}