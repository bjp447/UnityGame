using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_controller : MonoBehaviour 
{
	[SerializeField] private Button continueBtn, startNewBtn, lvlSelectBtn;

	[SerializeField] private Button lvl1Select, lvl2Select; 

	// Use this for initialization
	void Start () 
	{
		Global_controller globalController = GameObject.Find("Global_controller").GetComponent<Global_controller>();
		continueBtn.onClick.AddListener(globalController.continueGameClicked);
		startNewBtn.onClick.AddListener(globalController.startNewGameClicked);
		lvlSelectBtn.onClick.AddListener(globalController.levelSelectClicked);

		lvl1Select.onClick.AddListener(
			delegate { globalController.loadLevelOnClick(1); }
		);
		lvl2Select.onClick.AddListener(
			delegate { globalController.loadLevelOnClick(2); } );
	}
}
