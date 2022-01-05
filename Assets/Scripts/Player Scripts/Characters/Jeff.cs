using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class Jeff : MonoBehaviour
{
    [SerializeField]
    float RollSpeed = 5;
    float originalSpeed;
    PlayerMovement movement;
    bool holdingSprint;
    bool CheckOncePerRoll;
    private void Start()
    {
        originalSpeed = RollSpeed;
        movement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if (InputManager.GetButton("Right Mouse") && movement.anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            !movement.InWater && !movement.IsWallSliding && movement.playerInput.x != 0 
            ||
            InputManager.GetButton("Right Mouse") && movement.anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            !movement.InWater && !movement.IsWallSliding && movement.playerInput.y != 0)
        {
            Roll();
            movement.PlayAnimation("GroundPound");
            movement.Rolling = true;
        }
        if(InputManager.GetButtonUp("Right Mouse") || 
            movement.InWater || movement.IsWallSliding
            || movement.playerInput.y == 0 && movement.playerInput.x == 0)
        {
            movement.Rolling = false;
            holdingSprint = false;
            CheckOncePerRoll = false;
            RollSpeed = originalSpeed;
            movement.StopAnimation("GroundPound");
        }    
    }

    void Roll()
    {
            movement.Roll(RollSpeed);
    }

}
