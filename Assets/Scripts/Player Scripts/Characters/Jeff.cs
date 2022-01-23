using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class Jeff : MonoBehaviour
{
    float RollSpeed =1f;
    [SerializeField]
    float maxRollSpeed;
    [SerializeField]
    float acceleration;
    [SerializeField]
    float rollingSmoothTime = .2f;
    float originalSpeed;
    PlayerMovement movement;
    [SerializeField] ParticleSystem RollingParticle;
    public bool StopRolling;
    float OrgTurnSmoothTime;
    private void Start()
    {
        if (RollingParticle != null)
        {
            RollingParticle.Stop();
        }
        originalSpeed = RollSpeed;
        movement = GetComponent<PlayerMovement>();
        OrgTurnSmoothTime = movement.turnSmoothTime;
    }
    void Update()
    {
        if (InputManager.GetButton("Right Mouse") && movement.anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            !movement.InWater && !movement.IsWallSliding && movement.playerInput.x != 0 && !StopRolling 
            ||
            InputManager.GetButton("Right Mouse") && movement.anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            !movement.InWater && !movement.IsWallSliding && movement.playerInput.y != 0 && !StopRolling)
        {
            if (RollingParticle != null)
            {
                RollingParticle.Play();
            }
            movement.turnSmoothTime = rollingSmoothTime;
            movement.PlayAnimation("Roll");
            movement.Rolling = true;
        }
        if(InputManager.GetButtonUp("Right Mouse") || 
            movement.InWater || movement.IsWallSliding
            || movement.playerInput.y == 0 && movement.playerInput.x == 0 && movement.OnGround
            || StopRolling)
        {
            movement.Rolling = false;
            RollSpeed = originalSpeed;
            if (RollingParticle != null)
            {
                RollingParticle.Stop();
            }
            movement.turnSmoothTime = OrgTurnSmoothTime;
            RollSpeed = 1;
            movement.StopAnimation("Roll");
            StopRolling = false;
        }
        if (movement.Rolling)
        {
            if (RollSpeed < maxRollSpeed)
                RollSpeed += (acceleration * Time.deltaTime);
            Roll();
        }
    }

    void Roll()
    {
            movement.Roll(RollSpeed);
    }

}
