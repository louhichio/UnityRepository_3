namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;



	public class Waypoint : MonoBehaviour 
	{
		public enum Type
		{
			None,
			PingPong,
			ClosedLoop,
			Aleatoire
		}
		#region Properties		
		public Type type = Type.None;

		public int[] tiles_index;	

		public bool RetrieveTiles = false;

		public List<Tile> list_wayPoints;

		[HideInInspector]
		public bool isPathDefined = false;

		private int unitPos;
		private bool directionInverse = false;
		#endregion
			
			#region Events
			void OnEnable()
		{
			EventManager.gameReset += Reset;
		}		
		void OnDisable()
		{
			EventManager.gameReset -= Reset;
		}
		
		void Reset()
		{
			unitPos = 0;
			directionInverse = false;
		}
		#endregion
		#region Unity
		void OnDrawGizmos()
		{
			if(type != Type.None && RetrieveTiles && tiles_index.Length > 0)		
			{
				print ("RetrieveTiles");
				list_wayPoints.Clear();
				list_wayPoints.Add(MapManager.Instance.InitializeUnit(transform.position, gameObject));
				MapManager.Instance.SetWayPoints(ref list_wayPoints, tiles_index);
				RetrieveTiles = false;
			}
		}
		#endregion

		#region Private

		public void Initialise()
		{
			SetIsPathDefined();
		}

		private void SetIsPathDefined()
		{
			if(type != Type.None && (List<Tile>.ReferenceEquals(list_wayPoints, null) || list_wayPoints.Count < 2))
			{
				isPathDefined =  false;
				return;
			}
			isPathDefined = true;
		}

		public Tile GetNextWayPoint()
		{
			switch (type)
			{
			case Type.PingPong:
				if(directionInverse)
				{
					unitPos++;
					
					if(unitPos > list_wayPoints.Count)
					{
						unitPos = list_wayPoints.Count - 1;
						directionInverse = true;
					}
				}
				else
				{
					unitPos--;
					
					if(unitPos < 0)
					{
						unitPos = 1;
						directionInverse = false;
					}
				}
				break;
			case Type.ClosedLoop:
				unitPos++;
				
				if(unitPos > list_wayPoints.Count)
					unitPos = 0;
				break;
			}
			return list_wayPoints[unitPos];
		}
		#endregion
	}
}
