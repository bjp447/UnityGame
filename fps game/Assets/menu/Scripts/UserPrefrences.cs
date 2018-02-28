using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityStandardAssets.Cameras; //need this to access FreeLookCam from unitys standard assets library

public class UserPrefrences : MonoBehaviour {

	public static UserPrefrences control;

	public float defaultSensitivity = 2.0f;
	public float mouseSensitivity = 2.0f;

	public float FXvolume;
	public float defaultFXvolume = 2.0f;

	public float brightness;
	public float defaultBrightness = 2.0f;

	public string mouseFire;
	public string fireToggle;
	public string pauseMenu;

	public string forward;
	public string backward;
	public string left;
	public string right;
	public string jump;

	//private GameObject sens;
	//private Slider sens;

	void Awake () 
	{
		if (control == null)//if control hasnt been asigned yet
		{
			DontDestroyOnLoad (gameObject);
			control = this; //this becomes the object referenced
		}
		else if (control != this) //if it exists, and this isnt it, destroy this
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	//void Start () 
	//{
		/*
		GameObject temp = GameObject.FindGameObjectWithTag ("SensSlider");
		if (temp != null)
		{
			sens = temp.GetComponent<Slider>();
			if (sens != null)
			{
				sens.value = mouseSensitivity; //sets slider value to player's sensitivity
			}
		}
		*/

		/*
		GameObject rig = GameObject.Find("Scores");
		Scoring rtspeed = rig.GetComponent<Scoring>();//rig.GetComponent<FreeLookCam> ();
		rtspeed.lvl2Time = 5;
		*/
		//GameObject rig = GameObject.Find("Player Camera");
		//float rotspeed = rig.GetComponent<CameraController>().rotateSpeed;//rig.GetComponent<FreeLookCam> ();

		//GameObject sens = GameObject.FindGameObjectWithTag ("SensSlider");
		//mouseSensitivity = sens.GetComponent<Slider>().value;
		//mouseSensitivity = sens.value;

		//GameObject rig = GameObject.Find("FreeLookCameraRig");
		//rig.GetComponent<FreeLookCam>().m_TurnSpeed = val1;//rig.GetComponent<FreeLookCam> (); //sets the camera/mouse turn speed to 10
		//turnSpeed = 10;
		
	//}

	//Callback executed when the value of the slider is changed.
	//newValue is connected to the slider object this script is connected to
	//public void OnValueChanged(float newValue)
	//{
	//	mouseSensitivity = newValue;
	//}
}
