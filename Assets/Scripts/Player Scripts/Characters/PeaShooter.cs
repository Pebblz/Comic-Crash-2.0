using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class PeaShooter : MonoBehaviour
{

    private MainCamera camera;

    private float NextAttack;

    [SerializeField, Tooltip("The prefab used for the bullet")]
    GameObject BulletPrefab;

    [SerializeField]
    GameObject GunTip;

    [SerializeField, Range(1000f, 5000f), Tooltip("The speed that the bullet goes when being fired")]
    float shotSpeed;

    public float MouseSensitivityY;
    public float MouseSensitivityX;

    float originalMouseSensitivityY;
    float originalMouseSensitivityX;

    private Animator anim;

    PhotonView photonView;

    PlayerMovement movement;

    Pause pause;

    float originalDistance;

    [SerializeField] GameObject spine;

    Luminosity.IO.Examples.GamepadToggle toggle;
    #region MonoBehaviours
    void Start()
    {
        pause = FindObjectOfType<Pause>();
        camera = FindObjectOfType<MainCamera>();
        originalMouseSensitivityY = camera.ySpeed;
        originalMouseSensitivityX = camera.xSpeed;
        photonView = GetComponent<PhotonView>();
        movement = GetComponent<PlayerMovement>();

        toggle = FindObjectOfType<Luminosity.IO.Examples.GamepadToggle>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            if (!pause.isPaused)
            {
                if (InputManager.GetButton("Right Mouse") && movement.OnGround)
                {

                    GetComponent<PlayerMovement>().CantMove = true;

                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
                        camera.transform.localEulerAngles.y, transform.localEulerAngles.z);

                    GunTip.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, 0 , camera.transform.localEulerAngles.z);
                    spine.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, spine.transform.localEulerAngles.y,spine.transform.localEulerAngles.z);
                    SetCamera();
                }
                if (InputManager.GetButtonUp("Right Mouse") || !movement.OnGround)
                {
                    GetComponent<PlayerMovement>().CantMove = false;
                    UnSetCamera();
                }

                if (InputManager.GetButton("Left Mouse") && NextAttack <= 0)
                {
                    Attack();
                }
            }
        }
        NextAttack -= Time.deltaTime;
    }
    #endregion

    void SetCamera()
    {
        if(originalDistance == 0)
            originalDistance = camera.distance;
        if (!toggle.m_gamepadOn)
        {
            camera.ySpeed = MouseSensitivityY;
            camera.xSpeed = MouseSensitivityX;
        }
        else
        {
            camera.ySpeed = originalMouseSensitivityY;
            camera.xSpeed = originalMouseSensitivityX;
        }
        camera.ShootingMode = true;

        movement.PlayAnimation("Aiming");
    }
    void UnSetCamera()
    {
        camera.ySpeed = originalMouseSensitivityY;
        camera.xSpeed = originalMouseSensitivityX;
        camera.ShootingMode = false;

        if(originalDistance != 0)
            camera.distance = originalDistance;

        originalDistance = 0;
        movement.StopAnimation("Aiming");
    }
    Quaternion lookAtSlowly(Transform t, Vector3 target, float speed)
    {
        Vector3 relativePos = target - t.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        return Quaternion.Lerp(t.rotation, toRotation, speed * Time.deltaTime);
    }
    Quaternion lookAtSlowly(Transform t, Quaternion targetRot, float speed)
    {
        return Quaternion.Lerp(t.rotation, targetRot, speed * Time.deltaTime);
    }
    private void OnDestroy()
    {
        UnSetCamera();
    }
    void Attack()
    {
        if (InputManager.GetButton("Right Mouse"))
        {
            GameObject tempbul = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TempBullet"), GunTip.transform.position, GunTip.transform.rotation) ;
            tempbul.GetComponent<PhotonView>().ViewID = GetComponent<PhotonView>().ViewID;
            tempbul.GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Takeover;
            tempbul.GetComponent<PhotonView>().TransferOwnership(GetComponent<PhotonView>().Owner);
            tempbul.GetComponent<Rigidbody>().AddForce(tempbul.transform.forward * shotSpeed);
        }
        NextAttack = 1f;
    }
}
