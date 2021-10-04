using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class PeaShooter : MonoBehaviour
{

    private GameObject camera;

    private float NextAttack;

    [SerializeField, Tooltip("The prefab used for the bullet")]
    GameObject BulletPrefab;

    [SerializeField, Range(1000f, 5000f), Tooltip("The speed that the bullet goes when being fired")]
    float shotSpeed;


    private Animator anim;

    PhotonView photonView;

    Pause pause;
    #region MonoBehaviours
    void Start()
    {
        pause = FindObjectOfType<Pause>();
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        anim = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (!pause.isPaused)
            {
                if (InputManager.GetButtonDown("Right Mouse"))
                {
                    camera.GetComponent<MainCamera>().thirdPersonCamera = false;
                    GetComponent<PlayerMovement>().CantMove = true;
                    PlayAnimation("Aiming");
                }
                if (InputManager.GetButtonUp("Right Mouse"))
                {
                    camera.GetComponent<MainCamera>().thirdPersonCamera = true;
                    GetComponent<PlayerMovement>().CantMove = false;
                    StopAnimation("Aiming");
                }

                if (InputManager.GetButtonDown("Left Mouse") && NextAttack <= 0)
                {
                    Attack();
                }
            }
        }
        NextAttack -= Time.deltaTime;
    }
    #endregion


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
            GameObject tempbul = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TempBullet"), camera.transform.position, camera.transform.rotation);
            tempbul.GetComponent<Rigidbody>().AddForce(tempbul.transform.forward * shotSpeed);
        }
        NextAttack = 1f;
    }
}
