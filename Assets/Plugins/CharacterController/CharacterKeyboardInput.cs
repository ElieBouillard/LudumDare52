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

		public bool IsZqsd;
		
        public override float GetHorizontalMovementInput()
        {
	        if (IsInputLock) return 0f;

	        float horizontalValue = 0;
	        if (IsZqsd)
	        {
		        if (Input.GetKey(KeyCode.Q)) horizontalValue = -1;
		        else if (Input.GetKey(KeyCode.D)) horizontalValue = 1;
		        else horizontalValue = 0;
	        }
	        else
	        {
		        if (Input.GetKey(KeyCode.A)) horizontalValue = -1;
		        else if (Input.GetKey(KeyCode.D)) horizontalValue = 1;
		        else horizontalValue = 0;
	        }

	        return horizontalValue;
        }

		public override float GetVerticalMovementInput()
		{
	        if (IsInputLock) return 0f;
	        
	        float verticalValue = 0;
	        
	        if (IsZqsd)
	        {
		        if (Input.GetKey(KeyCode.Z)) verticalValue = 1;
		        else if (Input.GetKey(KeyCode.S)) verticalValue = -1;
		        else verticalValue = 0;
	        }
	        else
	        {
		        if (Input.GetKey(KeyCode.W)) verticalValue = 1;
		        else if (Input.GetKey(KeyCode.S)) verticalValue = -1;
		        else verticalValue = 0;
	        }

	        return verticalValue;
		}

		public override bool IsJumpKeyPressed()
		{
	        if (IsInputLock) return false;
			
			return Input.GetKey(jumpKey);
		}
    }
}
