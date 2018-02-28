using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoContainer : MonoBehaviour 
{
	//ammo
	public int magCount = 30;
	public int ammoCount = 250;

	// Use this for initialization
	void Start () 
	{
		ammoCount += magCount;
	}

	void LateUpdate()
	{
		if (ammoCount == 0)
		{
			Destroy(this.gameObject);
		}
	}

}
