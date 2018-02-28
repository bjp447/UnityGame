using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//restores the default mouse sensitivity
public class RestoreDefaultsOnClick : MonoBehaviour 
{
	public Button restoreMouseSensButton;
	public Button audioButton;

	//private GameObject temp;
	private Slider temp;
	//private Slider sens;

	// Use this for initialization
	void Start () 
	{
		//GameObject temp = GameObject.FindGameObjectWithTag ("SensSlider");
		//sens = temp.GetComponent<Slider>();

		//Button restoreButton = mouseSensButton.GetComponent<Button> ();
		//restoreButton.onClick.AddListener (restore);

		//temp = GameObject.FindGameObjectWithTag ("SensSlider");
		temp = GameObject.FindGameObjectWithTag ("SensSlider").GetComponent<Slider>();
		restoreMouseSensButton.GetComponent<Button>().onClick.AddListener (restore);

		//temp = GameObject.FindGameObjectWithTag ("AudioSlider");
		//sens = temp.GetComponent<Slider>();

		//restoreButton = audioButton.GetComponent<Button> ();
		//restoreButton.onClick.AddListener (restore);
	}

	void restore()
	{
		UserPrefrences.control.mouseSensitivity = UserPrefrences.control.defaultSensitivity;
		//sens.value = UserPrefrences.control.defaultSensitivity;
		//temp.GetComponent<Slider>().value = UserPrefrences.control.defaultSensitivity;
		temp.value = UserPrefrences.control.defaultSensitivity;
	}



	// Update is called once per frame
	void Update () {
		
	}
}
