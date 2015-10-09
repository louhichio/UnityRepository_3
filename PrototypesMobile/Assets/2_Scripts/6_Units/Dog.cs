namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	
	public class Dog : Enemy
	{
		public override EnemyType enemy_Type {get{return EnemyType.Dog;}}
	}
}