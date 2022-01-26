using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Cinemachine;
using Photon.Pun;
public class ChangeCinemachineInput : MonoBehaviour
{
    Luminosity.IO.Examples.GamepadToggle toggle;

    [SerializeField]
    string YInputMouse, XInputMouse, YInputController, XInputController;

    CinemachineFreeLook FreeLook;
    Pause pause;
    private SoundManager soundManager;

    private float ghostPositionY;

    PlayerMovement player;
    [SerializeField]
    Camera cam;

    Vector3 vel;
    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        toggle = FindObjectOfType<Luminosity.IO.Examples.GamepadToggle>();
        FreeLook = GetComponent<CinemachineFreeLook>();
        pause = FindObjectOfType<Pause>();

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause.isPaused)
        {

            if (!toggle.m_gamepadOn)
            {
                FreeLook.m_YAxis.m_InputAxisName = YInputMouse;
                FreeLook.m_XAxis.m_InputAxisName = XInputMouse;
            }
            if (toggle.m_gamepadOn)
            {
                FreeLook.m_YAxis.m_InputAxisName = YInputController;
                FreeLook.m_XAxis.m_InputAxisName = XInputController;
            }
        }
        else
        {
            //stops camera movement when paused
            FreeLook.m_YAxis.m_InputAxisName = "";
            FreeLook.m_XAxis.m_InputAxisName = "";
            FreeLook.m_XAxis.Reset();
            FreeLook.m_YAxis.Reset();
        }
        if (player != null)
        {
            OnLeaveGround();
        }
        else
        {
            player = PhotonFindCurrentClient().GetComponent<PlayerMovement>();
        }
    }
    void OnLeaveGround()
    {
        // update Y for behavior 3
        ghostPositionY = player.transform.position.y;
    }
    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 characterViewPos = cam.WorldToViewportPoint(player.transform.position + player.velocity * Time.deltaTime);

            // behavior 2
            if (cam.transform.position.y > 0.85f || cam.transform.position.y < 0.3f)
            {
                ghostPositionY = player.transform.position.y;
            }
            // behavior 4
            else if (player.OnGround)
            {
                ghostPositionY = player.transform.position.y;
            }    // behavior 5
            var desiredPosition = new Vector3(player.transform.position.x, ghostPositionY, player.transform.position.z); transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref vel, .1f, 5);
        }
        else
        {
            player = PhotonFindCurrentClient().GetComponent<PlayerMovement>();
        }
    }

    public void Underwater()
    {
        RenderSettings.fog = true;
        soundManager.to_underwater();
    }
    public void NotUnderwater()
    {
        RenderSettings.fog = false;
        soundManager.to_normal_from_water();
    }
    GameObject PhotonFindCurrentClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject g in players)
        {
            if (g.GetComponent<PhotonView>().IsMine)
                return g;
        }
        return null;
    }
}
