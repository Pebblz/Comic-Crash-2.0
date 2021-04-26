using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevelTrigger : MonoBehaviour
{
    LevelManager Lm;

    [SerializeField, Tooltip("This is for the level index in the build settings. I have it to the hub at default")]
    int LevelToLoad = 1;

    void Start()
    {
        Lm = FindObjectOfType<LevelManager>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Lm.LoadLevel(LevelToLoad);
        }
    }
}
