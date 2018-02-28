using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//class that deals with player controller collisions
public class PlayerOnColliderHit : MonoBehaviour
{
	private CharacterController controller;
	public GameObject ragdoll; //player ragdoll, when player dies
	public Transform cameraTR; //transform of player camera

	//locations on player for weapons
	public Transform rightShoulderTR;
	private Vector3 shoulderPos; //sets location of gun on player

	public Transform middleBackTR;
	public Transform rightThighTR;

	//player health
	private Text healthText; //text the displays player health
	public float playerHealth = 100f;
	public Image healthBarImage;

	public Image weaponImage;
	public Image weaponReticleImage;

	//player WeaponsInventory
	private List<GameObject> weaponsInventory = new List<GameObject>(2);//size of two

	//current weapon equiped
	private GameObject currentWeapon; //the current weapon held by player
	private BallProjectile currentWeaponBP; //the BallProjectile of current weapon
	private Transform currentWeaponTR; //gets assigned after player has a gun

	private bool isOverGun = false; //is player over a weapon
	//bool isMoving = false;

	//ADS stuff
	private Vector3 adsPos;
	private bool toggleADSHold = false;
	private bool aimDownSight = false;
	private float startFOV;

	//pick up timer
	private float keyTimer = 0; // iterates from last frame
	public float keyHoldLength = 0.4f; //length player must hold to activate something
	//====================================================================

