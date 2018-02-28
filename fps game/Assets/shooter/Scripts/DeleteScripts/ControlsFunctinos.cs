using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlsFunctinos
{
	public class ControlsFunctinos : MonoBehaviour 
	{
		private bool toggleCrouchHold = true; //True: holdToCrouch
		private bool toggleSprintHold = true; //True: holdToSprint

		public void toggleHoldToSprint()
		{
			if (Input.GetKeyDown("1")) 
			{
				toggleSprintHold = !toggleSprintHold;
				//print ("toggleHoldToSprint: " + toggleSprintHold);
			}
		}

		// True: hold to crouch
		// False: Player is always crouching/standing 
		private void toggleHoldToCrouch()
		{
			if (Input.GetKeyDown("2")) 
			{
				toggleCrouchHold = !toggleCrouchHold;
				//print ("toggleHoldToCrouch: " + toggleCrouchHold);
			}
		}
	}
}
