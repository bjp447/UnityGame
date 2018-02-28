using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class Inventory_controller : MonoBehaviour 
{
	public GameObject[] weaponsInventory = new GameObject[2];
	//public GameObject gunPrefab;

	[Header("\t Frag Grenade Info")]
	public GameObject fragGernadePrefab;
	public int maxFragsAmount = 2;
	public int fragGrenadesAmount;

	private levelData currentLvlData;
	private Global_controller globalController;

    //called by OnPlayerDeath() in Player_Inventory
    public void DropInventory()
	{
		//drop weapons
		for (int i=0; i<weaponsInventory.Length; i++) {
            if (weaponsInventory[i] != null) {
                //weaponsInventory[i].SetActive(true); //guns are no longer set inactive
                weaponsInventory[i].GetComponent<Rigidbody>().isKinematic = false;
                weaponsInventory[i].GetComponent<BallProjectile>().enabled = false;
                weaponsInventory[i].transform.parent = null;
            }
		}

		//drop frag gernades
		for (int i=0; i<fragGrenadesAmount; i++) {
			print ("dropped grenade");
			Instantiate(fragGernadePrefab, transform.position, Quaternion.identity);
		}
	}

	public void LoadDefaultLevelInventory()
	{
		currentLvlData = GameObject.Find("LevelData").GetComponent<levelData>();
		//weapons_Inventory[0] = gunPrefab;
		fragGrenadesAmount = currentLvlData.startingInventory.fragGrenadeAmount;
	}

	#region Save/Load (playerInventory.dat)
	//saves inventory at the end of a level to use for the beggining of the next level
	public void SaveInventory()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		if (File.Exists (Application.persistentDataPath + "/playerInventory.dat")) 
			file = File.Open (Application.persistentDataPath + "/playerInventory.dat", FileMode.Open);
		else
			file = File.Create(Application.persistentDataPath + "/playerInventory.dat");

		InventoryToSave saveInventory = new InventoryToSave();
		//saveInventory.weapons_Invent = weapons_Inventory;
		saveInventory.fragGrenadesAmount = fragGrenadesAmount;

		print("inventory saved: " + saveInventory.fragGrenadesAmount + " in " + SceneManager.GetActiveScene().name);

		bf.Serialize(file, saveInventory);
		file.Close();
	}

	//loads inventory at the beggining of a level
	public void LoadInventory()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInventory.dat") &&
		    new FileInfo (Application.persistentDataPath + "/playerInventory.dat").Length > 0) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerInventory.dat", FileMode.Open);

			InventoryToSave saveInventory = (InventoryToSave)bf.Deserialize(file);
			//weapons_Inventory = saveInventory.weapons_Invent;
			fragGrenadesAmount = saveInventory.fragGrenadesAmount;

			print("inventory loaded: " + fragGrenadesAmount +  " in " + SceneManager.GetActiveScene().name);
			file.Close ();
		}
	}

	public void SaveCheckpointInventory()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if (File.Exists (Application.persistentDataPath + "/playerCheckpointInventory.dat")) 
			file = File.Open (Application.persistentDataPath + "/playerCheckpointInventory.dat", FileMode.Open);
		else
			file = File.Create(Application.persistentDataPath + "/playerCheckpointInventory.dat");

		InventoryToSave saveInventory = new InventoryToSave();
		//saveInventory.weapons_Invent = weapons_Inventory;
		saveInventory.fragGrenadesAmount = fragGrenadesAmount;

		bf.Serialize(file, saveInventory);
		file.Close();
	}

	public void LoadCheckpointInventory()
	{
		if (File.Exists (Application.persistentDataPath + "/playerCheckpointInventory.dat") &&
		    new FileInfo (Application.persistentDataPath + "/playerCheckpointInventory.dat").Length > 0) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/playerCheckpointInventory.dat", FileMode.Open);

			InventoryToSave saveInventory = (InventoryToSave)bf.Deserialize (file);
			//weapons_Inventory = saveInventory.weapons_Invent;
			fragGrenadesAmount = saveInventory.fragGrenadesAmount;

			//set inventory


			file.Close ();
		} else
			LoadInventory();
	}
	#endregion
}

[Serializable]
class InventoryToSave
{
	//public GameObject[] weaponsInventory;
	public int fragGrenadesAmount;
}