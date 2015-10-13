namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class Tile : MonoBehaviour
	{
	    // Fields
		// ========================================================================

		public int index;
		public string Name;
		public int X;
		public int Y;
		public int Z;
	    public List<Tile> Neighbours;
	    public bool AllowsTraversal = true; // Indicates whether this particular type of Tile allows traversal. You can set this value in classes inherited from the Tile class.
	    public int TraversalCost;
	    public bool IsTraversable; // Indicates whether the tile is traversable, or buried under other tiles in the level. This value is set by the Graph class automatically.

	    protected Vector3 CentreUnitOffset; // Offset of the unit position when it is at the center of this tile.
	    protected Vector3 CentreUnitRotation; // Rotation of the unit position when it is at the center of this tile.
	    protected float[] ActualBoundaryHeights; // Heights of the boundaries used for the unit's traversal over the terrain. Should correspond to accurate world units.
	    protected float[] VirtualBoundaryHeights; // Heights of the boundaries used to determine if any two neighbouring tile boundary can be traversed. Check out the interaction between the SlopeTile and the BlockTile for details.

		public bool isEnemyOn;
		public bool isPlayerOn;
		public int enemyCount;

		public bool isFoVDetect;
		public bool isFoVView;

	    // Public methods
	    // ========================================================================
		public abstract void Initialize(int index);

		public abstract void Reset();
		
		public void ResetTiles()
		{
			foreach(Tile te in Neighbours)
			{
				if(!Tile.ReferenceEquals(te, null))
					te.Reset();
			}
		}

	    public virtual int[] ArrayCoordinateFromPosition(Vector3 position)
	    {
	        return new int[]
	        {
	            Mathf.RoundToInt(position.x),
	            Mathf.RoundToInt(position.y),
	            Mathf.RoundToInt(position.z),
	        };
		}

		public virtual void SetTilesState(TileState tile_state){}
		public virtual void SetTileState(TileState tile_state){}

		public virtual void SetTileHeight(float height)
		{	
			Y = Mathf.RoundToInt(height);
			Vector3 pos = transform.position;
			pos.y = height;
			transform.position = pos;
		}

	    public Vector3 GetTilePosition()
	    {
	        return transform.position;
	    }

	    public Quaternion GetUnitRotation(Quaternion unitRotation)
	    {   
	        Quaternion finalRotation = transform.rotation * Quaternion.Euler(CentreUnitRotation);
	        float requiredYawRotation = unitRotation.eulerAngles.y - finalRotation.eulerAngles.y;
	        finalRotation = finalRotation * Quaternion.AngleAxis(requiredYawRotation, Vector3.up);
	        return finalRotation;
	    }		
		
		public List<Tile> GetTilesWithinCost(int cost)
		{
			List<Tile> tiles = new List<Tile>();
			WithinCost(this, cost, ref tiles);
			return tiles;
		}

		private void WithinCost(Tile current, int cost, ref List<Tile> tiles)
		{
			if (cost < 0) { return; }

			if (current.IsTraversable && !tiles.Contains(current))
			{
				tiles.Add(current);
			}
			
			foreach (var neighbour in current.Neighbours)
			{
				WithinCost(neighbour, cost - current.TraversalCost, ref tiles);
			}
		}

		public void AddUnit(object obj)
		{
			if(obj is Enemy)
			{
				enemyCount++;
				isEnemyOn = true;
				
//				print("AddUnit" + this + "    " + obj + "  " + enemyCount);
			}
			else if(obj is Player)
			{
				isPlayerOn = true;
			}
		}
		public void RemoveUnit(object obj)
		{
			if(obj is Enemy)
			{
				enemyCount--;
				if(enemyCount == 0)
					isEnemyOn = false;
				
//				print("RemoveUnit" + this + "    " + obj + "  " + enemyCount);
			}
			else if(obj is Player)
			{
				isPlayerOn = false;
			}
		}

	    // Private methods
	    // ========================================================================
		protected int NormalizeRotation(int angle)
		{
			angle %= 360;
			if (angle < 0)
			{
				angle += 360;
			}

			return angle;
		}

	    // Equivalence methods
	    // ========================================================================
		public override bool Equals (object obj)
		{
			// If parameter is null return false.
			if (obj == null)
			{
				return false;
			}
			
			// If parameter cannot be cast to Point return false.
			var p = obj as Tile;
			if ((System.Object)p == null)
			{
				return false;
			}
			
			// Return true if the fields match:
			return (index == p.index);
		}
		
		public bool Equals(Tile p)
		{
			// If parameter is null return false:
			if ((object)p == null)
			{
				return false;
			}
			
			// Return true if the fields match:
			return (index == p.index);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
	}
}

