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

		public bool retrieveTiles
		
		public List<Tile> list_wayPoints;

		public bool isPathDefined = false;
		#endregion

		#region Unity
		void OnDrawGizmos()
		{
			if()
		}
		#endregion

		#region Private

//		public void Initialise()
//		{
//			SetIsPathDefined();
//		}
//
//		private void SetIsPathDefined()
//		{
//			if(waypointType != WaypointType.None && (list_wayPoints || list_wayPoints.Count < 2))
//			{
//				isPathDefined =  false;
//				return;
//			}
//			isPathDefined = true;
//		}
		#endregion
	}
}
