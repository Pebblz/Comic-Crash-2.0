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
    [SerializeField]
    private void Start()
    {
        Gm = FindObjectOfType<GameManager>();
        ShopPage.SetActive(false);
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
                IsShopOpen(active);
                timer = .3f;
            }
            if(active && Input.GetKeyDown(KeyCode.Escape))
            {
                IsShopOpen(false);
                active = false;
            }
        }
        timer -= Time.deltaTime;
    }
    public void IsShopOpen(bool maybe)
    {
        ShopPage.SetActive(maybe);
        player.GetComponent<PlayerMovement>().enabled = !maybe;
        if (maybe)
        {
            Gm.unlockCursor();
        }
        else
        {
            Gm.lockCursor();
        }
    }
}
