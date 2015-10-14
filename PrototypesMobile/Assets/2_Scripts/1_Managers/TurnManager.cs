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
		[SerializeField]
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
			turnState = TurnState.EnemyTurn;
			yield return new WaitForSeconds(turnDelay);

			if(enemyCount_Max > 0)
			{
				EventManager.Instance.StartTurn_Enemy();
			}
			else
			{
				EventManager.Instance.StartTurn_Player();
				turnState = TurnState.PlayerTurn;
			}

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
