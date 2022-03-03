using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class Junkey : MonoBehaviour
{
    [SerializeField] GameObject targetedEnemy;

    [SerializeField] GameObject junk;

    [SerializeField] GameObject junkProjectile;

    [SerializeField] Transform junkSpawnPosition;

    [SerializeField] Animator anim;

    float spawnCooldown;
    private void Update()
    {
        if (targetedEnemy != null)
        {
            if (!targetedEnemy.GetComponent<PlayerDeath>().isdead && junk != null)
            {
                anim.SetBool("Attacking", true);
                if (anim.GetAnimatorTransitionInfo(0).IsName("Attack -> CoolDown") && spawnCooldown <= 0)
                {
                    SpawnProjectile();
                    spawnCooldown = .12f;
                }
            }
            else
            {
                anim.SetBool("Idle", true);
                anim.SetBool("Attacking", false);
                targetedEnemy = null;
            }
            spawnCooldown -= Time.deltaTime;
        }
        else
        {
            anim.SetBool("Idle", true);
            anim.SetBool("Attacking", false);
        }
        if (junk == null)
        {
            anim.SetBool("Dead", true);
        }
    }

    private void SpawnProjectile()
    {
        string name = Path.Combine("PhotonPrefabs", junkProjectile.name);
        var obj = PhotonNetwork.Instantiate(name, junkSpawnPosition.position, new Quaternion());
        obj.GetComponent<JunkProjectile>().TargetObject = targetedEnemy.transform;
    }

    private void OnTriggerStay(Collider col)
    {

        if (col.gameObject.tag == "Player" && junk != null)
        {
            if (targetedEnemy == null)
            {
                targetedEnemy = col.gameObject;
            }
        }
        if (junk == null)
        {
            targetedEnemy = null;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject == targetedEnemy)
        {
            targetedEnemy = null;
        }
    }
}