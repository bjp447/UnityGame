using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_controller : MonoBehaviour 
{
    public GameObject weaponPrefab;

	private CharacterController charController;
    private NavMeshAgent navMeshAgent;
    private SphereCollider sphereColider;
    private Inventory_controller inventory;
    private Transform camTR;
    [SerializeField] private GameObject ragdoll;
	[SerializeField] private float enemyHealth = 100;


	private Vector3 moveDirection = Vector3.zero;
	[SerializeField] private float gravity = 20f;

    public float viewAngle;
    private bool playerInView = false;
    private bool activalyTargetingPlayer = false;
    private Vector3 lastPlayerSighting;

    void Start () 
	{
		charController = GetComponent<CharacterController>();
		Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), charController);

        navMeshAgent = GetComponent<NavMeshAgent>();
        sphereColider = GetComponent<SphereCollider>();
        camTR = transform.GetChild(0).GetComponent<Camera>().transform;

        lastPlayerSighting = Vector3.zero;
        inventory.weaponsInventory[0] = Instantiate(weaponPrefab);
    }

    private void Update()
    {
        /*
        if (playerInView == true)
        {
            transform.LookAt(lastPlayerSighting);
        }
        */
    }

    private IEnumerator W()
    {
        yield return new WaitForSeconds(0.5f);
        navMeshAgent.destination = lastPlayerSighting;
        activalyTargetingPlayer = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 directionToTarget = (other.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(camTR.position, directionToTarget, out hit, sphereColider.radius))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        Debug.DrawLine(camTR.position, hit.transform.position, Color.red);
                        //print("found Player");
                        //playerInView = true;
                        lastPlayerSighting = hit.transform.position;
                        transform.LookAt(lastPlayerSighting);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("player exited view");
            //playerInView = false;

            navMeshAgent.destination = lastPlayerSighting;
        }
    }


    #region (A.I. and movement)
    #endregion

    #region (Inventory)
    #endregion

    /*
    #region (Hit ditection)
    //when something enters the player's trigger Capsule Collider
    private void OnTriggerEnter(Collider hitObject)
	{
		if (hitObject.gameObject.CompareTag("Projectile")) {
			addDamage(hitObject);
		}
	}

	//do stuff if the character is hit by a bullet
	private void addDamage(Collider hitObject)
	{
		//if player gets hit, reduce the health and change the text
		if (hitObject.gameObject.CompareTag("Projectile")) {
			int bulletDamage = hitObject.gameObject.GetComponent<ProjectileProperties>().bulletDamage;
			enemyHealth -= bulletDamage;
			//enemyHealth.fillAmount -= ((float)bulletDamage/100);
		}

		if (enemyHealth <= 0) {
			onPlayerDeath();
		}
		//setText();
		//play effects
		//play audio
	}

	//called via SendMessage in explosive's property script
	private void addDamage(float explosionDamage)
	{
		//print("hit by explosion");
		enemyHealth -= explosionDamage;

		if (enemyHealth <= 0) {
			onPlayerDeath();
		}
		//setText();
	}

	private void onPlayerDeath()
	{
		GameObject playerRagdoll = Instantiate(ragdoll, transform.position, transform.rotation) as GameObject;
		dropInventory();
		Destroy(gameObject);
	}

	private void dropInventory()
	{
		
	}
	#endregion

	#region (Camera and look)
	#endregion
    */
}
