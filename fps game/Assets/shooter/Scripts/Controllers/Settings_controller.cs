//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;

public class Settings_controller : MonoBehaviour
{
    //publicly accessed settings values
    [Range(0.0f, 10.0f)] public float xSens, ySens, aimXSens, aimYSens;
    public bool invertY = false, holdToCrouch = false, holdToADS = false, holdToSprint = false;
    [Range(1.0f, 179.0f)] public float fOV;

	private Slider xSensSlider, ySensSlider, fOVSlider, aimXSensSlider, aimYSensSlider;
    private Text xSensTxt, ySensTxt, fOVSliderTxt, aimXSensTxt, aimYSensTxt;
	//[Header("\t      Misc")]
	private Toggle invertYToggle, toggleCrouch, toggleADS, toggleSprint;
	private Button saveButton1; //save button for settings panel
    private Button saveButton2; //save button for keyBinds, not used yet

    //public Dictionary<string, float> sliderVals = new Dictionary<string, float>();

	//KeyBinds
	public Dictionary<string, KeyCode> keyBinds = new Dictionary<string, KeyCode>();
    //private Text[] keysUI;
	private GameObject currentClickedObject;
    private Text currSelectedText; //uesd only for updating InteractText, got via OnClick

	private void Start()
	{
        //settings Panel
        FindSettingsGameobjects();
		LoadSettings();
        Camera.main.fieldOfView = fOV;
        SetMenuSettings();
        saveButton1.interactable = false;

        //SaveSettings() are on button clicks

        //KeyBinds Panel
        DefaultPlayerKeys();
        FindAndSetKeyBindUIobjs();
        //SetControlsText();
        //SetInteractTexts();
        SetStrOfInteractTxtObj(GameObject.FindGameObjectWithTag("UI").transform.
            Find("InteractText").gameObject.GetComponent<Text>(), "interactKey", false);

        //print("Joysticks:" + Input.GetJoystickNames()[0]);
    }

    #region (Toggle functions)
    // False: Sprint is always on/off; // True: holdToSprint
    public void ToggleHoldToSprint()
    {
        holdToSprint = !holdToSprint;
        setMenuSettingsOnPlayerHotKeyPress();
    }

    // False: Player is always crouching/standing; // True: hold to crouch
    public void ToggleHoldToCrouch()
    {
        holdToCrouch = !holdToCrouch;
        setMenuSettingsOnPlayerHotKeyPress();
    }

    public void ToggleHoldToADS()
    {
        holdToADS = !holdToADS;
        setMenuSettingsOnPlayerHotKeyPress();
    }
    #endregion


    #region (Settings Panel)
    private void FindSettingsGameobjects()
	{
		xSensSlider = GameObject.Find("HorizontalSensSlider").GetComponent<Slider>();
		ySensSlider = GameObject.Find("VerticalSensSlider").GetComponent<Slider>();
        aimXSensSlider = GameObject.Find("AimXSensSlider").GetComponent<Slider>(); //
        aimYSensSlider = GameObject.Find("AimYSensSlider").GetComponent<Slider>(); //
        fOVSlider = GameObject.Find("FieldOfViewSlider").GetComponent<Slider>();
        xSensTxt = GameObject.Find("HorizontalSensValueText").GetComponent<Text>();
		ySensTxt = GameObject.Find("VerticalSensValueText").GetComponent<Text>();
        aimXSensTxt = GameObject.Find("AimXValueText").GetComponent<Text>(); //
        aimYSensTxt = GameObject.Find("AimYValueText").GetComponent<Text>(); //
        fOVSliderTxt = GameObject.Find("FieldOfViewValueTxt").GetComponent<Text>();
        invertYToggle = GameObject.Find("InvertYToggle").GetComponent<Toggle>();
		toggleCrouch = GameObject.Find("ToggleCrouch").GetComponent<Toggle>();
		toggleADS = GameObject.Find("ToggleADS").GetComponent<Toggle>();
		toggleSprint = GameObject.Find("ToggleSprint").GetComponent<Toggle>();

        saveButton1 = GameObject.Find("SaveBtn1").GetComponent<Button>();
	}

