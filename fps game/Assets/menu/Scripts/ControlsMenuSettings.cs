using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenuSettings : MonoBehaviour 
{
	private Slider sens;

	private static int loadNum = 1;

	// Use this for initialization
	void Start ()
	{
		Debug.Log("Load: " + loadNum++);

		//script object must be near slider object (under same Parent)
		GameObject temp = GameObject.FindGameObjectWithTag ("SensSlider");
		if (temp != null)
		{
			sens = temp.GetComponent<Slider>();
			if (sens != null)
			{
				//print ("mouseSens: " + UserPrefrences.control.mouseSensitivity);
				sens.value = UserPrefrences.control.mouseSensitivity; //sets slider value to player's sensitivity
			}
			else 
				Debug.LogError("error: Slider");
		}
		else
			Debug.LogError("error: gameObject");
	}

	//Callback executed when the value of the slider is changed.
	//newValue is connected to the slider object this script is connected to
	public void OnValueChanged(float newValue)
	{
		UserPrefrences.control.mouseSensitivity = newValue;
		//print ("mouseSens: " + UserPrefrences.control.mouseSensitivity);
	}
}
