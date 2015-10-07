namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public enum EnemyBehaviour
	{
		Idle,
		Roam,
	}
	public enum EnemyType
	{
		Guard,
		Dog,
	}
	public abstract class Enemy : Unit 
	{
		public EnemyBehaviour enemy_Behaviour;
		[HideInInspector]
		public EnemyType enemy_Type;
		[HideInInspector]
		public FOV fov;

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
		
		public virtual void Init()
		{			
			Initialize(MapManager.Instance.InitializeUnit(transform.position, gameObject));

//			SetUnitNeighboursTilesState(TileState.EnemyDetect);
			tile_current.AddUnit(this);

			if(enemy_Behaviour != EnemyBehaviour.Idle)
				TurnManager.Instance.enemyCount_Max++;

			fov = GetComponent<FOV>();
			fov.Initialize(tile_current);
		}
		public void StartTurn()
		{
			if(enemy_Behaviour != EnemyBehaviour.Idle)
			{
				var list = tile_current.GetTilesWithinCost(step_Max);
				list.Remove(tile_current);
				TravelTo(list[Random.Range(0, list.Count - 1)]);
			}
		}
		public void Reset()
		{
			
			//			ResetUnitNeighboursTiles();
			tile_current.RemoveUnit(this);

			path.Clear();
			waypoints.Clear();

			transform.position = position_Init;
			
			tile_current = tile_init;			
			tile_current.AddUnit(this);
			fov.EnableFov(tile_current);

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
//			SetUnitNeighboursTilesState(TileState.EnemyDetect);

			moveState = MoveState.None;
		}

		public override void TravelTo(Tile destination)
		{
			if(Tile.ReferenceEquals(destination, null))
				return;
			
			List<Tile> path = AStar.FindPath(tile_current, destination);
			if (path.Count == 0)
			{
				path = null;
				return;
			}
//			SetUnitNeighboursTilesState(TileState.Clear);
			fov.DisableFov();
			tile_current.RemoveUnit(this);
			
			this.path = path;
			waypoints = GetWaypointsFromPath(path);
		}
		public override void TravelFinished()
		{
			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			//			SetUnitNeighboursTilesState(TileState.EnemyDetect);
			
			tile_current.AddUnit(this);	

			fov.EnableFov(tile_current);
			if(fov.isPlayerDetected())
				GameManager.Instance.StartCoroutine("PlayerLost");	
			else
			{
				TurnManager.Instance.StopCoroutine("EnemyMoved");
				TurnManager.Instance.StartCoroutine("EnemyMoved");
			}
		}

		#endregion
		
	}
}
