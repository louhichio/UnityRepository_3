namespace TheVandals
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using UnityEngine;

	public static class AStar
	{	
		
		// Public methods
		// ========================================================================
		public static List<Tile> FindPath(Tile from, Tile to)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			
			List<Tile> shortestPath = new List<Tile>();
			List<AStarNode> openList = new List<AStarNode>();
			List<AStarNode> closedList = new List<AStarNode>();
			
			AStarNode goalNode = new AStarNode(to, null, null, 0);
			AStarNode startNode = new AStarNode(from, null, goalNode, 0);
			
			openList.Add(startNode);
			
			while (openList.Count > 0)
			{
				AStarNode currentNode = openList.Min();
				openList.RemoveAll(n => n.Equals(currentNode));
				closedList.Add(currentNode);

				if(currentNode.Equals(goalNode)) 
				{
					while(currentNode != null) 
					{
						shortestPath.Insert(0,currentNode.Tile);
						currentNode = currentNode.Parent;
					}
					break;		
				}

				List<AStarNode> successors = currentNode.GetSuccessors();
//				UnityEngine.Debug.Log(goalNode.Tile + "   " + currentNode.Tile + "  " + currentNode.Tile.Neighbours.Count);
				foreach (AStarNode successor in successors)
				{
					if (closedList.FirstOrDefault(n =>  n.Equals(successor)) != null)
						continue;
					
					AStarNode successorInOpenList = openList.FirstOrDefault(n =>  n.Equals(successor));
					if (successorInOpenList == null)
					{
						openList.Add(successor);
					}
					else
					{
						if (successor.Cost < successorInOpenList.Cost)
						{
							successorInOpenList.Parent = currentNode;
						}
					}
				}
			}
			
			stopWatch.Stop();
			
			//PrintDebugInfo(shortestPath, stopWatch.ElapsedMilliseconds);
			
			return shortestPath;
		}
		
		// Private methods
		// ========================================================================
		private static void PrintDebugInfo(List<Tile> shortestPath, long elapsedMilliseconds)
		{
			int cost = 0;
			for(int i = 1; i < shortestPath.Count; i++)
			{
				cost += shortestPath[i].TraversalCost;
			}
			UnityEngine.Debug.Log(string.Format ("Algorithm found {0} steps in {1} ms at {2} cost", shortestPath.Count, elapsedMilliseconds, cost));
		}


	}
}