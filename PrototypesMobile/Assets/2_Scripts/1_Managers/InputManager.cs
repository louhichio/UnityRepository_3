namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class InputManager : Singleton<InputManager>
	{
		private bool isStop = false;
		#region Events
		void OnEnable()
		{
			EventManager.gameOver += GameOver;
			EventManager.gameReset += GameReset;
		}		
		void OnDisable()
		{
			EventManager.gameOver -= GameOver;
			EventManager.gameReset -= GameReset;
		}
		private void GameOver(string status)
		{
			isStop = true;
		}
		
		private void GameReset()
		{
			isStop = false;
		}
		#endregion

		void Update()
		{
			if (!isStop && Input.GetKeyDown(KeyCode.Escape)) 
				Application.Quit();
		}		
		
		void OnTap(TapGesture gesture) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
			   PlayerManager.Instance.moveState == MoveState.None)
			{
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity))
				{				
					PlayerManager.Instance.SetUnitPosition(
						MapManager.Instance.GetTapTilePosition(hit.point, PlayerManager.Instance.cross_current));
				}
			}
		}
		
		void OnSwipe( SwipeGesture gesture ) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn && 
			   PlayerManager.Instance.moveState == MoveState.None && 
			   !CameraManager.Instance.isScreenSizing)
			{
				PlayerManager.Instance.SetUnitPosition(
					MapManager.Instance.GetSwipeTilePosition(PlayerManager.Instance.cross_current, gesture.Direction));
			}
		}

		void OnPinch(PinchGesture gesture) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn)
				CameraManager.Instance.OnPinch(gesture);
		}
	}
}
