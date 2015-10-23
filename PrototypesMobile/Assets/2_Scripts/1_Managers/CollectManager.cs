namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	
	public class CollectManager : Singleton<CollectManager> 
	{
		#region Properties
		[HideInInspector]
		public int collectables_Count;
		[HideInInspector]
		public int collected;
		private List<TileCollectable> list_Collectables = new List<TileCollectable>();
		
		//		private List<PaintingEntity> listPaintingsInJson = new List<PaintingEntity>();
		#endregion

		#region Unity
		void Awake() 
		{
//			ReadJSON rj = new ReadJSON();
//			listPaintingsInJson = rj.ReadJson();
		}
		#endregion

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameReset += Reset;
		}		
		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameReset += Reset;
		}
		
		private void Init()
		{			
			collectables_Count = transform.childCount;
			collected = 0;

			foreach(Transform child in transform)
				list_Collectables.Add(child.GetComponent<TileCollectable>());
		}
		private void Reset()
		{					
			collected = 0;
		}
		#endregion

		public void PlayerCollectedObj(Tile t)
		{
			EventManager.Instance.Pause();
			collected++;
			TileCollectable tc = list_Collectables.Find(x=>x.tile == t);
			tc.SetActive(false);
			UIManager.Instance.UpdatePlayerInfoCollectables(collected, collectables_Count);
			UIManager.Instance.StartCoroutine("StartCaptureOeuvre", tc.painting_Sprite);
		}
	}
}
