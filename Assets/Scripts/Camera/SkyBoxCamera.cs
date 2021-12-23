using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxCamera : MonoBehaviour
{
    Transform playercam;
    [SerializeField] private float skyboxScale;

    private void Start()
    {
        playercam = FindObjectOfType<MainCamera>().transform;
    }
    void Update()
    {
        transform.rotation = playercam.rotation;
        transform.localPosition = playercam.position / skyboxScale;
    }
}
