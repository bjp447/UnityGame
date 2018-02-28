using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tempScript : MonoBehaviour
{
	public Transform cameraTr; //transform of player camera
	public Image weaponImage;
	public Image weaponReticleImage;

	//player WeaponsInventory
	private List<GameObject> weaponsInventory = new List<GameObject>(2);//size of two

	//current weapon equiped
	public GameObject currentWeapon; //the current weapon held by player
	private BallProjectile currentWeaponBP; //the BallProjectile of current weapon
	private Transform currentWeaponTR; //gets assigned after player has a gun

	private bool isOverGun = false; //is player over a weapon

	//pick up timer
	private float keyTimer = 0; // iterates from last frame
	public float keyHoldLength = 0.4f; //length player must hold to activate something

	private void changeWeapons()
	{
		if ((Input.GetKeyDown("l")) && (weaponsInventory.Count == 2)) 
		{
			//kickOutOfADS();
			setOldWeaponOnChange();

			if (currentWeapon == weaponsInventory[0]) 
				currentWeapon = weaponsInventory[1];
			else 
				currentWeapon = weaponsInventory[0];
			setNewWeaponOnChange ();
		}
	}

	private void setOldWeaponOnChange()
	{
		currentWeaponBP.enabled = false;
		currentWeaponTR.localPosition = transform.localPosition; // and some other stuff

	}

	private void setNewWeaponOnChange()
	{
		//gets current weapon's stuff
		currentWeaponTR = currentWeapon.transform; // sets variable to current weapon's transform
		currentWeaponBP = currentWeapon.GetComponent<BallProjectile>(); //sets to current BallProjectile
		WeaponProperties weaponProp = currentWeapon.GetComponent<WeaponProperties>();

		//activates stuff
		currentWeaponBP.enabled = true;

		//activates image of the weapon in the UI
		weaponImage.gameObject.transform.parent.gameObject.SetActive(true);
		weaponImage.sprite = weaponProp.weaponSprite;

		//activates reticle in the UI
		weaponReticleImage.sprite = weaponProp.reticleTexture;

		//sets weapon location
		currentWeaponTR.localPosition = cameraTr.localPosition;// + spawn; //set to shoulder
		currentWeaponTR.rotation = cameraTr.rotation; // sets to camera rotation
	}

	private void setWeaponOnDrop()
	{
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
		currentWeaponTR.SetParent(cameraTr);
		currentWeapon.GetComponent<Rigidbody>().isKinematic = true; //disable objects physics

		//disable collisions with the gun
		currentWeapon.GetComponent<SphereCollider>().enabled = false;
		Physics.IgnoreCollision(currentWeapon.GetComponent<BoxCollider>(), 
			GetComponent<CapsuleCollider>(), true);
	}



	private void pickUpOrDropWeapon(Collider hitObject)
	{
		if (hitObject.gameObject.CompareTag ("Gun")) 
		{
			if (currentWeapon != null)
				isOverGun = true;

			if ((currentWeapon == null) && (weaponsInventory.Count == 0)) 
			{
				currentWeapon = hitObject.gameObject;
				weaponsInventory.Add (currentWeapon);
				setNewWeaponOnChange (); //sets current stuff
				setWeaponOnPickUp (); //disables/enables stuff when on player
			} 
			else if (Input.GetKey ("e")) 
			{
				keyTimer += Time.deltaTime;

				if (keyTimer > keyHoldLength && Mathf.Abs(hitObject.attachedRigidbody.velocity.y) < 0.02f) 
				{
					keyTimer = 0;

					if (weaponsInventory.Count == 1)
					{
						setOldWeaponOnChange (); //sets old weapon to player's back

						//currentWeapon.SetActive (false);
						//adds new weapon to inventory
						currentWeapon = hitObject.gameObject;
						weaponsInventory.Add (currentWeapon);
					} 
					else if (weaponsInventory.Count == 2) 
					{
						setWeaponOnDrop (); // sets stuff for the weapon to be dropped

						//removes current (old) weapon from Inventory
						int weaponNumber = weaponsInventory.IndexOf (currentWeapon);
						weaponsInventory.RemoveAt (weaponNumber);

						//adds new weapon to inventory
						currentWeapon = hitObject.gameObject;
						weaponsInventory.Insert (weaponNumber, currentWeapon);
					}
					setNewWeaponOnChange (); //sets the new current weapon stuff
					setWeaponOnPickUp (); //disables/enables stuff when on player
				}
			}
			if (Input.GetKeyUp("e"))
				keyTimer = 0;
		} 
		else
			isOverGun = false;
	}
}