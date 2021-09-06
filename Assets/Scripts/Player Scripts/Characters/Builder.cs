using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Builder : MonoBehaviour
{
    [SerializeField] GameObject block;
    [SerializeField] float MaxDigTimer;
    float blocktimer, digTimer, BuildingAnimTimer;
    [SerializeField] int CurrentBlockStorage;
    [SerializeField] int MaxSpawnableBlocks = 2;
    [SerializeField, Range(.01f, 1f)] float BlockPlacementYOffset;
    List<DestroyBlock> blocksSpawned = new List<DestroyBlock>(2);
    Animator anim;
    bool buildingOnGround, digging;
    PlayerMovement movement;
    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        for (int i = 0; i < blocksSpawned.Count; i++)
            blocksSpawned.Add(FindObjectOfType<DestroyBlock>());
    }
    void Update()
    {

        if (buildingOnGround && BuildingAnimTimer > 0)
        {
            BuildingAnimTimer -= Time.deltaTime;
        }
        if (buildingOnGround && BuildingAnimTimer < 0)
        {
            buildingOnGround = false;
            StopAnimation("Building");
        }
        if (GetComponent<PlayerMovement>().OnGround)
        {
            StopAnimation("BuildingAir");
        }

        if (digging && movement.OnGround && CurrentBlockStorage < MaxSpawnableBlocks && !movement.onBlock)
        {
            PlayAnimation("Digging");
            movement.CantMove = true;
            if (digTimer < 0)
            {
                CurrentBlockStorage += 1;
                digTimer = MaxDigTimer;
            }
        }

        if (digging)
        {
            digTimer -= Time.deltaTime;
        }
        blocktimer -= Time.deltaTime;
    }
    public void LeftMouse(InputAction.CallbackContext context)
    {
        if (CurrentBlockStorage >= MaxSpawnableBlocks)
        {
            StopAnimation("Digging");
            movement.CantMove = false;
            digging = false;
            digTimer = MaxDigTimer;
        }
        else
        {
            digTimer = MaxDigTimer;
            digging = true;
        }
    }
    public void RightMouse(InputAction.CallbackContext context)
    {
        if (blocktimer <= 0 && blocksSpawned.Count < MaxSpawnableBlocks && CurrentBlockStorage > 0)
        {
            if (GetComponent<PlayerMovement>().OnGround)
            {
                GameObject temp = Instantiate(block, transform.position + (transform.forward * 2f) + new Vector3(0, BlockPlacementYOffset, 0), transform.rotation);
                blocksSpawned.Add(temp.GetComponent<DestroyBlock>());
                PlayAnimation("Building");
                CurrentBlockStorage--;
                BuildingAnimTimer = .4f;
                buildingOnGround = true;
                blocktimer = .5f;
            }
            else
            {
                GameObject temp = Instantiate(block, transform.position + (-transform.up * 1.2f), transform.rotation);
                blocksSpawned.Add(temp.GetComponent<DestroyBlock>());
                PlayAnimation("BuildingAir");
                CurrentBlockStorage--;
                blocktimer = .5f;
            }
        }
    }
    public void RemoveFromList(DestroyBlock temp)
    {
        blocksSpawned.Remove(temp);
    }

    #region Animation
    /// <summary>
    /// Call this for anytime you need to play an animation 
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnimation(string BoolName)
    {
        anim.SetBool(BoolName, true);
    }
    /// <summary>
    /// Call this for anytime you need to stop an animation
    /// </summary>
    /// <param name="BoolName"></param>
    public void StopAnimation(string BoolName)
    {
        anim.SetBool(BoolName, false);
    }
    #endregion
}
