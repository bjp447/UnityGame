using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityStandardAssets.Cameras; //need this to access FreeLookCam from unitys standard assets library
using UnityEngine.UI;

// rename to general weapon fireing.
// should handle reticle setting, recoil, ammo, reload, animations involving a weapon
// shooting, Instantiate-ing, 
public class BallProjectile : MonoBehaviour
{
	//projectile prefabs
	public GameObject projectilePrefab;
	public GameObject casingPrefab;
	public float casingVelocity = 7.5f;

	//gun
	//public Transform gunTransform;
	public Transform barrelEnd;
	public Transform ejectionPort; //spot where casing comes out

	//camera
	private Transform cameraTR;

	//fire rate
	private float tempRate = 0.0f; //allows for fire on start
	private float lastShot = 0.0f; //gets modified

    private Settings_controller playerSettings;
    //fire toggle
    //private string fireToggleBtn = "t";
    private enum FireToggle {single, full, burst};
	private FireToggle playerFireToggle;

	//cassing rotation
	float[] rotateSpot = {0.09f, 0.04f, 0.10f};
	private int i = 0;

	//ammo
	public int magSize;// = 30;
	public int maxAmmoCount;// = 250; //accessed elseware

	public int currentMagCount;
	public int currentAmmoCount;
	private bool isMagEmpty = false;
	public bool isReloading = false;
	//private bool isMagFull = true;

	//UI
	private Text magAmmoText;
	private Text ammoText;

	public getWeaponsInfo weaponInfo;

	Coroutine reloadingCoroutine = null;
	private Player_controller playerController;
	//private WeaponProperties weaponProperties;
	//----------------------------------------------------------------------


	//struct for information about the weapon
	public struct getWeaponsInfo
	{
		public float projectileVelocity;
		public float fireDelay;
		public float fireRate;
		public float reloadTime;
		public Vector3 recoilPosEffect;
		public Vector3 recoilRotationEffect;
		public string weaponClass;
		public float zoomMultiplier;
		//public float reticleSize;
		//public Sprite reticleTexture;
		//public Sprite gunSprite;
	
		public getWeaponsInfo(WeaponProperties weaponProperty)
		{
			this.projectileVelocity = weaponProperty.bulletVelocity;
			this.fireDelay = weaponProperty.fireDelay;
			this.fireRate = weaponProperty.fireRate;
			this.reloadTime = weaponProperty.reloadTime;
			this.recoilPosEffect = weaponProperty.recoilPosEffect;
			this.recoilRotationEffect = weaponProperty.recoilRotationEffect;
			this.weaponClass = weaponProperty.weaponClass;
			this.zoomMultiplier = weaponProperty.scopeZoomMultiplier;
			//this.reticleSize = weaponProperty.reticleSize;
			//this.reticleTexture = weaponProperty.reticleTexture;
			//this.gunSprite = weaponProperty.gunSprite;
		}
	}


	private void OnPickUp()
	{
		magSize = GetComponent<GunAmmoContainer>().maxMag;
		maxAmmoCount = GetComponent<GunAmmoContainer>().maxAmmo;
		currentMagCount = GetComponent<GunAmmoContainer>().magCount;
		currentAmmoCount = GetComponent<GunAmmoContainer>().ammoCount;
		//currentMagCount = magSize + 1;
	}

	//called when diabled, when the gun is dropped or swapped
	private void OnDisable() //private void onDrop()
	{
		GetComponent<GunAmmoContainer>().maxMag = magSize;
		GetComponent<GunAmmoContainer>().maxAmmo = maxAmmoCount;
		GetComponent<GunAmmoContainer>().magCount = currentMagCount;
		GetComponent<GunAmmoContainer>().ammoCount = currentAmmoCount;
		//print ("called");
	}

	// Use this for initialization
	void Start () 
	{
		//print ("called");
		weaponInfo = new getWeaponsInfo(GetComponent<WeaponProperties>());
        //weaponProperties = GetComponent<WeaponProperties>();
        OnPickUp();

		playerController = transform.parent.parent.GetComponent<Player_controller> ();

		magAmmoText = GameObject.Find ("AmmoInMag").GetComponent<Text>();
		ammoText = GameObject.Find ("AmmoCount").GetComponent<Text>();

		magAmmoText.text = "";
		ammoText.text = "";

		cameraTR = Camera.main.transform;

		print ("fireToggle: " + playerFireToggle);

        playerSettings = GameObject.Find("PlayerSettings").GetComponent<Settings_controller>();
    }

	void Update() 
	{
        if (isMagEmpty == false && isReloading == false) {
			if (playerFireToggle == FireToggle.single) {
				if (Input.GetButtonDown("Fire1") || Input.GetAxis("XboxRT") == 1)
                    FireSingleShot();
			} else if (playerFireToggle == FireToggle.full) {
				if (Input.GetButton("Fire1") || Input.GetAxis("XboxRT") == 1)
                    FireFullAutoShot();
			} else if (playerFireToggle == FireToggle.burst) {
				if (Input.GetButtonDown("Fire1") || Input.GetAxis("XboxRT") == 1)
                    FireBurstShot();
			}
		} else {
			//play audio. empty sound
		}

		Reload(); //reloads weapon
        SetText(); //sets ammo text

		if (Input.GetKeyDown(playerSettings.keyBinds["fireModeKey"]) && isReloading == false) //key for CycleFireTiggle. Default 't'
            ToggleFire();
	}

