using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Properties of the weapon, changable in the inspector
public class WeaponProperties : MonoBehaviour 
{
    //fire toggle
    public int amountOfFireTypes = 3;
    public enum FireToggle { single, full, burst };
    public FireToggle currentFireToggle;

    public GameObject projectilePrefab;
    public GameObject casingPrefab;
    public float bulletVelocity = 50f;
    public Vector3 casingDirection;
    public float casingVelocity = 7.5f;

    public Quaternion minBulletSpreadRotation;
    public Quaternion maxBulletSpreadRotation;

    public Sprite weaponSprite;

	public int reticleSize = 8;
	public Sprite reticleTexture;

	public Animator reloadAnim;
	public float reloadTime;

	//public Animator recoilAnim;
	public Vector3 recoilPosEffect;
	public Vector3 recoilRotationEffect;

	public string weaponName = "";
	public string weaponClass = "";

	public float fireDelay; //delay between each shot
	public float fireRate; //bullets per second

	//scope sight stuff
	public Transform scopePrefabTR;
	public Transform scopeLocationTR;
	public float scopeZoomMultiplier = 1f;

    public float timeToADS = 1f; //in seconds

    private Transform instantiatedScopeTR;

    //projectile exits
    public Transform barrelEnd; //spot where bullets spawn
    public Transform ejectionPort; //spot where casing comes out

    // Use this for initialization
    void Start () 
	{
		// weapon will have Iron Sights by default
		//if (scopeTR != null) 
		//{
			//remove default iron sights
			//3Destroy(scopeLocationTR.GetChild(0).gameObject);
			//add new sight
		instantiatedScopeTR = Instantiate (scopePrefabTR, scopeLocationTR);

        if (scopePrefabTR != null)
		    scopeZoomMultiplier = scopePrefabTR.GetComponent<WeaponScopeProperty>().zoomMultiplier;
		//}
	}

	private void OnDestroy()
	{
		Destroy (instantiatedScopeTR.gameObject);
		//print("Script was destroyed");
	}
}