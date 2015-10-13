namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	
	public class TileHide : MonoBehaviour 
	{
		public int index;

		
		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
		}		
		
		void OnDisable()
		{
			EventManager.initialise -= Init;
		}
		
		private void Init()
		{			
			Vector3 initPos;
			Tile t;

			t = MapManager.Instance.InitializeUnit(transform.position);
			
			t.isHide = true;

			initPos = t.transform.position;
			initPos.y += (transform.localScale.y / 2) + 0.002f;

			transform.position = initPos;
			index = t.index;
		}
		#endregion

	}
}