	//--------------------------------------------------------------------
	void Start () 
	{
		controller = GetComponent<CharacterController>();

		healthText = GameObject.Find("Health").GetComponent<Text>();
		healthText.text = "100";

		shoulderPos = rightShoulderTR.localPosition;//new Vector3 (0.25f, -1.3f, 0.1f); //sets location of gun
		adsPos = new Vector3 (cameraTR.localPosition.x, 
			shoulderPos.y, cameraTR.localPosition.z);

		startFOV = cameraTR.GetComponent<Camera>().fieldOfView;

		Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), controller);
	}

	// Update is called once per frame
	void Update () 
	{		
		//spawn ragdol if player health reaches 0
		if (playerHealth <= 0) {
			//isPlayerAlive = false;
			GameObject playerRagdoll = Instantiate(ragdoll, transform.position, transform.rotation) as GameObject;
			for (int i=0; i<weaponsInventory.Count; i++) {
				weaponsInventory[i].SetActive(true);
				weaponsInventory[i].GetComponent<Rigidbody>().isKinematic = false;
				weaponsInventory[i].GetComponent<BallProjectile>().enabled = false;
				weaponsInventory[i].transform.parent = null;
			}
			//cameraTR.DetachChildren();
			Destroy(gameObject);
		}
		changeWeapons(); //change weapons from inventory
		aimDownSights();
		toggleHoldToADS();
	}

	void OnGUI()
	{
		if (isOverGun == true) {
			//GUI.enabled = true;
			GUI.Label (new Rect (150, 225, 100, 20), "Hold 'e' to pick up");
		} else {
			//GUI.enabled = false;
		}
	}

	////Start: trigger functions ------------------------------------------------------------
	//when something enters the player's trigger Capsule Collider
	private void OnTriggerEnter(Collider hitObject)
	{
		gotHit(hitObject);
		pickUpHealth(hitObject);
		pickUpAmmo(hitObject);

		if (hitObject.CompareTag ("Gun")) {
			Physics.IgnoreCollision (hitObject.GetComponent<BoxCollider> (), 
				controller, true);
		}
	}

	private void OnTriggerStay(Collider hitObject)
	{
		pickUpOrDropWeapon(hitObject);
	}

	//do stuff if the character is hit by a bullet
	private void gotHit(Collider hitObject)
	{
		loseHealth(hitObject);
		//also other stuff, like blood effects
	}

	//update player's health when hit
	private void loseHealth(Collider hitObject)
	{
		//if player gets hit, reduce the health and change the text
		if (hitObject.gameObject.CompareTag ("Projectile")) {
			int bulletDamage = hitObject.gameObject.GetComponent<ProjectileProperties>().bulletDamage;
			playerHealth -= bulletDamage;
			healthBarImage.fillAmount -= ((float)bulletDamage/100);
		}
		setText();
	}

	private void pickUpHealth(Collider hitObject)
	{
		if (hitObject.gameObject.CompareTag("HealthPack")) {
			HealthPack healthPack = hitObject.gameObject.GetComponent<HealthPack>();
			playerHealth += healthPack.health;
			healthBarImage.fillAmount += ((float)healthPack.health/100);
			healthPack.health = 0;
		}
		setText();
	}

	//set Health text
	private void setText()
	{
		healthText.text = playerHealth.ToString();
	}

	//ability to pick up ammo from ammo containers
	private void pickUpAmmo(Collider hitObject)
	{
		if (hitObject.gameObject.CompareTag("Gun") || hitObject.gameObject.CompareTag("Ammo")) {
			GunAmmoContainer container = hitObject.gameObject.GetComponent<GunAmmoContainer> ();
			int containerAmmo = container.ammoCount;
			//if the weapon doesn't have full ammo
			if ((currentWeaponBP != null) && (currentWeaponBP.currentAmmoCount < currentWeaponBP.maxAmmoCount)) {
				//if the container has more ammo than the weapon can hold
				if (containerAmmo > currentWeaponBP.maxAmmoCount) {
					container.ammoCount = containerAmmo - currentWeaponBP.maxAmmoCount; //sets the new container ammo count
					currentWeaponBP.currentAmmoCount = currentWeaponBP.maxAmmoCount;//fills weapons ammo to full
				} else { //(if ammo < currentWeaponBP.maxAmmoCount)//if the container's ammo is less than the max of the weapon
					if (currentWeaponBP.currentAmmoCount + containerAmmo < currentWeaponBP.maxAmmoCount) {
						currentWeaponBP.currentAmmoCount += containerAmmo;
						container.ammoCount = 0;
					} else { //if there is more ammo in currentAmmoCount+containerAmmo than the max 
						container.ammoCount = currentWeaponBP.currentAmmoCount+containerAmmo-currentWeaponBP.maxAmmoCount;
						currentWeaponBP.currentAmmoCount = currentWeaponBP.maxAmmoCount;
					}
				}
			}
		}
	}
	//End: trigger functions ================================================================

	//Start: ADS functions ------------------------------------------------------------------
	private void toggleHoldToADS()
	{
		if (Input.GetKeyDown ("3")) {
			toggleADSHold = !toggleADSHold;
			print ("toggleADSHold: " + toggleADSHold);
		}
	}

	private void lerpToADS()
	{
		if(currentWeaponTR.localPosition != adsPos) {
			float step = 1f * Time.deltaTime;
			float currentFOV = cameraTR.GetComponent<Camera>().fieldOfView;
			float targetFOV = startFOV / currentWeaponBP.weaponInfo.zoomMultiplier;

			//print ("lerping to face");
			currentWeaponTR.localPosition = Vector3.MoveTowards(currentWeaponTR.localPosition, adsPos, step);
			cameraTR.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(currentFOV, targetFOV, step*110);
		}
	}

	private void lerpToShoulder()
	{
		if (currentWeaponTR.localPosition != shoulderPos) {
			float step = 1f * Time.deltaTime;
			float currentFOV = cameraTR.GetComponent<Camera>().fieldOfView;
			float targetFOV = startFOV;

			//print ("lerping to shoulder");
			currentWeaponTR.localPosition = Vector3.MoveTowards (currentWeaponTR.localPosition, shoulderPos, step);
			cameraTR.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(currentFOV, targetFOV, step*110);
		}
	}

	private void aimDownSights()
	{
		PlayerControllerFirst controllerFirstScript = GetComponent<PlayerControllerFirst>();
		float runSpeed = controllerFirstScript.runSpeed;
		float currentSpeed = controllerFirstScript.currentPlayerSpeed;

		if (currentWeapon != null)
		{
			if (toggleADSHold == true)
				aimDownSight = Input.GetButton ("Fire2");
			else {
				if (Input.GetButtonDown ("Fire2"))
					aimDownSight = !aimDownSight;
			}

			if (aimDownSight == true)
			{
				if (currentSpeed == runSpeed && controller.velocity != Vector3.zero || currentWeaponBP.isReloading == true)
					aimDownSight = false;
			}

			if (aimDownSight == true)
				lerpToADS ();
			else //if (aimDownSight == false )
				lerpToShoulder ();
		}
	}

	private void kickOutOfADS()
	{
		if (currentWeapon != null && aimDownSight == true) {
			aimDownSight = false;
			currentWeaponTR.localPosition = shoulderPos;
		}
	}
	//End: ADS functions ====================================================================


	//Start: weapon on player functions -----------------------------------------------------
	private void changeWeapons()
	{
		if ((Input.GetKeyDown("l")) && (weaponsInventory.Count == 2)) {
			setOldWeaponOnChange();
			if (currentWeapon == weaponsInventory[0]) 
				currentWeapon = weaponsInventory[1];
			else 
				currentWeapon = weaponsInventory[0];
			setNewWeaponOnChange();
		}
	}

	private void setOldWeaponOnChange()
	{
		kickOutOfADS();
		currentWeaponBP.enabled = false;
		currentWeapon.GetComponent<WeaponSway>().enabled = false;

		//set weapon on back or right thigh
		//print("weapon class: " + currentWeaponBP.weaponInfo.weaponClass);
		if (currentWeaponBP.weaponInfo.weaponClass.Equals("pistol") == false) {
			setWeaponOnPlayersBack();
		} else
			setWeaponOnPlayersThigh();
	}

	private void setWeaponOnPlayersBack()
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

	private void setWeaponOnPlayersThigh()
	{
		//print ("on Thigh");
		currentWeaponTR.SetParent(rightThighTR);
	}

	private void setNewWeaponOnChange()
	{
		//gets current weapon's stuff
		currentWeaponTR = currentWeapon.transform; // sets variable to current weapon's transform
		currentWeaponBP = currentWeapon.GetComponent<BallProjectile>(); //sets to current BallProjectile
		WeaponProperties weaponProp = currentWeapon.GetComponent<WeaponProperties>();

		currentWeaponTR.SetParent(cameraTR);

		//activates stuff
		currentWeaponBP.enabled = true;
		currentWeapon.GetComponent<WeaponSway>().enabled = true;
		//activates image of the weapon in the UI
		weaponImage.gameObject.transform.parent.gameObject.SetActive(true);
		weaponImage.sprite = weaponProp.weaponSprite;
		//activates reticle in the UI
		weaponReticleImage.sprite = weaponProp.reticleTexture;
		//sets weapon location
		currentWeaponTR.localPosition = shoulderPos;///shoulderPos; //set to shoulder
		currentWeaponTR.rotation = cameraTR.rotation; // sets to camera rotation
	}

	private void setWeaponOnDrop()
	{
		kickOutOfADS();
		currentWeaponTR.parent = null;
		currentWeaponBP.enabled = false; //disables weapon's ability to shoot
		currentWeapon.GetComponent<Rigidbody>().isKinematic = false; //dropped weapon is manipulatible again
		//renables player's Trigger's ability to collide with weapon
		currentWeapon.GetComponent<SphereCollider>().enabled = true; //enable AOE collider for pick-up
		Physics.IgnoreCollision(currentWeapon.GetComponent<BoxCollider>(), 
			GetComponent<CapsuleCollider>(), false); //player can collide with gun's Trigger again
	}

	private void setWeaponOnPickUp()
	{
		currentWeapon.transform.SetParent(cameraTR);
		currentWeapon.GetComponent<Rigidbody>().isKinematic = true; //disable objects physics
		//disable collisions with the gun
		currentWeapon.GetComponent<SphereCollider>().enabled = false;
		Physics.IgnoreCollision(currentWeapon.GetComponent<BoxCollider>(), 
			GetComponent<CapsuleCollider>(), true);
	}

	private void pickUpOrDropWeapon(Collider hitObject)
	{
		if (hitObject.gameObject.CompareTag ("Gun")) {
			if (currentWeapon != null)
				isOverGun = true;

			if ((currentWeapon == null) && (weaponsInventory.Count == 0)) {
				currentWeapon = hitObject.gameObject;
				weaponsInventory.Add (currentWeapon);
				setWeaponOnPickUp (); //disables/enables stuff when on player
				setNewWeaponOnChange (); //sets current stuff
			} else if (Input.GetKey ("e")) {
				keyTimer += Time.deltaTime;
				if (keyTimer > keyHoldLength && Mathf.Abs(hitObject.attachedRigidbody.velocity.y) < 0.02f) {
					keyTimer = 0;
					if (weaponsInventory.Count == 1) {
						setOldWeaponOnChange (); //sets old weapon to player's back
						//adds new weapon to inventory
						currentWeapon = hitObject.gameObject;
						weaponsInventory.Add (currentWeapon);
					} 
					else if (weaponsInventory.Count == 2) {
						setWeaponOnDrop(); // sets stuff for the weapon to be dropped
						//removes current (old) weapon from Inventory
						int weaponNumber = weaponsInventory.IndexOf (currentWeapon);
						weaponsInventory.RemoveAt (weaponNumber);
						//adds new weapon to inventory
						currentWeapon = hitObject.gameObject;
						weaponsInventory.Insert (weaponNumber, currentWeapon);
					}
					setWeaponOnPickUp (); //disables/enables stuff when on player
					setNewWeaponOnChange (); //sets the new current weapon stuff
				}
			}
			if (Input.GetKeyUp("e"))
				keyTimer = 0;
		} 
		else
			isOverGun = false;
	}
	//End: weapon on player functions =======================================================

	/*
	private struct currentWeaponInfo
	{
		public GameObject currentWeapon;
		public Transform currentWeaponTR;
		public BallProjectile currentWeaponBP;
		public Rigidbody currentWeaponRB;
		
	}
	*/

}