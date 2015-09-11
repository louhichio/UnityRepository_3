namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class PlayerManager: Singleton<PlayerManager>
	{
		public Transform player;

		void Start()
		{
		}
		
		void Update()
		{
		}

		public void MovePlayer(Vector3 position)
		{
			player.position = position;
		}
	}
}