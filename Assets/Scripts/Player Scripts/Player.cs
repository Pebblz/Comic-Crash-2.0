using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("This is for where the player would respawn at when he dies")]
    public Vector3 respawnPoint;

    Quaternion rotation;
    GameObject Camera;
    // Start is called before the first frame update
    void Start()
    {
        
        rotation = transform.rotation;
        if(respawnPoint == new Vector3(0,0,0))
            respawnPoint = gameObject.transform.position;
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    private void Update()
    {
        if (transform.parent != null)
        {
            transform.rotation = new Quaternion(rotation.x, transform.rotation.y, rotation.z, transform.rotation.w);
        }
    }
    public void RepoPlayer()
    {
        Camera.GetComponent<Camera>().ResetCamera();
        transform.position = respawnPoint;
    }
}
