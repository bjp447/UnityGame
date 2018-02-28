using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour 
{
	public float elevateRate = 1f;
	public float elevateHieght = 10f; //in y-axis coords

	public Transform platformTR;
	public Transform controlPanelTR;

	public bool reachedTop = false;
	public bool isCalled = false;
	//public bool havePauses = false; //for multiple levels 

	private float originalYPos;

	// Use this for initialization
	void Start () 
	{
		originalYPos = platformTR.position.y;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isCalled == true && reachedTop == false) {
            Elevate();
		}
		if (isCalled == true && reachedTop == true) {
            Lower();
		}
	}

	private void OnTriggerEnter(Collider hitObject)
	{
		print ("Triigered with " + hitObject);
		if (hitObject.CompareTag ("Player") || hitObject.CompareTag ("enemy"))
			hitObject.transform.parent = platformTR;
	}

	private void OnTriggerExit(Collider hitObject)
	{
		if (hitObject.CompareTag ("Player") || hitObject.CompareTag ("enemy"))
			hitObject.transform.parent = null;
	}


	private void Elevate()
	{
		isCalled = true;

		if (platformTR.position.y < originalYPos + elevateHieght && reachedTop == false) {
			platformTR.Translate (Vector3.up * elevateRate * Time.deltaTime);
		} else {
			isCalled = false;
			reachedTop = true;
		}
	}

	private void Lower()
	{
		isCalled = true;

		if (platformTR.position.y > originalYPos && reachedTop == true) {
			platformTR.Translate (Vector3.down * elevateRate * Time.deltaTime);
		} else {
			isCalled = false;
			reachedTop = false;
		}
	}
}
