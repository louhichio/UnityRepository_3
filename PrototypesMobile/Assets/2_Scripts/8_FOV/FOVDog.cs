namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class FOVDog : FOV 
	{
		public override int count {get{return 21;}}		
		public override int max_Step {get{return 4;}}
		
		public override void SetFovDirection(int Angle)
		{
			switch (Angle)
			{
			case 0:
				pos2D_Detect = new int[,] { {0, 0}, {0, 1}, {0, -1}, {1, 0}, {1, 1}, {1, -1}, {2, 0}, {2, 1}, {2, -1}, {3, 0} };
				pos2D_View = new int[,] { {-1, 0}, {-1, -1}, {-1, 1}, {0, -2}, {0, 2},{1, -2}, {1, 2},{2, -2}, {2, 2}, {3, -1}, {3, 1} };
				break;
			case 90:
				pos2D_Detect = new int[,] { {0, 0}, {1, 0}, {-1, 0}, {0, 1}, {1, 1}, {-1, 1}, {0, 2}, {1, 2}, {-1, 2}, {0, 3} };
				pos2D_View = new int[,] { {0, -1}, {-1, -1}, {1, -1}, {-2, 0}, {2, 0},{-2, 1}, {2, 1},{-2, 2}, {2, 2}, {-1, 3}, {1, 3} };
				break;
			case 180:
				pos2D_Detect = new int[,] { {0, 0}, {0, -1}, {0, 1}, {-1, 0}, {-1, -1}, {-1, 1}, {-2, 0}, {-2, -1}, {-2, 1}, {-3, 0} };
				pos2D_View = new int[,] { {1, 0}, {1, 1}, {1, -1}, {0, 2}, {0, -2},{-1, 2}, {-1, -2},{-2, 2}, {-2, -2}, {-3, 1}, {-3, -1} };
				break;
			case 270:
				pos2D_Detect = new int[,] { {0, 0}, {-1, 0}, {1, 0}, {0, -1}, {-1, -1}, {1, -1}, {0, -2}, {-1, -2}, {1, -2}, {0, -3} };
				pos2D_View = new int[,] { {0, 1}, {1, 1}, {-1, 1}, {2, 0}, {-2, 0},{2, -1}, {-2, -1},{2, -2}, {-2, -2}, {1, -3}, {-1, -3} };
				break;
			default:
				pos2D_Detect = new int[,] { {0, 0}, {1, 0}, {-1, 0}, {0, 1}, {1, 1}, {-1, 1}, {0, 2}, {1, 2}, {-1, 2}, {0, 3} };
				pos2D_View = new int[,] { {0, -1}, {-1, -1}, {1, -1}, {-2, 0}, {2, 0},{-2, 1}, {2, 1},{-2, 2}, {2, 2}, {-1, 3}, {1, 3} };
				break;
			}
		}

		public override void EnableFov(Tile tile, int angle)
		{
			SetFovDirection(angle);
			
			tiles_Neighbours = tile.GetTilesWithinCost(max_Step, tile.transform.position.y);
			
			NeighboursRestriction(MapManager.Instance.GetFloorTile(pos2D_Detect, tile), ref tiles_Detect, angle);
			
			UpdateMapTiles(true);
			
			SetFoVTiles();
		}
	}
}