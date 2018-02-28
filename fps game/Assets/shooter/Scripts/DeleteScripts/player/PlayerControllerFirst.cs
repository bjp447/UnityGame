using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerControllerFirst : MonoBehaviour 
{
	//camera
	public Transform cameraTR;
	//private Vector3 cameraPos;

	//player
	public CharacterController controller;
	private Vector3 playerPos;
	public GameObject ragdoll;

	//player speeds
	//public float speed; //the current speed of the player
	public float currentPlayerSpeed; // = movementSpeed();
	public float crouchSpeed = 2f;
	public float walkSpeed = 8f;
	public float runSpeed = 12f;
	public float speedInibitorADS = 2f; //inhibits walk speed (walkSpeed/ADSspeed)

	//respawn
	private Vector3 respawnPoint;
	//private Vector3 spawn = Vector3.zero; //sets location of gun on player

	//settings Menu
	private GameObject settingsMenu;

	//UI
	public GameObject interactText;

	//gravity based stuff
	public float jumpHeight = 10f;
	public float gravity = 20f;
	private Vector3 moveDirection = Vector3.zero;

	//booleans
	private bool isWalking = true; //True: Player is walking
	private bool toggleSprintHold = true; //True: holdToSprint
	private bool isStanding = true; //True: Player is standing
	private bool toggleCrouchHold = true; //True: holdToCrouch
	private bool isPlayerCrouched = false;
	//private bool isClimbing = false; // not currently used
	//private bool isPlayerAlive = true;

	//from PlayerOnColliderHit script
	//private GameObject currentWeapon;
	private bool aimDownSight = false;
	//---------------------------------------------

	// Use this for initialization
	void Start() 
	{
		if (interactText.activeInHierarchy == true)
			interactText.SetActive(false);
		
		controller = GetComponent<CharacterController>();

		//respawn
		respawnPoint = GameObject.FindGameObjectWithTag ("Respawn").transform.position;
		respawnPoint.y += transform.position.y;

		settingsMenu = GameObject.Find("PauseMenu");
		if (settingsMenu.activeInHierarchy == true)
			settingsMenu.SetActive(false);
	}

	//a struct of information for the current elevtor hit by a Ray
	private struct getElevatorInfo
	{
		public Transform elevatorTR;
		private Elevator theElevator;
		public bool reachedTop;
		public bool isCalled;

		public getElevatorInfo(RaycastHit hitObject) 
		{
			this.elevatorTR = hitObject.transform.parent;
			this.theElevator = elevatorTR.GetComponent<Elevator>();
			this.reachedTop = theElevator.reachedTop;
			this.isCalled = theElevator.isCalled;
		}
	}

	private void callElevator(getElevatorInfo elevator)
	{
		bool isCalled = elevator.isCalled;
		bool reachedTop = elevator.reachedTop;

		if (isCalled == false && reachedTop == false)
			elevator.elevatorTR.SendMessage("elevate");

		if (isCalled == false && reachedTop == true) 
			elevator.elevatorTR.SendMessage("lower");
	}

	//using Character Controller
	void Update()
	{
		//Player_controller playerController = GetComponent<Player_controller>();
		//aimDownSight = playerController.aimDownSight;
		//currentWeapon = playerHitScript.currentWeapon;
		//print ("aimDownSight: " + aimDownSight);

		//ignores layer 8 in the layerMask
		int layerMask = 1 << 8;
		layerMask = ~layerMask;

		//ray and stuff for climbing
		Vector3 fwd = transform.TransformDirection(Vector3.forward) * 2; //transform of player
		Ray ray = new Ray(cameraTR.position, fwd); //at camera position
		Debug.DrawRay (cameraTR.position, fwd, Color.green);
		RaycastHit hitObject;

		//ray for elevator or 
		if (Physics.Raycast (ray, out hitObject, 2, layerMask)) {
			print("Object tag: " + hitObject.transform.tag);
			//activates 'interact key' text
			if (hitObject.transform.CompareTag ("LiftControlPanel")) {
				getElevatorInfo elevator = new getElevatorInfo(hitObject);

				if (interactText.activeInHierarchy == false)
					interactText.SetActive (true);

				if (Input.GetKeyDown("e"))
					callElevator (elevator);
			}
		} else {
			if (interactText.activeInHierarchy == true)
				interactText.SetActive (false);
		}

		//player control based function calls
		toggleHoldToSprint();
		toggleHoldToCrouch();
		currentPlayerSpeed = movementSpeed(); //sets movement speed
		//print (controller.velocity);
		playerCrouch(); //crouch/uncrouch
		displayMenu(); //displays menu

		//moves player
		if (controller.isGrounded) {
			moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 
				0f, Input.GetAxis ("Vertical"));

			moveDirection.Normalize();
			moveDirection = transform.TransformDirection(moveDirection) * currentPlayerSpeed;

			if (Input.GetButtonDown ("Jump"))
				moveDirection.y += jumpHeight;

			//
			//if (Physics.Raycast (ray, out hitObject, 2)) 
			//	climb (getHitObjectInfo (hitObject, ray)); //gets object info and pass it to climb for appropriate hieght
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
			
		//respawn
		if (transform.position.y < 0) {
			transform.position = respawnPoint;
		}
	}
	//==============================================
		
	private Vector3 getHitObjectInfo(RaycastHit hitObject, Ray ray)
	{
		Transform hitObjectTr = hitObject.transform;
		Vector3[] objectPoints;
		Vector3 frontTopEdgePoint;
		//Vector3 frontLeftPoint;
		//Vector3 frontRightPoint;

		int numOfchildren = hitObject.transform.childCount;
		objectPoints = new Vector3[numOfchildren];

		for (int c=0; c<numOfchildren; c++)
			objectPoints [c] = hitObject.transform.GetChild (c).transform.position;
			
		print ("frontTopEdgePoint: " + objectPoints[0]);
		frontTopEdgePoint = objectPoints[0];

		//--------------
		//Vector3 edgeDynamic = new Vector3 (transform.position.x, frontTopEdgePoint.y, frontTopEdgePoint.z);
		Vector3 edgeDynamic = new Vector3 (frontTopEdgePoint.x, frontTopEdgePoint.y, frontTopEdgePoint.z);
		//Vector3 dir = frontRightPoint - frontLeftPoint;
		Vector3 dir = hitObjectTr.right;
		Debug.DrawRay (edgeDynamic, dir, Color.green);
		//----------
		//objectPoints = null;
		return frontTopEdgePoint;

		/*
		Vector3 frontTopEdgePoint;
		Vector3 frontLeftPoint;
		Vector3 frontRightPoint;

		Transform hitObjectTr = hitObject.transform;
		Vector3 offset = (hitObjectTr.localScale / 2);
		//print ("offset: " + offset);
		//GameObject gb = Instantiate (ragdoll, offset, Quaternion.identity) as GameObject;

		if (hitObjectTr.parent != null) {
			offset.y *= hitObjectTr.parent.localScale.y;
			offset.z *= hitObjectTr.parent.localScale.z;
			offset.x *= hitObjectTr.parent.localScale.x;
		}

		//
		print("ray.direction: " + ray.direction);
		if (ray.direction.z < 0) 
			offset.z = -offset.z;
		else if (ray.direction.y < 0)
			offset.y = -offset.y;
		//else if (ray.direction.x < 0)
		//	offset.x = -offset.x;
		//

		//Vector3 topPoint = hitObjectTr.position + new Vector3(0, offset.y, 0); //middle top
		//GameObject gb = Instantiate (ragdoll, topPoint, Quaternion.identity) as GameObject;
		//print ("topPoint: " + topPoint);
		frontTopEdgePoint = hitObjectTr.localPosition + new Vector3 (0, offset.y, -offset.z); //middle top front of object (its original x)
		//frontTopEdgePoint = new Vector3 (hitObjectTr.position.x, hitObjectTr.position.y + offset.y, hitObjectTr.position.z -offset.z);
		frontLeftPoint = hitObjectTr.localPosition + new Vector3(offset.x, offset.y, -offset.z);
		frontRightPoint = hitObjectTr.localPosition + new Vector3(-offset.x, offset.y, -offset.z);

		GameObject gb1 = Instantiate (ragdoll, frontTopEdgePoint, Quaternion.identity) as GameObject;
		gb1.name = "middle";
		GameObject gb2 = Instantiate (ragdoll, frontLeftPoint, Quaternion.identity) as GameObject;
		gb2.name = "left";
		GameObject gb3 = Instantiate (ragdoll, frontRightPoint, Quaternion.identity) as GameObject;
		gb3.name = "right";

		//--------------
		Vector3 edgeDynamic = new Vector3 (transform.position.x, 
			hitObjectTr.position.y + offset.y, hitObjectTr.position.z -offset.z);
		Vector3 dir = frontRightPoint - frontLeftPoint;
		Debug.DrawRay (edgeDynamic, dir, Color.green);
		//----------

		return frontTopEdgePoint;
		//print ("frontTopEdgePoint: " + frontTopEdgePoint);
		*/
	}

	public float grabHeight = 1f;
	//player climbs
	private void climb(Vector3 frontTopEdgePoint)
	{
		if (Input.GetButtonDown("Jump") ) {//&& frontTopEdgePoint.y <= grabHeight + cameraTR.position.y)
			//move up
			//controller.Move(Vector3.up * (frontTopEdgePoint.y - cameraTR.position.y + 0.5f));
			//controller.Move(Vector3.up * ());
			//float leftHandY = transform.GetChild (0).transform.position.y;
			//leftHandY = frontTopEdgePoint.y;
			Vector3 hipPoint = frontTopEdgePoint - new Vector3(0, 0.4f, 1);
			//GameObject gb = Instantiate (ragdoll, hipPoint ,Quaternion.identity);
			transform.GetChild(1).transform.position = 
				new Vector3(transform.position.x, frontTopEdgePoint.y, transform.position.z);
			transform.position = new Vector3(transform.position.x, hipPoint.y, transform.position.z);
			//move forward 
			//controller.Move(Vector3.forward * 1.2f);
		} 
	}

	//display menu
	private void displayMenu()
	{
		if ((Input.GetKeyDown("m")) || (Input.GetKeyDown(KeyCode.Escape))) {
			if (settingsMenu.activeInHierarchy == true) {
				Time.timeScale = 1;
				settingsMenu.SetActive (false);
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			} else {
				Time.timeScale = 0;
				settingsMenu.SetActive (true);
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
			}
		}
	}

	// True: holdToSprint
	// False: Sprint is always on/off
	private void toggleHoldToSprint()
	{
		if (Input.GetKeyDown("1")) {
			toggleSprintHold = !toggleSprintHold;
			//print ("toggleHoldToSprint: " + toggleSprintHold);
		}
	}

	// True: hold to crouch
	// False: Player is always crouching/standing 
	private void toggleHoldToCrouch()
	{
		if (Input.GetKeyDown("2")) {
			toggleCrouchHold = !toggleCrouchHold;
			//print ("toggleHoldToCrouch: " + toggleCrouchHold);
		}
	}

	//returns crouch, run, or walk speed
	private float movementSpeed()
	{
		if (toggleSprintHold == true) 
			isWalking = !Input.GetKey(KeyCode.LeftShift); //true when LeftShit is not pressed
		else { //if (toggleHoldToSprint() == false)
			if (Input.GetKeyDown(KeyCode.LeftShift)) {
				isWalking = !isWalking;
				print("isWalking: " + isWalking);
			}
		}

		if (toggleCrouchHold == true) 
			isStanding = !Input.GetKey ("c"); //true if not pressed
		else { // if false
			if (Input.GetKeyDown("c")) {
				isStanding = !isStanding;
				//print("isStanding: " + isStanding);
			}
		}

		if ((isWalking == true) && (isStanding == true)) {
			if (aimDownSight == true)
				return walkSpeed / speedInibitorADS;
			else
				return walkSpeed;
		} else if ((isWalking == false) && (isStanding == true))
			return runSpeed;
		else //if (((isWalking == true) && (isStanding == false)) 
		{ //|| ((isWalking == false) && (isStanding == false)))
			return crouchSpeed;
		}
	}

	//crouch or un-crouch the player
	private void playerCrouch()
	{
		if ((isPlayerCrouched == false) && (isStanding == false)) {
			controller.height -= 1;
			controller.GetComponent<CapsuleCollider>().height -= 1;
			//controller.center -= new Vector3(0, 1, 0);
			isPlayerCrouched = true;
		}

		if ((isPlayerCrouched == true) && (isStanding == true)) {
			controller.height += 1;
			controller.GetComponent<CapsuleCollider>().height += 1;
			//controller.center += new Vector3(0, 1, 0);
			isPlayerCrouched = false;
		}
	}
}