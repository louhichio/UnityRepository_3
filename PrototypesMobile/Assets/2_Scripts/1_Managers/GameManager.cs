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
			if(Input.GetMouseButtonDown(0))
				TouchedScreen();
		}

		private void TouchedScreen()
		{
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(mouseRay, out hit,Mathf.Infinity))
			{
				Vector3 NavPoint = MapManager.Instance.GetTilePosition(hit.point);

				PlayerManager.Instance.MovePlayer(NavPoint);
			}
		}
	}
}