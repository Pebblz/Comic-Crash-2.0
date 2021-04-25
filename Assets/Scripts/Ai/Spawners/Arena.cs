using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] Spawner[] SpawnersInArena;
    [SerializeField] bool SpawnersActiveOnStart;
    public bool PlayerInArena;
    GameObject Player;
    List<GameObject> ColliderList;
    [SerializeField] int NumberToSpawn;
    void Start()
    {
        foreach (Spawner s in SpawnersInArena)
        {
            s.SpawnAmount = 0;
            if(!SpawnersActiveOnStart)
            {
                s.Active = false;
            }
        }
        Player = GameObject.FindGameObjectWithTag("Player");
        for(int i = 0; i<NumberToSpawn;i=0)
        {
            
            foreach(Spawner s in SpawnersInArena)
            {
                if (NumberToSpawn >= 0)
                {
                    s.SpawnAmount++;
                    NumberToSpawn--;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        if(PlayerInArena)
        {
            foreach(Spawner s in SpawnersInArena)
            {
                s.Active = true;
            }
        }
    }
    //void OnTriggerEnter(Collider col)
    //{
    //    if (!ColliderList.Contains(col.gameObject))
    //    {
    //        ColliderList.Add(col.gameObject);
    //    }
    //}
    //void OnTriggerStay(Collider col)
    //{

    //}
    //private void OnTriggerExit(Collider col)
    //{
    //    if (!ColliderList.Contains(col.gameObject))
    //    {
    //        ColliderList.Remove(col.gameObject);
    //    }
    //}
}