	public void onSliderChange(Slider currentSlider)
	{
        if (string.CompareOrdinal(currentSlider.name, "HorizontalSensSlider") == 0) {
            xSens = currentSlider.value;
            xSensTxt.text = xSens.ToString();
        } else if (string.CompareOrdinal(currentSlider.name, "VerticalSensSlider") == 0) {
            ySens = currentSlider.value;
            ySensTxt.text = ySens.ToString();
        } else if (string.CompareOrdinal(currentSlider.name, "FieldOfViewSlider") == 0) {
            fOV = currentSlider.value;
            fOVSliderTxt.text = fOV.ToString();
            Camera.main.fieldOfView = fOV;
        } else if (string.CompareOrdinal(currentSlider.name, "AimXSensSlider") == 0) {
            aimXSens = currentSlider.value;
            aimXSensTxt.text = aimXSens.ToString();
        } else if (string.CompareOrdinal(currentSlider.name, "AimYSensSlider") == 0) {
            aimYSens = currentSlider.value;
            aimYSensTxt.text = aimYSens.ToString();
        }
		onSettingChange();
	}

	public void onToggleChange(Toggle currentToggle)
	{
		if (currentToggle.name == "InvertYToggle")
			invertY = currentToggle.isOn;
		else if (currentToggle.name == "ToggleCrouch")
			holdToCrouch = currentToggle.isOn;
		else if (currentToggle.name == "ToggleADS")
			holdToADS = currentToggle.isOn;
		else if (currentToggle.name == "ToggleSprint")
			holdToSprint = currentToggle.isOn;
		onSettingChange();
	}

	public void onButtonClicked(Button currentButton)
	{
		if (currentButton.name == "SaveBtn1")
            saveButton1.interactable = false;
		else if (currentButton.name == "setDefaultsBtn")
            SetToDefaultSettings();
	}

	public void onSettingChange()
	{
		if (saveButton1.interactable == false)
            saveButton1.interactable = true;
	}

