namespace TheVandals
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityStandardAssets.ImageEffects;
	
	public class UIManager: Singleton<UIManager>
	{		
		#region Properties
		[Header("Configuration")]
		[Header("Panel GameOver")]
		[SerializeField]
		private GameObject panel_Busted;
		[SerializeField]
		private GameObject panel_EndScreen;
		[SerializeField]
		private Text text_CaptureCount;
		[SerializeField]
		private Text text_BustedCount;


		[Header("Panel PlayerInfo")]
		[SerializeField]
		private GameObject panel_PlayerInfo;

		[Header("Panel EnemyInfo")]
		[SerializeField]
		private GameObject panel_EnemyInfo;
		[SerializeField]
		private Text text_TurnStatus;

		[Header("Panel GameInfo")]
		[SerializeField]
		private GameObject panel_GameInfo;
		[SerializeField]
		private Text text_StepsLeft;
		[SerializeField]
		private Text text_Collectables;

		[Header("Panel GameInfoWorld")]
		[SerializeField]
		private GameObject panel_GameInfoWorld;
		[SerializeField]
		private GameObject prefab_Image_Exclamation;
		[SerializeField]
		private float img_excla_heightOffset = 2.0f; 

		[Header("Panel Paintings")]
		[SerializeField]
		private GameObject panel_PaintingInfo;
		[SerializeField]
		private Image img_Painting;

		[Header("Effects")]
		[SerializeField]
		private ScreenOverlay screenOverlay;
		[SerializeField]
		private float opacity_max = 1;
		[SerializeField]
		private Image image_PlayerStatus;
		[SerializeField]
		private Sprite[] vignetting;

		private bool isCapturing = false;
		public bool isPaused = false;
		#endregion

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameOver += GameOver;
			EventManager.gameReset += GameReset;
			EventManager.startTurn_Enemy += StartTurn_Enemy;
			EventManager.startTurn_Player += StartTurn_Player;
			EventManager.pause += Pause;
			EventManager.resume += Resume;
		}		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameOver -= GameOver;
			EventManager.gameReset -= GameReset;
			EventManager.startTurn_Enemy -= StartTurn_Enemy;
			EventManager.startTurn_Player-= StartTurn_Player;
			EventManager.pause -= Pause;
			EventManager.resume -= Resume;
		}

		public void Init()
		{			
			panel_Busted.SetActive(false);
			panel_EndScreen.SetActive(false);
			panel_PlayerInfo.SetActive(false);
			panel_EnemyInfo.SetActive(false);
			panel_PaintingInfo.SetActive(false);
			panel_GameInfo.SetActive(true);
			
			image_PlayerStatus.enabled = false;
			SetPlayerStatusAlpha(0);
		}

		private void GameOver(string status)
		{
			panel_PlayerInfo.SetActive(false);
			panel_EnemyInfo.SetActive(false);
			switch(status)
			{
			case "WON":	
				panel_EndScreen.SetActive(true);
				text_CaptureCount.text = CollectManager.Instance.collected + "/" + CollectManager.Instance.collectables_Count;
				text_BustedCount.text = Player.Instance.bustedCount.ToString();
				break;
			case "LOST":
				Player.Instance.bustedCount++;
				panel_Busted.SetActive(true);
				break;
			}
			StartTurn_Player();
		}

		private void GameReset()
		{
			panel_Busted.SetActive(false);
			panel_EndScreen.SetActive(false);
			panel_PlayerInfo.SetActive(false);
			panel_EnemyInfo.SetActive(false);
			panel_PaintingInfo.SetActive(false);

			StopAllCoroutines();
			image_PlayerStatus.enabled = false;
			SetPlayerStatusAlpha(0);
		}

		private void StartTurn_Enemy()
		{
			if(!isCapturing)
			{
				panel_PlayerInfo.SetActive(false);
				panel_EnemyInfo.SetActive(true);
			}
		}

		private void StartTurn_Player()
		{
			panel_EnemyInfo.SetActive(false);
			panel_PlayerInfo.SetActive(true);

			text_StepsLeft.text = "" + Player.Instance.turnSteps + "/" + Player.Instance.step_Max ;
			text_Collectables.text = CollectManager.Instance.collected + "/" + CollectManager.Instance.collectables_Count;
		}

		public void Pause()
		{
			isPaused = true;
		}
		
		public void Resume()
		{
			isPaused = false;
		}
		#endregion

		#region Private

		#endregion

		#region Public
		public void UpdatePlayerInfo(int maxSteps, int currentSteps)
		{			
			text_StepsLeft.text = "" + currentSteps + "/" + maxSteps ;
		}

		public void UpdatePlayerInfoCollectables(int collected, int collectables_Count)
		{			
			text_Collectables.text = "" + collected + "/" + collectables_Count;
		}

		public IEnumerator StartCaptureOeuvre(Sprite painting)
		{			
			isCapturing = true;
			float opacity = 0.1f;
			while(opacity <= opacity_max)
			{
				screenOverlay.intensity = opacity;
				opacity += Time.deltaTime * 10.0f;
				yield return null;
			}		

			panel_GameInfo.SetActive(false);		
			panel_EnemyInfo.SetActive(false);
			panel_PaintingInfo.SetActive(true);	
			img_Painting.sprite = painting;

			opacity = 2.0f;
			while(opacity >= 0.0f)
			{
				screenOverlay.intensity = opacity;
				opacity -= Time.deltaTime * 3.0f;
				yield return null;
			}
			screenOverlay.intensity = 0.0f;
		}
		
		public void EndCapture()
		{
			isCapturing = false;
//			Time.timeScale = 1;

			panel_PaintingInfo.SetActive(false);
			panel_EnemyInfo.SetActive(true);
			panel_GameInfo.SetActive(true);	
			EventManager.Instance.Resume();
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

		public IEnumerator FadeInPlayerStatus(int index)
		{			
			image_PlayerStatus.sprite = vignetting[index];
			image_PlayerStatus.enabled = true;

			Color c = image_PlayerStatus.color;
			float x = image_PlayerStatus.color.a;		
			
			while(x < 1)
			{ 			
				c.a = x;
				image_PlayerStatus.color = c;
				x += Time.deltaTime * 4;
				yield return null;
			}				
			c.a = 1;
			image_PlayerStatus.color = c;			
		}

		public IEnumerator FadeOutPlayerStatus()
		{			
			if(image_PlayerStatus.enabled)
			{
				Color c = image_PlayerStatus.color;
				float x = image_PlayerStatus.color.a;

				while(x > 0)
				{ 				
					c.a = x;
					image_PlayerStatus.color = c;
					x -= Time.deltaTime * 4;
					yield return null;
				}	

				c.a = 0;
				image_PlayerStatus.color = c;		
				image_PlayerStatus.enabled = false;
			}
		}

		public void SetPlayerStatusAlpha(float alpha)
		{
			Color c = image_PlayerStatus.color;
			c.a = alpha;
			image_PlayerStatus.color = c;
		}
	}
}
