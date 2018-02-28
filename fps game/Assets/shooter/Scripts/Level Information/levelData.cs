using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class levelData : MonoBehaviour 
{
	[SerializeField] public Sprite lvlSprite;
	[SerializeField] public string lvlDescription;

	public Transform initialSpawn;

	[Serializable] public class StartingInventory
	{
		[SerializeField] public int fragGrenadeAmount;
	}
	[SerializeField] public StartingInventory startingInventory;
}
