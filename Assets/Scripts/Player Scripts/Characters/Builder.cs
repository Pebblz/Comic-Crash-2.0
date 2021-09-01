using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] GameObject block;
    float blocktimer;
    int CurrentBlockStorage;
    [SerializeField] int MaxSpawnableBlocks = 2;
    [SerializeField, Range(.01f,1f)] float BlockPlacementYOffset;
    List<DestroyBlock> blocksSpawned = new List<DestroyBlock>(2);
    private void Start()
    {
        for(int i = 0; i < blocksSpawned.Count;i++)
            blocksSpawned.Add(FindObjectOfType<DestroyBlock>());
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.V) && blocktimer <= 0  && blocksSpawned.Count < MaxSpawnableBlocks)
        {
            if (GetComponent<PlayerMovement>().OnGround)
            {
                GameObject temp = Instantiate(block, transform.position + (transform.forward * 2f) + new Vector3(0, BlockPlacementYOffset, 0), transform.rotation);
                blocksSpawned.Add(temp.GetComponent<DestroyBlock>());
                blocktimer = .5f;
            }
            else
            {
                GameObject temp = Instantiate(block, transform.position + (-transform.up * 1.2f), transform.rotation);
                blocksSpawned.Add(temp.GetComponent<DestroyBlock>());
                blocktimer = .5f;
            }


        }

        blocktimer -= Time.deltaTime;
    }
    public void RemoveFromList(DestroyBlock temp)
    {
        blocksSpawned.Remove(temp);
    }
}
