namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	
	public class TileEndGame : MonoBehaviour 
	{
		int index;
	}
}

//public override void Initialize (int index)
//{
//	Name = "Floor";
//	TraversalCost = 1;
//	IsTraversable = true;
//	ActualBoundaryHeights = new float[]{1f, 1f, 1f, 1f};
//	VirtualBoundaryHeights = new float[]{10f, 10f, 10f, 10f};
//	CentreUnitOffset =  new Vector3(0, 1f, 0);
//	CentreUnitRotation = new Vector3(0, 0, 0);
//	this.index = index;
//	
//	int[] arrayCoordinate = ArrayCoordinateFromPosition(transform.position);
//	
//	X = arrayCoordinate[0];
//	Y = arrayCoordinate[1];
//	Z = arrayCoordinate[2];
//	
//	isEnemyOn = false;
//	isPlayerOn = false;
//	enemyCount = 0;
//}
//
//public override void Reset ()
//{
//	isEnemyOn = false;
//	isPlayerOn = false;
//	enemyCount = 0;
//}