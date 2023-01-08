using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace CMF
{
	//This character movement input class is an example of how to get input from a keyboard to control the character;
    public class CharacterKeyboardInput : CharacterInput
    {
		public string horizontalInputAxis = "Horizontal";
		public string verticalInputAxis = "Vertical";
		public KeyCode jumpKey = KeyCode.Space;

		//If this is enabled, Unity's internal input smoothing is bypassed;
		public bool useRawInput = true;

		public bool IsInputLock = false;
		
        public override float GetHorizontalMovementInput()
        {
	        if (IsInputLock) return 0f;
			
			if(useRawInput)
				return Input.GetAxisRaw(horizontalInputAxis);
			else
				return Input.GetAxis(horizontalInputAxis);
		}

		public override float GetVerticalMovementInput()
		{
	        if (IsInputLock) return 0f;
	        
			if(useRawInput)
				return Input.GetAxisRaw(verticalInputAxis);
			else
				return Input.GetAxis(verticalInputAxis);
		}

		public override bool IsJumpKeyPressed()
		{
	        if (IsInputLock) return false;
			
			return Input.GetKey(jumpKey);
		}
    }
}
