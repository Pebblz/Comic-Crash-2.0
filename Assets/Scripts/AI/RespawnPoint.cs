using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Gameobject to repsawn")]
    GameObject target;

    [SerializeField]
    [Tooltip("Distance a player has to be away for an entity to respawn")]
    [Range(5, 20)]
    float respawn_distance = 10f;

    [SerializeField]
    [Tooltip("Amount of time a player has to be away for an entity to respawn")]
    [Range(10, 100)]
    float respawn_timeout = 10f;
    float init_respawn_timeout;
    // Start is called before the first frame update
    private bool respawning = false;
    private void Awake()
    {
        init_respawn_timeout = respawn_timeout;
        this.transform.position = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target.activeInHierarchy == false)
        {
            respawning = true;
        }
        if (respawning)
        {
            if (players_out_of_range())
            {
                respawn_timeout -= Time.deltaTime;
            }
            if(respawn_timeout <= 0f)
            {
                target.SetActive(true);
                IRespawnable script = (IRespawnable)target.GetComponent(typeof(IRespawnable));
                script.reset_data();
                respawn_timeout = init_respawn_timeout;
                respawning = false;
            }
        }
    }

    public bool players_out_of_range()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if(Vector3.Distance(this.transform.position, player.transform.position) < respawn_distance)
            {
                return false;
            }
        }
        return true;
    }
}
