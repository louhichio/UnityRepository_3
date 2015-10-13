namespace TheVandals
{
	using System;
	using System.Collections;
	using UnityEngine;

	public enum TurnState
	{
		PlayerTurn,
		EnemyTurn
	}

	public class TurnManager : Singleton<TurnManager>
	{
		public TurnState turnState;
		private int enemyCount = 0;
		public int enemyCount_Max = 0;
		public float turnDelay = 0.5f;




		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameReset += Reset;
		}		
		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameReset -= Reset;
		}
		
		private void Init()
		{			
			turnState = TurnState.PlayerTurn;
		}
		private void Reset()
		{
			StopAllCoroutines();
			enemyCount = 0;
			turnState = TurnState.PlayerTurn;
		}
		#endregion

		public IEnumerator PlayerMoved()
		{		
			if(enemyCount_Max > 0)
			{
				turnState = TurnState.EnemyTurn;
				yield return new WaitForSeconds(turnDelay);

				EventManager.Instance.StartTurn_Enemy();
			}
			else
				yield return new WaitForSeconds(turnDelay);
		}

		public IEnumerator EnemyMoved()
		{
			enemyCount++;
			if(enemyCount == enemyCount_Max)
			{
				yield return new WaitForSeconds(0f);
				turnState = TurnState.PlayerTurn;
				enemyCount = 0;

				EventManager.Instance.StartTurn_Player();
			}
		}
	}
}
