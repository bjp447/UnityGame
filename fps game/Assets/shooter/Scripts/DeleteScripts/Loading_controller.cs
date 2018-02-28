using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Loading_controller : MonoBehaviour 
{
	private Global_controller globalController;
	private Inventory_controller inventoryController;
	private Menu_controller menuController;


	// Use this for initialization
	void Start () 
	{
		globalController = GameObject.Find("Global_controller").GetComponent<Global_controller>();
		inventoryController = GameObject.Find("PlayerController").GetComponent<Inventory_controller>();
		menuController = GameObject.Find("Menu_Controller").GetComponent<Menu_controller>();

		//if starting new game, then level index is 1:
		if (globalController.isStartingNewGame == true)
		{
			//load level1's default inventory
			//save player's start Inventory in SaveInventory()
			//save levelInfo in SaveLevel()
			//set isStartingNewGame to false for next levels
			//set isContinueingGame to false for next levels
			inventoryController.LoadDefaultLevelInventory();
			inventoryController.SaveInventory();
			//menuController.SaveLevel();
			globalController.isStartingNewGame = false;
			globalController.isContinueingGame = true;


		}
		//if player is continueing the game:
		else if (globalController.isContinueingGame == true )
		{
			//load in level from LoadLastCheckpoint or from LoadLevel.
				//check Checkpoint first
			//load in player's inventory from LoadCheckpointInventory or from LoadInventory
				//check Checkpoint inventory first

			//menuController.LoadLastCheckpoint(); //auto checks if file exists 
			inventoryController.LoadCheckpointInventory(); //auto checks if there is a checkpoint
			//menuController.SaveLevel();
			inventoryController.SaveInventory();

			//delete checkpoint files at the start of every level
			if (File.Exists(Application.persistentDataPath + "/lastCheckpointInformation.dat"))
				File.Delete(Application.persistentDataPath + "/lastCheckpointInformation.dat");
			if (File.Exists(Application.persistentDataPath + "/playerCheckpointInventory.dat"))
				File.Delete(Application.persistentDataPath + "/playerCheckpointInventory.dat");
		}
		else if (globalController.isFromLevelSelect == true)
		{
			//ignore level and inventory files
			inventoryController.LoadDefaultLevelInventory();
		}
	}
}
