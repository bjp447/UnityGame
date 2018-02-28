using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustScript : MonoBehaviour {

	void OnGUI()
	{
		if (GUI.Button (new Rect (10, 100, 100, 30), "Score up"))
		{
			//Scoring is the script
			//control is the static member
			//aScore is the public reference to the object
			Scoring.control.lvl1Score += 10;
		}
		if (GUI.Button (new Rect (10, 140, 100, 30), "Score down"))
		{
			Scoring.control.lvl1Score -= 10;
		}
	}
}
