namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	
	public class TileCollectable : MonoBehaviour 
	{
		[HideInInspector]
		public Tile tile;
		private MeshRenderer mesh_renderer;

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
			
			initPos = tile.transform.position;
			initPos.y = transform.position.y;
			
			transform.position = initPos;
		}
		private void Reset()
		{					
			tile.isCollectible = true;
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
					child.GetComponent<MeshRenderer>().enabled = isActive;
			}
		}
	}
}