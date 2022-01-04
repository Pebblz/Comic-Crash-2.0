using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{

    public GameObject respawn_point;
    private List<Enemy> enemies;


    private void find_all_enemies()
    {
        GameObject[] en = GameObject.FindGameObjectsWithTag("Enemy");
        foreach( var e in en)
        {
            Enemy enemy = e.GetComponent<Enemy>();
            if (enemy == null)
                continue;
            if (enemy.respawnable)
            {
                enemies.Add(enemy);
            }

        }
    }

    private void attach_respawn_points()
    {
        for(int i =0; i < enemies.Count; i++)
        {
            Enemy en = enemies[i];
            var obj = Instantiate(respawn_point);
            RespawnPoint pt = obj.GetComponent<RespawnPoint>();
            pt.target = en.gameObject;
            pt.respawn_distance = en.respawn_distance;
            pt.respawn_timeout = en.repspawn_time;
        }
    }

    private void Awake()
    {
        enemies = new List<Enemy>();
        find_all_enemies();
        attach_respawn_points();
    }
}
