namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class InputManager : Singleton<InputManager>
	{
		private bool isStop = false;
		[SerializeField]
		private bool isFingerClear = true;
		[SerializeField]
		private bool waitButtonUp = false;

		public bool isCapturing = false;
		
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
			isCapturing = false;
		}
		#endregion

		void Update()
		{
			if(Input.touchCount > 1 )
				isFingerClear = false;
			
			if(!isFingerClear && Input.touchCount == 0 && !waitButtonUp)
				isFingerClear = true;

			if (!isStop && Input.GetKeyDown(KeyCode.Escape)) 
				Application.Quit();

			if(Input.GetButtonDown("Fire1"))
			{
				if(!isStop && 
				   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
				   Player.Instance.moveState == MoveState.None && 
				   isFingerClear)
				{
					Tile t = RetrieveTouchTilePosition();
					if(t != null)
						Player.Instance.TouchOnDestinationTile(t);
				}
				else
				{
					if(isStop)
					{
						GameManager.Instance.reset = true;
						Player.Instance.bustedCount = 0;
						Player.Instance.detectedCount = 0;
						isFingerClear = false;
						waitButtonUp = true;
					}
				}
				if(isCapturing)
				{
					UIManager.Instance.EndCapture();
					isCapturing = false;
				}
			}else if(waitButtonUp && Input.GetButtonUp("Fire1"))
				waitButtonUp = false;
		}		
		
		void OnTap(TapGesture gesture) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
			   Player.Instance.moveState == MoveState.None && 
			   isFingerClear)
			{
				Tile t = RetrieveTouchTilePosition();

				if(t != null)					
					Player.Instance.TravelTo(t);		
			}
		}
		
		void OnSwipe(SwipeGesture gesture) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn && 
			   Player.Instance.moveState == MoveState.None && 
			   isFingerClear)
			{
				Player.Instance.TravelTo(
					MapManager.Instance.GetSwipeTilePosition(Player.Instance.tile_current, gesture.Direction));
			}
		}

		void OnPinch(PinchGesture gesture) 
		{
			Player.Instance.DisableDestinationTile();
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn)
				CameraManager.Instance.OnPinch(gesture);
		}

		void OnDrag( DragGesture gesture ) 
		{
			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
			   Player.Instance.moveState == MoveState.None &&
			   isFingerClear)
			{
				Tile t = RetrieveTouchTilePosition();

				if(t != null)
					Player.Instance.TouchOnDestinationTile(t);		
				else					
					Player.Instance.DisableDestinationTile();
				
				if(gesture.State == GestureRecognitionState.Ended)
				{		
					if(t != null)
					{
						Player.Instance.TouchOnDestinationTile(t);
						Player.Instance.TravelTo(t);						
					}
					else					
						Player.Instance.DisableDestinationTile();
				}
			}
			
		}
		
		private Tile RetrieveTouchTilePosition()
		{
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity))
			{
				return MapManager.Instance.GetTapTilePosition(hit.point, Player.Instance.tile_current, Player.Instance.stepsLeft);
			}
			return null;
		}
	}
}
