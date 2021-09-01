using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    [SerializeField] float TimeToDestroy = 5f;

    private void Awake()
    {
        Destroy(gameObject, TimeToDestroy);
    }

    private void OnDestroy()
    {
        if (FindObjectOfType<Builder>() != null)
        {
            Builder builder = FindObjectOfType<Builder>();
            builder.RemoveFromList(this);
        }
    }
}
