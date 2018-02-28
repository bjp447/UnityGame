using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
//using UnityStandardAssets.CrossPlatformInput;
//using UnityStandardAssets.Cameras; //need this to access FreeLookCam from unitys standard assets library

public class PlayerControllerOld : MonoBehaviour 
{
	public float speed;
	public float jumpForce;

	public Camera playerCamera;

	public Text countText;
	public Text winText;
	public Text timeText;

	private Rigidbody rb; //of object attached to
	private Transform tr; //of object attached to

	private GameObject settingsMenu;

	private int count;
	private Vector3 spawn;

	// Awake is used to initialize any variables or game state before the game starts.
	// Awake is called only once during the lifetime of the script instance.
	// You should use Awake to set up references between scripts, such as finding a gameobject
	// and use Start to pass any information back and forth. 
	// Awake is always called before any Start functions. This allows you to order initialization of scripts.
	void Start()
	{
		Renderer rend = GetComponent<Renderer> ();
		rend.material.color = Color.red;
		//Color.red;

		rb = GetComponent<Rigidbody>(); //the Rigidbody of the object the script is attached to
		tr = GetComponent<Transform>(); //the Transform of the object the script is attached to

		settingsMenu = GameObject.Find ("PauseMenu");
		if (settingsMenu == null)
			Debug.LogError ("error: gameObject");
		settingsMenu.SetActive (false);

		count = 0;
		SetCountText ();
		winText.text = "";

		spawn = new Vector3(0.0f, 0.5f, 0.0f);
		tr.transform.position = spawn;

		Time.timeScale = 1;
	}

	void FixedUpdate()
	{
		//if ( (rb.transform.position.y < 0.6f) && (rb.transform.position.y > 0.0f) )
		if (rb.velocity.y == 0)
		{
			if (Input.GetKey(KeyCode.Space))
			{
				rb.AddForce(Vector3.up * jumpForce);// .up is <0,1,0>
			}

			float moveHorizontal = Input.GetAxis("Horizontal"); //movement in x-axis
			float moveVertical = Input.GetAxis("Vertical"); //movement in z-axis

			//creates vector of change in x and z direction
			Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical); 

			//control relative to the camera direction
			//(Camera.main) the camera tagged with "MainCamera"
			//movement = Camera.main.transform.TransformDirection(movement);
			movement = playerCamera.transform.TransformDirection(movement);

			rb.AddForce(movement * speed); //applies movement force to the rigidbody of a gameObject
		}
		if (rb.transform.position.y < 0.0f) 
		{
			Respawn();
		}
	}
	void Update()
	{
		timeText.text = "Time: " + Math.Round(Time.timeSinceLevelLoad, 2) + " seconds";

		if (Input.GetKeyDown ("m")) 
			pauseMenuEnable();
		
		if (Input.GetKeyDown("escape"))
		{
			if (settingsMenu.activeInHierarchy == true)
				pauseMenuEnable ();

		}
	}

	void pauseMenuEnable()
	{
		//temp = !temp; //would need: bool temp=false in global
		//settingsMenu.SetActive (temp);
		if (settingsMenu.activeInHierarchy == true)
		{
			//print ("settingsMenu Inactive");
			settingsMenu.SetActive (false);
			Time.timeScale = 1; //resumes game
			return;
		}
		settingsMenu.SetActive (true);
		Time.timeScale = 0; //pauses game

		//print ("settingsMenu Active");
		//GUI.W
	}

	void OnGUI()
	{
		if (count >= 1)
		{
			Time.timeScale = 0;
			if (GUILayout.Button ("Main Menu")) 
			{
				SceneManager.LoadScene (0);
			}
			if (GUILayout.Button ("Replay")) 
			{
				SceneManager.LoadScene (3);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Pick Up")) 
		{
			other.gameObject.SetActive (false);
			count++;
			SetCountText ();
		}
	}

	void SetCountText()
	{
		countText.text = "Count: " + count.ToString ();

		if (count >= 1) 
		{
			winText.text = "You Win!";

			Scoring.control.lvl1Score = count;
			Scoring.control.lvl1Time = Math.Round (Time.timeSinceLevelLoad, 2);

			SceneManager.LoadScene (0);
		}
	}

	void Respawn()
	{
		transform.position = spawn;

		//clears force
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}
}