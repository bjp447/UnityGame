using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
//using UnityStandardAssets.CrossPlatformInput;

public class Player_controller : MonoBehaviour 
{   
    #region Global Variables ----------------------------
    //player
    [NonSerialized] public CharacterController controller;
	[SerializeField] private GameObject ragdoll; //player ragdoll, when player dies
    [SerializeField] private float playerHealth = 100f;
    private Animator anim;

    //gravity based stuff for movement
    private Vector3 moveDirection = Vector3.zero;
    [Header("\tPlayer Movement")]
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float gravity = 20f;

    //player speeds
    [Serializable] public class PlayerSpeeds {
        public float currentPlayerSpeed; // = MovementSpeed();
        public float crouchSpeed = 2f;
        public float walkSpeed = 8f;
        public float runSpeed = 12f;
        public float speedInibitorADS = 2f; //inhibits walk speed (walkSpeed/ADSspeed)
    }
    [SerializeField] public PlayerSpeeds playerSpeeds;

    //camera
    [SerializeField] private Transform playerCameraTR;
	private float startFOV = 0;

	//locations on player for weapons
	[Header("  Weapon Placement Locations on Player")]
	[SerializeField] private Transform rightShoulderTR;
	[SerializeField] private Transform middleBackTR;
	[SerializeField] private Transform rightThighTR;
	private Vector3 shoulderPos; //sets location of gun on player
	private Vector3 adsPos;

    //booleans: player
    private bool isMoving = false;
    private bool isWalking = true; //True: Player is walking
    private bool isStanding = true; //True: Player is standing
    private bool isPlayerCrouched = false;

    //boleans: weapon related
    [NonSerialized] public bool aimDownSight = false;
    private bool atShoulder = false;
    private bool isMagEmpty = false;
    private bool isReloading = false;
    //private bool isChanging = false;
    //private bool isOverGun = false; //is player over a weapon
    private bool isDoneThrowingObj = true;
    private bool xboxLTinUse = false; //is Left Trigger on Xbox controller pressed

    //current weapon equiped
    private GameObject currentWeapon; //the current weapon held by player
	private Transform currentWeaponTR; //gets assigned after player has a gun
    private WeaponProperties weaponProp = null;
    private GunAmmoContainer ammoContainer = null;

    //weapon bobbing
    private bool reachedTop = false;
    private Vector3 startBob;
    private Vector3 topBob;
    private Vector3 bottomBob;

    //script refrences
    private Menu_controller menuControl = null;
	private Settings_controller playerSettings = null;
	private Inventory_controller playerInventory = null;
    private UI_OverlayElements UIoverlay = null;

    //fire delay stuff
    private float tempRate = 0.0f; //allows for fire on start
    private float lastShot = 0.0f; //gets modified

    //pick up timer
    [Header("\t     Misc")]
    [SerializeField] private float keyHoldLength = 0.4f; //length player must hold to activate something
    private float keyTimer = 0; // iterates from last frame

    //misc
    private string lastHitObjectName = null;
    Coroutine reloadingCoroutine = null;
    #endregion

