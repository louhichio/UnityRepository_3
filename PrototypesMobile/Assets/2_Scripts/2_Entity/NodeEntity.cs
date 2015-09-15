namespace TheVandals
{
	using UnityEngine;
	using System;
	
	public class NodeEntity
	{		
		public Vector3 position;
		public bool isActive;
		public TileEntity[] tile_linked = new TileEntity[2];
		
		public NodeEntity (Vector3 position)
		{
			this.position = position;
			this.isActive = false;
		}

		public NodeEntity (Vector3 position, bool isActive, TileEntity tile_linked, TileEntity tile_secondlinked)
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
