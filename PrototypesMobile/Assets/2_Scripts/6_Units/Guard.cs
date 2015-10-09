namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public class Guard : Enemy
	{		
		public override EnemyType enemy_Type {get{return EnemyType.Guard;}}
	}
}