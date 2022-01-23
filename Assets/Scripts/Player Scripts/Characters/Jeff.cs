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
    float originalSpeed;
    PlayerMovement movement;
    [SerializeField] ParticleSystem RollingParticle;
    public bool StopRolling;
    private void Start()
    {
        if (RollingParticle != null)
        {
            RollingParticle.Stop();
        }
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
            if (RollingParticle != null)
            {
                RollingParticle.Play();
            }
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
