using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExitMenuBTN : MonoBehaviour
{
    GameManager Gm;
    void Start()
    {
        Gm = FindObjectOfType<GameManager>();
    }
    public void unPause()
    {
        Gm.GetComponent<Pause>().pause(false);
    }
}
