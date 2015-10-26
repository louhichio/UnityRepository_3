namespace TheVandals
{
	using System;
	using System.Collections;
	using UnityEngine;
	
	public class GameManager: Singleton<GameManager>
	{		
		public bool reset = false;
		public float enemyTurnDelay = 2.0f;

		void Start()
		{ 
			MapManager.Instance.Initialise();
			EventManager.Instance.Initialise();
			StartCoroutine("Init");
		}

		public IEnumerator PlayerWon()
		{
			EventManager.Instance.GameOver("WON");

			while(!reset)
				yield return null;

			reset = false;

			MapManager.Instance.ResetTiles();
			EventManager.Instance.GameReset();
			
			yield return new WaitForSeconds(enemyTurnDelay);
			EventManager.Instance.StartTurn_Enemy();
		}

		public IEnumerator PlayerLost()
		{
			EventManager.Instance.GameOver("LOST");

			yield return new WaitForSeconds(1.0f);
			
			reset = false;
			
			MapManager.Instance.ResetTiles();
			EventManager.Instance.GameReset();
			
			yield return new WaitForSeconds(enemyTurnDelay);
			EventManager.Instance.StartTurn_Enemy();
		}
		public IEnumerator Init()
		{
			yield return new WaitForSeconds(enemyTurnDelay);
			EventManager.Instance.StartTurn_Enemy();
		}
	}
}