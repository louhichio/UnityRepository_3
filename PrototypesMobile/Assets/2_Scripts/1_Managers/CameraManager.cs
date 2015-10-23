﻿namespace TheVandals
{
	using UnityEngine;
	using System;
	using System.Collections;
	using UnityStandardAssets.ImageEffects;
	
	public class CameraManager : Singleton<CameraManager>
	{
		#region Properties
		[SerializeField]
		private Vector2 ratio = new Vector2(10.0f, 10.0f);
		[SerializeField]
		private float screen_Size_Max = 7;	
		[SerializeField]
		private float screen_Size_Center = 5;
		[SerializeField]
		private float screen_Size_Min = 3;

		private float screen_Size;
		private float Screen_Diagonal;

		[SerializeField]
		private bool followPlayer = false;
		private Vector3 initPos;

		[SerializeField]
		private float follow_Speed = 1;
		[SerializeField]
		public float boundaries_xMin;
		[SerializeField]
		public float boundaries_xMax;
		[SerializeField]
		public float boundaries_zMin;
		[SerializeField]
		public float boundaries_zMax;
		[SerializeField]
		private Vector3 directionInverse;

		private Blur blur;

		private bool isPaused = false;
		#endregion

		#region Unity
		void Update()
		{
//			SetCameraAspect();
			if(!isPaused)
			{
				if(followPlayer && !Vector3.ReferenceEquals(directionInverse, null))
				{
					Vector3 destinationPos = Player.Instance.transform.position;
					destinationPos.x = Mathf.Clamp(destinationPos.x, boundaries_xMin, boundaries_xMax);
					destinationPos.z = Mathf.Clamp(destinationPos.z, boundaries_zMin, boundaries_zMax);

					destinationPos += directionInverse;
					
					if(Vector3.Distance(destinationPos, transform.position) > 0.1f)		
						transform.position = Vector3.Lerp(transform.position, destinationPos, Time.deltaTime * follow_Speed);
				}
				
				Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, screen_Size, Time.deltaTime * 3);
			}
		}		
		#endregion
		
		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameReset += Reset;
			EventManager.pause += Pause;
			EventManager.resume += Resume;
		}		
		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameReset -= Reset;
			EventManager.pause -= Pause;
			EventManager.resume -= Resume;
		}
		
		private void Init()
		{			
			screen_Size = screen_Size_Center;
			Screen_Diagonal = Mathf.Sqrt(Mathf.Pow(Screen.width,2) + Mathf.Pow(Screen.height,2));
			Camera.main.orthographicSize = screen_Size;

			directionInverse = transform.forward * -40;
			
			if(followPlayer)
			{
				initPos = Player.Instance.transform.position;

				initPos.x = Mathf.Clamp(initPos.x, boundaries_xMin, boundaries_xMax);
				initPos.z = Mathf.Clamp(initPos.z, boundaries_zMin, boundaries_zMax);

				initPos += directionInverse;

				transform.position = initPos;
			}

			blur = GetComponent<Blur>();
		}
		private void Reset()
		{
			transform.position = initPos;
		}

		public void Pause()
		{
			isPaused = true;
		}
		
		public void Resume()
		{
			isPaused = false;
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

		#region Public		
		public void OnPinch(PinchGesture gesture) 
		{	
			if(gesture.Phase == ContinuousGesturePhase.Ended)
			{
				screen_Size = screen_Size_Center;
				return;
			}

			float delta = gesture.Delta * 10/ Screen_Diagonal;
			screen_Size = Mathf.Clamp(screen_Size - delta, screen_Size_Min, screen_Size_Max);
		}
		#endregion
	}
}
