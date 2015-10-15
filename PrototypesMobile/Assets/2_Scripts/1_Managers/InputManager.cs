namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class InputManager : Singleton<InputManager>
	{
		private bool isStop = false;
		[SerializeField]
		private bool isPinching = false;
		[SerializeField]
		private bool isTap = false;
		[SerializeField]
		private bool isSwipe = false;
		[SerializeField]
		private bool isDrag = false;

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
				   Player.Instance.moveState == MoveState.None && 
				   !isPinching)
				{
					Tile t = RetrieveTouchTilePosition();
					if(t != null)
						Player.Instance.TouchOnDestinationTile(t);
				}
			}

			print ("isPinching: " + isPinching + 
			       "  isTap: " + isTap +
			       "  isSwipe:  " + isSwipe +
			       "  isDrag:  " + isDrag + 
			       "  TouchCount:  " + Input.touchCount);
			isTap = false;
		}		
		
		void OnTap(TapGesture gesture) 
		{
			if(!isTap)
				isTap = true;

			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
			   Player.Instance.moveState == MoveState.None && 
			   !isPinching)
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
//			if(gesture.State == GestureRecognitionState.Ended)
//				isTap = false;
		}
		
		void OnSwipe(SwipeGesture gesture) 
		{
			if(!isSwipe)
				isSwipe = true;

			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn && 
			   Player.Instance.moveState == MoveState.None && 
			   !isPinching)
			{
				Player.Instance.TravelTo(
					MapManager.Instance.GetSwipeTilePosition(Player.Instance.tile_current, gesture.Direction));
			}
			if(gesture.State == GestureRecognitionState.Ended)
				isSwipe = false;
		}

		void OnPinch(PinchGesture gesture) 
		{
			if(!isPinching)
			{
				isPinching = true;
				Player.Instance.DisableDestinationTile();
			}

			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn)
				CameraManager.Instance.OnPinch(gesture);

			if(gesture.State == GestureRecognitionState.Ended)
				isPinching = false;
		}

		void OnDrag( DragGesture gesture ) 
		{
			if(!isDrag)
				isDrag = true;

			if(!isStop && 
			   TurnManager.Instance.turnState == TurnState.PlayerTurn &&
			   Player.Instance.moveState == MoveState.None &&
			   !isPinching)
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

			if(gesture.State == GestureRecognitionState.Ended)
				isDrag = false;
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
