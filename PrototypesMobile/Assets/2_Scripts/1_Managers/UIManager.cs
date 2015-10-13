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

		[SerializeField]
		private GameObject panel_PlayerInfo;
		[SerializeField]
		private Text text_StepsLeft;

		[SerializeField]
		private GameObject panel_EnemyInfo;
		[SerializeField]
		private Text text_TurnStatus;
		#endregion

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameOver += GameOver;
			EventManager.gameReset += GameReset;
			EventManager.startTurn_Enemy += StartTurn_Enemy;
			EventManager.startTurn_Player += StartTurn_Player;
		}		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameOver -= GameOver;
			EventManager.gameReset -= GameReset;
			EventManager.startTurn_Enemy -= StartTurn_Enemy;
			EventManager.startTurn_Player-= StartTurn_Player;
		}

		public void Init()
		{			
			panel_Gameover.SetActive(false);
			panel_PlayerInfo.SetActive(false);
			panel_EnemyInfo.SetActive(false);
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
			StartTurn_Player();
		}

		private void GameReset()
		{
			panel_Gameover.SetActive(false);
		}

		private void StartTurn_Enemy()
		{
			panel_PlayerInfo.SetActive(false);
			panel_EnemyInfo.SetActive(true);
		}

		private void StartTurn_Player()
		{
			panel_EnemyInfo.SetActive(false);
			panel_PlayerInfo.SetActive(true);
			text_StepsLeft.text = "StepsLeft: " + Player.Instance.turnSteps + "/" + Player.Instance.step_Max ;
		}
		#endregion

		#region Private

		#endregion

		#region Public
		public void UpdatePlayerInfo(int maxSteps, int currentSteps)
		{			
			text_StepsLeft.text = "StepsLeft: " + currentSteps + "/" + maxSteps ;
		}
		#endregion
	}
}
