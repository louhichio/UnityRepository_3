namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public enum EnemyBehaviour
	{
		Idle,
		Roam,
	}

	public class Enemy : Unit 
	{
		private EnemyBehaviour enemy_Behaviour = EnemyBehaviour.Roam;

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.startTurn_Enemy += StartTurn;
			EventManager.gameReset += Reset;
		}		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.startTurn_Enemy -= StartTurn;
			EventManager.gameReset -= Reset;
		}
		
		private void Init()
		{			
			Initialize(MapManager.Instance.InitializeUnit(transform.position, gameObject));

			SetUnitNeighboursTilesState(TileState.EnemyOn);
			tile_current.AddUnit(this);

			if(enemy_Behaviour != EnemyBehaviour.Idle)
				TurnManager.Instance.enemyCount_Max++;
		}
		private void StartTurn()
		{
			if(enemy_Behaviour != EnemyBehaviour.Idle)
			{
				var list = tile_current.GetTilesWithinCost(step_Max);
				list.Remove(tile_current);
				TravelTo(list[Random.Range(0, list.Count - 1)]);
//				if(path != null && path.Count > 0 && waypoints != null && waypoints.Count > 0)
//					tile_current.RemoveUnit(this);
			}
		}
		private void Reset()
		{
			StopAllCoroutines();
			
			ResetUnitNeighboursTiles();
			tile_current.RemoveUnit(this);

			path.Clear();
			waypoints.Clear();

			transform.position = position_Init;
			
			tile_current = tile_init;			
			tile_current.AddUnit(this);

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.EnemyOn);

			moveState = MoveState.None;
		}

		public override void TravelFinished()
		{
			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.EnemyOn);

			tile_current.AddUnit(this);

			if(list_UnitNeighbours.Contains(Player.Instance.tile_current))
			{
				GameManager.Instance.StartCoroutine("PlayerLost");
				return;
			}				

			TurnManager.Instance.StopCoroutine("EnemyMoved");
			TurnManager.Instance.StartCoroutine("EnemyMoved");
		}
		#endregion

	}
}
