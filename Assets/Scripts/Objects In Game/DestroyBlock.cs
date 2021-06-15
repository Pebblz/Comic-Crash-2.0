using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    [SerializeField]float TimeToDestroy = 3f;
    private void Awake()
    {
        Destroy(gameObject, TimeToDestroy);
    }
}
