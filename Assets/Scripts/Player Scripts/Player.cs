using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 respawnPoint;
    GameObject Camera;
    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = gameObject.transform.position;
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RepoPlayer()
    {
        Camera.GetComponent<Camera>().ResetCamera();
        transform.position = respawnPoint;
    }
}
