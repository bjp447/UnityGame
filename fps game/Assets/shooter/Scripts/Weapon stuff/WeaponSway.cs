using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
	//private float mouseX;
	//private float mouseY;
	private Quaternion rotationSpeed;

	public float speed;

	private Quaternion moveDirection;
	private Vector3 moveDir;

	// Update is called once per frame
	void Update () 
	{
		if (transform.parent != null) 
		{
			/*
			//sway on player move
			moveDirection = Quaternion.Euler (Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"), 0f);
			transform.localRotation = Quaternion.Slerp (transform.localRotation, 
				moveDirection, speed * Time.deltaTime);
			*/


			/*
			//sway on player step, bob weapon up and down //Input.GetAxis("Horizontal")
			moveDir = new Vector3(Mathf.Abs(Input.GetAxis("Horizontal")), 0f, 0f);
			transform.localPosition = Vector3.Lerp(transform.localPosition, 
				moveDir, speed * Time.deltaTime);
			*/
			/*
			transform.localPosition = Vector3.Lerp(moveDir, 
				transform.localPosition, speed * Time.deltaTime);
				*/



			//sway on mouse move 
			//mouseX = Input.GetAxis ("Mouse X");
			//mouseY = Input.GetAxis ("Mouse Y");
			//rotationSpeed = Quaternion.Euler (-mouseY, mouseX, 0);
			rotationSpeed = Quaternion.Euler (-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
			transform.localRotation = Quaternion.Slerp (transform.localRotation, 
				rotationSpeed, speed * Time.deltaTime);
		}
	}
	/*
	private IEnumerator weaponBob()
	{
		moveDir = new Vector3(Mathf.Abs(Input.GetAxis("Horizontal")), 0f, 0f);
		transform.localPosition = Vector3.Lerp(transform.localPosition, 
			moveDir, speed * Time.deltaTime);

	}
	*/
}
