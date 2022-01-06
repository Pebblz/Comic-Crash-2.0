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
    public bool StopRolling;
    private void Start()
    {
        originalSpeed = RollSpeed;
        movement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if (InputManager.GetButton("Right Mouse") && movement.anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            !movement.InWater && !movement.IsWallSliding && movement.playerInput.x != 0 && !StopRolling
            ||
            InputManager.GetButton("Right Mouse") && movement.anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            !movement.InWater && !movement.IsWallSliding && movement.playerInput.y != 0 && !StopRolling)
        {
            Roll();
            movement.PlayAnimation("GroundPound");
            movement.Rolling = true;
        }
        if(InputManager.GetButtonUp("Right Mouse") && movement.OnGround|| 
            movement.InWater || movement.IsWallSliding
            || movement.playerInput.y == 0 && movement.playerInput.x == 0 && movement.OnGround
            || StopRolling)
        {
            movement.Rolling = false;
            RollSpeed = originalSpeed;
            movement.StopAnimation("GroundPound");
            StopRolling = false;
        }    
    }

    void Roll()
    {
            movement.Roll(RollSpeed);
    }

}
