using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Luminosity.IO;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    GameObject ShopPage;
    GameObject player;
    float timer;
    public bool active;
    GameManager Gm;
    [SerializeField]
    float ShopButtonCooldown = .2f;
    [SerializeField]
    float distanceAwayToTrigger = 4f;
    GameObject FirstButton;
    Luminosity.IO.Examples.GamepadToggle toggle;
    private void Start()
    {
        Gm = FindObjectOfType<GameManager>();
        toggle = FindObjectOfType<Luminosity.IO.Examples.GamepadToggle>();
        ShopPage = GameObject.Find("ShopPage");
        ShopPage.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { CloseShop(); });
        FirstButton = ShopPage.transform.GetChild(1).gameObject;
        ShopPage.SetActive(false);
    }
    void Update()
    {
        if (player == null)
            player = FindObjectOfType<Player>().gameObject;

        if(Vector3.Distance(transform.position, player.transform.position) < distanceAwayToTrigger)
        {
            if(InputManager.GetButton("Left Mouse") && timer <= 0 && !active)
            {
                active = !active;
                IsShopOpen(active);
                timer = ShopButtonCooldown;
            }
            if(active && InputManager.GetButton("Pause"))
            {
                IsShopOpen(false);
                active = false;
            }

        }
        timer -= Time.deltaTime;
    }
    public void CloseShop()
    {
        active = !active;
        IsShopOpen(active);
        timer = ShopButtonCooldown;
    }
    public void IsShopOpen(bool Open)
    {
        Gm.GetComponent<PlayerSwitcher>().CanSwitch = !Gm.GetComponent<PlayerSwitcher>().CanSwitch;
        ShopPage.SetActive(Open);
        player.GetComponent<PlayerMovement>().CantMove  = !player.GetComponent<PlayerMovement>().CantMove;
        if (Open)
        {
            if (active && toggle.m_gamepadOn)
            {
                Gm.lockCursor();
            }
            if (active && !toggle.m_gamepadOn)
            {
                Gm.unlockCursor();
            }

            if (EventSystem.current != FirstButton)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(FirstButton);
            }
        }
        else
        {
            Gm.lockCursor();
        }
    }
}
