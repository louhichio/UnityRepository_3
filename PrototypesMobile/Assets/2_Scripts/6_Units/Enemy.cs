namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public enum EnemyType
	{
		Guard,
		Dog,
		Cam
	}

	public abstract class Enemy : Unit 
	{
		#region Properties
		[HideInInspector]
		public abstract EnemyType enemy_Type{get;}

		public FOV fov;

		public Waypoint waypoint;

		public Tile target;

		public GameObject psDetect;
		#endregion

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.startTurn_Enemy += StartTurn;
			EventManager.gameReset += Reset;
			EventManager.playerChangedTile += PlayerChangedTile;
		}		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.startTurn_Enemy -= StartTurn;
			EventManager.gameReset -= Reset;
			EventManager.playerChangedTile -= PlayerChangedTile;
		}
		
		public void Init()
		{
			target = null;
			psDetect.SetActive(false);

			Initialize(MapManager.Instance.InitializeUnit(transform.position, gameObject));

			tile_current.AddUnit(this);

			waypoint.Initialise();

			if(waypoint.type != Waypoint.Type.None)
				TurnManager.Instance.enemyCount_Max++;

			fov.Initialize(tile_current, (int)transform.eulerAngles.y);


			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.EnemyOn);
		}

		public void StartTurn()
		{			
			if(!Tile.ReferenceEquals(target, null))
			{
				TravelTo(target);	
			}
			else if(waypoint.type != Waypoint.Type.None)
			{
				if(!List<Tile>.ReferenceEquals(waypoint, null) && waypoint.isPathDefined)
				{
					TravelTo(waypoint.GetNextWayPoint(tile_current));
				}
				else
					print ("Waypoint path isn't defined");
			}
			else
				TravelTo(tile_init);
		}
		public void Reset()
		{			
			target = null;
			psDetect.SetActive(false);

			tile_current.RemoveUnit(this);

			path.Clear();
			waypoints.Clear();

			transform.position = position_Init;
			transform.eulerAngles = new Vector3(0, rotation_init, 0);

			tile_current = tile_init;			
			tile_current.AddUnit(this);
			fov.EnableFov(tile_current, (int)transform.eulerAngles.y);

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.EnemyOn);

			moveState = MoveState.None;
		}		

		public void PlayerChangedTile(Tile tile)
		{
			if(Tile.ReferenceEquals(target, null) || target != Player.Instance.tile_current)
			{
				target = Player.Instance.tile_current;
				
				psDetect.transform.position = target.transform.position;
				psDetect.SetActive(true);
			}
		}
		#endregion


		public override void TravelFinished()
		{
			SetUnitNeighboursTilesState(TileState.Clear);
			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.EnemyOn);
			SetFov(true, tile_current);	
			
			tile_current.AddUnit(this);

			if(tile_current == target)
			{
				target = null;
				psDetect.SetActive(false);
			}

			if(tile_current == Player.Instance.tile_current)
			{
				Stop();
				GameManager.Instance.StartCoroutine("PlayerLost");			
			}
			else
			{
				TurnManager.Instance.StopCoroutine("EnemyMoved");
				TurnManager.Instance.StartCoroutine("EnemyMoved");
			}
		}

		public override bool Check()
		{				
			if(tile_current == Player.Instance.tile_current)
			{
				Stop();
				GameManager.Instance.StartCoroutine("PlayerLost");	
				return true;
			}
			if(fov.isPlayerDetected())
			{
				if(Tile.ReferenceEquals(target, null) || target != Player.Instance.tile_current)
				{
					target = Player.Instance.tile_current;
					List<Tile> path = AStar.FindPath(tile_current, Player.Instance.tile_current);
					this.path = path;
					waypoints.Clear();
					waypoints = GetWaypointsFromPath(path);

					psDetect.transform.position = target.transform.position;
					psDetect.SetActive(true);
					
					Vector3 direction = waypoints[0] - transform.position;
					direction.y =0;						
					if(direction.normalized!= Vector3.zero)
						transform.forward = direction.normalized * 90;	
					SetFov(false, tile_current);					
					SetFov(true, path[1]);		
					return true;
				}
			}
			return false;
		}

		public override void SetFov(bool enable , Tile tile)
		{			
			if(enable)
				fov.EnableFov(tile, (int)transform.eulerAngles.y);
			else
			{
				fov.DisableFov();
			}
		}
	}
}
