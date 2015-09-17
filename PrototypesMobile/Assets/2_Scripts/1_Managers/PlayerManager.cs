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

		void Start()
		{
		}

		void Update()
		{
		}

		private IEnumerator MovePlayerHorizontal(Vector3 destination)
		{

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
		}

		private IEnumerator MovePlayerVertical(bool isMovingUp)
		{
			Vector3 destination = this.cross_current.tile_center.transform.position;
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
		}

		public void SetPlayerPosition(CrossEntity cross_current)
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
	}
}