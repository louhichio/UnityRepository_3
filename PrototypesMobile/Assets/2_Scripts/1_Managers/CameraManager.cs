namespace TheVandals
{
	using UnityEngine;
	using System;
	using System.Collections;
	
	public class CameraManager : Singleton<CameraManager>
	{
		#region Properties
		public Vector2 ratio = new Vector2(10.0f, 10.0f);

		public float screen_Size_Max = 7;	
		public float screen_Size_Min = 5;	
		private float screen_Size = 5;
		public bool isScreenSizing = false;
		private static float Screen_Diagonal;

		public bool followPlayer = false;
		private Vector3 initPos;

		[SerializeField]
		private float follow_Speed = 1;
		
//		CoroutineController LerpScreenSize_controller;
		#endregion

		#region Unity
		void Start()
		{		
//			screen_Size = screen_Size_Max;
//			Screen_Diagonal = Mathf.Sqrt(Mathf.Pow(Screen.width,2) + Mathf.Pow(Screen.height,2));
//			Camera.main.orthographicSize = screen_Size;
//
//			if(followPlayer)
//				initPos = transform.position;
		}

		void Update()
		{
//			SetCameraAspect();
			//////////////////////////////// Neeeeds Coroutine ////////////////////////////////
			if(followPlayer && Vector3.Distance(Player.Instance.transform.position, transform.position) > 0.1f)
				transform.position = Vector3.Lerp(transform.position, Player.Instance.transform.position, Time.deltaTime * follow_Speed);

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
			EventManager.gameReset -= Reset;
		}
		
		private void Init()
		{			
			screen_Size = screen_Size_Max;
			Screen_Diagonal = Mathf.Sqrt(Mathf.Pow(Screen.width,2) + Mathf.Pow(Screen.height,2));
			Camera.main.orthographicSize = screen_Size;
			
			if(followPlayer)
			{
				transform.position = Player.Instance.transform.position;
				initPos = transform.position;
			}
		}
		private void Reset()
		{
			transform.position = initPos;
		}
		#endregion

		#region Private
		private IEnumerator LerpScreenSize()
		{
			isScreenSizing = true;
			float t = 0;
			while(Camera.main.orthographicSize != screen_Size)
			{
				t += Time.deltaTime;
				Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, screen_Size, t);
				yield return null;
			}
			isScreenSizing = false;
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
		#endregion

		#region Public		
		public void OnPinch(PinchGesture gesture) 
		{	
			float delta = gesture.Delta * 10/ Screen_Diagonal;
			screen_Size = Mathf.Clamp(screen_Size - delta, screen_Size_Min, screen_Size_Max);
			
			if(!isScreenSizing && screen_Size != Camera.main.orthographicSize)
			{
				StopCoroutine("LerpScreenSize");
				StartCoroutine("LerpScreenSize");
			}
			
			if(gesture.Phase == ContinuousGesturePhase.Ended)
			{
				screen_Size = screen_Size_Max;
				StopCoroutine("LerpScreenSize");
				StartCoroutine("LerpScreenSize");
			}
		}
		#endregion
	}
}
