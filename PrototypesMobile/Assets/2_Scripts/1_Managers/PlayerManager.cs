namespace TheVandals
{
	using System;
	using System.Collections;
	using UnityEngine;
	
	public class PlayerManager: Singleton<PlayerManager>
	{
		public float speed = 1;

		public CrossEntity cross_current = new CrossEntity();

		public enum MoveState
		{
			None,
			Moving
		}

		public MoveState moveState = MoveState.None;

		void Start()
		{
		}

		void Update()
		{
		}

		public IEnumerator MovePlayer(Vector3 destination)
		{
			moveState = MoveState.Moving;
			float t = 0;
			do
			{
				t += (Time.deltaTime * speed) / Vector3.Distance(transform.position, destination);
				transform.position = Vector3.Lerp(transform.position, destination, t);
				yield return null;
			}while(t <= 1);
			
			this.cross_current.SetTilesState(TileState.PlayerOn);
			moveState = MoveState.None;
		}

		public void SetPlayerPosition(CrossEntity cross_current)
		{
			if(!CrossEntity.ReferenceEquals(this.cross_current, null))
				this.cross_current.SetTilesState(TileState.Clear);

			this.cross_current = new CrossEntity(cross_current);
			this.cross_current.Init();
		}
	}
}