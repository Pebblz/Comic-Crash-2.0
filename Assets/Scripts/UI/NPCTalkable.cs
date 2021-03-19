using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalkable : MonoBehaviour
{
    public Dialogue dialogue;

    [SerializeField]
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
                player = GameObject.FindGameObjectWithTag("Player");
            else
            {
                if (Vector3.Distance(transform.position, player.transform.position) < Talkdistance)
                {
                    if (Input.GetKeyDown(KeyCode.Q) && !talking)
                    {
                        TriggerDialogue();
                        Talking();
                    }
                    if (Input.GetKeyDown(KeyCode.Escape) && talking)
                    {
                        EndDialogue();
                    }
                }
            }
        }
    }
    public void Talking()
    {
        player.GetComponent<PlayerMovement>().enabled = false;
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
    }
    public void EndDialogue()
    {
        talking = false;
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
    enum WaysToStartConversation
    {
        GetClose,
        TriggerBox,
        KillAnEnemy
    }
}
