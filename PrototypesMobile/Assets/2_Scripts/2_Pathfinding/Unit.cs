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
	    // Fields
                                                                                	    // ========================================================================
		public Tile Tile;

	    public float Speed = 1f;
	    protected float AngularSpeed = 6.0f;

		public List<Tile> path;
		public List<Vector3> waypoints;
		public Quaternion targetRotation;
		public Vector3 position_Init = Vector3.zero;
		
		public int step_Max = 1;		
		public Tile tile_init;		
		public Tile tile_current;
		public List<Tile> list_UnitNeighbours;
		
		public MoveState moveState = MoveState.None;

		void Update () 
		{
			if (path != null && path.Count > 0 && waypoints != null && waypoints.Count > 0)
			{
				moveState = MoveState.Moving;
				MoveThroughWaypoints();
			}
			else if(moveState != MoveState.None)
			{
				TravelFinished();
			}
//			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
		}

		public virtual void Initialize(Tile tile)
		{			
			Tile = tile;
			
			position_Init = tile.GetUnitPosition();
			transform.position = position_Init;
			targetRotation = transform.rotation;
			
			tile_init = tile;		
			tile_current = tile;
			
			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
		}

		public virtual void TravelTo(Tile destination)
	    {
			if(Tile.ReferenceEquals(destination, null))
				return;

			List<Tile> path = AStar.FindPath(tile_current, destination);
	        if (path.Count == 0)
	        {
	            path = null;
	            return;
	        }
			SetUnitNeighboursTilesState(TileState.Clear);
			tile_current.RemoveUnit(this);

			this.path = path;
	        waypoints = GetWaypointsFromPath(path);
	    }
		public abstract void TravelFinished();
		
		public virtual bool? CheckTile(){return null;}

	    private List<Vector3> GetWaypointsFromPath(List<Tile> path)
	    {
	        var outWaypoints = new List<Vector3>();
	        outWaypoints.Add(path[0].GetUnitPosition());
	        for (int i = 1; i < path.Count ; i++)
	        {
				outWaypoints.Add(GetMidwayVector(path[i].GetUnitPosition(), path[i-1].GetUnitPosition()));
	            outWaypoints.Add(path[i].GetUnitPosition());
	        }
	        return outWaypoints;
	    }

		private Vector3 GetMidwayVector(Vector3 tile1, Vector3 tile2)
	    {
			Vector3 midVector = tile1.y >= tile2.y ? new Vector3(tile2.x,tile1.y,tile2.z) : new Vector3(tile1.x,tile2.y,tile1.z);
	        return midVector;
	    }

	    private void MoveThroughWaypoints()
		{
			Vector3 direction = waypoints[0] - transform.position;

			direction.Normalize();
	        transform.position += direction * Speed * Time.deltaTime;

	        if (Vector3.Distance(waypoints[0], transform.position) <= Speed * Time.deltaTime / 2.0f)
	        {
	            transform.position = waypoints[0];

	            if (waypoints.Count % 2 == 1)
	            {
	                Tile = path[0];
					tile_current = path[0];
					path.RemoveAt(0);
					if(CheckTile()!= null && CheckTile().Value)
						return;
	            }

	            waypoints.RemoveAt(0);

				if(waypoints.Count > 0)
				{
					direction = waypoints[0] - transform.position;
					direction.y =0;						
					if(direction.normalized!= Vector3.zero)
						transform.forward = direction.normalized * 90;	
				}
	        }
	    }

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
		}
	}
}
