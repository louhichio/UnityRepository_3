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

			if(Input.GetButtonDown("Fire1"))
			{
				if(!isStop && 
				   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
				   Player.Instance.moveState == MoveState.None)
				{
					Tile t = RetrieveTouchTilePosition();
					if(t != null)
						Player.Instance.TouchOnDestinationTile(t);
				}
			}
		}		
		
		void OnTap(TapGesture gesture) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
			   Player.Instance.moveState == MoveState.None)
			{
				Tile t = RetrieveTouchTilePosition();

				if(t != null && t != Player.Instance.tile_current)					
					Player.Instance.TravelTo(t);					
				else
				{
					Player.Instance.TravelFinished();
					if(t == Player.Instance.tile_current)
						TurnManager.Instance.StartCoroutine("PlayerMoved");
				}
			}
		}
		
		void OnSwipe(SwipeGesture gesture) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn && 
			   Player.Instance.moveState == MoveState.None && 
			   !CameraManager.Instance.isScreenSizing)
			{
				Player.Instance.TravelTo(
					MapManager.Instance.GetSwipeTilePosition(Player.Instance.tile_current, gesture.Direction));
			}
		}

		void OnPinch(PinchGesture gesture) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn)
				CameraManager.Instance.OnPinch(gesture);
		}

		void OnDrag( DragGesture gesture ) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
			   Player.Instance.moveState == MoveState.None)
			{
				Tile t = RetrieveTouchTilePosition();
				if(t != null)
					Player.Instance.TouchOnDestinationTile(t);

				if(gesture.State == GestureRecognitionState.Ended)
				{					
					if(t != null && t != Player.Instance.tile_current)					
						Player.Instance.TravelTo(t);					
					else
					{
						Player.Instance.TravelFinished();
						if(t == Player.Instance.tile_current)
							TurnManager.Instance.StartCoroutine("PlayerMoved");
					}
				}
			}
		}

		private Tile RetrieveTouchTilePosition()
		{
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity))
			{
				return MapManager.Instance.GetTapTilePosition(hit.point, Player.Instance.tile_current, Player.Instance.step_Max);
			}
			return null;
		}
	}
}
