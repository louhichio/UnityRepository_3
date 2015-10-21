namespace TheVandals
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using System.Collections.Generic;

	public class PaintingManager : Singleton<PaintingManager>
	{
		#region Properties
		[Header("Configuration")]
		public List<Sprite> listPaintingsSprite = new List<Sprite>();
		
//		private List<PaintingEntity> listPaintingsInJson = new List<PaintingEntity>();
		private List<TilePainting> listTilePaintings = new List<TilePainting>();

		private int paintingsInScene = 0;
		private int paintingsCaptured = 0;
		#endregion
		#region Unity
		void Awake() 
		{
//			ReadJSON rj = new ReadJSON();
//			listPaintingsInJson = rj.ReadJson();

			SetPaintingsCountInScene();
		}
		#endregion
		#region Private
		private void SetPaintingsCountInScene()
		{
			foreach (Transform child in transform)
			{
				if(child.gameObject.activeSelf && child.GetComponent<TilePainting>())
				{
					listTilePaintings.Add(child.GetComponent<TilePainting>());
					paintingsInScene += 1;
				}
			}
			UIManager.Instance.UpdatePaintingsInfo(paintingsCaptured, paintingsInScene);
		}
		#endregion

		#region Public
		public Sprite GetPaintingSprite(string paintingName)
		{
			return listPaintingsSprite.Find(x => x.name.Equals(paintingName));
		}
//		public void setPainting(string paintingName, Transform paintingTarget)
//		{
//			this.painting = paintingName;
//			
//			paintingToShow.sprite = GetPaintingSprite(paintingName);
//			
//			if(captureType == CaptureOeuvreType.TypeRTS)
//			{
//				panel_CaptureOeuvreRTS.SetActive(true);
//				StopCoroutine ("CaptureOeuvreCoroutineRTS");
//				StartCoroutine("CaptureOeuvreCoroutineRTS", paintingTarget);
//			}
//		}
		public void PlayerCapturedPainting(Tile t)
		{
			TilePainting tp = listTilePaintings.Find(x=>x.tile == t);
			paintingsCaptured++;
//			tp.image_UI_CaptureMark.enabled = false;
			UIManager.Instance.UpdatePaintingsInfo(paintingsCaptured, paintingsInScene);
			UIManager.Instance.StartCaptureOeuvre(GetPaintingSprite(tp.paint_SpriteName));
		}


		#endregion
	}
}
