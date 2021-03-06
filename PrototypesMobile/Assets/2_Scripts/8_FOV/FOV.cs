﻿namespace TheVandals
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
		public List<TileFOV> tiles_Visual;

		public GameObject prefab_TileFoV;
		public bool GenerateFoVTiles = false;

		public abstract int count{get;}
		public abstract int max_Step{get;}

		void OnDrawGizmos()
		{
			if(transform.childCount == 0 && GenerateFoVTiles && prefab_TileFoV != null)
			{
				GenerateFOVTiles();
				GenerateFoVTiles = false;
			}
		}

		public void Initialize(Tile tile, int angle)
		{
			if(transform.childCount == 0 && prefab_TileFoV != null)
			{
				GenerateFOVTiles();
				GenerateFoVTiles = false;
			}
			else
			{
				tiles_Visual.Clear();
				foreach (Transform child in transform)
				{
					tiles_Visual.Add(child.GetComponent<TileFOV>());
					tiles_Visual[tiles_Visual.Count-1].Initialise();
				}
			}

			EnableFov(tile, angle);
		}	

		public virtual void EnableFov(Tile tile, int angle)
		{
			SetFovDirection(angle);
			
			tiles_Neighbours = tile.GetTilesWithinCost(max_Step);

			NeighboursRestriction(MapManager.Instance.GetFloorTile(pos2D_Detect, tile), ref tiles_Detect, angle);
//			NeighboursRestriction(MapManager.Instance.GetFloorTile(pos2D_View, tile), ref tiles_View, angle);

			UpdateMapTiles(true);

			SetFoVTiles();
		}
		
		public virtual void DisableFov()
		{
			UpdateMapTiles(false);

			SetTilesState(TileState.Clear);			
		}
		
		public abstract void SetFovDirection(int Angle);

		public void NeighboursRestriction(List<Tile> From, ref List<Tile> to, int angle)
		{
			to.Clear();

			List<Tile> tiles_CantReach = new List<Tile>();

			foreach(var t in From)
			{
				if(tiles_Neighbours.Contains(t))
				{
					to.Add(t);
				}
				else
					tiles_CantReach.Add(t);
			}

			List<Tile> list_Temp  = new List<Tile>();
			float mapHeight = MapManager.Instance.mapHeight;

			switch (angle)
			{
			case 0:
				foreach(var t in tiles_CantReach)
				{		
					if((t.index % mapHeight) != 0)
					{
						list_Temp.Clear();
						list_Temp = to.FindAll(temp => temp.X == t.X && temp.Z >= t.Z && Vector2.Distance(new Vector2(temp.X,temp.Z), new Vector2(t.X,t.Z)) <= max_Step);
						
						foreach(var temp in list_Temp)
						{
							to.Remove(temp);
						}
					}
				}
				break;
			case 90:
				foreach(var t in tiles_CantReach)
				{		
					list_Temp.Clear();
					list_Temp = to.FindAll(temp => temp.Z == t.Z && temp.X >= t.X);
					
					foreach(var temp in list_Temp)
					{
						to.Remove(temp);
					}
				}
				break;
			case 180:
				foreach(var t in tiles_CantReach)
				{		
					if((t.index + 1 % MapManager.Instance.mapHeight) != 0)
					{
						list_Temp.Clear();
						list_Temp = to.FindAll(temp => temp.X == t.X && temp.Z <= t.Z && Vector2.Distance(new Vector2(temp.X,temp.Z), new Vector2(t.X,t.Z)) <= max_Step);
						
						foreach(var temp in list_Temp)
						{
							to.Remove(temp);
						}
					}
				}
				break;
			case 270:
				foreach(var t in tiles_CantReach)
				{		
					list_Temp.Clear();
					list_Temp = to.FindAll(temp => temp.Z == t.Z && temp.X <= t.X);
					
					foreach(var temp in list_Temp)
					{
						to.Remove(temp);
					}
				}
				break;
			}
		}

		public virtual bool isPlayerDetected()
		{	
			if(tiles_Detect.Contains(Player.Instance.tile_current))
			{
				return true;
			}			
			return false;
		}
		public virtual bool isPlayerDetected(Tile tile_player)
		{			
			if(tiles_Detect.Contains(tile_player))
			{
				return true;
			}			
			return false;
		}

		public void GenerateFOVTiles()
		{
			for(int i = 0; i < count; i++)
			{
				GameObject tile_obj = Instantiate(prefab_TileFoV, transform.position, Quaternion.Euler(Vector3.right * 90)) as GameObject;
				tile_obj.transform.parent = transform;
				tile_obj.name = "FOV_Tile_" + i;

				tiles_Visual.Add(tile_obj.GetComponent<TileFOV>());
			}
		}

		public void SetFoVTiles()
		{
			int count = 0;
			foreach(var t in tiles_Detect)
			{
				tiles_Visual[count].SetTile(t, TileState.EnemyDetect);
				count++;
			}

//			foreach(var t in tiles_View)
//			{
//				tiles_Visual[count].SetTile(t,TileState.EnemyView);
//				count++;
//			}

			while(count < this.count)
			{
				tiles_Visual[count].SetTile();
				count++;
			}
		}
		
		public void SetTilesState(TileState ts)
		{
			foreach(var t in tiles_Visual)
			{
				t.SetTileState(ts);
			}
		}	

		public void UpdateMapTiles(bool value)
		{
			foreach(var t in tiles_Detect)
			{
				t.isFoVDetect = value;
			}
			foreach(var t in tiles_View)
			{
				t.isFoVView = value;
			}
		}
	}
}
