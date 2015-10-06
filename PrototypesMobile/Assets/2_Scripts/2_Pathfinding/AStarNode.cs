namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class AStarNode : IComparable
	{
		// Fields
		// ========================================================================
		public Tile Tile;
		public AStarNode Parent;
		public AStarNode Goal;
		public int Cost;
		
		// Public methods
		// ========================================================================
		public AStarNode(Tile tile, AStarNode parent, AStarNode goal, int cost)
		{
			Tile = tile;
			Parent = parent;
			Goal = goal;
			Cost = cost;
		}
		
		public List<AStarNode> GetSuccessors()
		{
			List<AStarNode> successors = new List<AStarNode>();
			foreach (Tile neighbour in Tile.Neighbours)
			{
				if (neighbour.Equals(this) || !neighbour.IsTraversable)
					continue;
				
				successors.Add(new AStarNode(neighbour, this, Goal, Cost + neighbour.TraversalCost));
			}
			return successors;
		}
		
		public int TotalCost()
		{
			int manhattanDistance = Math.Abs(Tile.X - Goal.Tile.X) + Math.Abs(Tile.Z - Goal.Tile.Z);
			return manhattanDistance + Cost;
		}
		
		public int CompareTo(object obj)
		{
			return(TotalCost() - ((AStarNode)obj).TotalCost());
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
			AStarNode p = obj as AStarNode;
			if ((System.Object)p == null)
			{
				return false;
			}
			
			// Return true if the fields match:
			return Tile.Equals(p.Tile);
		}
		
		public bool Equals(AStarNode p)
		{
			// If parameter is null return false:
			if ((object)p == null)
			{
				return false;
			}
			
			// Return true if the fields match:
			return Tile.Equals(p.Tile);
		}
		
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
	}
}