namespace TheVandals
{
	using UnityEngine;
	using System;
	
	public class NodeEntity
	{		
		public Vector3 position;
		public bool isActive;
		public TileEntity tile_linked;
		
		public NodeEntity (Vector3 position)
		{
			this.position = position;
			this.isActive = false;
		}

		public NodeEntity (Vector3 position, bool isActive, TileEntity tile_linked)
		{
			this.position = position;
			this.isActive = isActive;
			this.tile_linked = tile_linked;
		}
	}
}
