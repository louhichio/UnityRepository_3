namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public enum TileCollectableType
	{
		CaptureOeuvre,
		CreateOeuvre,
	}
	public class TileCollectable : MonoBehaviour 
	{
		public TileCollectableType type = TileCollectableType.CaptureOeuvre;

		[Header("UI Sprite")]
		public Sprite painting_Sprite;

		[Header("World Sprite")]
		public SpriteRenderer painting_SpriteWorld;

		private MeshRenderer mesh_renderer;
		[HideInInspector]
		public Tile tile;


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
			mesh_renderer = GetComponent<MeshRenderer>();

			Vector3 initPos;
			
			tile = MapManager.Instance.InitializeUnit(transform.position);			
			tile.isCollectible = true;
			
			if(painting_Sprite)
			{
				if(type == TileCollectableType.CaptureOeuvre)
					tile.isCaptureOeuvre = true;
				
				if(type == TileCollectableType.CreateOeuvre)
				{
					tile.isCreateOeuvre = true;
					painting_SpriteWorld.enabled = false;
				}
			}

			initPos = tile.transform.position;
			initPos.y = transform.position.y;
			
			transform.position = initPos;
		}
		private void Reset()
		{					
			tile.isCollectible = true;

			if(painting_Sprite)
			{
				if(type == TileCollectableType.CaptureOeuvre)
					tile.isCaptureOeuvre = true;
				
				if(type == TileCollectableType.CreateOeuvre)
				{
					tile.isCreateOeuvre = true;
					painting_SpriteWorld.enabled = false;
				}
			}
			SetActive(true);
		}
		#endregion

		public void SetActive(bool isActive)
		{
			if(mesh_renderer)				
				mesh_renderer.enabled = isActive;
			else
			{
				foreach(Transform child in transform)
					if(child.GetComponent<MeshRenderer>())
						child.GetComponent<MeshRenderer>().enabled = isActive;
			}
		}

		public void DrawSprite()
		{			
			painting_SpriteWorld.enabled = true;
		}
	}
}