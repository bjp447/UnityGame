using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour 
{
	[SerializeField] private GameObject enemy;

	private void OnDestroy()
	{
		
		for (int i=0; i<transform.childCount; i++)
		{
			GameObject copy = Instantiate(enemy, transform.GetChild(i).transform.position, Quaternion.identity) as GameObject;//Destroy(transform.GetChild(i).gameObject);
		}
	}

	/*
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	*/
}
