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
            Roll();
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
            movement.StopAnimation("Roll");
            StopRolling = false;
        }    
    }

    void Roll()
    {
            movement.Roll(RollSpeed);
    }

}
