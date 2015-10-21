namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class Player : Unit 
	{
		private static Player _instance;
		public static Player Instance
		{
			get
			{		
				if (_instance == null)
				{
					_instance = (Player) FindObjectOfType(typeof(Player));

					if (FindObjectsOfType(typeof(Player)).Length > 1 )
					{
						return _instance;
					}
					
					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<Player>();
						singleton.name = "(singleton) "+ typeof(Player).ToString();
						
						DontDestroyOnLoad(singleton);
						
						Debug.Log("[Singleton] An instance of " + typeof(Player) + 
						          " is needed in the scene, so '" + singleton +
						          "' was created with DontDestroyOnLoad.");
					} 
				}				
				return _instance;
			}
		}
			
		public bool isHidden;

		public GameObject tile_Destination;

		public int stepsLeft
		{
			get { return step_Max - turnSteps; }
		}

		#region Events
			void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameReset += Reset;
			EventManager.startTurn_Player += StartTurn;
		}		
		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameReset -= Reset;
			EventManager.startTurn_Player -= StartTurn;
		}
		
		private void Init()
		{					
			Tile tile_temp = MapManager.Instance.InitializeUnit(transform.position);

			Initialize(tile_temp);

			tile_current.AddUnit(this);
			
			tile_Destination.SetActive(false);

			isHidden = false;

			MapManager.Instance.SetGameOverTile(tile_temp);
		}

		private void Reset()
		{		
			if(path != null)
				path.Clear();
			waypoints.Clear();

			transform.position = position_Init;
			transform.eulerAngles = new Vector3(0, rotation_init, 0);
			
			tile_current = tile_init;			
			tile_current.AddUnit(this);
			
			tile_Destination.SetActive(false);

			isHidden = false;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.PlayerOn);

			moveState = MoveState.None;

			StartTurn();
			
			anim.SetInteger("MoveState",0);
		}

		public void StartTurn()
		{						
			canMove = true;	
			turnSteps = 0;

			if(path != null)
				path.Clear();
			waypoints.Clear();		

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.PlayerOn);

			UIManager.Instance.UpdatePlayerInfo(step_Max, turnSteps);
		}
		#endregion

		public override void TravelFinished()
		{
//			List<Tile> tileTemp = tile_current.GetTilesWithinCost(stepsLeft);
//			foreach(var t in tileTemp)
//			{
//				if(list_UnitNeighbours.Contains(t))
//					tileTemp.Remove(t);
			//			}
			anim.SetInteger("MoveState",0);

			SetUnitNeighboursTilesState(TileState.Clear);

			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(stepsLeft);
			SetUnitNeighboursTilesState(TileState.PlayerOn);
			
			tile_Destination.SetActive(false);

			tile_current.AddUnit(this);

			if(turnSteps == step_Max)
			{
//				list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
//				SetUnitNeighboursTilesState(TileState.PlayerOn);

				if(TurnManager.Instance.turnState != TurnState.EnemyTurn)
					TurnManager.Instance.StartCoroutine("PlayerMoved");
			}
		}
		public override bool Check()
		{
			UIManager.Instance.UpdatePlayerInfo(step_Max, turnSteps);

			if(tile_current.isEnemyOn)
			{
				GameManager.Instance.StartCoroutine("PlayerLost");
				Stop();
				return true;
			}			
			
			if(tile_current == MapManager.Instance.tile_EndGame)
			{
				GameManager.Instance.StartCoroutine("PlayerWon");	
				Stop();			
				return true;
			}

			if(tile_current.isFoVDetect)
				EventManager.Instance.PlChangedTile(tile_current);
			else if(tile_current.isHide)
				isHidden = true;
			else if(isHidden)
				isHidden = false;

			if(tile_current.isCollectible)
			{
				tile_current.isCollectible = false;
				CollectManager.Instance.PlayerCollectedObj(tile_current);
			}

			if(tile_current.isCaptureOeuvre)
			{
				tile_current.isCaptureOeuvre = false;
				PaintingManager.Instance.PlayerCapturedPainting(tile_current);
			}
			return false;
		}

		public void TouchOnDestinationTile(Tile destination)
		{
			tile_Destination.transform.position = destination.transform.position;
			Vector3 pos = destination.transform.position;
			pos.y += 0.003f;
			tile_Destination.transform.position = pos;
			tile_Destination.SetActive(true);
		}

		public void DisableDestinationTile()
		{
			tile_Destination.SetActive(false);
		}
	}
}
