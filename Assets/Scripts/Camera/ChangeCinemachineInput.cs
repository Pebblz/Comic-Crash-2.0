using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Cinemachine;
public class ChangeCinemachineInput : MonoBehaviour
{
    Luminosity.IO.Examples.GamepadToggle toggle;

    [SerializeField]
    string YInputMouse, XInputMouse, YInputController, XInputController;

    CinemachineFreeLook FreeLook;

    // Start is called before the first frame update
    void Start()
    {
        toggle = FindObjectOfType<Luminosity.IO.Examples.GamepadToggle>();
        FreeLook = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!toggle.m_gamepadOn)
        {
            FreeLook.m_YAxis.m_InputAxisName = YInputMouse;
            FreeLook.m_XAxis.m_InputAxisName = XInputMouse;
        }
        else
        {
            FreeLook.m_YAxis.m_InputAxisName = YInputController;
            FreeLook.m_XAxis.m_InputAxisName = XInputController;
        }
    }
}
