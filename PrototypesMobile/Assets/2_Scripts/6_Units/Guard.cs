namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public class Guard : Enemy
	{		
		public override void Init()
		{			
			enemy_Type = EnemyType.Guard;
			enemy_Behaviour = EnemyBehaviour.Roam;

			Initialize(MapManager.Instance.InitializeUnit(transform.position, gameObject));

			tile_current.AddUnit(this);

			if(transform.eulerAngles.y != 0 && transform.eulerAngles.y != 90 && transform.eulerAngles.y != 180 && transform.eulerAngles.y != 270 )
				transform.rotation = Quaternion.Euler(Vector3.zero);
			
			if(enemy_Behaviour != EnemyBehaviour.Idle)
				TurnManager.Instance.enemyCount_Max++;

			fov = GetComponentInChildren<FOV>();
			fov.Initialize(tile_current);
			
			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.EnemyOn);
		}
	}
}