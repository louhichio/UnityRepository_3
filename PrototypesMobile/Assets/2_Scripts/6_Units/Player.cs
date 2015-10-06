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
			Tile tile_temp = MapManager.Instance.InitializeUnit(transform.position, gameObject);

			Initialize(tile_temp);
			SetUnitNeighboursTilesState(TileState.PlayerOn);
			tile_current.AddUnit(this);

			MapManager.Instance.SetGameOverTile(tile_temp);
		}

		private void Reset()
		{
			StopAllCoroutines();
			
			ResetUnitNeighboursTiles();	
			tile_current.RemoveUnit(this);

			path.Clear();
			waypoints.Clear();

			transform.position = position_Init;
			
			tile_current = tile_init;			
			tile_current.AddUnit(this);

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.PlayerOn);

			moveState = MoveState.None;
		}

		public override void TravelFinished()
		{
			moveState = MoveState.None;

			list_UnitNeighbours = tile_current.GetTilesWithinCost(step_Max);
			SetUnitNeighboursTilesState(TileState.PlayerOn);

			tile_current.AddUnit(this);
			if(!tile_current.isEnemyFOV().Value)
			{
				GameManager.Instance.StartCoroutine("PlayerLost");
				return;
			}
			if(tile_current == MapManager.Instance.tile_EndGame)
					GameManager.Instance.StartCoroutine("PlayerWon");	
			else
				TurnManager.Instance.StartCoroutine("PlayerMoved");
		}
		public override bool? CheckTile()
		{
			if(!tile_current.isEnemyFOV().Value)
			{
				Stop();
				GameManager.Instance.StartCoroutine("PlayerLost");
				return true;
			}
			//			foreach(var t in list_UnitNeighbours)
			//			{
			//				if(t.isEnemyOn)
			//				{
			//					GameManager.Instance.StartCoroutine("PlayerLost");
			//					return;
			//				}
			//			}	
			if(tile_current.index == MapManager.Instance.tile_EndGame.index)
			{
				GameManager.Instance.StartCoroutine("PlayerWon");				
				return true;
			}
			return false;
		}
		#endregion
	}
}
