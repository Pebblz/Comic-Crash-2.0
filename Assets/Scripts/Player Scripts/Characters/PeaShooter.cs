using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaShooter : MonoBehaviour
{
    private GameObject camera;
    private float NextAttack;

    [Tooltip("The prefab used for the bullet")]
    [SerializeField]
    GameObject BulletPrefab;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            camera.GetComponent<Camera>().thirdPersonCamera = false;

        }
        if (Input.GetMouseButtonUp(1))
        {
            camera.GetComponent<Camera>().thirdPersonCamera = true;
        }

        if (Input.GetMouseButtonDown(0) && NextAttack <= 0)
        {
            Attack();
        }

        NextAttack -= Time.deltaTime;
    }
    void Attack()
    {
        if (!camera.GetComponent<Camera>().thirdPersonCamera)
        {
            GameObject tempbul = Instantiate(BulletPrefab, camera.transform.position, camera.transform.rotation);
            tempbul.GetComponent<Rigidbody>().AddForce(tempbul.transform.forward * 3000);
        }
        NextAttack = 1f;
    }
}
