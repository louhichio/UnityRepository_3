namespace TheVandals
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	
	public class CrossEntity
	{
		
		public TileEntity tile_center = null;
		public TileEntity tile_forward = null;
		public TileEntity tile_back = null;
		public TileEntity tile_right = null;
		public TileEntity tile_left = null;

		public List<TileEntity> list_tiles = new List<TileEntity>();

		public CrossEntity ()
		{
		}

		public CrossEntity (TileEntity tile_center, TileEntity tile_forward, TileEntity tile_back, TileEntity tile_right, TileEntity tile_left)
		{
			this.tile_center = tile_center;
			this.tile_forward = tile_forward;
			this.tile_back = tile_back;
			this.tile_right = tile_right;
			this.tile_left = tile_left;
		}
		public CrossEntity (CrossEntity cross)
		{
			this.tile_center = cross.tile_center;
			this.tile_forward = cross.tile_forward;
			this.tile_back = cross.tile_back;
			this.tile_right = cross.tile_right;
			this.tile_left = cross.tile_left;

			list_tiles.Add(tile_center);
			list_tiles.Add(tile_forward);
			list_tiles.Add(tile_back);
			list_tiles.Add(tile_right);
			list_tiles.Add(tile_left);
		}

		public void SetTilesState(TileState tileState)
		{
			foreach(TileEntity te in list_tiles)
			{
				if(!TileEntity.ReferenceEquals(te,null))
				   te.SetTileState(tileState);
			}
		}

		public void Init()
		{
			list_tiles.Add(tile_center);
			list_tiles.Add(tile_forward);
			list_tiles.Add(tile_back);
			list_tiles.Add(tile_right);
			list_tiles.Add(tile_left);
		}

	}
}
