using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour 
{
	public int health = 100;
	
	// Update is called once per frame
	void Update () 
	{
		if (health == 0)
		{
			Destroy(this.gameObject);
		}
	}
}
