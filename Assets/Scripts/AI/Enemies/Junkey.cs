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

    [SerializeField] float attackResetTime = 1.5f;

    float attackResetTimer;
    private void Update()
    {
        if (targetedEnemy != null)
        {
            if (attackResetTimer <= 0)
            {
                if (!targetedEnemy.GetComponent<PlayerDeath>().isdead && junk != null)
                    SpawnProjectile();
                else
                {
                    targetedEnemy = null;
                }
            }
        }
        if (attackResetTimer > -.1f)
        {
            attackResetTimer -= Time.deltaTime;
        }
    }

    private void SpawnProjectile()
    {
        string name = Path.Combine("PhotonPrefabs", junkProjectile.name);
        var obj = PhotonNetwork.Instantiate(name, junkSpawnPosition.position, new Quaternion());
        obj.GetComponent<JunkProjectile>().TargetObject = targetedEnemy.transform;
        attackResetTimer = attackResetTime;
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
        if(junk != null)
            targetedEnemy = null;
    }
}

