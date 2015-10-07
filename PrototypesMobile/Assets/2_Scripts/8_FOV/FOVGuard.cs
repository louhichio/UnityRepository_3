namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public class FOVGuard : FOV 
	{
		public override void Initialize(Tile tile)
		{
			max_Step = 3;
			//////////// [Z,X] //////////
			pos2D_Detect = new int[,] { {0, 0}, {0, 1}, {1, 1}, {-1, 1}, {0, 2} };
			pos2D_View = new int[,] { {1, 0}, {-1, 0}, {2, 1}, {-2, 1}, {1, 2}, {-1, 2} };

			SetFovDirection((int)transform.eulerAngles.y);

			tiles_Neighbours = tile.GetTilesWithinCost(max_Step);

			NeighboursRestriction(MapManager.Instance.GetFloorTile(pos2D_Detect, tile), ref tiles_Detect);
			NeighboursRestriction(MapManager.Instance.GetFloorTile(pos2D_View, tile), ref tiles_View);

			SetTilesState(tiles_Detect, TileState.EnemyDetect);
			SetTilesState(tiles_View, TileState.EnemyView);
		}

		public override void SetFovDirection(int Angle)
		{
			switch (Angle)
			{
			case 0:
				pos2D_Detect = new int[,] { {0, 0}, {1, 0}, {1, 1}, {1, -1}, {2, 0} };
				pos2D_View = new int[,] { {0, 1}, {0, -1}, {1, 2}, {1, -2}, {2, 1}, {2, -1} };
				break;
			case 90:
				pos2D_Detect = new int[,] { {0, 0}, {0, 1}, {1, 1}, {-1, 1}, {0, 2} };
				pos2D_View = new int[,] { {1, 0}, {-1, 0}, {2, 1}, {-2, 1}, {1, 2}, {-1, 2} };
				break;
			case 180:
				pos2D_Detect = new int[,] { {0, 0}, {-1, 0}, {-1, -1}, {-1, 1}, {-2, 0} };
				pos2D_View = new int[,] { {0, -1}, {0, 1}, {-1, -2}, {-1, 2}, {-2, -1}, {-2, 1} };
				break;
			case 270:
				pos2D_Detect = new int[,] { {0, 0}, {0, -1}, {-1, -1}, {1, -1}, {0, -2} };
				pos2D_View = new int[,] { {-1, 0}, {1, 0}, {-2, -1}, {2, -1}, {-1, -2}, {1, -2} };
				break;
			}
		}
	}
}