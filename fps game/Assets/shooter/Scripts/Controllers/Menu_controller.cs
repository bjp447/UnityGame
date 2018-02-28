using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Menu_controller : MonoBehaviour 
{
	[SerializeField] public bool isPaused = false;
	[SerializeField] private GameObject pauseMenu;
	private levelData currentLvlData;
	private GameObject currentPanelOpen;

	//public Button LastCheckpointBtn, RestartLvlBtn, QuitLvlBtn;

	//private GameObject lvlInfoPanel;
	[Serializable] private class LvlInfoPanelObjects
	{
		//private GameObject lvlInfoPanel;
		[SerializeField] public Image lvlImage;
		[SerializeField] public Text lvlDescription;
		[SerializeField] public GameObject lvlObjectivesSection;
	}
	[SerializeField] private LvlInfoPanelObjects lvlInfoPanelObjects;

	private Global_controller globalController;
	//private Inventory_controller playerInventory;

	void Start ()
	{
        globalController = GameObject.Find("Global_controller").GetComponent<Global_controller>();

        foreach (GameObject cBtnObj in GameObject.FindGameObjectsWithTag("CheckpointBtn"))
            cBtnObj.GetComponent<Button>().onClick.AddListener(globalController.onLastCheckpointClick);
        foreach (GameObject rBtnObj in GameObject.FindGameObjectsWithTag("RestartBtn"))
            rBtnObj.GetComponent<Button>().onClick.AddListener(globalController.restartLevel);
        foreach (GameObject qBtnObj in GameObject.FindGameObjectsWithTag("QuitBtn"))
            qBtnObj.GetComponent<Button>().onClick.AddListener(globalController.quitLevel);

        currentPanelOpen = pauseMenu.transform.GetChild(0).gameObject;
        foreach (Transform childTR in pauseMenu.transform) {
            if (string.CompareOrdinal(childTR.gameObject.name, currentPanelOpen.name) != 0)
                childTR.gameObject.SetActive(false);
        }

        if (pauseMenu.activeInHierarchy == true)
			pauseMenu.SetActive(false);

        SetLvlInfoPanel();
    }

    private void SetLvlInfoPanel()
	{
		currentLvlData = GameObject.FindGameObjectWithTag("LvlData").GetComponent<levelData>();

		lvlInfoPanelObjects.lvlImage.sprite = currentLvlData.lvlSprite;
		lvlInfoPanelObjects.lvlDescription.text = currentLvlData.lvlDescription;
	}

	//called by Update in Player_controller and in the editor
	public void displayMenu()
	{
        GameObject startPanel = pauseMenu.transform.GetChild(0).gameObject;

        if (currentPanelOpen.name != "KeyBindsPanel") {
			if (pauseMenu.activeInHierarchy == true) { //disable menu
				Time.timeScale = 1;
				pauseMenu.SetActive(false);
				isPaused = false;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;

				if (currentPanelOpen != startPanel) {
					currentPanelOpen.SetActive(false);
                    startPanel.SetActive(true);
                }
            } else { //enable menu
				Time.timeScale = 0;
				pauseMenu.SetActive(true);
				isPaused = true;
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
			}
		}
	}

    //called by OnPlayerDeath() in Player_controller.cs
    public void DisplayDeathMenu()
    {
        foreach (Transform childTR in pauseMenu.transform) {
            if (string.CompareOrdinal(childTR.gameObject.name, "startPanel") == 0)
                childTR.gameObject.SetActive(false); //startPanel
            if (string.CompareOrdinal(childTR.gameObject.name, "deathPanel") == 0)
                childTR.gameObject.SetActive(true); //deathPanel
        }
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

	public void GetCurrentPanelOpen(GameObject openPanel)
	{ currentPanelOpen = openPanel; }

	//save two states of the game.
	//	One made at the begining of the level 	(lastLevel file)
	//	One made at the trigger of a checkpoint (lastCheckpoint file)

	//TODO: Last level file will contian player's inventory from the start of the level/ end of last level,
	//		the level index.
	//TODO: Last Checkpoint file will contain player's inventory from last checkpoint, 
	//		location, triggers already triggered, objects placed around,
	//		the level index.

	//if player continues the game from main menu:
	//	load last checkpoint or level files
	//		if last checkpoint file is empty, load Level file instead

	//if player quits before getting a checkpoint in new level, LastCheckpoint file will be empty

	//empty LastCheckpoint file on load of new level

	//if player is playing from Continue Game, inventory will be saved between levels, load in inventory
	//	both files will be used

	//if player starts new game
	//	both files will be emptyed
	//	both files will be used

	//if player uses level select
	//	neither files will be used
	//	load in preset inventory for specified level

	//save player's inventory at the end of a level and when hitting a checkpoint

	/*
	//if player is using level select, LastLevel file will become empty, 
	//	LastCheckpoint file will be used for it's checkpoint
	*/
}