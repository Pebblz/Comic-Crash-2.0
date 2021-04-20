using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    bool Falling;
    [SerializeField] float timeTillReset;
    private float timer;
    private float DownSpeed;
    private GameObject _fallingPlatformManager;
    private Vector3 OriginalPos;
    private Quaternion Originalrot;
    private void Awake()
    {
        timer = timeTillReset;
        _fallingPlatformManager = FindObjectOfType<FallingPlatformManager>().gameObject;
        OriginalPos = transform.position;
        Originalrot = transform.rotation;
    }
    private void Update()
    {
        if (Falling)
        {
            DownSpeed += Time.fixedDeltaTime / 20;
            transform.position = new Vector3(transform.position.x,
                transform.position.y - DownSpeed, transform.position.z);
            if (timer < 0)
            {
                _fallingPlatformManager.GetComponent<FallingPlatformManager>().PlatformFalling(gameObject, OriginalPos, Originalrot);
            }
            timer -= Time.fixedDeltaTime;
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Falling = true;
        }
    }
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Falling = true;
        }
    }
}
