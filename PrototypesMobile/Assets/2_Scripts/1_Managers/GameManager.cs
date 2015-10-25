namespace TheVandals
{
	using System;
	using System.Collections;
	using UnityEngine;
	
	public class GameManager: Singleton<GameManager>
	{		
		public bool reset = false;

		void Start()
		{ 
			MapManager.Instance.Initialise();
			EventManager.Instance.Initialise();
			EventManager.Instance.StartTurn_Enemy();
		}

		public IEnumerator PlayerWon()
		{
			EventManager.Instance.GameOver("WON");

			while(!reset)
				yield return null;

			reset = false;

			MapManager.Instance.ResetTiles();
			EventManager.Instance.GameReset();
			EventManager.Instance.StartTurn_Enemy();
		}

		public IEnumerator PlayerLost()
		{
			EventManager.Instance.GameOver("LOST");

			yield return new WaitForSeconds(1.0f);
			
			reset = false;
			
			MapManager.Instance.ResetTiles();
			EventManager.Instance.GameReset();
			EventManager.Instance.StartTurn_Enemy();
		}
	}
}