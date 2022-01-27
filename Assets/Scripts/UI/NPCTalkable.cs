using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Pun;
public class NPCTalkable : MonoBehaviour
{
    public Dialogue dialogue;

    [SerializeField, Tooltip("This is for the different ways the player can talk to the NPC")]
    WaysToStartConversation ways;


    [SerializeField, Tooltip("These are the possible things that the player can recive if talked to the bot")]
    ThingsToGivePlayer thingsGivenToPlayer;

    [SerializeField, Range(1, 5)]
    int amountGiven;

    private bool GavePlayerAlready = false;

    GameObject player;

    [SerializeField, Tooltip("Set this if Ways == GetClose")]
    float Talkdistance;

    bool talking = false;

    DialogueManager manager;

    float Timer;
    private void Start()
    {
        manager = FindObjectOfType<DialogueManager>();
    }

    private void Update()
    {
        if (ways == WaysToStartConversation.GetClose)
        {
            if (player == null)
            {
                //this needs to be here because the player can switch characters
                player = PhotonFindCurrentClient();
            }
            else
            {
                if (Vector3.Distance(transform.position, player.transform.position) < Talkdistance)
                {
                    if (InputManager.GetButton("Interact") && !talking)
                    {
                        TriggerDialogue();
                        Timer = .5f;
                    }
                    //this is a way for the player to skip dialogue if he's talking to the npc
                    if (InputManager.GetButton("Pause") && talking)
                    {
                        EndDialogue();
                    }
                    //return means enter
                    if (talking && InputManager.GetButton("Enter") && Timer < 0)
                    {
                        manager.DisplayNextSentence();
                        Timer = .5f;
                    }
                }
                else
                {
                    talking = false;
                }
            }
            Timer -= Time.deltaTime;

        }
    }
    public void DoneTalking()
    {
        if (!GavePlayerAlready)
            GivePlayerItem();

        player.GetComponent<PlayerMovement>().CantMove = false;
        talking = false;
    }
    /// <summary>
    /// Call this when you need to talk to the player 
    /// </summary>
    public void TriggerDialogue()
    {
        talking = true;
        manager.StartDialogue(dialogue);
        manager.NPC = this.gameObject;
        player.GetComponent<PlayerMovement>().CantMove = true;
    }
    public void EndDialogue()
    {
        if(!GavePlayerAlready)
            GivePlayerItem();

        talking = false;
        manager.EndDialogue();
        player.GetComponent<PlayerMovement>().CantMove = false;
    }
    void GivePlayerItem()
    {
        if (!GavePlayerAlready && thingsGivenToPlayer != ThingsToGivePlayer.None)
        {
            if (thingsGivenToPlayer == ThingsToGivePlayer.Coin)
            {
                FindObjectOfType<GameManager>().coinCount += amountGiven;
                print(FindObjectOfType<GameManager>().coinCount);
            }
            else if (thingsGivenToPlayer == ThingsToGivePlayer.Collectible)
            {
                FindObjectOfType<GameManager>().CollectibleCount += amountGiven;
                print(FindObjectOfType<GameManager>().CollectibleCount);
            }
            GavePlayerAlready = true;
        }
    }
    GameObject PhotonFindCurrentClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject g in players)
        {
            if (g.GetComponent<PhotonView>().IsMine)
                return g;
        }
        return null;
    }
    //these are the different ways that the player can talk to the npc
    enum WaysToStartConversation
    {
        GetClose,
        TriggerBox,
        KillAnEnemy
    }
    enum ThingsToGivePlayer
    {
        None,
        Coin,
        Collectible
    }
}
