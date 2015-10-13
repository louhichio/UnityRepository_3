namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

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
			SetUnitNeighboursTilesState(TileState.PlayerOn);
			tile_current.AddUnit(this);

			isHidden = false;

			MapManager.Instance.SetGameOverTile(tile_temp);

			StartTurn();
		}

		private void Reset()
		{				
			path.Clear();
			waypoints.Clear();

			transform.position = position_Init;
			
			tile_current = tile_init;			
			tile_current.AddUnit(this);

			isHidden = false;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.PlayerOn);

			moveState = MoveState.None;

			StartTurn();
		}

		public void StartTurn()
		{						
			canMove = true;	
			turnSteps = 0;	
			
			path.Clear();
			waypoints.Clear();		
			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.PlayerOn);
		}
		#endregion

		public override void TravelFinished()
		{
			SetUnitNeighboursTilesState(TileState.Clear);

			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max - turnSteps);
			SetUnitNeighboursTilesState(TileState.PlayerOn);

			tile_current.AddUnit(this);

			if(turnSteps == step_Max)
			{
				list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
				SetUnitNeighboursTilesState(TileState.PlayerOn);

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

			if(tile_current.isFoVDetect)
				EventManager.Instance.PlChangedTile(tile_current);
			else if(tile_current.isHide)
				isHidden = true;
			else if(isHidden)
				isHidden = false;

			if(tile_current == MapManager.Instance.tile_EndGame)
			{
				GameManager.Instance.StartCoroutine("PlayerWon");				
				return true;
			}
			return false;
		}
	}
}
