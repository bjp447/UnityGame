using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class Global_controller : MonoBehaviour 
{
	public bool isContinueingGame = false;
	public bool isStartingNewGame = false;
	public bool isFromLevelSelect = false;

	private Inventory_controller inventoryController;

	public static Global_controller instance = null;
	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	//------------------
	#region clicked from Main Menu
	public void continueGameClicked()
	{
		isContinueingGame = true;
		isStartingNewGame = false;
		isFromLevelSelect = false;

		LoadLastCheckpoint();
		//StartCoroutine (continueGameWait());

		//use and load level files
	}
	//------------------------

	//----------------
	public void startNewGameClicked()
	{
		print ("Starting New Game...");
		isContinueingGame = false;
		isStartingNewGame = true;
		isFromLevelSelect = false;

		//delete previous checkpoin files if they exist
		if (File.Exists (Application.persistentDataPath + "/lastCheckpointInformation.dat")) {
			print("level checkpoint deleted");
			File.Delete (Application.persistentDataPath + "/lastCheckpointInformation.dat");
		}
		if (File.Exists (Application.persistentDataPath + "/playerCheckpointInventory.dat")) {
			print ("checkpoint inventory deleted");
			File.Delete (Application.persistentDataPath + "/playerCheckpointInventory.dat");
		}

		SceneManager.LoadScene(1);
		StartCoroutine (startNewWait());

		//empty and load level files
	}
	private IEnumerator startNewWait()
	{
		yield return null;
		SaveLevel();
		inventoryController = GameObject.Find("PlayerController").GetComponent<Inventory_controller>();
		inventoryController.LoadDefaultLevelInventory();
		inventoryController.SaveInventory();
	}
	//-------------------------

	public void levelSelectClicked()
	{
		isContinueingGame = false;
		isStartingNewGame = false;
		isFromLevelSelect = true;
	}

	//-------------
	public void loadLevelOnClick(int index)
	{ 
		StartCoroutine(loadOnClickWait(index));
	}
	private IEnumerator loadOnClickWait(int index)
	{
		SceneManager.LoadScene(index, LoadSceneMode.Single);
		//print ("loading...");
		yield return null;

		//print (SceneManager.GetSceneByBuildIndex(1).isLoaded);
		//print(GameObject.Find ("LevelData").name);
		inventoryController = GameObject.Find("PlayerController").GetComponent<Inventory_controller>();
		inventoryController.LoadDefaultLevelInventory();
	}
	//-----------------
	#endregion


	public void onLevelComplete()
	{
		if (isFromLevelSelect == false) {
			inventoryController.SaveInventory();
			loadNextLevel();
		} else {
			quitLevel();
			print ("fromLevelSelect");
		}
	}

	public void onCheckpointTriggered()
	{
		//print("hit Checkpoint");

		if (isFromLevelSelect == false) {
			SaveLastCheckpoint();
			inventoryController.SaveCheckpointInventory();
		} else {
			//quitLevel();
			print ("fromLevelSelect");
		}
	}

	//-----------
	private void loadNextLevel()
	{
		int theLastScene = SceneManager.sceneCountInBuildSettings - 1; //scenes starts at index 0
		int currentScene = SceneManager.GetActiveScene().buildIndex;
		//print(SceneManager.GetActiveScene().buildIndex);
		//print (SceneManager.sceneCountInBuildSettings);

		//if last level quit to menu
		//last scene is currently 2
		if (currentScene == theLastScene) {
			quitLevel();
		} else {
			SceneManager.LoadScene(currentScene+1, LoadSceneMode.Single); //loads next level
			StartCoroutine(loadNextWait());

		}
	}
	private IEnumerator loadNextWait()
	{
		yield return null;
		SaveLevel();
		inventoryController = GameObject.Find("PlayerController").GetComponent<Inventory_controller>();
		inventoryController.LoadInventory();
		//inventoryController.SaveInventory();
		File.Delete(Application.persistentDataPath + "/lastCheckpointInformation.dat");
		File.Delete(Application.persistentDataPath + "/playerCheckpointInventory.dat");
	}
	//----------------------

	#region clicked from Pause Menu
	//-------------------
	public void onLastCheckpointClick()
	{
		Time.timeScale = 1;
		if (isFromLevelSelect == false) {
			LoadLastCheckpoint ();
		} else {
			print ("fromLevelSelect");
			int currentScene = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene (currentScene);


			StartCoroutine(lastCheckpointWait2()); //fix for level Selects own checkpoint file
		}
	}
	private IEnumerator lastCheckpointWait1(Vector3 lastPlayerPos)
	{
		yield return null;
		inventoryController = GameObject.Find("PlayerController").GetComponent<Inventory_controller>();
		inventoryController.LoadCheckpointInventory();

		inventoryController.transform.position = lastPlayerPos;
	}
	private IEnumerator lastCheckpointWait2()
	{		
		yield return null;
		inventoryController = GameObject.Find("PlayerController").GetComponent<Inventory_controller>();
		inventoryController.LoadCheckpointInventory();
	}
	//-----------------------

	//---------------
	public void restartLevel()
	{
		Time.timeScale = 1;
		if (isFromLevelSelect == false) {
			LoadLevel ();
			//inventoryController.LoadInventory ();
			StartCoroutine(restartWait1());
		} else {
			print ("fromLevelSelect");
			int currentScene = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene (currentScene);
			StartCoroutine(restartWait2());
			//inventoryController.LoadDefaultLevelInventory();

		}
	}
	private IEnumerator restartWait1()
	{
		yield return null;
		inventoryController = GameObject.Find("PlayerController").GetComponent<Inventory_controller>();
		inventoryController.LoadInventory();
	}

	private IEnumerator restartWait2()
	{
		yield return null;
		inventoryController = GameObject.Find("PlayerController").GetComponent<Inventory_controller>();
		inventoryController.LoadDefaultLevelInventory();
	}
	//------------------

	public void quitLevel()
	{
		Time.timeScale = 1;
		//GameObject.Find("Menu_Controller").GetComponent<Menu_controller>().displayMenu();
		SceneManager.LoadScene(0, LoadSceneMode.Single); //load main menu
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
	}
	#endregion

	#region Save and Load files
	//called when player begins a level
	public void SaveLevel()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		if (File.Exists(Application.persistentDataPath + "/lastLevelInformation.dat"))
			file = File.Open(Application.persistentDataPath + "/lastLevelInformation.dat",  FileMode.Open);
		else
			file = File.Create(Application.persistentDataPath + "/lastLevelInformation.dat");

		SaveLevelInformation lvlInfo = new SaveLevelInformation ();
		lvlInfo.sceneIndex = SceneManager.GetActiveScene().buildIndex;
		//lvlInfo.playerPos = GameObject.Find("PlayerController").transform.position;

		//GameObject.Find("PlayerController").GetComponent<Player_controller>();
		//serialize player's Inventory at the beggining of the level

		bf.Serialize(file, lvlInfo);
		file.Close();

		print ("level: " + SceneManager.GetActiveScene().buildIndex + " Saved");
	}

	//called when player continues from the main menu
	public void LoadLevel()
	{
		if (File.Exists (Application.persistentDataPath + "/lastLevelInformation.dat") &&
		    new FileInfo (Application.persistentDataPath + "/lastLevelInformation.dat").Length > 0) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/lastLevelInformation.dat", FileMode.Open);

			SaveLevelInformation lvlInfo = (SaveLevelInformation)bf.Deserialize (file);

			print("loaded level: " + lvlInfo.sceneIndex);
			file.Close ();

			SceneManager.LoadScene (lvlInfo.sceneIndex);
		} else
			Debug.LogError("error");
	}

	//called when player triggers a checkpoint, loads in to the game from the main menu,
	public void SaveLastCheckpoint()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		if (File.Exists(Application.persistentDataPath + "/lastCheckpointInformation.dat"))
			file = File.Open(Application.persistentDataPath + "/lastCheckpointInformation.dat",  FileMode.Open);
		else
			file = File.Create(Application.persistentDataPath + "/lastCheckpointInformation.dat");

		//----
		SaveLevelInformation lvlInfo = new SaveLevelInformation ();

		lvlInfo.sceneIndex = SceneManager.GetActiveScene().buildIndex; //scene Index
        //player's position
        Vector3 lastPlayerPos = GameObject.Find("PlayerController").transform.position;
		lvlInfo.playerPos.x = lastPlayerPos.x;
		lvlInfo.playerPos.y = lastPlayerPos.y;
		lvlInfo.playerPos.z = lastPlayerPos.z;


        //checkpointTriggers
        GameObject[] checkpoints =  GameObject.FindGameObjectsWithTag("CheckpointTrigger");
        print("amount in scene: " + checkpoints.Length);
        //SaveLevelInformation.SaveCheckpointTriggers[] checkTriggers = new SaveLevelInformation.SaveCheckpointTriggers[checkpoints.Length];

        for (int i=0; i<checkpoints.Length; i++)
        {
            Transform triggerTransform = checkpoints[i].transform;

            //position
            lvlInfo.checkpointTriggers[i].transform.position.x = triggerTransform.position.x;

            //checkTriggers[i].transform.position.x = triggerTransform.position.x;
            lvlInfo.checkpointTriggers[i].transform.position.y = triggerTransform.position.y;
            lvlInfo.checkpointTriggers[i].transform.position.z = triggerTransform.position.z;

            //rotation
            lvlInfo.checkpointTriggers[i].transform.rotation.x = triggerTransform.rotation.x;
            lvlInfo.checkpointTriggers[i].transform.rotation.y = triggerTransform.rotation.y;
            lvlInfo.checkpointTriggers[i].transform.rotation.z = triggerTransform.rotation.z;

            //scale
            lvlInfo.checkpointTriggers[i].transform.scale.x = triggerTransform.localScale.x;
            lvlInfo.checkpointTriggers[i].transform.scale.y = triggerTransform.localScale.y;
            lvlInfo.checkpointTriggers[i].transform.scale.z = triggerTransform.localScale.z;

            //Trigger's Collider
            BoxCollider checkpointBox = checkpoints[i].GetComponent<BoxCollider>();
            lvlInfo.checkpointTriggers[i].isTrigger = checkpointBox.isTrigger; //isTrigger

            //size
            lvlInfo.checkpointTriggers[i].size.x = checkpointBox.size.x;
            lvlInfo.checkpointTriggers[i].size.y = checkpointBox.size.y;
            lvlInfo.checkpointTriggers[i].size.z = checkpointBox.size.z;
        }
        //----

        bf.Serialize(file, lvlInfo);
		file.Close();

		print ("saved last checkpoint");
	}

	//called when player dies, continues from the main menu, or by reverting to last checkpoint
	public void LoadLastCheckpoint()
	{
		if (isFromLevelSelect == false) {
			int lvlIndex = 1;
			Vector3 lastPlayerPos;
			if (File.Exists (Application.persistentDataPath + "/lastCheckpointInformation.dat") &&
			    new FileInfo (Application.persistentDataPath + "/lastCheckpointInformation.dat").Length > 0) {
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (Application.persistentDataPath + "/lastCheckpointInformation.dat", FileMode.Open);

				//-----
				SaveLevelInformation lvlInfo = (SaveLevelInformation)bf.Deserialize(file);
				lvlIndex = lvlInfo.sceneIndex;

				//send spawn point to be this
				lastPlayerPos = new Vector3(lvlInfo.playerPos.x, lvlInfo.playerPos.y, lvlInfo.playerPos.z);

				StartCoroutine (lastCheckpointWait1(lastPlayerPos));
                //-----

                //load CheckpointTriggers
                print("amount of checkpoints saved: " + lvlInfo.checkpointTriggers.Length);

				file.Close ();

				SceneManager.LoadScene (lvlIndex);

				print ("loaded last checkpoint");
			} else
				LoadLevel ();
		} else
			print("fromLevelSelect");
	}
}
	
