namespace TheVandals
{
	using System;
	using System.Collections;
	using UnityEngine;
	
	public class GameManager: Singleton<GameManager>
	{		

		void Start()
		{ 
			MapManager.Instance.Initialise();
			EventManager.Instance.Initialise();
		}

		public IEnumerator PlayerWon()
		{
			EventManager.Instance.GameOver("WON");

			yield return new WaitForSeconds(1.0f);
			
			MapManager.Instance.ResetTiles();
			EventManager.Instance.GameReset();
		}

		public IEnumerator PlayerLost()
		{
			EventManager.Instance.GameOver("LOST");

			yield return new WaitForSeconds(1.0f);
			
			MapManager.Instance.ResetTiles();
			EventManager.Instance.GameReset();
		}
	}
}