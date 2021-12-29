﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class DivingBoard : MonoBehaviour
{
    [SerializeField] Animator anim;

    [SerializeField] Rings[] Rings;


    [SerializeField] GameObject CollectibleToSpawn;

    bool playerOnTop;
    bool StartCountdown;
    int HitRings;
    float waitALittle;
    void Update()
    {
        if(playerOnTop && InputManager.GetButtonDown("Jump"))
        {
            PlayAnimation("Wiggle");
        }
        if (!playerOnTop)
            StopAnimation("Wiggle");

        if (Rings[0].Hit)
        {
            for (int i = 0; i < Rings.Length; i++)
            {
                if (Rings[i].Hit)
                {
                    HitRings += 1;
                    if (HitRings == i + 1)
                    {
                        CollectibleToSpawn.SetActive(true);
                    }
                }
            }
        }
        if(waitALittle <= 0 && StartCountdown)
        {
            playerOnTop = false;
            StartCountdown = false;
        }
        waitALittle -= Time.deltaTime;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerMovement>().OnDivingBoard = true;
            playerOnTop = true;
        }
    }
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerMovement>().OnDivingBoard = true;
            playerOnTop = true;
            
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerMovement>().OnDivingBoard = false;
            StartCountdown = true;
            waitALittle = 2f;
        }
    }
    #region anim
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