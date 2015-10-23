namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	
	public class Dog : Enemy
	{
		public override EnemyType type {get{return EnemyType.Dog;}}
	}
}