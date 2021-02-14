using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading_Screen : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FadeOut()
    {
        anim.SetBool("FadeOut", true);
    }
    public void FadeIn()
    {
        anim.SetBool("FadeOut", false);
    }
}
