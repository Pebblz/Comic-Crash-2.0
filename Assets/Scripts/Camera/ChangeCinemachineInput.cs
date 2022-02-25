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
    Shop shop;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        toggle = FindObjectOfType<Luminosity.IO.Examples.GamepadToggle>();
        FreeLook = GetComponent<CinemachineFreeLook>();
        pause = FindObjectOfType<Pause>();
        shop = FindObjectOfType<Shop>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shop != null)
        {
            if (!pause.isPaused && !shop.active)
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
        }
        else
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

}
