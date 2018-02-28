using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour 
{
	public InputField forwardIn, backwardIn, FireModeIn, leftIn, rightIn, jumpIn;

	private GameObject currentObject = null;
	private static int loadNum = 1;

	void Awake () 
	{
		Debug.Log("Load: " + loadNum++);

		//inField = GameObject.FindGameObjectWithTag ("WalkForward").GetComponent<InputField>();
		if (forwardIn.CompareTag("WalkForward"))
			forwardIn.text = UserPrefrences.control.forward;
		else
			Debug.LogError("error: forward");
		
		if (backwardIn.CompareTag("WalkBackward"))
			backwardIn.text = UserPrefrences.control.backward;
		else
			Debug.LogError("error: backward");
			
		FireModeIn.text = UserPrefrences.control.fireToggle;
		leftIn.text = UserPrefrences.control.left;
		rightIn.text = UserPrefrences.control.right;
		jumpIn.text = UserPrefrences.control.jump;
	}

	public void onClick(GameObject clickedObject)
	{
		currentObject = null;
		currentObject = clickedObject;
	}

	public void OnChange(string newValue)
	{
		if (currentObject.CompareTag("WalkForward"))
		{
			UserPrefrences.control.forward = newValue;
			print (newValue);
			return;
		}
		else if (currentObject.CompareTag("WalkBackward"))
		{
			UserPrefrences.control.backward = newValue;
			return;
		}
		else if (currentObject.CompareTag("StrafeLeft"))//
		{
			UserPrefrences.control.left = newValue;
			return;
		}
		else if (currentObject.CompareTag("StrafeRight"))//
		{
			UserPrefrences.control.right = newValue;
			return;
		}
		else if (currentObject.CompareTag("Jump"))//
		{
			UserPrefrences.control.jump = newValue;
			return;
		}
		else if (currentObject.CompareTag("CycleFireMode"))//CycleFireMode
		{
			UserPrefrences.control.fireToggle = newValue;
			return;
		}
	}
}
