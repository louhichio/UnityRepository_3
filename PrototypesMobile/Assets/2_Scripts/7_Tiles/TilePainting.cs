namespace TheVandals
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;

	public enum PaintingType
	{
		Capture,
		Create
	}
	
	public class TilePainting : MonoBehaviour 
	{
		public PaintingType type;
		public int tile_index;

		[HideInInspector]
		public Tile tile;	
		[HideInInspector]
		public string paint_SpriteName;
		
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
			MapManager.Instance.GetMapTileByIndex(tile_index, ref tile);

			if(GetComponent<MeshRenderer>() && !Tile.ReferenceEquals(tile, null))
			{				
//				tile = MapManager.Instance.InitializeUnit(image_UI_CaptureMark.transform.position);
				
				if(type == PaintingType.Capture)
					tile.isCaptureOeuvre = true;

				Vector3 initPos;
				initPos = tile.transform.position;
				initPos.y = transform.position.y;
				
//				image_UI_CaptureMark.transform.position = initPos;
				
				paint_SpriteName = GetComponent<MeshRenderer>().material.mainTexture.name;

//				image_UI_CaptureMark.sprite = PaintingManager.Instance.GetPaintingSprite(paint_SpriteName);
			}
			else
				gameObject.SetActive(false);
		}
		#endregion
		
	}
}