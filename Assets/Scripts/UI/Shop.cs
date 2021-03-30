using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject ShopPage;
    GameObject player;
    float timer;
    bool active;
    GameManager Gm;
    private void Start()
    {
        Gm = FindObjectOfType<GameManager>();
    }
    void Update()
    {
        if (player == null)
            player = FindObjectOfType<Player>().gameObject;

        if(Vector3.Distance(transform.position, player.transform.position)< 4)
        {
            if(Input.GetKeyDown(KeyCode.Q) && timer <= 0)
            {
                active = !active;
                ShopPage.SetActive(active);
                player.GetComponent<PlayerMovement>().enabled = !active;
                timer = .3f;
                if(active)
                {
                    Gm.unlockCursor();
                } else
                {
                    Gm.unlockCursor();
                }
            }
            if(active && Input.GetKeyDown(KeyCode.Escape))
            {
                active = false;
                player.GetComponent<PlayerMovement>().enabled = true;
            }
        }
        timer -= Time.deltaTime;
    }
}
