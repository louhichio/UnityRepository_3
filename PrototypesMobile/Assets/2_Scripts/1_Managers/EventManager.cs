
namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class EventManager: Singleton<EventManager>
	{		
		public delegate void emptyDel();
		public static event emptyDel initialise;
		public static event emptyDel gameReset;
		public static event emptyDel startTurn_Enemy;

		public delegate void gameoverDel(string status);
		public static event gameoverDel gameOver;

		public void Initialise()
		{			
			if(initialise != null)
				initialise();
		}

		public void StartTurn_Enemy()
		{
			if(startTurn_Enemy != null)
				startTurn_Enemy();
		}

		public void GameReset()
		{
			if(gameReset != null)
				gameReset();
		}

		public void GameOver(string status)
		{
			if(gameOver != null)
				gameOver(status);
		}
	}
}