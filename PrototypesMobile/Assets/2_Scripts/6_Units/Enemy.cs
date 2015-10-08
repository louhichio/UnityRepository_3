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
		Cam
	}
	public enum Waypoint
	{

	}

	public abstract class Enemy : Unit 
	{
		#region Properties
		public EnemyBehaviour enemy_Behaviour;
		[HideInInspector]
		public EnemyType enemy_Type;
		[HideInInspector]
		public FOV fov;

		public List<Tile> wayPoints;
		#endregion

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
		
		public abstract void Init();

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
			tile_current.RemoveUnit(this);

			path.Clear();
			waypoints.Clear();

			transform.position = position_Init;
			
			tile_current = tile_init;			
			tile_current.AddUnit(this);
			fov.EnableFov(tile_current);

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.EnemyOn);

			moveState = MoveState.None;
		}		
		#endregion

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

			fov.DisableFov();
			SetUnitNeighboursTilesState(TileState.Clear);

			tile_current.RemoveUnit(this);
			
			this.path = path;
			waypoints = GetWaypointsFromPath(path);
		}
		public override void TravelFinished()
		{
			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.EnemyOn);
			
			tile_current.AddUnit(this);	

			fov.EnableFov(tile_current);

			if(!Check())
			{
				TurnManager.Instance.StopCoroutine("EnemyMoved");
				TurnManager.Instance.StartCoroutine("EnemyMoved");
			}
		}
		public override bool Check()
		{
			if(fov.isPlayerDetected())
			{
				Stop();
				GameManager.Instance.StartCoroutine("PlayerLost");			
				return true;	
			}
			return false;
		}
		
	}
}
