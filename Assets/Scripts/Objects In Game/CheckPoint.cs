using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    //uncomment the animator to add animations to the checkpoints
    //Animator anim;
    private void Start()
    {
       // anim = GetComponent<Animator>();
    }
    //private void Update()
    //{
    //    if(anim.GetCurrentAnimatorStateInfo(0).length > anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
    //    {
    //        anim.SetBool("Spin", false);
    //    }
    //}
    private void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<Player>(out var player))
        {
            // anim.SetBool("Spin", true);
            player.respawnPoint = transform.position + new Vector3(0,.5f,0);
        }
    }
}