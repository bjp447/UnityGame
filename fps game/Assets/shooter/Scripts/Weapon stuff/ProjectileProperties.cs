using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Properties of a bullet
// will have several bullet prefabs with this script
// values based on the bullet prefabs
public class ProjectileProperties : MonoBehaviour 
{
	//should probably be in projectileProperties script
	public int bulletDamage = 25;
	public string ammoType = "";

    [SerializeField] private GameObject bulletHoleTexture;
    [SerializeField] private GameObject[] fleshHitEffects;
	//public GameObject bulletPrefab;
	//public Texture ammoSkin;

	//private float velocity = 10f;
	private float tr;

    [SerializeField] private LayerMask layerMask;

	void Awake()
	{
		//velocity = GameObject.FindGameObjectWithTag("Gun").GetComponent<WeaponProperties>().bulletVelocity;
	}
	// Use this for initialization
	void Start () 
	{
		transform.Rotate(0,90,90);

        //layerMask = 1 << 8;
	}

    /*
	//destroy the bullet when it hits an object
	void OnCollisionEnter(Collision collision)
	{
		print ("Collision with " + collision.gameObject.name);
		Destroy(this.gameObject);
	}
	*/

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.up, Color.red);
        RaycastHit hit;

        //if bullet hits something, that is not a Trigger
        if (Physics.Raycast(transform.position, transform.up, out hit, 1.5f, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.sharedMaterial != null)
            {
                string objectMateral = hit.collider.sharedMaterial.name;
                switch (objectMateral)
                {
                    case ("Character"):
                        DisplayBulletHole(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
                        break;
                    default:
                        DisplayBulletHole(hit, bulletHoleTexture);
                        break;
                }
            }
            else
            {
                DisplayBulletHole(hit, bulletHoleTexture);
            }

            Destroy(this.gameObject); //destroy this bullet
        }
    }

    //Display the bullet hole of specified type on hit object
    private void DisplayBulletHole(RaycastHit hit, GameObject prefab)
    {
        // print("hit: " + hit.transform.name);
        GameObject hole = Instantiate(bulletHoleTexture, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        hole.transform.SetParent(hit.transform);
        Destroy(hole, 20f); //destroy the buller hole after specified seconds
    }

    /*
    //destroy the bullet when it hits an object
    void OnTriggerEnter(Collider collision)
	//void OnCollisionEnter(Collision collision)
	{
		print ("Triigered with " + collision);
        Destroy(this.gameObject);
        if (collision.CompareTag("Player") == true || collision.CompareTag("enemy") == true)
        {
            //blood effect at point of impact

        }
        else
        {
            //bullet hole effect on on sentient objects
            print("created bullet hole");
           // Quaternion hitRotation = Quaternion.FromToRotation(Vector3.up, collision.norma);
            Instantiate(bulletHoleTexture, transform.position, Quaternion.identity, collision.transform);
        }
	}
    */



    /*
	void DestroyOnCharcaterHit()
	{
		Destroy (this.gameObject);
	}
	*/

    // Update is called once per frame
    //void Update () 
    //{
    /*
    Vector3 rayDir = transform.TransformDirection(Vector3.up) * 1;//transform.TransformDirection(Vector3.forward) * 2;
    Ray ray = new Ray(transform.position, rayDir); //at camera position
    Debug.DrawRay(transform.position, rayDir, Color.green);
    RaycastHit hitObject;

    if (Physics.Raycast(ray, out hitObject, 1f))
    {
        print("Object name: " + hitObject.transform.name);
    }
    */

    //transform.Rotate(Vector3.down * 100 * Time.deltaTime); //spins along x-axis
    //transform.Rotate(Vector3.forward * velocity * Time.deltaTime);

    //transform.Rotate(0, 0, velocity * Time.deltaTime);

    //transform.Rotate(0, velocity * Time.deltaTime, 0);
    //}
}
