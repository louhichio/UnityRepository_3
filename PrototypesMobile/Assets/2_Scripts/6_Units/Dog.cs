namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	
	public class Dog : Enemy
	{
		public override void Init()
		{			
			enemy_Type = EnemyType.Dog;
			enemy_Behaviour = EnemyBehaviour.Roam;
			
			Initialize(MapManager.Instance.InitializeUnit(transform.position, gameObject));
			
			tile_current.AddUnit(this);
			
			if(enemy_Behaviour != EnemyBehaviour.Idle)
				TurnManager.Instance.enemyCount_Max++;
			
			fov = GetComponent<FOV>();
			fov.Initialize(tile_current);
		}
	}
}