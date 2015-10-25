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
		
		[HideInInspector]
		public bool isHidden;		
		[HideInInspector]
		public GameObject tile_Destination;
		[HideInInspector]
		public int bustedCount = 0;

		public Tile tileEndGame;
		
		public int stepsLeft
		{
			get { return step_Max - turnSteps; }
		}		
		public List<Enemy> list_UnitsDetect = new List<Enemy>();

		#region Events
			void OnEnable()
		{
			EventManager.initialise += Init;
			EventManager.gameReset += Reset;
			EventManager.startTurn_Player += StartTurn;
			EventManager.pause += Pause;
			EventManager.resume += Resume;
		}		
		
		void OnDisable()
		{
			EventManager.initialise -= Init;
			EventManager.gameReset -= Reset;
			EventManager.startTurn_Player -= StartTurn;
			EventManager.pause -= Pause;
			EventManager.resume -= Resume;
		}
		
		private void Init()
		{					
			Tile tile_temp = MapManager.Instance.InitializeUnit(transform.position);

			Initialize(tile_temp);

			tile_current.AddUnit(this);
			
			tile_Destination.SetActive(false);

			SetIsHidden(false);

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
			
			SetIsHidden(false);

			list_UnitsDetect.Clear();

			moveState = MoveState.None;
			
			anim.SetInteger("MoveState",0);
			
			bustedCount = 0;

			endedGame = false;

			MapManager.Instance.DisableEndGamePath();
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

		public void Pause()
		{
			canMove = false;
			anim.speed = 0;
		}

		public void Resume()
		{
			canMove = true;
			anim.speed = 1;
		}
		#endregion
		public override void TravelTo(Tile destination)
		{
			if(Tile.ReferenceEquals(destination, null))
				return;
			
			List<Tile> path = AStar.FindPath(tile_current, destination);

			if (path.Count <= 1)
			{
				this.path = null;
				turnSteps = step_Max;
				TravelFinished();
				return;
			}
			
			tile_current.RemoveUnit(this);
			
			this.path = path;
			waypoints = GetWaypointsFromPath(path);
			
			moveState = MoveState.Moving;
			if(anim)
			{
				anim.SetInteger("MoveState",1);
			}
			
			SetUnitNeighboursTilesState(TileState.Clear);
			
			if(!endedGame)
				destination.SetTileState(TileState.PlayerOn);
		}

		public override void TravelFinished()
		{			
			anim.SetInteger("MoveState",0);
			
			SetUnitNeighboursTilesState(TileState.Clear);
			
			moveState = MoveState.None;

			tile_Destination.SetActive(false);
			if(endedGame)
			{
				if(waypoints.Count == 0)
				{
					Stop();
					GameManager.Instance.StartCoroutine("PlayerWon");
				}
				return;
			}

			list_UnitNeighbours = tile_current.GetTilesWithinCost(stepsLeft);
			SetUnitNeighboursTilesState(TileState.PlayerOn);

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
			if(tile_current == MapManager.Instance.tile_EndGame && !endedGame)
			{
				tileEndGame = MapManager.Instance.EnableEndGamePath();
				endedGame = true;
				TravelTo(tileEndGame);
				return true;
			}
			if(tile_current.isEnemyOn)
			{
				Stop();
				GameManager.Instance.StartCoroutine("PlayerLost");
				return true;
			}			

			if(tile_current.isFoVDetect)
				EventManager.Instance.PlChangedTile(tile_current);
			else if(tile_current.isHide)
				SetIsHidden(true);
			else if(isHidden)
				SetIsHidden(false);

			if(tile_current.isCollectible)
			{
				tile_current.isCollectible = false;
				tile_current.isCaptureOeuvre = false;
				Stop ();
				turnSteps = step_Max;
				TravelFinished();
				CollectManager.Instance.PlayerCollectedObj(tile_current);
				return true;
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

		public void Detected(Enemy en)
		{
			if(!list_UnitsDetect.Contains(en))
			{
				if(list_UnitsDetect.Count == 0)
				{
					UIManager.Instance.StopCoroutine("FadeInPlayerStatus");
					UIManager.Instance.StartCoroutine("FadeInPlayerStatus", 1);
				}
				list_UnitsDetect.Add(en);
			}
		}
		public void NotDetected(Enemy en)
		{
			if(list_UnitsDetect.Contains(en))
			{
				list_UnitsDetect.Remove(en);
				isDetected();
			}
		}
		public bool isDetected()
		{
			if(list_UnitsDetect.Count == 0)
			{
				UIManager.Instance.StopCoroutine("FadeOutPlayerStatus");
				UIManager.Instance.StartCoroutine("FadeOutPlayerStatus");
				return false;
			}		
			return true;

		}

		public void SetIsHidden(bool value)
		{
			isHidden = value;

			if(isHidden)
			{
				UIManager.Instance.StopCoroutine("FadeInPlayerStatus");
				UIManager.Instance.StartCoroutine("FadeInPlayerStatus", 0);
			}
			else
				isDetected();
		}
	}
}
