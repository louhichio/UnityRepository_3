
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
		public static event emptyDel startTurn_Player;
		public static event emptyDel pause;
		public static event emptyDel resume;

		public delegate void stringPara(string status);
		public static event stringPara gameOver;

		public delegate void tilePara(Tile tile);
		public static event tilePara playerChangedTile;


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

		public void PlChangedTile(Tile tile)
		{
			if(playerChangedTile != null)
				playerChangedTile(tile);
		}	

		public void StartTurn_Player()
		{
			if(startTurn_Player != null)
				startTurn_Player();
		}

		public void Pause()
		{
			if(pause != null)
				pause();
		}
		public void Resume()
		{
			if(resume != null)
				resume();
		}
	}
}