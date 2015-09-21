namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class InputManager : Singleton<InputManager>
	{
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape)) 
				Application.Quit(); 
		}		
		
		void OnTap(TapGesture gesture) 
		{
			if(TurnManager.Instance.turnState == TurnState.PlayerTurn && PlayerManager.Instance.moveState == MoveState.None)
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
			if(TurnManager.Instance.turnState == TurnState.PlayerTurn && 
			   PlayerManager.Instance.moveState == MoveState.None && 
			   !CameraManager.Instance.isScreenSizing)
			{
				PlayerManager.Instance.SetUnitPosition(
					MapManager.Instance.GetSwipeTilePosition(PlayerManager.Instance.cross_current, gesture.Direction));
			}
		}

		void OnPinch(PinchGesture gesture) 
		{
			if(TurnManager.Instance.turnState == TurnState.PlayerTurn)
				CameraManager.Instance.OnPinch(gesture);
		}
	}
}
