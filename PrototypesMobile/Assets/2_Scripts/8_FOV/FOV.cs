namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class FOV : MonoBehaviour 
	{		
		public int[,] pos2D_Detect;
		public int[,] pos2D_View;
		[HideInInspector]
		public List<Tile> tiles_Detect;
		[HideInInspector]
		public List<Tile> tiles_View;		
		[HideInInspector]
		public List<Tile> tiles_Neighbours;
		[HideInInspector]
		public int max_Step;

		public abstract void Initialize(Tile tile);

		public void SetTilesState(List<Tile> l, TileState ts)
		{
			foreach(var t in l)
			{
				t.SetTileState(ts);
			}
		}
		public void NeighboursRestriction(List<Tile> From, ref List<Tile> to)
		{
			to.Clear();
			foreach(var t in From)
			{
				if(tiles_Neighbours.Contains(t))
					to.Add(t);
			}
		}
		public abstract void SetFovDirection(int Angle);

		public virtual void EnableFov(Tile tile)
		{
			SetFovDirection((int)transform.eulerAngles.y);

			tiles_Neighbours = tile.GetTilesWithinCost(max_Step);

			NeighboursRestriction(MapManager.Instance.GetFloorTile(pos2D_Detect, tile), ref tiles_Detect);
			NeighboursRestriction(MapManager.Instance.GetFloorTile(pos2D_View, tile), ref tiles_View);

			SetTilesState(tiles_Detect, TileState.EnemyDetect);
			SetTilesState(tiles_View, TileState.EnemyView);
		}

		public virtual void DisableFov()
		{
			SetTilesState(tiles_Neighbours, TileState.Clear);			
//			SetTilesState(tiles_View, TileState.Clear);
		}

		public virtual bool isPlayerDetected()
		{			
			if(tiles_Detect.Contains(Player.Instance.tile_current))
			{
				return true;
			}			
			return false;
		}
	}
}
