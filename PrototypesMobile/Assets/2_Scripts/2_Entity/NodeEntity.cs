namespace TheVandals
{
	using UnityEngine;
	using System;
	
	public class NodeEntity
	{		
		public Vector3 position;
		public bool isActive;
		public TileObject[] tile_linked = new TileObject[2];
		
		public NodeEntity (Vector3 position)
		{
			this.position = position;
			this.isActive = false;
		}

		public NodeEntity (Vector3 position, bool isActive, TileObject tile_linked, TileObject tile_secondlinked)
		{
			this.position = position;
			this.isActive = isActive;
			this.tile_linked[0] = tile_linked;
			this.tile_linked[1] = tile_secondlinked;
		}
		public void UpdateNodePosition()
		{
			position = (tile_linked[0].transform.position + tile_linked[1].transform.position) / 2;
		}
	}
}
