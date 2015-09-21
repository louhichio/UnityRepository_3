
namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class EventManager: Singleton<EventManager>
	{		
		public delegate void Init();
		public static event Init initialise;

		public delegate void StartTurn();
		public static event StartTurn startTurn_Enemy;

		public delegate void Reset();
		public static event Reset reset_game;

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

		public void Reset_game()
		{
			if(reset_game != null)
				reset_game();
		}
	}
}