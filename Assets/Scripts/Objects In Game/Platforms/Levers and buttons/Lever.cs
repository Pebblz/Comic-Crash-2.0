using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField, Tooltip("The Object that'll be getting changed based on the Action enum")]
    GameObject ImpactedGameobject;

    [SerializeField, Tooltip("Change this to decide what you want to change with the lever switch")]
    Action action;

    private GameObject player;

    private float InputTimer;
    void Update()
    {
        //this is here because we switch players
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (Vector3.Distance(transform.position, player.transform.position) < 1)
        {
            if (Input.GetKeyDown(KeyCode.Q) && InputTimer <= 0)
            {
                DoAction();
            }
        }
        InputTimer -= Time.deltaTime;
    }
    enum Action
    {
        ConveyerBelt,
        movingPlatform,
        rotatingPlatform
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
        else if (action == Action.rotatingPlatform)
        {
            ImpactedGameobject.GetComponent<RotatingPlatform>().active =
                !ImpactedGameobject.GetComponent<RotatingPlatform>().active;
        }
        InputTimer = .5f;
    }
}
