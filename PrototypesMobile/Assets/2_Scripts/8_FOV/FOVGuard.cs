﻿namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public class FOVGuard : FOV 
	{
		public override int count {get{return 11;}}
		public override int max_Step {get{return 3;}}

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
			default:
				pos2D_Detect = new int[,] { {0, 0}, {0, 1}, {1, 1}, {-1, 1}, {0, 2} };
				pos2D_View = new int[,] { {1, 0}, {-1, 0}, {2, 1}, {-2, 1}, {1, 2}, {-1, 2} };
				break;
			}
		}
	}
}