    #region (MonoBehaviour functions) ----------------------------
    void Start () 
	{
        //component and script refrences
		menuControl = GameObject.Find("Menu_Controller").GetComponent<Menu_controller>();
		playerSettings = GameObject.Find("PlayerSettings").GetComponent<Settings_controller>();
		playerInventory = GetComponent<Inventory_controller>();
        UIoverlay = GetComponent<UI_OverlayElements>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        //weapon ADS stuff
		shoulderPos = rightShoulderTR.localPosition;//new Vector3 (0.25f, -1.3f, 0.1f); //sets location of gun
		adsPos = new Vector3 (playerCameraTR.localPosition.x, rightShoulderTR.localPosition.y, 
			playerCameraTR.localPosition.z);

        //playerCameraTR.GetComponent<Camera>().fieldOfView = playerSettings.fOV;
        //startFOV = playerCameraTR.GetComponent<Camera>().fieldOfView;

        //other
		Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), controller);

        //transform.position = GameObject.Find("LevelData").GetComponent<levelData>().initialSpawn.position; //initial spawn location on load

        //Quaternion rd = UnityEngine.Random.rotation;

        //FIX: lerping to scopoe zoom and changing FOV
    }

    // Update is called once per frame
    void Update () 
	{
        if (startFOV != playerSettings.fOV)
        {
            startFOV = playerSettings.fOV;
            KickOutOfADS();
            print("startFOV: " + startFOV);
        }

        if ((Input.GetKeyDown(playerSettings.keyBinds["pauseMenuKey"])) || (Input.GetKeyDown(KeyCode.Escape))) 
			menuControl.displayMenu();

        if (menuControl.isPaused == false)
        {
            if (Input.GetKeyDown(playerSettings.keyBinds["weaponSwapKey"]))
                ChangeWeapons(); //change weapons from inventory

            if (Input.GetKeyDown(playerSettings.keyBinds["sprintToggleKey"]))
                playerSettings.ToggleHoldToSprint();

            if (Input.GetKeyDown(playerSettings.keyBinds["crouchToggleKey"]))
                playerSettings.ToggleHoldToCrouch();

            if (Input.GetKeyDown(playerSettings.keyBinds["adsToggleKey"]))
                playerSettings.ToggleHoldToADS();

            if (Input.GetKeyDown(playerSettings.keyBinds["throwObject"]))
                ThrowGrenade();

            playerSpeeds.currentPlayerSpeed = MovementSpeed(); //sets movement speed

            if (controller.velocity != Vector3.zero)
                isMoving = true;
            else
                isMoving = false;

            PlayerCrouch(); //crouch/uncrouch
            //MovePlayer(); //moves player
            RayCasting();

            if (currentWeaponTR != null)
            {
                AimDownSights();
                Reload(); //reloads weapon
                WeaponBobbing();

                if (Input.GetKeyDown(playerSettings.keyBinds["fireModeKey"]) && isReloading == false)
                    ToggleFire();

                if (isMagEmpty == false && isReloading == false)
                {
                    if (weaponProp.currentFireToggle == WeaponProperties.FireToggle.single)
                    {
                        if (Input.GetButtonDown("Fire1") || Input.GetAxis("XboxRT") == 1)
                            FireSingleShot();
                    }
                    else if (weaponProp.currentFireToggle == WeaponProperties.FireToggle.full)
                    {
                        if (Input.GetButton("Fire1") || Input.GetAxis("XboxRT") == 1)
                            FireFullAutoShot();
                    }
                    else if (weaponProp.currentFireToggle == WeaponProperties.FireToggle.burst)
                    {
                        if (Input.GetButtonDown("Fire1") || Input.GetAxis("XboxRT") == 1)
                            FireBurstShot();
                    }
                }
                else
                {
                    //play audio. empty sound
                }
            }
        }
	}

    private void FixedUpdate()
    {
        if (menuControl.isPaused == false)
        {
            MovePlayer(); //moves player
        }
    }
    #endregion


    #region(RayCasting) ----------------------------
    private void RayCasting()
	{
		//ignores layer 8 in the layerMask
		int layerMask = 1 << 8;
		layerMask = ~layerMask;

		//ray and stuff 
		Vector3 fwd = transform.TransformDirection (Vector3.forward) * 2; //transform of player
		Ray ray = new Ray (playerCameraTR.position, fwd); //at camera position
		Debug.DrawRay (playerCameraTR.position, fwd, Color.green);
		RaycastHit hitObject;
		string currentHitObjectName = null;

		//ray for elevator or 
		if (Physics.Raycast (ray, out hitObject, 2, layerMask))
        {
			currentHitObjectName = hitObject.transform.name;

			if (hitObject.transform.CompareTag("LiftControlPanel"))
            {
				if (string.CompareOrdinal(currentHitObjectName, lastHitObjectName) != 0)//currentHitObjectName != lastHitObjectName)
					print("found new elevator");

				if (Input.GetKeyDown(playerSettings.keyBinds["interactKey"]))
					CallElevator(hitObject.transform.gameObject);

                UIoverlay.SetInteractTextActive(true);

				lastHitObjectName = currentHitObjectName;
			}
		}
        else
        {
            UIoverlay.SetInteractTextActive(false);
		}
	}
    #endregion

    /*
    private void CancelProccess()
    {
        if (isReloading == true)
        {

        }
    }
    */

    #region (Fireing)
    //reloads weapon
    private void Reload()
    {
        if (Input.GetKeyDown(playerSettings.keyBinds["reloadKey"]) && ammoContainer.magCount < ammoContainer.maxMag + 1//currentMagCount<magSize+1
            && ammoContainer.ammoCount != 0 && isReloading == false)
        {
            if (playerSpeeds.currentPlayerSpeed != playerSpeeds.runSpeed ||
                controller.velocity == Vector3.zero)
            {
                reloadingCoroutine = StartCoroutine(ReloadingIE());
            }
        }

        if (isReloading == true && playerSpeeds.currentPlayerSpeed == playerSpeeds.runSpeed) // (|| weapon has changed)
        {
            StopCoroutine(reloadingCoroutine);
            print("reloading canceled");
            isReloading = false;
        }
    }

    IEnumerator ReloadingIE()
    {
        isReloading = true;
        //print ("isReloading: " + isReloading);
        print("reloading...");

        yield return new WaitForSeconds(weaponProp.reloadTime);

        ammoContainer.ammoCount += ammoContainer.magCount;

        if (ammoContainer.ammoCount <= ammoContainer.maxMag)
        {
            ammoContainer.magCount = ammoContainer.ammoCount;
            ammoContainer.ammoCount = 0;
        }
        else
        {
            if (ammoContainer.magCount == 0)
            {
                ammoContainer.magCount = ammoContainer.maxMag;
                ammoContainer.ammoCount -= ammoContainer.maxMag;
            }
            else
            {
                ammoContainer.magCount = ammoContainer.maxMag + 1;
                ammoContainer.ammoCount -= (ammoContainer.maxMag + 1);
            }
        }
        UIoverlay.SetAmmoText(ammoContainer);//SetAmmoText(); //sets ammo text

        isMagEmpty = false;
        isReloading = false;
        //print ("isReloading: " + isReloading);
        print("done");
    }

    private void CreateProjectileClone()
    {
        //Debug.Break();
        //bullets
        float xRotation = UnityEngine.Random.Range(weaponProp.minBulletSpreadRotation.x, weaponProp.maxBulletSpreadRotation.x);
        float yRotation = UnityEngine.Random.Range(weaponProp.minBulletSpreadRotation.y, weaponProp.maxBulletSpreadRotation.y);
        float zRotation = UnityEngine.Random.Range(weaponProp.minBulletSpreadRotation.z, weaponProp.maxBulletSpreadRotation.z);

        GameObject clone = Instantiate(weaponProp.projectilePrefab, weaponProp.barrelEnd.position, weaponProp.barrelEnd.rotation); //'*' combines Quaternions
        clone.transform.eulerAngles = new Vector3(xRotation + clone.transform.eulerAngles.x, yRotation + clone.transform.eulerAngles.y, zRotation + clone.transform.eulerAngles.z);

        clone.GetComponent<Rigidbody>().velocity = clone.transform.forward * weaponProp.bulletVelocity;
        Debug.DrawRay(weaponProp.barrelEnd.position, clone.transform.forward*2, Color.cyan, Mathf.Infinity);

        //bullet casings
        GameObject casing = Instantiate(weaponProp.casingPrefab, weaponProp.ejectionPort.position, weaponProp.ejectionPort.rotation);
        Rigidbody casingRB = casing.GetComponent<Rigidbody>();
        casingRB.velocity = playerCameraTR.TransformDirection(weaponProp.casingDirection * weaponProp.casingVelocity);
        casingRB.AddForceAtPosition(weaponProp.casingDirection * 3.5f, casing.transform.position);
        Destroy(casing, 15);

        StartCoroutine(RecoileIE());
        transform.SendMessage("LookRecoil", weaponProp.recoilRotationEffect); //SendMessage message from player to camera
    }

    //recoiles the gun back 
    private IEnumerator RecoileIE()
    {
        currentWeaponTR.Translate(weaponProp.recoilPosEffect);
        yield return new WaitForSeconds(0.01f);
        currentWeaponTR.Translate(-weaponProp.recoilPosEffect);
    }

    private void DecreaseAmmo()
    {
        ammoContainer.magCount -= 1;
        if (ammoContainer.magCount == 0)
            isMagEmpty = true;
    }

    //Fires a single projectile every specified interval
    private void FireSingleShot()
    {
        if (Time.time >= (tempRate + lastShot))
        {
            if (tempRate != weaponProp.fireDelay)
                tempRate = weaponProp.fireDelay;
            CreateProjectileClone();
            lastShot = Time.time;
            DecreaseAmmo();
            UIoverlay.SetAmmoText(ammoContainer); //sets ammo text
        }
    }

    //Fires specified amount of projectiles per second
    private void FireFullAutoShot()
    {
        if ((Time.time - lastShot) > (1 / weaponProp.fireRate))
        {
            CreateProjectileClone();
            lastShot = Time.time;
            DecreaseAmmo();
            UIoverlay.SetAmmoText(ammoContainer); //sets ammo text
        }
    }

    //Fires a burst every specified interval
    private void FireBurstShot()
    {
        if (Time.time >= (tempRate + lastShot))
        {
            if (tempRate != weaponProp.fireDelay)//weaponInfo.fireDelay)
                tempRate = weaponProp.fireDelay;
            StartCoroutine(BurstIE());
            lastShot = Time.time;
        }
    }

    //creates a projectile every 0.05 seconds
    private IEnumerator BurstIE()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateProjectileClone();
            DecreaseAmmo();
            UIoverlay.SetAmmoText(ammoContainer); //sets ammo text
            if (ammoContainer.magCount == 0)
                yield break;
            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion


    #region (Elevator functions) ----------------------------
    private void CallElevator(GameObject hitElevator)
    {
        Elevator elevator = hitElevator.transform.parent.GetComponent<Elevator>();
        bool isCalled = elevator.isCalled;
        bool reachedTop = elevator.reachedTop;

        if (isCalled == false && reachedTop == false)
            elevator.SendMessage("Elevate");

        if (isCalled == false && reachedTop == true)
            elevator.SendMessage("Lower");
    }
    #endregion

  
    #region (Toggle Controls) ----------------------------
   /*
    // False: Sprint is always on/off; // True: holdToSprint
    private void ToggleHoldToSprint()
	{ 
		playerSettings.holdToSprint = !playerSettings.holdToSprint;
		playerSettings.setMenuSettingsOnPlayerHotKeyPress();
	}

    // False: Player is always crouching/standing; // True: hold to crouch
    private void ToggleHoldToCrouch()
	{ 
		playerSettings.holdToCrouch = !playerSettings.holdToCrouch;
		playerSettings.setMenuSettingsOnPlayerHotKeyPress();
	}

	private void ToggleHoldToADS()
	{
		playerSettings.holdToADS = !playerSettings.holdToADS;
		playerSettings.setMenuSettingsOnPlayerHotKeyPress();
	}
    */

    //toggles fire between semi-, fully-, and 3-round burst
    private void ToggleFire()
    {
        weaponProp.currentFireToggle++;
        if (weaponProp.currentFireToggle > WeaponProperties.FireToggle.burst)
            weaponProp.currentFireToggle = WeaponProperties.FireToggle.single;
        print("fireToggle: " + weaponProp.currentFireToggle);
    }
    #endregion


    #region (Grenade functions) ---------------------------
    private void ThrowGrenade()
	{
		if (playerInventory.fragGrenadesAmount > 0 && isDoneThrowingObj == true)
		{
			StartCoroutine(ThrowAnim());
			//create the grenade -------------
			GameObject thrownGrenade =  Instantiate(playerInventory.fragGernadePrefab, 
				transform.position + new Vector3(1, 1f, 1f), Quaternion.identity) as GameObject;

			thrownGrenade.GetComponent<Rigidbody>().velocity = playerCameraTR.TransformDirection
				(Vector3.forward * 20);

			SphereCollider[] grenadeColliders = thrownGrenade.GetComponents<SphereCollider>();
			for (int i=0; i<grenadeColliders.Length; i++)
            {
				Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), grenadeColliders[i]);
				Physics.IgnoreCollision(controller, grenadeColliders[i]);
			}

			thrownGrenade.SendMessage("Explode");
			playerInventory.fragGrenadesAmount--;
			// ----------
			StartCoroutine(ArmRestAnimFromThrow());
		}
	}

	//start throw animation
	private IEnumerator ThrowAnim()
	{
		isDoneThrowingObj = false;
		yield return new WaitForSeconds(0.4f);
	}

	//start arm rest animation after object is thrown
	private IEnumerator ArmRestAnimFromThrow()
	{
		yield return new WaitForSeconds(0.4f);
		isDoneThrowingObj = true;
	}
	#endregion


	#region (Movement and player location) ----------------------------
	//returns crouch, run, or walk speed
	private float MovementSpeed()
	{
		if (playerSettings.holdToSprint == true) 
			isWalking = !Input.GetKey(playerSettings.keyBinds["sprintKey"]); //true when LeftShit is not pressed
		else
        { //if (ToggleHoldToSprint() == false)
            if (Input.GetKeyDown(playerSettings.keyBinds["sprintKey"]))
            {
				isWalking = !isWalking;
				print("isWalking: " + isWalking);
			}
		}

        if (playerSettings.holdToCrouch == true)
        {
            isStanding = !Input.GetKey(playerSettings.keyBinds["crouchKey"]); //true if not pressed
        }
        else
        { // if false
            if (Input.GetKeyDown(playerSettings.keyBinds["crouchKey"]))
            {
                isStanding = !isStanding;
                //print("isStanding: " + isStanding);
            }
        }

        anim.SetBool("isStanding", isStanding);
        anim.SetBool("isCrouching", !isStanding);
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isRunning", !isWalking);

        if ((isWalking == true) && (isStanding == true))
        {
			if (aimDownSight == true)
				return playerSpeeds.walkSpeed / playerSpeeds.speedInibitorADS;
			else
				return playerSpeeds.walkSpeed;
		}
        else if ((isWalking == false) && (isStanding == true))
        {
			//print ("called sprint");
			return playerSpeeds.runSpeed;
		}
		else //if (((isWalking == true) && (isStanding == false)) 
		{ //|| ((isWalking == false) && (isStanding == false)))
			return playerSpeeds.crouchSpeed;
		}
	}

	private void MovePlayer()
	{
		if (controller.isGrounded)
        {
            float inputH = Input.GetAxis("Horizontal"); //x
            float inputV = Input.GetAxis("Vertical"); //z

            //print("inputH: " + inputH);
            moveDirection = new Vector3(inputH, 0f, inputV);

            if (moveDirection.magnitude > 1f)
                moveDirection.Normalize();

            /*
            float forward = moveDirection.z;
            float strafe = moveDirection.x;
            float up = moveDirection.z; 
            */

            anim.SetFloat("inputH", inputH, 0.1f, Time.deltaTime);
            anim.SetFloat("inputV", inputV, 0.1f, Time.deltaTime);

            if (inputV < 0) //is negative, player is going backwards
                moveDirection = transform.TransformDirection(moveDirection) * playerSpeeds.walkSpeed / 2;
            else
                moveDirection = transform.TransformDirection(moveDirection) * playerSpeeds.currentPlayerSpeed;

            if (Input.GetKeyDown(playerSettings.keyBinds["jumpKey"]))
            {
                moveDirection.y += jumpHeight;
                anim.SetBool("jump", true);
                StartCoroutine(jumpIE());
            }
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}

    IEnumerator jumpIE()
    {
        yield return new WaitForEndOfFrame();
        anim.SetBool("jump", false);

    }

	//crouch or un-crouch the player
	private void PlayerCrouch()
	{
		if ((isPlayerCrouched == false) && (isStanding == false))
        {
            playerCameraTR.localPosition = new Vector3(playerCameraTR.localPosition.x, playerCameraTR.localPosition.y / 2, playerCameraTR.localPosition.z);
            Vector3 weaponSpotsPos = transform.GetChild(1).localPosition;
            transform.GetChild(1).localPosition = new Vector3(weaponSpotsPos.x, weaponSpotsPos.y/2, weaponSpotsPos.z);
            controller.height /= 2;
            controller.center /= 2;
            CapsuleCollider capsule = controller.GetComponent<CapsuleCollider>();
            capsule.center /= 2;
            capsule.height /= 2;
			isPlayerCrouched = true;
		}
        else if ((isPlayerCrouched == true) && (isStanding == true))
        {
            playerCameraTR.localPosition = new Vector3(playerCameraTR.localPosition.x, playerCameraTR.localPosition.y * 2, playerCameraTR.localPosition.z);
            Vector3 weaponSpotsPos = transform.GetChild(1).localPosition;
            transform.GetChild(1).localPosition = new Vector3(weaponSpotsPos.x, weaponSpotsPos.y * 2, weaponSpotsPos.z);
            controller.height *= 2;
            controller.center *= 2;
            CapsuleCollider capsule = controller.GetComponent<CapsuleCollider>();
            capsule.center *= 2;
            capsule.height *= 2;
            isPlayerCrouched = false;
		}
	}

	private void OnPlayerDeath()
	{
		GameObject playerRagdoll = Instantiate(ragdoll, transform.position, transform.rotation) as GameObject; //spawn player's ragdoll
        GameObject deathCam = Instantiate(playerCameraTR.gameObject, transform.position, transform.rotation) as GameObject;
        foreach (Transform childTR in deathCam.transform)
            Destroy(childTR.gameObject);
        playerInventory.DropInventory(); //drop player's inventory
		Destroy(gameObject); //destroy player

        menuControl.DisplayDeathMenu(); //display death menu (from in pauseMenu)
	}
	#endregion


	#region (onTrigger or onCollision) ----------------------------
	//when something enters the player's trigger Capsule Collider
	private void OnTriggerEnter(Collider hitObject)
	{
		if (hitObject.CompareTag("Projectile"))
            AddDamage(hitObject);

		if (hitObject.CompareTag("HealthPack"))
            PickUpHealth(hitObject);

		if (hitObject.CompareTag("Gun") || hitObject.gameObject.CompareTag("Ammo"))
            PickUpAmmo(hitObject);

		if (hitObject.CompareTag("fragGrenade"))
            PickUpGrenade(hitObject);

		if (hitObject.CompareTag("Gun"))
			Physics.IgnoreCollision(hitObject.GetComponent<BoxCollider>(), controller, true);

		if (hitObject.name == "EnemySpawnTrigger")
			Destroy(hitObject.gameObject);

        if (hitObject.name == "EndLvlTrigger")
        {
            UIoverlay.DisplaySavingImage(1f, 3);
            GameObject.Find("Global_controller").GetComponent<Global_controller>().onLevelComplete();
        }

		if (hitObject.tag == "CheckpointTrigger")
        {
            print("checkpoint hit");
            UIoverlay.DisplaySavingImage(1f, 3);
            Destroy(hitObject.gameObject);
            StartCoroutine(Test());
           // GameObject.Find("Global_controller").GetComponent<Global_controller>().onCheckpointTriggered();
		}
	}

    private IEnumerator Test()
    {
        yield return null;
        GameObject.Find("Global_controller").GetComponent<Global_controller>().onCheckpointTriggered();
    }

	private void OnTriggerStay(Collider hitObject)
	{ 
		if (hitObject.gameObject.CompareTag("Gun"))
            PickUpOrDropWeapon(hitObject); 
	}

	private void OnTriggerExit(Collider hitObject)
	{
		if (hitObject.CompareTag("Gun"))
			keyTimer = 0;
	}
	#endregion


	#region (Called by OnTrigger or OnCollision) ----------------------------
	private void PickUpGrenade(Collider hitObject)
	{
		if (playerInventory.fragGrenadesAmount < playerInventory.maxFragsAmount)
        {
			playerInventory.fragGrenadesAmount++;
			//print (playerInventory.fragGrenadesAmount);
			Destroy(hitObject.gameObject);
		}
	}

	//do stuff if the character is hit by a bullet
	private void AddDamage(Collider hitObject)
	{
		//if player gets hit, reduce the health and change the text
		if (hitObject.gameObject.CompareTag("Projectile"))
        {
			float bulletDamage = hitObject.gameObject.GetComponent<ProjectileProperties>().bulletDamage;
			playerHealth -= bulletDamage;
            UIoverlay.UpdateHealthImage(bulletDamage);
		}

		if (playerHealth <= 0)
            OnPlayerDeath();
        UIoverlay. SetHealthText(playerHealth);
		//play effects
		//play audio
	}

	//called via SendMessage in explosive's property script
	private void AddDamage(float explosionDamage)
	{
		//print("hit by explosion");
		playerHealth -= explosionDamage;
        UIoverlay.UpdateHealthImage(explosionDamage);

		if (playerHealth <= 0)
            OnPlayerDeath();
        UIoverlay.SetHealthText(playerHealth);
	}

	private void PickUpHealth(Collider hitObject)
	{
		HealthPack healthPack = hitObject.gameObject.GetComponent<HealthPack>();
		playerHealth += healthPack.health;
        UIoverlay.UpdateHealthImage(healthPack.health);
        healthPack.health = 0;

        UIoverlay.SetHealthText(playerHealth);
	}

	//ability to pick up ammo from ammo containers
	private void PickUpAmmo(Collider hitObject)
	{
        GunAmmoContainer otherContainer = hitObject.gameObject.GetComponent<GunAmmoContainer>();
        int otherContainerAmmo = otherContainer.ammoCount;

        if (ammoContainer != null && ammoContainer.ammoCount < ammoContainer.maxAmmo)
        {
            if (otherContainerAmmo > ammoContainer.maxAmmo)
            {
                otherContainer.ammoCount = otherContainerAmmo - ammoContainer.maxAmmo;
                ammoContainer.ammoCount = ammoContainer.maxAmmo;
            }
            else
            {
                if (ammoContainer.ammoCount + otherContainerAmmo < ammoContainer.maxAmmo)
                {
                    ammoContainer.ammoCount += otherContainerAmmo;
                    otherContainer.ammoCount = 0;
                }
                else
                {
                    otherContainer.ammoCount = ammoContainer.ammoCount + otherContainerAmmo - ammoContainer.maxAmmo;
                    ammoContainer.ammoCount = ammoContainer.maxAmmo;
                }
            }
            UIoverlay.SetAmmoText(ammoContainer); //sets ammo text
        }
    }
    #endregion


    #region (Aim Down Sights methods) ----------------------------
    private void WeaponBobbing()
    {
        if (isMoving == true && atShoulder == true)
        {
            float step = 0.05f * Time.deltaTime;

            if (currentWeaponTR.localPosition.y >= topBob.y)
                reachedTop = true;
            else if (currentWeaponTR.localPosition.y <= bottomBob.y)
                reachedTop = false;

            if (reachedTop == true)
                currentWeaponTR.localPosition = Vector3.MoveTowards(currentWeaponTR.localPosition, bottomBob, step);
            else if (reachedTop == false)
                currentWeaponTR.localPosition = Vector3.MoveTowards(currentWeaponTR.localPosition, topBob, step);
        }
        else if (isMoving == false && currentWeaponTR.localPosition.y != startBob.y)
            currentWeaponTR.localPosition = Vector3.MoveTowards(currentWeaponTR.localPosition, startBob, 0.05f * Time.deltaTime);
    }

	private void LerpToADS()
	{
		if (currentWeaponTR.localPosition != adsPos)
        {
			float currentFOV = playerCameraTR.GetComponent<Camera>().fieldOfView;
            float targetFOV = startFOV / weaponProp.scopeZoomMultiplier;
            print("weaponProp.scopeZoomMultiplier: " + weaponProp.scopeZoomMultiplier);
            print("targetFOV: " + targetFOV);
            float adsSpeed = (Mathf.Abs(Vector3.Distance(adsPos, shoulderPos)) / weaponProp.timeToADS) * Time.deltaTime; //distance per second
            float fovSpeed = (Mathf.Abs(currentFOV - targetFOV) / weaponProp.timeToADS) * Time.deltaTime;  //speed = distance/time

            print("lerping to face");
			currentWeaponTR.localPosition = Vector3.MoveTowards(currentWeaponTR.localPosition, adsPos, adsSpeed);
			playerCameraTR.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(currentFOV, targetFOV, fovSpeed*3);
		}
	}

	private void LerpToShoulder()
	{
        if (currentWeaponTR.localPosition != shoulderPos)
        {
            float currentFOV = playerCameraTR.GetComponent<Camera>().fieldOfView;
            float targetFOV = startFOV;
            float adsSpeed = (Mathf.Abs(Vector3.Distance(adsPos, shoulderPos)) / weaponProp.timeToADS) * Time.deltaTime; //distance per second
            float fovSpeed = (Mathf.Abs(currentFOV - targetFOV) / weaponProp.timeToADS) * Time.deltaTime;  //speed = distance/time
             
            print("lerping to shoulder");
            currentWeaponTR.localPosition = Vector3.MoveTowards(currentWeaponTR.localPosition, shoulderPos, adsSpeed);
            playerCameraTR.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(currentFOV, targetFOV, fovSpeed*3);
        }
        else
            atShoulder = true;
	}

	private void AimDownSights()
	{
		if (currentWeapon != null)
        {
            if (playerSettings.holdToADS == true)
            {
                aimDownSight = Input.GetKey(playerSettings.keyBinds["adsKey"]);

                if (aimDownSight == false)
                { //if keyboard isnt being used check controller
                    //ADS with Controller
                    if (Input.GetAxis("XboxLT") == 1) //if Left Trigger is pressed
                        aimDownSight = true;
                    else
                        aimDownSight = false;
                }
                //print("aimDownSight: " + aimDownSight);
            }
            else
            {
                if (Input.GetKeyDown(playerSettings.keyBinds["adsKey"]))
                    aimDownSight = !aimDownSight;
                //ADS with controller
                if (Input.GetAxis("XboxLT") == 1)
                {
                    if (xboxLTinUse == false)
                    {
                        aimDownSight = !aimDownSight;
                        xboxLTinUse = true;
                    }
                }
                else
                    xboxLTinUse = false;
            }

			if (aimDownSight == true)
            {
                if (playerSpeeds.currentPlayerSpeed == playerSpeeds.runSpeed &&
                    controller.velocity != Vector3.zero || isReloading == true) //currentWeaponBP.isReloading == true)
                {
                    print("kicking ads");
                    aimDownSight = false;
                }
			}

            if (aimDownSight == true)
            {
                LerpToADS();
                atShoulder = false;
            }
            else if (aimDownSight == false && atShoulder == false)
                LerpToShoulder();
		}
	}

	private void KickOutOfADS()
	{
		//print ("called");
		if (currentWeapon != null && aimDownSight == true)
        {
			aimDownSight = false;
			currentWeaponTR.localPosition = shoulderPos;
		}
		playerCameraTR.GetComponent<Camera>().fieldOfView = startFOV;
	}
	#endregion


	#region (Weapon Changes / PickUp / Drop) ----------------------------
    //swaps from inventory
	private void ChangeWeapons()
	{
        //print ("changed weapons");
        if (playerInventory.weaponsInventory[0] != null && playerInventory.weaponsInventory[1] != null
            && aimDownSight == false)
        {
            SetOldWeaponOnChange();
            if (currentWeapon == playerInventory.weaponsInventory[0])
                currentWeapon = playerInventory.weaponsInventory[1];
            else
                currentWeapon = playerInventory.weaponsInventory[0];

            if (isReloading == true)
            {
                StopCoroutine(reloadingCoroutine);
                print("reloading canceled");
                isReloading = false;
            }
            SetNewWeaponOnChange();
		}
	}

	private void SetWeaponOnPlayersBack()
	{
		//print ("on Back");
		currentWeaponTR.SetParent(middleBackTR);

		currentWeaponTR.rotation = new Quaternion(0, currentWeaponTR.rotation.y, 
			0, currentWeaponTR.rotation.w);
		currentWeaponTR.localPosition = Vector3.zero;

		currentWeaponTR.Rotate(120, 90, 0);
		currentWeaponTR.localPosition = new Vector3(currentWeaponTR.localPosition.x + 0.3f, 
			currentWeaponTR.localPosition.y + 0.5f, currentWeaponTR.localPosition.z);
	}

	private void SetWeaponOnPlayersThigh()
	{
		//print ("on Thigh");
		currentWeaponTR.SetParent(rightThighTR);
	}

	private void SetOldWeaponOnChange()
	{
        KickOutOfADS();
		currentWeapon.GetComponent<WeaponSway>().enabled = false;

		//set weapon on back or right thigh
		if (weaponProp.weaponClass.Equals("pistol") == false)
            SetWeaponOnPlayersBack();
        else
            SetWeaponOnPlayersThigh();
	}

    //gets current weapon's stuff
    private void SetNewWeaponOnChange()
	{
		currentWeaponTR = currentWeapon.transform; //sets variable to current weapon's transform
        currentWeaponTR.SetParent(playerCameraTR);

        weaponProp = currentWeapon.GetComponent<WeaponProperties>();
        ammoContainer = currentWeapon.GetComponent<GunAmmoContainer>();
        currentWeapon.GetComponent<WeaponSway>().enabled = true; //activates stuff

        UIoverlay.SetAmmoText(ammoContainer); //sets ammo text

        UIoverlay.SetWeaponImages(weaponProp.weaponSprite, weaponProp.reticleTexture); //set images on UI for weapon
		
        //sets weapon location
		currentWeaponTR.localPosition = shoulderPos;///shoulderPos; //set to shoulder
		currentWeaponTR.rotation = playerCameraTR.rotation; // sets to camera rotation
	}

	private void SetWeaponOnDrop()
	{
        KickOutOfADS();
		currentWeaponTR.parent = null;
		currentWeapon.GetComponent<Rigidbody>().isKinematic = false; //dropped weapon is manipulatible again
		//renables player's Trigger's ability to collide with weapon
		currentWeapon.GetComponent<SphereCollider>().enabled = true; //enable AOE collider for pick-up
		Physics.IgnoreCollision(currentWeapon.GetComponent<BoxCollider>(), 
			GetComponent<CapsuleCollider>(), false); //player can collide with gun's Trigger again
	}

	private void SetWeaponOnPickUp()
	{
        print("setting");

		currentWeapon.transform.SetParent(playerCameraTR);
		currentWeapon.GetComponent<Rigidbody>().isKinematic = true; //disable objects physics
		//disable collisions with the gun
		currentWeapon.GetComponent<SphereCollider>().enabled = false;
		Physics.IgnoreCollision(currentWeapon.GetComponent<BoxCollider>(), 
			GetComponent<CapsuleCollider>(), true);
	}

	private void PickUpOrDropWeapon(Collider hitObject)
	{
		if (currentWeapon == null && playerInventory.weaponsInventory[0] == null)
        {
			currentWeapon = hitObject.gameObject;
			playerInventory.weaponsInventory[0] = currentWeapon; //saves first weapon
            print("picked up first weapon: " + playerInventory.weaponsInventory[0].name);
            UIoverlay.ActivateWeaponUI(); //UI for weapons is turned on
            SetWeaponOnPickUp(); //disables/enables stuff when on player
            SetNewWeaponOnChange(); //sets current stuff

            //weapon bobbing
            startBob = new Vector3(currentWeaponTR.localPosition.x, currentWeaponTR.localPosition.y, currentWeaponTR.localPosition.z);
            topBob = new Vector3(startBob.x, startBob.y +0.006f, startBob.z);
            bottomBob = new Vector3(startBob.x, startBob.y -0.006f, startBob.z);

        }
        else if (Input.GetKey(playerSettings.keyBinds["interactKey"]))
        {
			print(keyTimer);
			keyTimer += Time.deltaTime;
			if (keyTimer > keyHoldLength && Mathf.Abs(hitObject.attachedRigidbody.velocity.y) < 0.02f)
            {
				keyTimer = 0;
				if (playerInventory.weaponsInventory[1] == null && playerInventory.weaponsInventory[0] != null)
                {
                    SetOldWeaponOnChange();
					currentWeapon = hitObject.gameObject;
					playerInventory.weaponsInventory[1] = currentWeapon;
				}
                else if (playerInventory.weaponsInventory[1] != null && playerInventory.weaponsInventory[0] != null)
                {
                    SetWeaponOnDrop();
					//replace spot in array
					if (currentWeapon == playerInventory.weaponsInventory[0])
                    {
						currentWeapon = hitObject.gameObject;
						playerInventory.weaponsInventory[0] = currentWeapon;
					}
                    else
                    {//if (currentWeapon == playerInventory.weaponsInventory[1])
						currentWeapon = hitObject.gameObject;
						playerInventory.weaponsInventory[1] = currentWeapon;
					}
				}
                SetWeaponOnPickUp(); //disables/enables stuff when on player
                SetNewWeaponOnChange(); //sets the new current weapon stuff
            }
		}
		if (Input.GetKeyUp(playerSettings.keyBinds["interactKey"]))
        {
			print ("key lifted");
			keyTimer = 0;
		}
	}
	#endregion
}
