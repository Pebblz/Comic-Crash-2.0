using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditText : MonoBehaviour
{
    [SerializeField, Range(20f, 100f)]
    float speed;
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }
}
