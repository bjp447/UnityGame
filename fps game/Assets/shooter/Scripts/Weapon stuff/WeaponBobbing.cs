using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBobbing : MonoBehaviour
{
    private bool reachedTop = false;
    private Transform currentWeaponTR;
    public bool isMoving = false;

    private Vector3 startBob;//new Vector3(playerCameraTR.localPosition.x, playerCameraTR.localPosition.y, playerCameraTR.localPosition.z);
    Vector3 topBob;
    Vector3 bottomBob;

    private void Start()
    {
        currentWeaponTR = this.transform;

        startBob = new Vector3(currentWeaponTR.localPosition.x, currentWeaponTR.localPosition.y, currentWeaponTR.localPosition.z);
        topBob = new Vector3(startBob.x, startBob.y + 0.5f, startBob.z);
        bottomBob = new Vector3(startBob.x, startBob.y - 0.5f, startBob.z);
    }

    private void Update()
    {
        if (isMoving == true)
        {
            float step = 1f * Time.deltaTime;
            //print("weaponPos: " + (currentWeaponTR.localPosition.y));
            //print("TobBob: " + (topBob.y));
            //print("BottonBob: " + (bottomBob.y));
            print("startBob: " + startBob.y);

            if (currentWeaponTR.localPosition.y >= topBob.y)//((currentWeaponTR.localPosition.y) >= (topBob.y) <= currentWeaponTR.localPosition.y - 0.01)
            {
                reachedTop = true;
                //print("reachedTop: " + reachedTop);
            }
            else if (currentWeaponTR.localPosition.y <= bottomBob.y)//((currentWeaponTR.localPosition.y) <= (bottomBob.y) >= currentWeaponTR.localPosition.y-0.01)
            {
                reachedTop = false;
            }

            if (reachedTop == true)
            {
                currentWeaponTR.localPosition = Vector3.MoveTowards(
                        currentWeaponTR.localPosition, bottomBob, step);
                print("bobbing down");
                //wB.BobDown(bottomBob, currentWeaponTR);
            }
            else if (reachedTop == false)
            {
                currentWeaponTR.localPosition = Vector3.MoveTowards(
                        currentWeaponTR.localPosition, topBob, step);
                print("bobbing up...");
                //wB.BobUp(topBob, currentWeaponTR);
            }
        }
        else if (isMoving == false && currentWeaponTR.localPosition.y != startBob.y)
            currentWeaponTR.localPosition = Vector3.MoveTowards(
                     currentWeaponTR.localPosition, startBob, 1f * Time.deltaTime);
    }


/*
    public void BobUp(Vector3 topBob, Transform currentWeaponTR)
    {
        //print("bobing up...");
        currentWeaponTR.localPosition = Vector3.MoveTowards(currentWeaponTR.localPosition, topBob, 0.1f);
    }
    
    public void BobDown(Vector3 bottomBob, Transform currentWeaponTR)
    {
        //Vector3 bottomBob = new Vector3(startBob.x, startBob.y-1.0f, startBob.z);
        //print("bobing down...");
        currentWeaponTR.localPosition = Vector3.MoveTowards(currentWeaponTR.localPosition, bottomBob, 0.1f);
    }
  */  
}
