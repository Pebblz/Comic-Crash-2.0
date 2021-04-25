using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("If This is tied to an arena the arena will auto set this field")]
    public int SpawnAmount;
    private int spawned;
    public bool Active;
    private GameObject CurrentSpawnedEnemy;
    public bool Beaten;
    private void Start()
    {
        if (Active)
        {
            SpawnEnemy();
        }
    }
    void Update()
    {

        if (CurrentSpawnedEnemy == null && Active && spawned < SpawnAmount)
        {
            SpawnEnemy();
            spawned++;
        }
        if(spawned == SpawnAmount)
        {
            Beaten = true;
        }
    }
    void SpawnEnemy()
    {

    }
    public void ResetSpawner()
    {
        spawned = 0;
        Beaten = false;
        Active = false;
    }
}
