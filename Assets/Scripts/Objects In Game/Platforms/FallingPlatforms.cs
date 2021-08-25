using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    bool Falling;
    [SerializeField] float timeTillReset;
    [SerializeField] public float timeTillPlatformFalls;
    private float timer;
    private float DownSpeed;
    private GameObject _fallingPlatformManager;
    private Vector3 OriginalPos;
    private Quaternion Originalrot;
    private void Start()
    {
        //this is here so when the platform respawns it'll not instently fall
        if(timeTillPlatformFalls < 0)
            timeTillPlatformFalls = Random.Range(.5f, 3f);

        timer = timeTillReset;
        _fallingPlatformManager = FindObjectOfType<FallingPlatformManager>().gameObject;
        OriginalPos = transform.position;
        Originalrot = transform.rotation;
    }
    private void Update()
    {
        if (Falling)
        {
            if (timeTillPlatformFalls <= 0)
            {
                DownSpeed += Time.fixedDeltaTime / 20;
                transform.position = new Vector3(transform.position.x,
                    transform.position.y - DownSpeed, transform.position.z);
                if (timer < 0)
                {
                    _fallingPlatformManager.GetComponent<FallingPlatformManager>().PlatformFalling(gameObject, OriginalPos, Originalrot);
                }
            }
            timer -= Time.fixedDeltaTime;
            timeTillPlatformFalls -= Time.fixedDeltaTime;
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
