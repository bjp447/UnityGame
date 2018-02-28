using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeProperties : MonoBehaviour
{
	public float grenadeDamage = 40f;
	public float timeToExplode = 5f;

    public float force = 100f;
    public float upwardModifier = 0.0f;

    //called by ThrowGrenade() in Player_controller 
    private IEnumerator Explode()
	{
		print ("explode called...");
		yield return new WaitForSeconds(timeToExplode);
		print ("...exploded");
        //play audio
        //play effects
        Destroy(this.gameObject);
        ExplosionDamage(transform.position, 4f);
    }

    private void ExplosionDamage(Vector3 center, float radius)
	{
        //Debug.Break();
        foreach (Collider col in Physics.OverlapSphere(center, radius)) {
            if (col.GetComponent<Rigidbody>() != null) {
                RaycastHit hit;
                if (Physics.Raycast(center, col.transform.position-center, out hit, Mathf.Infinity)) {
                   // Debug.DrawRay(center, col.transform.position - center, Color.green);
                    if (hit.collider == col) {
                        if ((col.CompareTag("enemy") || col.CompareTag("Player"))) {
                            print("collider's object name: " + col);
                            col.SendMessage("AddDamage", grenadeDamage, SendMessageOptions.DontRequireReceiver);
                        }
                        //print(col.name);
                        col.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius, upwardModifier, ForceMode.Impulse);
                    }
                }
            }
        }
    }

	private void OnDrawGizmos() 
	{
		Gizmos.color = Color.red;
		//Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
		Gizmos.DrawWireSphere (transform.position, 4f);
	}
}
