namespace TheVandals
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	
	public class CrossEntity
	{	
		public TileObject tile_center = null;
		public TileObject tile_forward = null;
		public TileObject tile_back = null;
		public TileObject tile_right = null;
		public TileObject tile_left = null;

		public bool IsPlayerOnCross = false;
		public bool IsEnemyOnCross = false;

		public List<TileObject> list_tiles = new List<TileObject>();
		public List<LinkObject> list_TileCenter_Links = new List<LinkObject>();

		public CrossEntity ()
		{
		}

		public CrossEntity (TileObject tile_center, 
		                    TileObject tile_forward, 
		                    TileObject tile_back,
		                    TileObject tile_right,
		                    TileObject tile_left, 
		                    List<LinkObject> list_TileCenter_Links)
		{
			this.tile_center = tile_center;
			this.tile_forward = tile_forward;
			this.tile_back = tile_back;
			this.tile_right = tile_right;
			this.tile_left = tile_left;

			this.list_TileCenter_Links = list_TileCenter_Links;
		}

		public CrossEntity (CrossEntity cross)
		{
			this.tile_center = cross.tile_center;
			this.tile_forward = cross.tile_forward;
			this.tile_back = cross.tile_back;
			this.tile_right = cross.tile_right;
			this.tile_left = cross.tile_left;

			this.list_tiles.Add(tile_center);
			this.list_tiles.Add(tile_forward);
			this.list_tiles.Add(tile_back);
			this.list_tiles.Add(tile_right);
			this.list_tiles.Add(tile_left);

			this.list_TileCenter_Links = cross.list_TileCenter_Links;
		}

		public void SetTilesState(TileState tileState)
		{
			IsPlayerOnCross = false;
			IsEnemyOnCross = false;
			foreach(TileObject te in this.list_tiles)
			{
				if(!TileObject.ReferenceEquals(te, null) &&
				   (te == tile_center || list_TileCenter_Links.Find(x => x.tile_LinkStart == te || x.tile_LinkEnd == te).isActive))
				{
				   te.SetTileState(tileState);
					if(te.IsPlayerOnTile())
						IsPlayerOnCross = true;
				}
			}			
			if(!TileObject.ReferenceEquals(this.tile_center, null) && !this.tile_center.enemyLeft)
				IsEnemyOnCross = true;
		}

		public void ResetTiles()
		{
			foreach(TileObject te in this.list_tiles)
			{
				if(!TileObject.ReferenceEquals(te, null))
					te.Reset();
			}
		}
	}
}
