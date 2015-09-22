namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public class Enemy : MonoBehaviour 
	{
		public float speed = 1;
		public CrossEntity cross_current = new CrossEntity();
		public MoveState moveState = MoveState.None;

		private bool isInitialized = false;

		private CrossEntity cross_init = new CrossEntity();
		private Vector3 position_Init = Vector3.zero;

		#region Unity
		#endregion

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.startTurn_Enemy += StartTurn;
			EventManager.gameReset += Reset;
		}		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.startTurn_Enemy -= StartTurn;
			EventManager.gameReset -= Reset;
		}

		private void Init()
		{			
			MapManager.Instance.InitUnitCross(transform.position, gameObject);
			TurnManager.Instance.enemyCount_Max++;
		}
		private void StartTurn()
		{
			FingerGestures.SwipeDirection dir = new FingerGestures.SwipeDirection();
			dir = (FingerGestures.SwipeDirection)(1 << Random.Range(0,3));

			CrossEntity cross_destination = MapManager.Instance.GetSwipeTilePosition(cross_current, dir);
			SetUnitPosition(cross_destination);
		}
		private void Reset()
		{
			StopAllCoroutines();

			this.cross_current.ResetTiles();
			transform.position = position_Init;

			cross_current = new CrossEntity(cross_init);
			this.cross_current.SetTilesState(TileState.EnemyOn);
			moveState = MoveState.None;
		}
		#endregion

		#region Private			
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
			
			this.cross_current.SetTilesState(TileState.EnemyOn);
			moveState = MoveState.None;

			if(isInitialized)
			{
				if(cross_current.IsPlayerOnCross)
					GameManager.Instance.StartCoroutine("PlayerLost");
				else
				{
					TurnManager.Instance.StopCoroutine("EnemyMoved");
					TurnManager.Instance.StartCoroutine("EnemyMoved");
				}
			}
			else
			{				
				cross_init = this.cross_current;
				position_Init = destination;
				isInitialized = true;
			}
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
			
			this.cross_current.SetTilesState(TileState.EnemyOn);
			moveState = MoveState.None;

			if(isInitialized)
			{
				if(cross_current.IsPlayerOnCross)
					GameManager.Instance.StartCoroutine("PlayerLost");
				else
				{
					TurnManager.Instance.StopCoroutine("EnemyMoved");
					TurnManager.Instance.StartCoroutine("EnemyMoved");
				}
			}
			else
			{				
				cross_init = this.cross_current;
				position_Init = destination;
				isInitialized = true;
			}
		}
		
		public void SetUnitPosition(CrossEntity cross_current)
		{
			if(!CrossEntity.ReferenceEquals(cross_current, null))
			{
				if(!CrossEntity.ReferenceEquals(this.cross_current, null))
				{
					this.cross_current.SetTilesState(TileState.Clear);
				}
				
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
			else
			{
				TurnManager.Instance.StopCoroutine("EnemyMoved");
				TurnManager.Instance.StartCoroutine("EnemyMoved");
			}
		}
		#endregion
	}
}
