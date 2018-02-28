using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimber : MonoBehaviour 
{
	public Transform handTr;
	public Animator animator;
	public Rigidbody rigid;
	public PlayerControllerFirst TPUC;
	//public Fir

	public float climbForce;

	public Climbingsort currentSort;

	private Vector3 targetPoint;
	private Vector3 targetNormal;

	private float lastTime;
	private float beginDistance;

	/*
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	*/

	public void MoveTowardsPoint()
	{
		transform.position = Vector3.Lerp(transform.position,
			(targetPoint - transform.rotation* handTr.localPosition), Time.deltaTime*climbForce);

		Quaternion lookRotation = Quaternion.LookRotation (-targetNormal);

		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * climbForce);

		animator.SetBool ("OnGround", false);
		float distance = Vector3.Distance (transform.position, 
			                 (targetPoint - transform.rotation * handTr.localPosition));
		float percent = -9 * (beginDistance - distance) / beginDistance;
		animator.SetFloat ("Jump", percent);

		if (distance <= 0.01f && currentSort == Climbingsort.ClimbingTowardsPoint) 
		{
			transform.position = targetPoint - transform.rotation * handTr.localPosition;
			transform.rotation = lookRotation;

			lastTime = Time.time;
			currentSort = Climbingsort.Climbing;
		}

		if (distance <= 0.01f && currentSort == Climbingsort.ClimbingTowardsPlateau) 
		{
			transform.position = targetPoint - transform.rotation * handTr.localPosition;
			transform.rotation = lookRotation;

			lastTime = Time.time;
			currentSort = Climbingsort.Walking;

			rigid.isKinematic = false;
			TPUC.enabled = true;
		}
	}
}

[System.Serializable]
public enum Climbingsort
{
	Walking, 
	Junping,
	Falling,
	Climbing,
	ClimbingTowardsPoint,
	ClimbingTowardsPlateau,
	checkingForClimbStart
}