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
		
		public virtual void Init()
		{
			target = null;
			psDetect.SetActive(false);

			Initialize(MapManager.Instance.InitializeUnit(transform.position));

			tile_current.AddUnit(this);

			waypoint.Initialise();

			TurnManager.Instance.enemyCount_Max++;

			fov.Initialize(tile_current, (int)transform.eulerAngles.y);


			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
//			SetUnitNeighboursTilesState(TileState.EnemyOn);
		}

		public void StartTurn()
		{					
			canMove = true;	
			turnSteps = 0;	
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
				{
					TravelTo(tile_init);
				}
			}
			else
				TravelTo(tile_init);

			if(path == null)
			{
				canMove = false;
				TravelFinished();
				return;
			}

			if(waypoints.Count > 1)
			{
				Vector3 directionOr = transform.eulerAngles;
				Vector3 direction = waypoints[1] - transform.position;
				direction.y =0;						
				if(direction.normalized!= Vector3.zero)
					transform.forward = direction.normalized * 90;	
				if(transform.eulerAngles != directionOr)
				{
					SetFov(false, tile_current);					
					SetFov(true, path[0]);	
				}
				Check();	
			}
		}
		public void Reset()
		{			
			target = null;
			psDetect.SetActive(false);

			if(path != null)
			{
				path.Clear();
				waypoints.Clear();
			}

			transform.position = position_Init;
			transform.eulerAngles = new Vector3(0, rotation_init, 0);

			tile_current = tile_init;			
			tile_current.AddUnit(this);
			fov.EnableFov(tile_current, (int)transform.eulerAngles.y);

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
//			SetUnitNeighboursTilesState(TileState.EnemyOn);

			moveState = MoveState.None;
		}		

		public void PlayerChangedTile(Tile tile)
		{
			if(Tile.ReferenceEquals(target, null) || target != Player.Instance.tile_current)
			{
				if(fov.isPlayerDetected())
				{
					target = Player.Instance.tile_current;

					Vector3 pos = target.transform.position;
					pos.y += 0.003f;
					psDetect.transform.position = pos;
					psDetect.SetActive(true);
				}
			}
		}
		#endregion


		public override void TravelFinished()
		{
//			SetUnitNeighboursTilesState(TileState.Clear);
			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
//			SetUnitNeighboursTilesState(TileState.EnemyOn);
			SetFov(true, tile_current);	

			Check();
			
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
//				print (transform.parent);
//				TurnManager.Instance.StopCoroutine("EnemyMoved");
				TurnManager.Instance.StartCoroutine("EnemyMoved");
			}
		}

		public override bool Check()
		{				
			if(!Player.Instance.isHidden)
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
						
						Vector3 pos = target.transform.position;
						pos.y += 0.003f;
						psDetect.transform.position = pos;
						psDetect.SetActive(true);
						
						Vector3 direction = waypoints[0] - transform.position;
						direction.y =0;						
						if(direction.normalized!= Vector3.zero)
							transform.forward = direction.normalized * 90;	
						SetFov(false, tile_current);					
						SetFov(true, path[0]);		
						return true;
					}
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
