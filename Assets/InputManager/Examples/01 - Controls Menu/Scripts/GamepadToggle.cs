#region [Copyright (c) 2018 Cristian Alexandru Geambasu]
//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2018 Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
//	and associated documentation files (the "Software"), to deal in the Software without restriction, 
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial 
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
using UnityEngine;
using UnityEngine.UI;

namespace Luminosity.IO.Examples
{
	public class GamepadToggle : MonoBehaviour 
	{
		[SerializeField]
		private string m_keyboardScheme = null;
		[SerializeField]
		private string m_gamepadScheme = null;

		public bool m_gamepadOn;

		private void Awake()
		{
			if(InputManager.PlayerOneControlScheme.Name == m_keyboardScheme)
			{
				m_gamepadOn = false;
			}
			else
			{
				m_gamepadOn = true;
			}

			InputManager.Loaded += HandleInputLoaded;
		}
        private void Update()
        {
            if (m_gamepadOn)
            {
				if (InputManager.GetAxis("MLookHorizontal") != 0 || InputManager.GetAxis("MLookVertical") != 0 ||
					InputManager.GetAxis("MHorizontal") != 0 || InputManager.GetAxis("MVertical") != 0 ||
					InputManager.GetButtonDown("MJump") || InputManager.GetButtonDown("M1") || 
					InputManager.GetButtonDown("M2") || InputManager.GetButtonDown("MCrouch") || InputManager.GetButtonDown("MSprint") ||
					InputManager.GetButtonDown("MUpDown") || InputManager.GetButtonDown("MLeft Mouse") || InputManager.GetButtonDown("MRight Mouse")
					|| InputManager.GetButtonDown("MPause"))
				{
					InputManager.SetControlScheme(m_keyboardScheme, PlayerID.One);
					m_gamepadOn = false;
				}
				
			}
            else
            {
				if(InputManager.GetAxis("GLookHorizontal") != 0 || InputManager.GetAxis("GLookVertical") != 0 || 
				   InputManager.GetAxis("GHorizontal") != 0 || InputManager.GetAxis("GVertical") != 0 ||
					InputManager.GetButton("GJump") || InputManager.GetButtonDown("G1") ||
					InputManager.GetButtonDown("G2") || InputManager.GetButton("GCrouch") || InputManager.GetButtonDown("GRight Mouse") ||
					InputManager.GetButtonDown("GLeft Mouse") || InputManager.GetButtonDown("GUpDown") || InputManager.GetButtonDown("GPause"))
                {
					InputManager.SetControlScheme(m_gamepadScheme, PlayerID.One);
					m_gamepadOn = true;
				}
            }
		}
        private void OnDestroy()
		{
			InputManager.Loaded -= HandleInputLoaded;
		}

		private void HandleInputLoaded()
		{
			if(m_gamepadOn)
			{
				InputManager.SetControlScheme(m_gamepadScheme, PlayerID.One);
			}
			else
			{
				InputManager.SetControlScheme(m_keyboardScheme, PlayerID.One);
			}
		}

		public void Toggle()
		{
			if(m_gamepadOn)
			{
				InputManager.SetControlScheme(m_keyboardScheme, PlayerID.One);
				m_gamepadOn = false;
			}
			else
			{
				InputManager.SetControlScheme(m_gamepadScheme, PlayerID.One);
				m_gamepadOn = true;
			}
		}
	}
}