    public void SaveSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/playerSettings.dat")) {
            file = File.Open(Application.persistentDataPath + "/playerSettings.dat", FileMode.Open);
            //print ("file opened");
        }
        else
            file = File.Create(Application.persistentDataPath + "/playerSettings.dat");

        PlayerSettings playerSettings = new PlayerSettings();
		playerSettings.xSens = xSens;
		playerSettings.ySens = ySens;
        playerSettings.aimXSens = aimXSens;
        playerSettings.aimYSens = aimYSens;
        playerSettings.fOV = fOV;
		playerSettings.invertY = invertY;
		playerSettings.holdToCrouch = holdToCrouch;
		playerSettings.holdToADS = holdToADS;
		playerSettings.holdToSprint = holdToSprint;

		bf.Serialize(file, playerSettings);
		file.Close();
	}

	private void LoadSettings()
	{
		if (File.Exists (Application.persistentDataPath + "/playerSettings.dat") && 
			new FileInfo(Application.persistentDataPath + "/playerSettings.dat").Length > 0) 
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerSettings.dat", FileMode.Open);

			PlayerSettings playerSettings = (PlayerSettings)bf.Deserialize (file);
			xSens = playerSettings.xSens;
			ySens = playerSettings.ySens;
            aimXSens = playerSettings.aimXSens;
            aimYSens = playerSettings.aimYSens;
            fOV = playerSettings.fOV;
			invertY = playerSettings.invertY;
			holdToCrouch = playerSettings.holdToCrouch;
			holdToADS = playerSettings.holdToADS;
			holdToSprint = playerSettings.holdToSprint;

			file.Close();
		} else
            SetToDefaultSettings();
	}

	private void SetMenuSettings()
	{
		xSensSlider.value = xSens;
		ySensSlider.value = ySens;
        aimXSensSlider.value = aimXSens;
        aimYSensSlider.value = aimYSens;
        fOVSlider.value = fOV;
		xSensTxt.text = xSens.ToString();
		ySensTxt.text = ySens.ToString();
        aimXSensTxt.text = aimXSens.ToString();
        aimYSensTxt.text = aimYSens.ToString();
        fOVSliderTxt.text = fOV.ToString();

		invertYToggle.isOn = invertY;
		toggleCrouch.isOn = holdToCrouch;
		toggleADS.isOn = holdToADS;
		toggleSprint.isOn = holdToSprint;
	}

	private void SetToDefaultSettings()
	{
		xSensSlider.value = 5;
		ySensSlider.value = 5;
        aimXSensSlider.value = 5;
        aimYSensSlider.value = 5;
        fOVSlider.value = 60;
		xSensTxt.text = xSensSlider.value.ToString();
		ySensTxt.text = ySensSlider.value.ToString();
        aimXSensTxt.text = aimXSensSlider.value.ToString();
        aimYSensTxt.text = aimYSensSlider.value.ToString();
        fOVSliderTxt.text = fOVSlider.value.ToString();
        Camera.main.fieldOfView = 60;

		invertYToggle.isOn = false;
		toggleCrouch.isOn = false;
		toggleADS.isOn = false;
		toggleSprint.isOn = true;
	}

	public void setMenuSettingsOnPlayerHotKeyPress()
	{
		invertYToggle.isOn = invertY;
		toggleCrouch.isOn = holdToCrouch;
		toggleADS.isOn = holdToADS;
		toggleSprint.isOn = holdToSprint;
	}
    #endregion


    #region U.I.
    public void GetTextObjOnClick(Text textObj)
    {
        currSelectedText = textObj;
    }

    //params:   Text- the text object to be modified
    //          String- the keybind name that the Text.text will change to
    public void SetStrOfInteractTxtObj(Text textObj, string str, bool hold)
    {
        if (hold == true)
            textObj.text = "Hold ";
        else
            textObj.text = "";

        //general gameplay
        if (string.CompareOrdinal(str, "interactKey") == 1)
            textObj.text = textObj.text + "[" + keyBinds[str].ToString() + "] to interact";

        //tip pop-ups
        else if (string.CompareOrdinal(str, "reloadKey") == 1)
            textObj.text = textObj.text + "[" + keyBinds[str].ToString() + "] to reload";
        else if (string.CompareOrdinal(str, "crouchToggleKey") == 1)
            textObj.text = textObj.text + "[" + keyBinds[str].ToString() + "] to crotch";


        else if (string.CompareOrdinal(str, "interactKey") == 1) //&& isOverWeapon
        {
            textObj.text = textObj.text + "[" + keyBinds[str].ToString() + "] to pick up";

        }


        //hold to pick up (weapons)
        //press to melee
        //press to 
    }

    /*
    public void SetInteractTexts()
    {
        //press to interact
        GameObject.FindGameObjectWithTag("UI").transform.Find("InteractText").gameObject.
            GetComponent<Text>().text = "[" + keyBinds["interactKey"].ToString() + "] to interact";
    }
    */
    #endregion


    #region (KeyBindsPanel)
    //called on Start when applying save data
    private void FindAndSetKeyBindUIobjs()
    {
        GameObject keysPanel = GameObject.Find("KeyBindsPanel"); //Keys panel in KeyBindsPanel
        Button[] buttonKeys = keysPanel.GetComponentsInChildren<Button>();

        foreach (Button b in buttonKeys)
        {
            if (b.CompareTag("KeyButton") == true)
                b.GetComponentInChildren<Text>().text = keyBinds[b.name].ToString();
        }
        /*
        int len = buttonKeys.Length;
        for (i = 0; i < len; i++)
            print(keysUI[i].transform.parent.name);
        */
    }

	private void OnGUI()
	{
		if (currentClickedObject != null)
		{
			Event theEvent = Event.current;
			if (theEvent.isKey && theEvent.type == EventType.KeyUp) { //if ( theEvent.isKey && theEvent.type == EventType.KeyUp )
				print("" + theEvent.keyCode);
				keyBinds[currentClickedObject.name] = theEvent.keyCode; //updates keyBind
                currentClickedObject.GetComponentInChildren<Text>().text = theEvent.keyCode.ToString(); //updates keyBind's Text

                if (string.CompareOrdinal(currentClickedObject.name, "interactKey") == 0)
                {
                    SetStrOfInteractTxtObj(currSelectedText, "interactKey", false);//updates interactText
                    currSelectedText = null;
                }
                currentClickedObject = null;

            } 
			else if (theEvent.shift)
			{
				print(theEvent.modifiers);
				keyBinds[currentClickedObject.name] = KeyCode.LeftShift;
				currentClickedObject.GetComponentInChildren<Text>().text = KeyCode.LeftShift.ToString();
				currentClickedObject = null;
			}
		}
	}

	//gets currently clicked object
	public void OnClick(GameObject clickedObject)
	{
		currentClickedObject = clickedObject;
	}

	private void DefaultPlayerKeys()
	{
		keyBinds.Add ("sprintKey", KeyCode.LeftShift);
		keyBinds.Add ("crouchKey", KeyCode.C);
		keyBinds.Add ("fireKey", KeyCode.Mouse0);
        keyBinds.Add ("meleeKey", KeyCode.X);
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

    //keycodes work for buttons only. Axis like the Triggers, Sticks don't work.
    private void DefaultXboxControllerKeys()
    {
        keyBinds.Add("jump", KeyCode.Joystick1Button0); //'A' button
    }
	#endregion
}

[Serializable]
class PlayerSettings
{
	public float xSens;
	public float ySens;
    public float aimXSens;
    public float aimYSens;
    public float fOV;
    public bool invertY;
	public bool holdToCrouch;
	public bool holdToADS;
	public bool holdToSprint;
}