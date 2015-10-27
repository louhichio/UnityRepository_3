namespace TheVandals
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityStandardAssets.ImageEffects;
	
	public class CameraManager : Singleton<CameraManager>
	{
		#region Properties
		
		
		[Header("SetCameraAspect")]	
		[SerializeField]
		private Vector2 ratio = new Vector2(10.0f, 10.0f);

		[Header("Zoom")]	
		[SerializeField]
		private float screen_Size_Max = 7;	
		[SerializeField]
		private float screen_Size_Center = 5;
		[SerializeField]
		private float screen_Size_Min = 3;
		[SerializeField]
		private float zoomSpeed = 2.5f;

		private float screen_Size;
		private float Screen_Diagonal;

		[Header("Follow Player")]
		[SerializeField]
		private bool followPlayer = false;
		[SerializeField]
		private float follow_Speed = 1;	
		
		[Header("Boundaries")]
		[SerializeField]
		private bool CenterCamera = false;
		[SerializeField]
		private float boundaries_xMin;
		[SerializeField]
		private float boundaries_xMax;
		[SerializeField]
		private float boundaries_zMin;
		[SerializeField]
		private float boundaries_zMax;
		[SerializeField]
		private Vector3 CameraOffSet;


		private Vector3 initPos;
		private bool isPaused = false;	

		private Vector3 point1;
		private Vector3 point2;
		private Vector3 point3;
		private Vector3 point4;

		private Vector3 restrictPoint_UpLeft;
		private Vector3 restrictPoint_UpRight;
		private Vector3 restrictPoint_BotLeft;	
		private Vector3 restrictPoint_BotRight;

		private Vector2 min = Vector2.zero;
		private Vector2 max = Vector2.zero;

		private Vector3 cameraRelative;		
		private Vector3 Difference;
		private Vector3 directionInverse;	
		private Vector2 viewPortDimensions;
		#endregion
		#region Unity
		void OnDrawGizmos()
		{			
			SetRestrictions();
			
			Gizmos.color = Color.red;
			
			Gizmos.DrawCube(restrictPoint_UpLeft, Vector3.one * 0.5f);
			Gizmos.DrawCube(restrictPoint_BotRight, Vector3.one * 0.5f);
			Gizmos.DrawCube(restrictPoint_UpRight, Vector3.one * 0.5f);
			Gizmos.DrawCube(restrictPoint_BotLeft, Vector3.one * 0.5f);
			
			Gizmos.color = Color.green;
			
			Gizmos.DrawCube(point1, Vector3.one * 0.5f);
			Gizmos.DrawCube(point2, Vector3.one * 0.5f);
			Gizmos.DrawCube(point3, Vector3.one * 0.5f);
			Gizmos.DrawCube(point4, Vector3.one * 0.5f);			
			
			Gizmos.DrawLine(point1, point2);
			Gizmos.DrawLine(point3, point4);
			Gizmos.DrawLine(point2, point4);
			Gizmos.DrawLine(point1, point3);
			
			if(!Application.isPlaying)	
			{
				if(CenterCamera)
				{
					transform.position = directionInverse;
					CenterCamera = false;
				}

//				cameraRelative = transform.InverseTransformPoint(Player.Instance.transform.position);
//				Vector3 destinationPos = Difference + cameraRelative;
//				destinationPos.x = Mathf.Clamp(destinationPos.x, min.x, max.x);
//				destinationPos.y = Mathf.Clamp(destinationPos.y, min.y, max.y);
//				transform.position = transform.TransformPoint(destinationPos - Difference)+ directionInverse;
			}	
		}

		void Update()
		{
//			SetCameraAspect();
			if(!isPaused)
			{
				if(followPlayer && !Vector3.ReferenceEquals(directionInverse, null))
				{
					transform.position = Vector3.Lerp(transform.position, DestinationPos(), Time.deltaTime * follow_Speed);
				}
				
				Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, screen_Size, Time.deltaTime * zoomSpeed);
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

			SetRestrictions();
			
			initPos = transform.position;
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

		private Vector3 DestinationPos()
		{
			if(min.x < max.x && min.y < max.y)
			{
				cameraRelative = transform.InverseTransformPoint(Player.Instance.transform.position);
				Vector3 destinationPos = Difference + cameraRelative;
				destinationPos.x = Mathf.Clamp(destinationPos.x, min.x, max.x);
				destinationPos.y = Mathf.Clamp(destinationPos.y, min.y, max.y);
				return transform.TransformPoint(destinationPos - Difference + CameraOffSet) + directionInverse;
			}
			return transform.position;
		}

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

		private void SetScreenSizeInWorldCoords ()
		{
			var cam = Camera.main;
			var p1 = cam.ViewportToWorldPoint(new Vector3(0,0,cam.nearClipPlane));  
			var p2 = cam.ViewportToWorldPoint(new Vector3(1,0,cam.nearClipPlane));
			var p3 = cam.ViewportToWorldPoint(new Vector3(1,1,cam.nearClipPlane));
			
			float width = (p2 - p1).magnitude;
			float height = (p3 - p2).magnitude;
			
			viewPortDimensions = new Vector2(width,height);
		}

		private void SetRestrictions()
		{
			directionInverse = transform.forward * -40;			
			Difference = -transform.InverseTransformPoint(directionInverse);	
			
			min.x = (boundaries_xMin + viewPortDimensions.x / 2);
			max.x = (boundaries_xMax - viewPortDimensions.x / 2);			
			min.y = (boundaries_zMin + viewPortDimensions.y / 2);
			max.y = (boundaries_zMax - viewPortDimensions.y / 2);
			
			point1 = (transform.right * boundaries_xMin) + (transform.up * boundaries_zMax) + directionInverse;
			point2 = (transform.right * boundaries_xMax) + (transform.up * boundaries_zMax) + directionInverse;
			point3 = (transform.right * boundaries_xMin) + (transform.up * boundaries_zMin) + directionInverse;
			point4 = (transform.right * boundaries_xMax) + (transform.up * boundaries_zMin) + directionInverse;
			
			SetScreenSizeInWorldCoords();
			
			restrictPoint_UpLeft = point1 + ((viewPortDimensions.x / 2 * transform.right) + (viewPortDimensions.y / 2 * -transform.up));			
			restrictPoint_UpRight = point2 + ((viewPortDimensions.x / 2 * -transform.right) + (viewPortDimensions.y / 2 * -transform.up));
			restrictPoint_BotLeft = point3 + ((viewPortDimensions.x / 2 * transform.right) + (viewPortDimensions.y / 2 * transform.up));
			restrictPoint_BotRight = point4 + ((viewPortDimensions.x / 2 * -transform.right) + (viewPortDimensions.y / 2 * transform.up));
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
