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

//		public bool RetrieveTiles = false;

		[HideInInspector]
		public List<Tile> list_wayPoints;

		[HideInInspector]
		public bool isPathDefined = false;

		public int unitPos;
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
//		void OnDrawGizmos()
//		{
//			if(type != Type.None && RetrieveTiles && tiles_index.Length > 0)		
//			{
//				print ("RetrieveTiles");
//				list_wayPoints.Clear();
//				list_wayPoints.Add(MapManager.Instance.InitializeUnit(transform.position, gameObject));
//				MapManager.Instance.SetWayPoints(ref list_wayPoints, tiles_index);
//				RetrieveTiles = false;
//			}
//		}
		#endregion

		#region Private

		public void Initialise()
		{
			if(type != Type.None)		
			{
				list_wayPoints.Clear();
				list_wayPoints.Add(MapManager.Instance.InitializeUnit(transform.position));
				MapManager.Instance.SetWayPoints(ref list_wayPoints, tiles_index);
//				RetrieveTiles = false;
			}
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

		public Tile GetNextWayPoint(Tile tile_Unit)
		{
			if(list_wayPoints.Count > 1)
			{				
				if(tile_Unit != list_wayPoints[unitPos])
					return list_wayPoints[unitPos];
				switch (type)
				{
				case Type.Aleatoire:
					int initPos = unitPos;
					while(unitPos == initPos)
					{
						unitPos = UnityEngine.Random.Range(0, list_wayPoints.Count -1);
					}
					break;
				case Type.PingPong:
					if(!directionInverse)
					{
						unitPos++;
						
						if(unitPos >= list_wayPoints.Count)
						{
							unitPos = list_wayPoints.Count - 2;
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
					
					if(unitPos >= list_wayPoints.Count)
						unitPos = 0;
					break;
				}
			}
			return list_wayPoints[unitPos];
		}
		#endregion
	}
}