	//sets ammo text for UI
	private void SetText()
	{
 		magAmmoText.text = currentMagCount.ToString();
		ammoText.text = currentAmmoCount.ToString();
	}

	//reloads weapon
	private void Reload()
	{
		float runSpeed = playerController.playerSpeeds.runSpeed;
		float currentSpeed = playerController.playerSpeeds.currentPlayerSpeed;

		if (Input.GetKeyDown(playerSettings.keyBinds["reloadKey"]) && currentMagCount < magSize+1 
			&& currentAmmoCount != 0 && isReloading == false)
		{
			CharacterController controller = playerController.controller;
			if (currentSpeed != runSpeed || controller.velocity == Vector3.zero)
				reloadingCoroutine = StartCoroutine(ReloadingIE());
		}

		if (isReloading == true && currentSpeed == runSpeed) {
			StopCoroutine (reloadingCoroutine);
            print("reloading canceled");
			isReloading = false;
		}
	}

	IEnumerator ReloadingIE()
	{
		isReloading = true;
		//print ("isReloading: " + isReloading);
		print ("reloading...");

		yield return new WaitForSeconds(weaponInfo.reloadTime); 

		currentAmmoCount += currentMagCount;

		if (currentAmmoCount <= magSize) {
			currentMagCount = currentAmmoCount;
			currentAmmoCount = 0;
		} else {
			if (currentMagCount == 0) {
				currentMagCount = magSize;
				currentAmmoCount -= magSize;
			} else {
				currentMagCount = magSize + 1;
				currentAmmoCount -= (magSize + 1);
			}
		}
		isMagEmpty = false;
		isReloading = false;

		//print ("isReloading: " + isReloading);
		print ("done");
	}

	//toggles fire between semi-, fully-, and 3-round burst
	private void ToggleFire()
	{
		playerFireToggle++;
		if (playerFireToggle > FireToggle.burst)
			playerFireToggle = FireToggle.single;

		print ("fireToggle: " + playerFireToggle);
	}

	private void CreateClone()
	{
		if (i > 2)
			i = 0;
		//print ("i: " +i);

		//bullets
		GameObject clone = Instantiate(projectilePrefab, barrelEnd.position, 
			barrelEnd.rotation) as GameObject; //'*' combines Quaternions
		//clone.transform.Rotate(90,0,0);
		clone.GetComponent<Rigidbody>().velocity = cameraTR.TransformDirection
			(Vector3.forward * weaponInfo.projectileVelocity);

		//bullet casings
		GameObject casing = Instantiate (casingPrefab, ejectionPort.position, 
			ejectionPort.rotation) as GameObject;

		Rigidbody casingRB = casing.GetComponent<Rigidbody> ();

		casingRB.velocity = cameraTR.TransformDirection 
			(Vector3.right * casingVelocity);

		Vector3 spot = new Vector3 (0, -0.002f, rotateSpot[i]);
		i++;

		casingRB.AddForceAtPosition(Vector3.right * 3.5f, 
			casing.transform.position + spot);
		Destroy (casing, 15);

		//----------------
		StartCoroutine(RecoileIE());
		transform.parent.parent.SendMessage("lookRecoil", weaponInfo.recoilRotationEffect);
		//------------------
	}

	//recoiles the gun back 
	private IEnumerator RecoileIE()
	{
		transform.Translate(weaponInfo.recoilPosEffect);
		yield return new WaitForSeconds (0.01f);
		transform.Translate(-weaponInfo.recoilPosEffect);
	}

	private void DecreaseAmmo()
	{
		currentMagCount = currentMagCount - 1;

		if (currentMagCount == 0)
			isMagEmpty = true;
	}
		
	//Fires a single projectile every specified interval
	private void FireSingleShot()
	{
		if (Time.time >= (tempRate + lastShot)) 
		{
			if (tempRate != weaponInfo.fireDelay)
				tempRate = weaponInfo.fireDelay;

            CreateClone();
			lastShot = Time.time;

            DecreaseAmmo();
		}
	}

	//Fires specified amount of projectiles per second
	private void FireFullAutoShot()
	{
		if ((Time.time - lastShot) > (1 / weaponInfo.fireRate)) {
            CreateClone();
			lastShot = Time.time;

            DecreaseAmmo();
		}
	}

	//Fires a burst every specified interval
	private void FireBurstShot()
	{
		if (Time.time >= (tempRate + lastShot)) {
			if (tempRate != weaponInfo.fireDelay)
				tempRate = weaponInfo.fireDelay;

			StartCoroutine (BurstIE());
			lastShot = Time.time;
		}
	}

	//creates a projectile every 0.05 seconds
	private IEnumerator BurstIE()
	{
		for (int i=0; i<3; i++) {
            CreateClone();
            DecreaseAmmo();
			if (currentMagCount == 0)
				yield break;
			yield return new WaitForSeconds(0.05f);
		}
	}
}