namespace TheVandals
{
	using System;
	using System.Collections;
	using UnityEngine;
	
	public class GameManager: Singleton<GameManager>
	{		

		void Start()
		{
			MapManager.Instance.Init();
			EventManager.Instance.Initialise();
		}
		void OnGUI()
		{
			if(GUI.Button(new Rect(0,0,100,100), "Reset"))
				EventManager.Instance.Reset_game();
		}
	}
}