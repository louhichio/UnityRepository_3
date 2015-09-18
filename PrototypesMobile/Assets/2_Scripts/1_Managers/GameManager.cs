namespace TheVandals
{
	using System;
	using UnityEngine;
	
	public class GameManager: Singleton<GameManager>
	{
		private bool onTap = false;
		private bool onSwipe = false;
		void Start()
		{
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape)) 
				Application.Quit(); 
			onTap = false;
			onSwipe = false;
			CameraManager.Instance.onPinch = false;
		}		
		
		void OnTap(TapGesture gesture) 
		{
			onTap = true;
			if(PlayerManager.Instance.moveState == MoveState.None)
			{
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity))
				{
					
					PlayerManager.Instance.SetPlayerPosition(
						MapManager.Instance.GetTapTilePosition(hit.point, PlayerManager.Instance.cross_current));
				}
			}
		}

		void OnSwipe( SwipeGesture gesture ) 
		{
			onSwipe = true;
			if(PlayerManager.Instance.moveState == MoveState.None && !CameraManager.Instance.isScreenSizing)
				PlayerManager.Instance.SetPlayerPosition(
					MapManager.Instance.GetSwipeTilePosition(PlayerManager.Instance.cross_current, gesture.Direction));
		}
	}
}