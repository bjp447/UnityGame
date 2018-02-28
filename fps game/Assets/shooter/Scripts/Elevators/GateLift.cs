using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateLift : MonoBehaviour 
{
	public float elevateRate = 1f;
	public float dropRate = 10f;
	public float elevateHieght = 10f; //in y-axis coords
	public float upTime = 1f;
	public float stopDelay = 0.5f;

	public Transform platformTR;
	public Transform controlPanelTR;

	public bool reachedTop = false;
	public bool isCalled = false;
	public bool goingUp = false;

	private float originalYPos;

	// Use this for initialization
	void Start () 
	{
		originalYPos = platformTR.position.y;

		//StartCoroutine (elevate());
	}

	// Update is called once per frame
	void Update () 
	{
		//isCalled == true
		if (reachedTop == false && goingUp == true)
		{
			elevateLift();
		}


		if (isCalled == true && reachedTop == true) {
			lower();
		}

		if (reachedTop == true)
		{
			StopCoroutine(elevate());
			StopCoroutine(stopTime());
		}
		//print ("reachedTop: " + reachedTop);
	}

	IEnumerator elevate()
	{
		//isCalled = true;
		goingUp = true;
		print ("elevating...");
		yield return new WaitForSeconds(upTime);

		goingUp = false;
		//isCalled = false;
		StartCoroutine (stopTime());
	}

	IEnumerator stopTime()
	{
		print ("stoping...");
		yield return new WaitForSeconds(stopDelay);
		StartCoroutine (elevate());
	}

	private void elevateLift()
	{
		//isCalled = true;

		if (platformTR.position.y < originalYPos + elevateHieght && reachedTop == false) {
			platformTR.Translate (Vector3.up * elevateRate * Time.deltaTime);
		} else {
			isCalled = false;
			reachedTop = true;
		}
	}

	private void lower()
	{
		isCalled = true;

		if (platformTR.position.y > originalYPos && reachedTop == true) 
		{
			platformTR.Translate (Vector3.down * dropRate * Time.deltaTime);
		} 
		else 
		{
			isCalled = false;
			reachedTop = false;
		}
	}
}
