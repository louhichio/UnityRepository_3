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


		void Start()
		{
			turnState = TurnState.PlayerTurn;
		}

		public IEnumerator PlayerMoved()
		{			
			turnState = TurnState.EnemyTurn;
			yield return new WaitForSeconds(turnDelay);

			EventManager.Instance.StartTurn_Enemy();
		}

		public IEnumerator EnemyMoved()
		{
			enemyCount++;
			if(enemyCount == enemyCount_Max)
			{
				yield return new WaitForSeconds(0f);
				turnState = TurnState.PlayerTurn;
				enemyCount = 0;
			}
		}
	}
}
