using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterTree : MonoBehaviour
{
    int level = 0;
    public static int exp;
    int lastexpValue;
    public static int skillPoints = 0;
    [SerializeField] int expToNextLevel = 20;
    int overFillExp;
    ExpBar Bar;
    void Start()
    {
        Bar = GetComponent<ExpBar>();
        //here you would load all the level save data
    }

    void Update()
    {
        if(exp >= expToNextLevel)
        {
            LevelUp();
        }
        if(exp != lastexpValue)
        {
            GainExp();
        }
    }
    //call this whenever you want the player to gain exp
    void GainExp()
    {
        Bar.UpdateValue(exp);
        lastexpValue = exp;
    }
    void LevelUp()
    {
        level++;
        CheckForExpOverFill();
        //this makes it so levels will take longer and longer to achive
        expToNextLevel = (int)Mathf.Round(expToNextLevel * 1.5f);
        //add level up effect here (Particle effects)
        skillPoints++;
        exp = overFillExp;
        Bar.newMax(expToNextLevel,exp);
    }
    void CheckForExpOverFill()
    {
        overFillExp = exp - expToNextLevel;
    }
}
