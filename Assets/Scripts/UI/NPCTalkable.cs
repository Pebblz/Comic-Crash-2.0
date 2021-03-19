using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalkable : MonoBehaviour
{
    public Dialogue dialogue;

    [SerializeField]
    [Tooltip("This is for the different ways the player can talk to the NPC")]
    WaysToStartConversation ways;

    GameObject player;

    [Tooltip("Set this if Ways == GetClose")]
    [SerializeField]
    float Talkdistance;

    bool talking = false;

    private void Update()
    {
        if (ways == WaysToStartConversation.GetClose)
        {
            if (player == null)
            {
                //this needs to be here because the player can switch characters
                player = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                if (Vector3.Distance(transform.position, player.transform.position) < Talkdistance)
                {
                    if (Input.GetKeyDown(KeyCode.Q) && !talking)
                    {
                        TriggerDialogue();
                    }
                    //this is a way for the player to skip dialogue if he's talking to the npc
                    if (Input.GetKeyDown(KeyCode.Escape) && talking)
                    {
                        EndDialogue();
                    }
                }
            }
        }
    }
    public void DoneTalking()
    {
        player.GetComponent<PlayerMovement>().enabled = true;
        talking = false;
    }
    /// <summary>
    /// Call this when you need to talk to the player 
    /// </summary>
    public void TriggerDialogue()
    {
        talking = true;
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        FindObjectOfType<DialogueManager>().NPC = this.gameObject;
        player.GetComponent<PlayerMovement>().enabled = false;
    }
    public void EndDialogue()
    {
        talking = false;
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
    //these are the different ways that the player can talk to the npc
    enum WaysToStartConversation
    {
        GetClose,
        TriggerBox,
        KillAnEnemy
    }
}
