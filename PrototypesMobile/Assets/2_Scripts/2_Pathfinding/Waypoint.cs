namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public enum WaypointType
	{
		None,
		PingPong,
		ClosedLoop,
		Aleatoire
	}

	public class Waypoint : MonoBehaviour 
	{
		#region Properties		
		public WaypointType waypointType = WaypointType.None;

		public int[] tiles_index;

		public bool isTilesSet = false;
		[SerializeField]
		private List<Tile> list_wayPoints;

		private bool isPathDefined = false;
		#endregion

		#region Unity
		void OnDrawGizmos()
		{
			if(waypointType != WaypointType.None && !isTilesSet && tiles_index.Length > 1)		
			{
				print ("isTilesSet");
				MapManager.Instance.SetWayPoints(ref list_wayPoints, tiles_index);
				isTilesSet = true;
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
			if(waypointType != WaypointType.None && (List<Tile>.ReferenceEquals(list_wayPoints, null) || list_wayPoints.Count < 2))
			{
				isPathDefined =  false;
				return;
			}
			isPathDefined = true;
		}
		#endregion
	}
}
