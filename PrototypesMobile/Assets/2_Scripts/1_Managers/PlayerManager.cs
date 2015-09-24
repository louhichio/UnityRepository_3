namespace TheVandals
{
	using System;
	using System.Collections;
	using UnityEngine;

	public enum MoveState
	{
		None,
		Moving
	}
	
	public class PlayerManager: Singleton<PlayerManager>
	{
		public float speed = 1;
		public CrossEntity cross_current = new CrossEntity();
		public MoveState moveState = MoveState.None;
		[HideInInspector]
		public bool isInitialized = false;
		private Vector3 position_Init = Vector3.zero;
		private CrossEntity cross_init = new CrossEntity();

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameReset += Reset;
		}		
		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameReset -= Reset;
		}
		
		private void Init()
		{			
			MapManager.Instance.InitUnitCross(transform.position, gameObject);
		}
		private void Reset()
		{
			StopAllCoroutines();
			
			this.cross_current.ResetTiles();
			transform.position = position_Init;
			
			cross_current = new CrossEntity(cross_init);
			this.cross_current.SetTilesState(TileState.PlayerOn);
			moveState = MoveState.None;
		}
		#endregion

		#region Private
		private IEnumerator MovePlayerHorizontal(Vector3 destination)
		{
			
			Vector3 direction = destination - transform.position;
			direction.y = 0;
			transform.forward = direction.normalized * 90;

			destination.y += transform.localScale.y / 2;
			moveState = MoveState.Moving;
			float t = 0;
			while(t <= 1)
			{
				t += (Time.deltaTime * speed) / Vector3.Distance(transform.position, destination);
				transform.position = Vector3.Lerp(transform.position, destination, t);
				yield return null;
			}
			
			this.cross_current.SetTilesState(TileState.PlayerOn);
			moveState = MoveState.None;
			
			if(isInitialized)
			{
				if(cross_current.IsEnemyOnCross)
					GameManager.Instance.StartCoroutine("PlayerLost");
				else 
					if(this.cross_current.tile_center == MapManager.Instance.tile_GameOver)
						GameManager.Instance.StartCoroutine("PlayerWon");	
				else
					TurnManager.Instance.StartCoroutine("PlayerMoved");
			}
			else
			{				
				cross_init = this.cross_current;
				position_Init = destination;
				isInitialized = true;
				
				MapManager.Instance.SetGameOverTile(cross_init.tile_center);
			}
		}
		
		private IEnumerator MovePlayerVertical(bool isMovingUp)
		{			
			Vector3 destination = this.cross_current.tile_center.transform.position;

			Vector3 direction = destination - transform.position;
			direction.y =0;
			transform.forward = direction.normalized * 90;

			Vector3 node_first;
			if(isMovingUp)
			{
				node_first = transform.position;
				node_first.y = destination.y + transform.localScale.y / 2;
				destination.y = node_first.y;
			}
			else
			{
				node_first = destination;
				node_first.y = transform.position.y ;
				destination.y += transform.localScale.y / 2;
			}
			
			moveState = MoveState.Moving;
			
			float t = 0;
			while(t <= 1)
			{
				t += (Time.deltaTime * speed) / Vector3.Distance(transform.position, node_first);
				transform.position = Vector3.Lerp(transform.position, node_first, t);
				yield return null;
			}
			
			t = 0;
			while(t <= 1)
			{
				t += (Time.deltaTime * speed) / Vector3.Distance(transform.position, destination);
				transform.position = Vector3.Lerp(transform.position, destination, t);
				yield return null;
			}
			
			this.cross_current.SetTilesState(TileState.PlayerOn);
			moveState = MoveState.None;
			
			if(isInitialized)
			{
				if(cross_current.IsEnemyOnCross)
					GameManager.Instance.StartCoroutine("PlayerLost");
				else 
					if(this.cross_current.tile_center == MapManager.Instance.tile_GameOver)
						GameManager.Instance.StartCoroutine("PlayerWon");	
				else
					TurnManager.Instance.StartCoroutine("PlayerMoved");
			}
			else
			{				
				cross_init = this.cross_current;
				position_Init = destination;
				isInitialized = true;
				
				MapManager.Instance.SetGameOverTile(cross_init.tile_center);
			}
		}
		#endregion

		#region Public
		public void SetUnitPosition(CrossEntity cross_current)
		{
			if(!CrossEntity.ReferenceEquals(cross_current, null))
			{
				if(!CrossEntity.ReferenceEquals(this.cross_current, null))
					this.cross_current.SetTilesState(TileState.Clear);
				
				this.cross_current = new CrossEntity(cross_current);
				float nextPosHeight = cross_current.tile_center.transform.position.y;
				
				if(transform.position.y > nextPosHeight)
				{
					StopCoroutine("MovePlayerVertical");
					StartCoroutine("MovePlayerVertical", false);
				}
				else if(transform.position.y < nextPosHeight)
				{
					StopCoroutine("MovePlayerVertical");
					StartCoroutine("MovePlayerVertical", true);
				}
				else
				{
					StopCoroutine("MovePlayerHorizontal");
					StartCoroutine("MovePlayerHorizontal", this.cross_current.tile_center.transform.position);
				}
			}
		}
		#endregion
	}
}