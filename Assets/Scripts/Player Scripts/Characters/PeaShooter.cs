using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PeaShooter : MonoBehaviour
{

    private GameObject camera;

    private float NextAttack;

    [SerializeField, Tooltip("The prefab used for the bullet")]
    GameObject BulletPrefab;

    [SerializeField, Range(1000f, 5000f), Tooltip("The speed that the bullet goes when being fired")]
    float shotSpeed;

    private Animator anim;

    bool AimDown;

    bool Shoot;
    #region MonoBehaviours
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AimDown)
        {
            camera.GetComponent<MainCamera>().thirdPersonCamera = false;
            PlayAnimation("Aiming");
        }
        if (!AimDown)
        {
            camera.GetComponent<MainCamera>().thirdPersonCamera = true;
            StopAnimation("Aiming");
        }

        if (Shoot && NextAttack <= 0)
        {
            Attack();
        }

        NextAttack -= Time.deltaTime;
    }
    #endregion
    public void LeftMouse(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Shoot = true;
        }
        if (context.canceled)
        {
            Shoot = false;
        }
    }
    public void RightMouse(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AimDown = true;
        }
        if (context.canceled)
        {
            AimDown = false;
        }

    }

    void PlayAnimation(string animName)
    {
        anim.SetBool(animName, true);
    }
    void StopAnimation(string animName)
    {
        anim.SetBool(animName, false);
    }
    void Attack()
    {
        if (!camera.GetComponent<MainCamera>().thirdPersonCamera)
        {
            GameObject tempbul = Instantiate(BulletPrefab, camera.transform.position, camera.transform.rotation);
            tempbul.GetComponent<Rigidbody>().AddForce(tempbul.transform.forward * shotSpeed);
        }
        NextAttack = 1f;
    }
}
