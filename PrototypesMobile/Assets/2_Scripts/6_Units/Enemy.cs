﻿namespace TheVandals
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
		public abstract EnemyType type{get;}
		
		public float markHeight;

		public FOV fov;

		public Waypoint waypoint;

		public Tile target;

		public GameObject psDetect;

		private GameObject prefab_ImageExclamation;

		private bool iswaiting = false;
		#endregion

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.startTurn_Enemy += StartTurn;
			EventManager.gameReset += Reset;
			EventManager.playerChangedTile += PlayerChangedTile;
			EventManager.pause += Pause;
			EventManager.resume += Resume;
		}		

		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.startTurn_Enemy -= StartTurn;
			EventManager.gameReset -= Reset;
			EventManager.playerChangedTile -= PlayerChangedTile;
			EventManager.pause -= Pause;
			EventManager.resume -= Resume;
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

			UIManager.Instance.GenerateUnitExclamationMark(ref prefab_ImageExclamation);
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
				Check(false);	
			}
		}

		public void Reset()
		{			
			target = null;
			psDetect.SetActive(false);
			UIManager.Instance.UpdateExclamationMark(prefab_ImageExclamation, transform, false, markHeight);
			StopCoroutine("UpdateExclamationMark");

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

			if(anim)
				anim.SetInteger("MoveState",0);
			
			iswaiting = false;
		}		

		public override void TravelTo(Tile destination)
		{
			if(Tile.ReferenceEquals(destination, null))
				return;
			
			List<Tile> path = (type != EnemyType.Dog) ? 
					AStar.FindPath(tile_current, destination):
					AStar.FindPathInHeight(tile_current, destination, tile_current.transform.position.y);
			
			if (path.Count <= 1)
			{
				this.path = null;
				turnSteps = step_Max;
				TravelFinished();
				return;
			}
			
			tile_current.RemoveUnit(this);
			
			this.path = path;
			waypoints = GetWaypointsFromPath(path);
			
			moveState = MoveState.Moving;
			if(anim)
				anim.SetInteger("MoveState",1);
		}

		public void PlayerChangedTile(Tile tile)
		{
			if(fov.isPlayerDetected() && (Tile.ReferenceEquals(target, null) || target != Player.Instance.tile_current))
			{
				if(Tile.ReferenceEquals(target, null))				
					SoundManager.Instance.PlayClip(1);

				target = Player.Instance.tile_current;
				
				Vector3 pos = target.transform.position;
				pos.y += 0.003f;
				psDetect.transform.position = pos;
				psDetect.SetActive(true);				
				if(!prefab_ImageExclamation.activeSelf)
					StartCoroutine("UpdateExclamationMark");
				
				Player.Instance.Detected(this);
			}
			else if(Tile.ReferenceEquals(target, null))
				Player.Instance.NotDetected(this);

		}

		public void Pause()
		{
			isPaused = true;
			if(anim)
				anim.speed = 0;
		}
		
		public void Resume()
		{
			isPaused = false;
			
			if(anim)
				anim.speed = 1;
		}
		#endregion



		public override void TravelFinished()
		{
//			SetUnitNeighboursTilesState(TileState.Clear);
			if(anim)
				anim.SetInteger("MoveState",0);

			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
//			SetUnitNeighboursTilesState(TileState.EnemyOn);
			SetFov(true, tile_current);				
			
			Check(false);

			if(tile_current == target)
			{
				target = null;
				psDetect.SetActive(false);
				UIManager.Instance.UpdateExclamationMark(prefab_ImageExclamation, transform, false, markHeight);
				Player.Instance.NotDetected(this);
				StopCoroutine("UpdateExclamationMark");
			}
			
			tile_current.AddUnit(this);

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

		public override bool Check(bool addStep)
		{				
			if(!Player.Instance.isHidden)
			{
				if(tile_current == Player.Instance.tile_current)
				{
					Stop();
					GameManager.Instance.StartCoroutine("PlayerLost");	
					return true;
				}

				if(fov.isPlayerDetected() && (Tile.ReferenceEquals(target, null) || target != Player.Instance.tile_current))
				{
					if(!Tile.ReferenceEquals(target, null) && target != Player.Instance.tile_current)
						PlayerDetected(addStep);
					else if(!iswaiting)
					{
						StartCoroutine("waitForSeconds", addStep);
					}
					return true;
				}
				else if(Tile.ReferenceEquals(target, null))
					Player.Instance.NotDetected(this);

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

		IEnumerator UpdateExclamationMark()
		{				
			while(true)
			{
				UIManager.Instance.UpdateExclamationMark(prefab_ImageExclamation, transform, true, markHeight);
				yield return null;
			}
		}

		IEnumerator waitForSeconds(bool addStep)
		{
			iswaiting = true;

			Stop ();

			if(addStep)
				turnSteps++;

			
			target = Player.Instance.tile_current;
			
			List<Tile> path = AStar.FindPath(tile_current, Player.Instance.tile_current);
			this.path = path;
			waypoints.Clear();
			waypoints = GetWaypointsFromPath(path);
			
			Vector3 direction = waypoints[0] - transform.position;
			direction.y =0;						
			if(direction.normalized!= Vector3.zero)
				transform.forward = direction.normalized * 90;	
			
			Vector3 pos = target.transform.position;
			pos.y += 0.003f;
			psDetect.transform.position = pos;
			psDetect.SetActive(true);
			
			StartCoroutine("UpdateExclamationMark");
			
			SetFov(false, tile_current);					
			SetFov(true, path[0]);		

			SoundManager.Instance.PlayClip(1);
			
			Player.Instance.Detected(this);

			yield return new WaitForSeconds(1.0f);

			iswaiting = false;
			if(turnSteps < step_Max)
			{
				moveState = MoveState.Moving;
				canMove = true;
				if(anim)
					anim.SetInteger("MoveState",1);	
			}
			else
				TravelFinished();
		}
		public void PlayerDetected(bool addStep)
		{
			if(addStep)
			{
				turnSteps++;
				if(turnSteps >= step_Max)
					canMove = false;
			}

			target = Player.Instance.tile_current;
			
			List<Tile> path = AStar.FindPath(tile_current, Player.Instance.tile_current);
			this.path = path;
			waypoints.Clear();
			waypoints = GetWaypointsFromPath(path);
			
			Vector3 direction = waypoints[0] - transform.position;
			direction.y =0;						
			if(direction.normalized!= Vector3.zero)
				transform.forward = direction.normalized * 90;	
			
			Vector3 pos = target.transform.position;
			pos.y += 0.003f;
			psDetect.transform.position = pos;
			psDetect.SetActive(true);
			
			StartCoroutine("UpdateExclamationMark");
			
			SetFov(false, tile_current);					
			SetFov(true, path[0]);	
			
			Player.Instance.Detected(this);

			if(turnSteps < step_Max)
			{
				moveState = MoveState.Moving;
				canMove = true;
				if(anim)
					anim.SetInteger("MoveState",1);	
			}
			else
				TravelFinished();
		}
	}
}
