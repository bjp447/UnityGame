using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDataOld : MonoBehaviour {

	public Text scoreText;
	public Text timeText;
	public int level;

	// Use this for initialization
	void Start () 
	{
		SetCountText ();
		setTimeText ();

		//timeText.text = "Best Time: " + Scoring.control.lvl1Time.ToString("F") + " seconds";
		//timeText.text = "Best Time: " + Scoring.control.lvl2Time.ToString ("F") + " seconds";
	}

	void setTimeText()
	{
		string lvlTime = "";

		if (level == 1) 
		{
			lvlTime = Scoring.control.lvl1Time.ToString ("F");
		}
		if (level == 2) 
		{
			lvlTime = Scoring.control.lvl2Time.ToString ("F");
		}

		timeText.text = "Best Time: " + lvlTime + " seconds";
	}

	void SetCountText()
	{
		int lvlScore = 0;

		if (level == 1)
		{
			lvlScore = Scoring.control.lvl1Score;
		}
		if (level == 2)
		{
			lvlScore = Scoring.control.lvl2Score;
		}

		scoreText.text = "Score: " + lvlScore;
		//scoreText.text = "Score: " + lvlScore.ToString ();
	}
}
