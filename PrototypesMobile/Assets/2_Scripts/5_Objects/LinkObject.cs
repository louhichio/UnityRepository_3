namespace TheVandals
{
	using UnityEngine;
	using System;

	public class LinkObject : MonoBehaviour
	{
		public TileObject tile_LinkStart;
		public TileObject tile_LinkEnd;
		public bool isActive;
		public int index;

		void OnDrawGizmos()
		{			
			if(tile_LinkStart && tile_LinkEnd)
			{
				if(!tile_LinkStart.isActive || !tile_LinkEnd.isActive)
				{
					isActive = false;
				}

				Gizmos.color = isActive ? Color.green : Color.red;
				Gizmos.DrawLine(tile_LinkStart.transform.position, tile_LinkEnd.transform.position);
			}
		}

		public void InitLink (TileObject tile_LinkStart, TileObject tile_LinkEnd, bool isActive, int index)
		{
			this.tile_LinkStart = tile_LinkStart;
			this.tile_LinkEnd = tile_LinkEnd;
			this.isActive = isActive;
			this.index = index;

			transform.position = ((tile_LinkStart.transform.position - tile_LinkEnd.transform.position) / 2) + tile_LinkEnd.transform.position ;
		}

	}
}