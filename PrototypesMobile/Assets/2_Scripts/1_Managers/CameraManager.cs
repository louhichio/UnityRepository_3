﻿namespace TheVandals
{
	using UnityEngine;
	using System;
	
	public class CameraManager : Singleton<CameraManager>
	{
		public Vector2 ratio = new Vector2(10.0f, 10.0f);
		#region Unity
		void Start()
		{					
		}

		void Update()
		{
			SetCameraAspect();
		}		
		
		void OnPinch(PinchGesture gesture) 
		{	
			// Current gap distance between the two fingers
			float gap = gesture.Gap;
			gap = Mathf.Clamp(4.80f + gap,5,7);
			Camera.main.orthographicSize = gap;
		}
		#endregion
		#region Private
		private void SetCameraAspect()
		{
			// normalement 16/9
			float targetaspect = ratio.x / ratio.y;
			
			// determine the game window's current aspect ratio
			float windowaspect = (float)Screen.width / (float)Screen.height;
			
			// current viewport height should be scaled by this amount
			float scaleheight = windowaspect / targetaspect;
			
			// obtain camera component so we can modify its viewport
			Camera camera = GetComponent<Camera>();
			
			// if scaled height is less than current height, add letterbox
			if (scaleheight < 1.0f)
			{
				Rect rect = camera.rect;
				
				rect.width = 1.0f;
				rect.height = scaleheight;
				rect.x = 0;
				rect.y = (1.0f - scaleheight) / 2.0f;
				
				Camera.main.rect = rect;
			}
			else // add pillarbox
			{
				float scalewidth = 1.0f / scaleheight;
				
				Rect rect = camera.rect;
				
				rect.width = scalewidth;
				rect.height = 1.0f;
				rect.x = (1.0f - scalewidth) / 2.0f;
				rect.y = 0;
				
				Camera.main.rect = rect;
			}
		}		
		#endregion
	}
}
