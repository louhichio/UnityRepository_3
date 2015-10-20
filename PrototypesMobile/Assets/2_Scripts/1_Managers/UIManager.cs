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
		private GameObject panel_EnemyInfo;
		[SerializeField]
		private Text text_TurnStatus;

		[SerializeField]
		private GameObject panel_GameInfo;
		[SerializeField]
		private Text text_StepsLeft;
		[SerializeField]
		private Text text_Collectables;

		[SerializeField]
		private GameObject panel_GameInfoWorld;
		[SerializeField]
		private GameObject prefab_Image_Exclamation;
		[SerializeField]
		private float img_excla_heightOffset = 2.0f; 
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
			panel_GameInfo.SetActive(true);
		}

		private void GameOver(string status)
		{
			panel_PlayerInfo.SetActive(false);
			panel_EnemyInfo.SetActive(false);
			panel_Gameover.SetActive(true);
			panel_GameInfo.SetActive(true);
			
			if(text_Gameover)
			{
				switch(status)
				{
				case "WON":
					text_Gameover.text = "VICTOIRE";
					break;
				case "LOST":
					text_Gameover.text = "POLICE";
					break;
				}
			}
			text_Gameover.enabled = true;
			StartTurn_Player();
		}

		private void GameReset()
		{
			panel_Gameover.SetActive(false);
			panel_PlayerInfo.SetActive(false);
			panel_EnemyInfo.SetActive(false);
			panel_GameInfo.SetActive(true);
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

			text_StepsLeft.text = "STEPS: " + Player.Instance.turnSteps + "/" + Player.Instance.step_Max ;
			text_Collectables.text = "BONUS: " + CollectManager.Instance.collected + "/" + CollectManager.Instance.collectables_Count;
		}
		#endregion

		#region Private

		#endregion

		#region Public
		public void UpdatePlayerInfo(int maxSteps, int currentSteps)
		{			
			text_StepsLeft.text = "StepsLeft: " + currentSteps + "/" + maxSteps ;
		}

		public void UpdatePlayerInfoCollectables(int collected, int collectables_Count)
		{			
			text_Collectables.text = "Collectables: " + collected + "/" + collectables_Count;
		}

		public void GenerateUnitExclamationMark(ref GameObject go)
		{
			go = Instantiate(prefab_Image_Exclamation) as GameObject;
			go.transform.SetParent(panel_GameInfoWorld.transform, false);
		}

		public void UpdateExclamationMark(GameObject go, Transform Unit, bool active)
		{			
			go.SetActive(active);

			if(!active)
				return;

			Vector3 worldPos = Unit.position + (Vector3.up * img_excla_heightOffset);
			go.transform.position = worldPos;
			go.transform.LookAt(Camera.main.transform.position);
		}
		#endregion
	}
}
