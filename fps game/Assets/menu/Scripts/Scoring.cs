using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//stores scoring for all levels to be displayed in the Level List menu panel
//works across scenes.
public class Scoring : MonoBehaviour {

	public static Scoring control;

	public int lvl1Score = 0;
	public int lvl2Score = 0;
	public int lvl3Score = 0;
	public int lvl4Score = 0;
	public int lvl5Score = 0;
	public int lvl6Score = 0;

	public double lvl1Time = 0.0;
	public double lvl2Time = 0.0;

	void Awake () 
	{
		if (control == null)//if control hasnt been asigned yet
		{
			DontDestroyOnLoad (gameObject);
			control = this; //this becomes the object referenced
		}
		else if (control != this) //if it exists, and this isnt it, destroy this
		{
			Destroy(gameObject);
		}
	}

	/*
	void OnGUI () 
	{
		//scoreText.text = "";
		//SetScoringText ();
		GUI.Label (new Rect(10, 10, 100, 30), "Score: " + aScore);
	}
*/
	/*
	void SetScoringText()
	{
		scoreText.text = "High Score: " + aScore.ToString ();
		//if (count >= 12) 
		//{
		//	scoreText.text = "You Win!";
		//	SceneManager.LoadScene (0);
		//}
	}
*/
}
