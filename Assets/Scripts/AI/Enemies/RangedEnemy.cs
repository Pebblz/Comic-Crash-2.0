using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] GameObject targetedEnemy;

    [SerializeField] GameObject Projectile;

    [SerializeField] Transform junkSpawnPosition;

    [SerializeField] float attackResetTime = 1.5f, projectileSpeed = 20f;

    float attackResetTimer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
