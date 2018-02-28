using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPersonCamera : MonoBehaviour 
{
	public Transform playerTR;
	public Transform cameraTR;

	public bool clampVerticalRotation = true;
	public float MinimumX = -90F;
	public float MaximumX = 90F;
	public bool lockCursor = true;

	private bool m_cursorIsLocked = true;

	private Quaternion characterTargetRotation;
	private Quaternion cameraTargetRotation;

    //
    [SerializeField] private GameObject pauseMenu;
    private Player_controller playerController;
	private Settings_controller playerSettings;

	void Start () 
	{
		characterTargetRotation = playerTR.localRotation;
		cameraTargetRotation = cameraTR.localRotation;

		//pauseMenu = GameObject.Find("PauseMenu");
		playerSettings = GameObject.Find("PlayerSettings").GetComponent<Settings_controller>();
        playerController = GetComponent<Player_controller>();
    }

	void Update()
	{
		/*
		WeaponProperties weaponProp = GetComponentInChildren<WeaponProperties>();
		Vector3 recoilRotationEffect = Vector3.zero;
		if (weaponProp != null)
		{
			recoilRotationEffect = weaponProp.recoilRotationEffect;
		}
		*/
		
		//update look when pause menu is not displayed
		if (pauseMenu.activeInHierarchy == false)
        {
            float xRotation;
            float yRotation;
 
            if (playerController.aimDownSight == false)
            {
                if (playerSettings.invertY == true)
                    xRotation = -Input.GetAxis("Mouse Y") * playerSettings.xSens; //ySensitivity;
                else
                    xRotation = Input.GetAxis("Mouse Y") * playerSettings.ySens;

                yRotation = Input.GetAxis("Mouse X") * playerSettings.xSens; //xSensitivity;
            }
            else
            {
                if (playerSettings.invertY == true)
                    xRotation = -Input.GetAxis("Mouse Y") * playerSettings.aimXSens; //ySensitivity while aiming;
                else
                    xRotation = Input.GetAxis("Mouse Y") * playerSettings.aimXSens;

                yRotation = Input.GetAxis("Mouse X") * playerSettings.aimYSens; //xSensitivity while aiming;
            }

            characterTargetRotation *= Quaternion.Euler (0f, yRotation, 0f);
			cameraTargetRotation *= Quaternion.Euler (-xRotation, 0f, 0f);

			//lookRecoil(recoilRotationEffect);

			if (clampVerticalRotation) {//makes it so it doesnt go passed 90 and -90 degrees
				cameraTargetRotation.x /= cameraTargetRotation.w;
				cameraTargetRotation.y /= cameraTargetRotation.w;
				cameraTargetRotation.z /= cameraTargetRotation.w;
				cameraTargetRotation.w = 1.0f;

				float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (cameraTargetRotation.x);
				angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
				cameraTargetRotation.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
			}
			playerTR.localRotation = characterTargetRotation; //horizontal
			cameraTR.localRotation = cameraTargetRotation; //vertical

			UpdateCursorLock ();
		} else { // if (pauseMenu.activeInHierarchy == true)
			// when game is paused, player look direction will be the same as just before it was paused
			playerTR.localRotation = characterTargetRotation;
			cameraTR.localRotation = cameraTargetRotation;
			//print ("Pause");
		}
	}

    //called via SendMessage in Player_controller
	private void LookRecoil(Vector3 recoilRotationEffect)
	{
		//if (Input.GetKeyDown("0"))
		//{
			//print ("called");
			characterTargetRotation *= Quaternion.Euler (0f, recoilRotationEffect.y, 0f);
			cameraTargetRotation *= Quaternion.Euler (-recoilRotationEffect.x, 0f, 0f);
		//}
	}

	public void SetCursorLock(bool value)
	{
		lockCursor = value;
		if(!lockCursor)
		{//we force unlock the cursor if the user disable the cursor locking helper
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void UpdateCursorLock()
	{
		//if the user set "lockCursor" we check & properly lock the cursor
		if (lockCursor)
			InternalLockUpdate();
	}

	private void InternalLockUpdate()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			m_cursorIsLocked = false;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			m_cursorIsLocked = true;
		}

		if (m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if (!m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

}
