using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [Tooltip("The Object that'll be getting changed based on the Action enum")]
    [SerializeField]
    GameObject ImpactedGameobject;

    [Tooltip("Change this to decide what you want to change with the lever switch")]
    [SerializeField]
    Action action;

    private GameObject player;

    private float InputTimer;
    void Update()
    {
        //this is here because we switch players
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if(Vector3.Distance(transform.position, player.transform.position) < 1)
        {
            if(Input.GetKeyDown(KeyCode.Q) && InputTimer <= 0)
            {
                DoAction();
            }
        }
        InputTimer -= Time.deltaTime;
    }
    enum Action
    {
        ConveyerBelt,
        movingPlatform
    }
    void DoAction()
    {
        if (action == Action.ConveyerBelt)
        {
            ImpactedGameobject.GetComponent<ConveyerBelt>().SwitchBeltDirection();
        }
        else if (action == Action.movingPlatform)
        {
            ImpactedGameobject.GetComponent<MovingPlatforms>().active =
                !ImpactedGameobject.GetComponent<MovingPlatforms>().active;
        }
        InputTimer = .5f;
    }
}
