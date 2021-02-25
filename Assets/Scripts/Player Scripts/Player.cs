using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Vector3 respawnPoint;

    Quaternion rotation;
    GameObject Camera;

    #region MonoBehaviours
    void Start()
    {
        rotation = transform.rotation;
        if (respawnPoint == new Vector3(0, 0, 0))
            respawnPoint = gameObject.transform.position;
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    private void Update()
    {
        if (transform.parent != null)
        {
            transform.rotation = new Quaternion(rotation.x, transform.rotation.y, rotation.z, transform.rotation.w);
        }
        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
    }
    #endregion
    public void RepoPlayer()
    {
        Camera.GetComponent<Camera>().ResetCamera();
        transform.position = respawnPoint;
    }
}