#endregion

//saves relevent informantion about a level
[Serializable] class SaveLevelInformation
{
	public int sceneIndex;

    [Serializable] public struct SaveEnemySpawnTriggerZones
    {
        //public float x, y, z;
        [Serializable] public struct TransformPosition
        { public float x, y, z; }
        public TransformPosition tranformPos;

        public bool isTrigger;

        [Serializable] public struct Size
        { public float x, y, z; }
        public Size size;

        // many spawn points
        [Serializable] public struct SpawnPointsTransform
        { public float x, y, z; }
        public SpawnPointsTransform spawnTransforms;
    }
    public SaveEnemySpawnTriggerZones enemyTriggerZone;

    [Serializable] public struct SaveCheckpointTriggers
    {
       [Serializable] public struct Transfrom
        {
            [Serializable] public struct Position
            { public float x, y, z; }
            public Position position;

            [Serializable] public struct Rotation
            { public float x, y, z; }
            public Rotation rotation;

            [Serializable] public struct Scale
            { public float x, y, z; }
            public Scale scale;
        }
        public Transfrom transform;

        public bool isTrigger;

        [Serializable] public struct Size
        { public float x, y, z; }
        public Size size;
    }
    public SaveCheckpointTriggers[] checkpointTriggers = new SaveCheckpointTriggers[GameObject.FindGameObjectsWithTag("CheckpointTrigger").Length];

    [Serializable] public struct SaveCutSceneTriggers
    {

    }

    [Serializable] public struct SavePlayerPosition
	{
		public float x, y, z;
	}
	public SavePlayerPosition playerPos;
}