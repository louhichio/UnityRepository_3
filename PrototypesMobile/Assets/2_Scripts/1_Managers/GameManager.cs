namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class GameManager: Singleton<GameManager>
	{
		void Start()
		{
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape)) 
				Application.Quit(); 
		}		
		
		void OnTap(TapGesture gesture) 
		{
			if(PlayerManager.Instance.moveState == PlayerManager.MoveState.None)
			{
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if (Physics.Raycast(mouseRay, out hit,Mathf.Infinity))
				{
					MapManager.Instance.GetTileByCross(hit.point, PlayerManager.Instance.cross_current);
				}
			}
		}

		void OnSwipe( SwipeGesture gesture ) 
		{
			FingerGestures.SwipeDirection direction = gesture.Direction;

			if(direction != FingerGestures.SwipeDirection.Up || 
			   direction != FingerGestures.SwipeDirection.Down ||
			   direction != FingerGestures.SwipeDirection.Right || 
			   direction != FingerGestures.SwipeDirection.Left)
			{
				if(PlayerManager.Instance.moveState == PlayerManager.MoveState.None)
					MapManager.Instance.GetSwipeTilePosition(PlayerManager.Instance.cross_current.tile_center.index, direction);
			}
		}
	}
}