using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
//using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO; 

public class KeyBindsScript : MonoBehaviour 
{
	public Dictionary<string, KeyCode> keyBinds = new Dictionary<string, KeyCode>();

	public Text sprintKey, crouchKey, fireKey, adsKey, sprintToggleKey, crouchToggleKey, adsToggleKey, 
		interactKey, weaponSwapKey, pauseMenuKey, jumpKey, reloadKey, fireModeKey, throwObject;

	private GameObject currentClickedObject;


	//have this stay active for all scenes, a global script
	//on start, apply player's control prefrences from save file and applt them to controls menu text
	//

	// Use this for initialization
	void Start () 
	{
		//Load();
		defaultPlayerKeys();
		setControlsText();

		//DontDestroyOnLoad(this.gameObject);
	}
	//Input.cr

	private void OnGUI()
	{
		if (currentClickedObject != null)
		{
			Event theEvent = Event.current;
			if (theEvent.isKey && theEvent.type == EventType.KeyUp) { //if ( theEvent.isKey && theEvent.type == EventType.KeyUp )
				print ("" + theEvent.keyCode);
				keyBinds [currentClickedObject.name] = theEvent.keyCode;
				currentClickedObject.GetComponentInChildren<Text>().text = theEvent.keyCode.ToString();
				currentClickedObject = null;
			} 
			else if (theEvent.shift)
			{
				print (theEvent.modifiers);
				keyBinds [currentClickedObject.name] = KeyCode.LeftShift;
				currentClickedObject.GetComponentInChildren<Text>().text = KeyCode.LeftShift.ToString();
				currentClickedObject = null;
			}
		}
	}

	public void OnClick(GameObject clickedObject)
	{
		currentClickedObject = clickedObject;
	}

	/*
	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerKeyBinds.dat");
		PlayerKeyBinds playerBinds = new PlayerKeyBinds();
		playerBinds.adsKey = keyBinds["adsKey"];
		//keyBinds = new Dictionary<string, KeyCode>();
		bf.Serialize(file, playerBinds);
		file.Close();
	}

	public void Load()
	{
		if (File.Exists (Application.persistentDataPath + "/playerKeyBinds.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerKeyBinds.dat", FileMode.Open);
			PlayerKeyBinds playerBinds = (PlayerKeyBinds)bf.Deserialize(file);
		
			keyBinds ["adsKey"] = playerBinds.adsKey;
		}
		else
			defaultPlayerKeys();
	}
	/*
	private void savedPlayerKeys()
	{
		keyBinds.Add ("sprintKey", KeyCode.LeftShift);
		keyBinds.Add ("crouchKey", KeyCode.C);
		keyBinds.Add ("fireKey", KeyCode.Mouse0);
		keyBinds.Add ("adsKey", KeyCode.Mouse1);
		keyBinds.Add ("sprintToggleKey", KeyCode.Alpha1);
		keyBinds.Add ("crouchToggleKey", KeyCode.Alpha2);
		keyBinds.Add ("adsToggleKey", KeyCode.Alpha3);
		keyBinds.Add ("interactKey", KeyCode.E);

		keyBinds.Add ("forwardKey", KeyCode.W);
		keyBinds.Add ("backwardKey", KeyCode.S);
		keyBinds.Add ("leftStrafeKey", KeyCode.A);
		keyBinds.Add ("rightStrafeKey", KeyCode.D);

		keyBinds.Add ("weaponSwapKey", KeyCode.Q);
		keyBinds.Add ("pauseMenuKey", KeyCode.M);
		keyBinds.Add ("jumpKey", KeyCode.Space);
		keyBinds.Add ("reloadKey", KeyCode.R);
		keyBinds.Add ("fireModeKey", KeyCode.T);
		keyBinds.Add ("throwObject", KeyCode.F);
	}
	*/

	private void defaultPlayerKeys()
	{
		keyBinds.Add ("sprintKey", KeyCode.LeftShift);
		keyBinds.Add ("crouchKey", KeyCode.C);
		keyBinds.Add ("fireKey", KeyCode.Mouse0);
		keyBinds.Add ("adsKey", KeyCode.Mouse1);
		keyBinds.Add ("sprintToggleKey", KeyCode.Alpha1);
		keyBinds.Add ("crouchToggleKey", KeyCode.Alpha2);
		keyBinds.Add ("adsToggleKey", KeyCode.Alpha3);
		keyBinds.Add ("interactKey", KeyCode.E);

		keyBinds.Add ("forwardKey", KeyCode.W);
		keyBinds.Add ("backwardKey", KeyCode.S);
		keyBinds.Add ("leftStrafeKey", KeyCode.A);
		keyBinds.Add ("rightStrafeKey", KeyCode.D);

		keyBinds.Add ("weaponSwapKey", KeyCode.Q);
		keyBinds.Add ("pauseMenuKey", KeyCode.M);
		keyBinds.Add ("jumpKey", KeyCode.Space);
		keyBinds.Add ("reloadKey", KeyCode.R);
		keyBinds.Add ("fireModeKey", KeyCode.T);
		keyBinds.Add ("throwObject", KeyCode.F);
	}

	private void setControlsText()
	{
		sprintKey.text = keyBinds["sprintKey"].ToString ();
		crouchKey.text = keyBinds["crouchKey"].ToString ();
		fireKey.text = keyBinds["fireKey"].ToString ();
		adsKey.text = keyBinds["adsKey"].ToString ();
		sprintToggleKey.text = keyBinds["sprintToggleKey"].ToString ();
		crouchToggleKey.text = keyBinds["crouchToggleKey"].ToString ();
		adsToggleKey.text = keyBinds["adsToggleKey"].ToString ();
		interactKey.text = keyBinds["interactKey"].ToString ();
		weaponSwapKey.text = keyBinds["weaponSwapKey"].ToString ();
		pauseMenuKey.text = keyBinds["pauseMenuKey"].ToString ();
		jumpKey.text = keyBinds["jumpKey"].ToString ();
		reloadKey.text = keyBinds["reloadKey"].ToString ();
		fireModeKey.text = keyBinds["fireModeKey"].ToString ();
		throwObject.text = keyBinds["throwObject"].ToString ();
	}
}
/*
[Serializable]
class PlayerKeyBinds
{
	 public KeyCode sprintKey; //Left Shift
	 public KeyCode crouchKey; //c
	 public KeyCode fireKey; //fire1
	 public KeyCode adsKey; //fire2
	public KeyCode sprintToggleKey; //1
	public KeyCode crouchToggleKey; //2
	public KeyCode adsToggleKey; //3
	public KeyCode interactKey; //e
	public KeyCode forwardKey;  //w
	public KeyCode backwardKey; //s
	public KeyCode leftStrafeKey; //a
	public KeyCode rightStrafeKey; //d
	public KeyCode weaponSwapKey;  //l
	public KeyCode pauseMenuKey; //m
	public KeyCode jumpKey; //Space
	public KeyCode reloadKey; //r
	public KeyCode fireModeKey; //t
}
*/