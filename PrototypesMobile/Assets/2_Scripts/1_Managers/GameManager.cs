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
			if(PlayerManager.Instance.moveState == MoveState.None)
				PlayerManager.Instance.SetPlayerPosition(
					MapManager.Instance.GetSwipeTilePosition(PlayerManager.Instance.cross_current, gesture.Direction));

		}
	}
}