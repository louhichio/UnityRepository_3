namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public enum MoveState
	{
		None,
		Moving
	}
	public abstract class Unit : MonoBehaviour 
	{
	    public float Speed = 1f;

//	    protected float AngularSpeed = 6.0f;
//		[HideInInspector]
		public List<Tile> path;
		[HideInInspector]
		public List<Vector3> waypoints;
		[HideInInspector]
		public Quaternion targetRotation;
		[HideInInspector]
		public Vector3 position_Init = Vector3.zero;
		
		public int step_Max = 1;
		[HideInInspector]
		public Tile tile_init;		
		public Tile tile_current;
		
		[HideInInspector]
		public int rotation_init;

		[HideInInspector]
		public List<Tile> list_UnitNeighbours;
		
		public MoveState moveState = MoveState.None;
		public bool canMove = false;

//		[HideInInspector]
		public int turnSteps;
		[HideInInspector]
		public Animator anim;			
		[HideInInspector]
		public bool isPaused = false;		
		[HideInInspector]
		public bool endedGame = false;

		void Update () 
		{
			if(!isPaused)
			{
				if (canMove && path != null && path.Count > 0 && waypoints != null && waypoints.Count > 0)
				{
					MoveThroughWaypoints();
				}
				else if(moveState != MoveState.None)
				{
					TravelFinished();
				}
			}
//			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
		}

		public virtual void Initialize(Tile tile)
		{						
			turnSteps = 0;

			moveState = MoveState.None;

			position_Init = tile.GetTilePosition();
			transform.position = position_Init;			

			int angle = (int)transform.eulerAngles.y;
			if(angle != 0 && angle != 90.0f && angle != 180.0f && angle != 270.0f )
				transform.rotation = Quaternion.Euler(Vector3.zero);
			
			targetRotation = transform.rotation;
			rotation_init = (int)transform.eulerAngles.y;			

			tile_init = tile;		
			tile_current = tile;
			
			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
            
			if(GetComponent<Animator>())
			{
				anim = GetComponent<Animator>();
				anim.SetInteger("MoveState",0);			
			}
		}

		public abstract void TravelTo(Tile destination);

		public abstract void TravelFinished();
		
		public abstract bool Check(bool addStep);

	    public List<Vector3> GetWaypointsFromPath(List<Tile> path)
	    {
	        var outWaypoints = new List<Vector3>();
			outWaypoints.Add(path[0].GetTilePosition());
	        for (int i = 1; i < path.Count ; i++)
	        {
				outWaypoints.Add(GetMidwayVector(path[i].GetTilePosition(), path[i-1].GetTilePosition()));
				outWaypoints.Add(path[i].GetTilePosition());
	        }
	        return outWaypoints;
	    }

		private Vector3 GetMidwayVector(Vector3 tile1, Vector3 tile2)
	    {
			Vector3 midVector;
			if(tile1.y == tile2.y)			
				midVector = (tile2 + tile1)/2;			
			else
				midVector = tile1.y > tile2.y ? new Vector3(tile2.x,tile1.y,tile2.z) : new Vector3(tile1.x,tile2.y,tile1.z);
	        return midVector;
	    }

		public void MoveThroughWaypoints()
		{
			Vector3 direction = waypoints[0] - transform.position;
			
			direction.Normalize();
			transform.position += direction * Speed * Time.deltaTime;

			if (Vector3.Distance(waypoints[0], transform.position) <= Speed * Time.deltaTime / 2.0f)
			{
				transform.position = waypoints[0];
				
				if (waypoints.Count % 2 == 1)
				{
					tile_current = path[0];
					path.RemoveAt(0);					
					
					if(Check(true))
						return;	

					if(direction != Vector3.zero && !endedGame)
					{
						turnSteps++;
						if(turnSteps >= step_Max)
							canMove = false;
					}			
				}
				else
				{						
					SetFov(false, tile_current);					
					SetFov(true, path[0]);			
				}
				
				waypoints.RemoveAt(0);
				
				if(waypoints.Count > 0)
				{
					direction = waypoints[0] - transform.position;
					direction.y = 0;						
					if(direction.normalized!= Vector3.zero)
						transform.forward = direction.normalized * 90;
				}
			}
		}
		
		public virtual void SetFov(bool enable , Tile tile){}
		public virtual void UpdateUI(){}

		public void SetUnitNeighboursTilesState(TileState ts)
		{
			if(step_Max > 1)
			{
				foreach(var t in list_UnitNeighbours)
				{
					t.SetTileState(ts);
				}
				return;
			}
			else
			{
				tile_current.SetTilesState(ts);
			}
		}
		public void SetUnitNeighboursTilesState(TileState ts, List<Tile> tiles)
		{
			if(step_Max > 1)
			{
				foreach(var t in tiles)
				{
					t.SetTileState(ts);
				}
				return;
			}
			else
			{
				tile_current.SetTilesState(ts);
			}
		}

		public void ResetUnitNeighboursTiles()
		{

			if(step_Max > 1)
			{
				foreach(var t in list_UnitNeighbours)
				{
					t.Reset();
				}
				return;
			}
			else
			{
				tile_current.ResetTiles();
			}
		}

		public void Stop()
		{
			path.Clear();
			waypoints.Clear();
			canMove = false;
			moveState = MoveState.None;
			if(anim)
				anim.SetInteger("MoveState",0);
		}

		public void CorrectUnitRotation()
		{
			int angle = (int)transform.eulerAngles.y;
			Vector3 pos = transform.eulerAngles;
			if(angle != 0 && angle != 90.0f && angle != 180.0f && angle != 270.0f )
			{
				if(angle > 315 && angle <= 45)
				{
					pos.y = 0;
					transform.eulerAngles = pos;
				}
				else if(angle > 45 && angle <= 135)
				{
					pos.y = 90;
					transform.eulerAngles = pos;
				}
				else if(angle > 135 && angle <= 225)
				{
					pos.y = 180;
					transform.eulerAngles = pos;
				}
				else if(angle > 225 && angle <= 315)
				{
					pos.y = 270;
					transform.eulerAngles = pos;
				}	
				
			}
		}
	}
}
