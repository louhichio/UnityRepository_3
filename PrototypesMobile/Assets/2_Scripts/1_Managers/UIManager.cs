namespace TheVandals
{
	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	
	public class UIManager: Singleton<UIManager>
	{		
		#region Properties
		[SerializeField]
		private GameObject panel_Gameover;
		[SerializeField]
		private Text text_Gameover;
		#endregion

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameOver += GameOver;
			EventManager.gameReset += GameReset;
		}		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameOver -= GameOver;
			EventManager.gameReset -= GameReset;
		}

		public void Init()
		{			
			panel_Gameover.SetActive(false);
		}

		private void GameOver(string status)
		{
			panel_Gameover.SetActive(true);
			
			if(text_Gameover)
			{
				switch(status)
				{
				case "WON":
					text_Gameover.text = "CONGRATULATIONS YOU WON";
					break;
				case "LOST":
					text_Gameover.text = "GAME OVER";
					break;
				}
			}
			text_Gameover.enabled = true;
		}

		private void GameReset()
		{
			panel_Gameover.SetActive(false);
		}
		#endregion

		#region Private

		#endregion

		#region Public

		#endregion
	}
}
