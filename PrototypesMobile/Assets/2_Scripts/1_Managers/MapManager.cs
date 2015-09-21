namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Collections;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	public class MapManager: Singleton<MapManager>
	{
		#region Properties
		public struct SceneElement
		{
			public Transform trs;
			public List<Vector3> vertices;
			public Rect rect;
			public float height;

			public int xMin;
			public int yMin;			
			public int xMax;
			public int yMax;
		}


		[SerializeField]
		private Transform trs_Floor;
		[SerializeField]
		private GameObject obj_Environment;
		[SerializeField]
		private GameObject prefab_Tile;

		private List<SceneElement> listSceneElements = new List<SceneElement>();
		private List<TileEntity> map_tiles = new List<TileEntity>();
		
		private float mapWidth;
		private float mapHeight;
		public bool enableEditorMapTilesGeneration = false;
		public bool eraseTiles = false;
		#endregion

		#region Unity
		void OnDrawGizmos()
		{
			if(enableEditorMapTilesGeneration && transform.childCount == 0)
			{			
				transform.position = Vector3.up * (-0.001f);
				mapWidth = trs_Floor.localScale.x;
				mapHeight = trs_Floor.localScale.z;

				GenerateMapTiles();

				enableEditorMapTilesGeneration = false;
				transform.position = Vector3.zero;
			}

			if(eraseTiles)
			{
				foreach(Transform child in transform)
					DestroyImmediate(child.gameObject);
				eraseTiles = false;
			}
			if(Application.isPlaying)
			{
				Gizmos.color = Color.blue;
				foreach(TileEntity te in map_tiles)
				{
					if(te.tile_type == TileType.Horizontal)
					{
						Gizmos.DrawLine(new Vector3(te.rect.xMin, te.transform.position.y, te.rect.yMin),
						                new Vector3(te.rect.xMax, te.transform.position.y, te.rect.yMin));
						Gizmos.DrawLine(new Vector3(te.rect.xMin, te.transform.position.y, te.rect.yMax),
						                new Vector3(te.rect.xMax, te.transform.position.y, te.rect.yMax));
						Gizmos.DrawLine(new Vector3(te.rect.xMin, te.transform.position.y, te.rect.yMin),
						                new Vector3(te.rect.xMin, te.transform.position.y, te.rect.yMax));
						Gizmos.DrawLine(new Vector3(te.rect.xMax, te.transform.position.y, te.rect.yMin),
						                new Vector3(te.rect.xMax, te.transform.position.y, te.rect.yMax));
					}
				}
				///////////////////////////Show scene elements mesh////////////////////
//				Gizmos.color = Color.yellow;
//				foreach(SceneElement se in listSceneElements)
//				{
//					Vector3 pos = se.trs.position;
//					pos.y = se.height;
//					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMin),
//					                new Vector3(se.rect.xMax, se.height, se.rect.yMin));
//					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMax),
//					                new Vector3(se.rect.xMax, se.height, se.rect.yMax));
//					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMin),
//					                new Vector3(se.rect.xMin, se.height, se.rect.yMax));
//					Gizmos.DrawLine(new Vector3(se.rect.xMax, se.height, se.rect.yMin),
//					                new Vector3(se.rect.xMax, se.height, se.rect.yMax));
//				}
			}
		}
		#endregion

		#region Public
		public void Init()
		{
			mapWidth = trs_Floor.localScale.x;
			mapHeight = trs_Floor.localScale.z;
			
			if(transform.childCount == 0)
			{
				GenerateMapTiles();
			}
			else
			{
				foreach (Transform child in transform)
				{
					map_tiles.Add(child.GetComponent<TileEntity>());
				}
			}
		}

		//Get RayCasted Tile
		public void InitUnitCross(Vector3 position, GameObject obj)
		{			
			foreach(TileEntity t in map_tiles)
			{				
				if(position.x >= t.transform.position.x - 0.5f && 
				   position.x <= t.transform.position.x + 0.5f &&
				   position.z >= t.transform.position.z - 0.5f && 
				   position.z <= t.transform.position.z + 0.5f)
				{	
					if(obj.GetComponent<Enemy>())
						obj.GetComponent<Enemy>().SetUnitPosition(GenerateCross(t));
					else if(obj.GetComponent<PlayerManager>())
						obj.GetComponent<PlayerManager>().SetUnitPosition(GenerateCross(t));
					return;
				}
			}
		}		

		public CrossEntity GetTapTilePosition(Vector3 position, CrossEntity cross)
		{			
			foreach(TileEntity t in cross.list_tiles)
			{
				if(!TileEntity.ReferenceEquals(t,null)&&
				   position.x >= t.transform.position.x - 0.5f && 
				   position.x <= t.transform.position.x + 0.5f &&
				   position.z >= t.transform.position.z - 0.5f && 
				   position.z <= t.transform.position.z + 0.5f)
				{						
					if(t.index != cross.tile_center.index)			
						return GenerateCross(t);
					return null;
				}
			}
			return null;
		}

		public CrossEntity GetSwipeTilePosition(CrossEntity cross, FingerGestures.SwipeDirection dir)
		{
			int index = -1;

			switch (dir)
			{
			case FingerGestures.SwipeDirection.UpperRightDiagonal:
				if(!TileEntity.ReferenceEquals(cross.tile_forward, null))
					index = cross.tile_forward.index;
				break;
			case FingerGestures.SwipeDirection.Right:
				if(!TileEntity.ReferenceEquals(cross.tile_forward, null))
					index = cross.tile_forward.index;
				break;
			case FingerGestures.SwipeDirection.LowerRightDiagonal:
				if(!TileEntity.ReferenceEquals(cross.tile_right, null))
					index = cross.tile_right.index;
				break;
			case FingerGestures.SwipeDirection.Down:
				if(!TileEntity.ReferenceEquals(cross.tile_right, null))
					index = cross.tile_right.index;
				break;
			case FingerGestures.SwipeDirection.UpperLeftDiagonal:
				if(!TileEntity.ReferenceEquals(cross.tile_left, null))
					index = cross.tile_left.index;
				break;
			case FingerGestures.SwipeDirection.Up:
				if(!TileEntity.ReferenceEquals(cross.tile_left, null))
					index = cross.tile_left.index;
				break;
			case FingerGestures.SwipeDirection.LowerLeftDiagonal:
				if(!TileEntity.ReferenceEquals(cross.tile_back, null))
					index = cross.tile_back.index;
				break;
			case FingerGestures.SwipeDirection.Left:
				if(!TileEntity.ReferenceEquals(cross.tile_back, null))
					index = cross.tile_back.index;
				break;
			}

			if(index != -1)
			{
				if(map_tiles[index].index != cross.tile_center.index)			
					return GenerateCross(map_tiles[index]);
				return null;
			}
			return null;
		}	

		public CrossEntity GenerateCross(TileEntity t)
		{
			CrossEntity cross_temp = new CrossEntity();		
			float tile_height = t.transform.position.y;
			cross_temp.tile_center = t;

			if((t.index + mapHeight) < map_tiles.Count && 
			   Mathf.Abs(tile_height - map_tiles[t.index + (int)mapHeight].transform.position.y) < t.size * 2)
				cross_temp.tile_forward = map_tiles[t.index + (int)mapHeight];
			
			if((t.index - mapHeight) >= 0 && 
			   Mathf.Abs(tile_height - map_tiles[t.index - (int)mapHeight].transform.position.y) < t.size * 2)
				cross_temp.tile_back = map_tiles[t.index - (int)mapHeight];

			if((t.index + 1) < map_tiles.Count &&
			   (t.index + 1) % mapHeight != 0 && 
			   Mathf.Abs(tile_height - map_tiles[t.index + 1].transform.position.y) < t.size * 2)
				cross_temp.tile_left = map_tiles[t.index + 1];

			if((t.index - 1) >= 0 &&
			   t.index % mapHeight != 0 && 
			   Mathf.Abs(tile_height - map_tiles[t.index - 1].transform.position.y) < t.size * 2)
				cross_temp.tile_right = map_tiles[t.index - 1];
			return cross_temp;
		}
		
		public NodeEntity[] GeneratePath(TileEntity tile_from, TileEntity tile_to)
		{
			NodeEntity[] path = new NodeEntity[0];
			return path;
		}
		#endregion

		#region Private
		private void GenerateMapTiles()
		{
			foreach(MeshFilter mesh in obj_Environment.GetComponentsInChildren<MeshFilter>())
			{
				SceneElement temp_SceneElement = new SceneElement();
				GenerateHeightMap(mesh, temp_SceneElement);
			}
			//			obj_Environment.SetActive(false);

			//////////////////////////////////Generate Horizontal Tiles /////////////////////////////////////////////////////////////////
			Vector3 tile_position = Vector3.zero;
			
			for(int x = 0; x < trs_Floor.localScale.x; x++)
			{
				tile_position.x = x - (trs_Floor.localScale.x / 2) + 0.5f; 
				
				for(int z = 0; z < trs_Floor.localScale.z; z++)
				{
					tile_position.z = z - (trs_Floor.localScale.z / 2) + 0.5f;
					
					GameObject tile_obj = Instantiate(prefab_Tile, tile_position, Quaternion.Euler(Vector3.right * 90)) as GameObject;
					tile_obj.transform.parent = transform;

					TileEntity tile_temp = tile_obj.GetComponent<TileEntity>();

					map_tiles.Add(tile_temp);
					tile_temp.SetInit(TileType.Horizontal, map_tiles.Count - 1, Vector2.zero);
				}
			}
			print ("GenerateHorizontalTiles Done");
			
			//////////////////////////////////Set Tiles Height/////////////////////////////////////////////////////////////////
			/// 
			foreach(SceneElement se in listSceneElements)
			{
				for(int x = se.xMin; x <= se.xMax; x++)
				{
					for(int y = se.yMin; y <= se.yMax; y++)
					{
						int index = x *(int)(mapHeight) + y;

						if(map_tiles[index].transform.position.y < se.height)
						{
							map_tiles[index].SetTileHeight(se.height);
							map_tiles[index].SetInit(TileType.Horizontal, index, new Vector2(x,y));
						}
					}
				}
			}
			
			print ("SetTilesHeight Done");
		}

		private void GenerateHeightMap(MeshFilter mesh, SceneElement se)
		{
			se.trs = mesh.transform;
			se.vertices = new List<Vector3>();
			se.height = 0;

			Vector3 initVertice = se.trs.TransformPoint(mesh.sharedMesh.vertices[0]);
			se.rect = new Rect(initVertice.x,initVertice.z,0,0);

			foreach(Vector3 vertice in mesh.sharedMesh.vertices)
			{
				Vector3 temp_Vertice = se.trs.TransformPoint(vertice);

				if(!se.vertices.Contains(temp_Vertice))
				{
					se.vertices.Add (temp_Vertice);

					if(se.height< temp_Vertice.y)
						se.height = temp_Vertice.y;

					if(se.rect.xMin > temp_Vertice.x)
						se.rect.xMin = temp_Vertice.x;

					if(se.rect.xMax < temp_Vertice.x)
						se.rect.xMax = temp_Vertice.x;

					if(se.rect.yMin > temp_Vertice.z)
						se.rect.yMin = temp_Vertice.z;
					
					if(se.rect.yMax < temp_Vertice.z)
						se.rect.yMax = temp_Vertice.z;
				}
			}
			
			float xMin = Mathf.Clamp(Mathf.Floor(se.rect.xMin + (mapWidth / 2)), 0, mapWidth -1);
			float yMin = Mathf.Clamp(Mathf.Floor(se.rect.yMin + (mapHeight / 2)), 0, mapHeight -1 );
			
			float xMax = Mathf.Clamp(Mathf.Ceil(se.rect.xMax + (mapWidth / 2) - 1), 0, mapWidth -1);
			float yMax = Mathf.Clamp(Mathf.Ceil(se.rect.yMax + (mapHeight / 2)- 1), 0, mapHeight -1);

			se.xMin = (int)xMin;
			se.yMin = (int)yMin;
			se.xMax = (int)xMax;
			se.yMax = (int)yMax;
			
			listSceneElements.Add(se);
			print ("GenerateHeightMap Done");
		}
		
		#endregion
	}
}


#region GetSwipeTilePosition
//public void GetSwipeTilePosition(int tile_index, FingerGestures.SwipeDirection dir)
//{
//	int x = (tile_index / mapHeight);
//	int y = (tile_index % mapHeight);
//	
//	switch (dir)
//	{
//	case FingerGestures.SwipeDirection.UpperRightDiagonal:
//		x++;
//		break;
//	case FingerGestures.SwipeDirection.LowerRightDiagonal:
//		y--;
//		break;
//	case FingerGestures.SwipeDirection.UpperLeftDiagonal:
//		y++;
//		break;
//	case FingerGestures.SwipeDirection.LowerLeftDiagonal:
//		x--;
//		break;
//	}
//	int index = x *(mapHeight) + y;
//	
//	if(index < map_tiles.Count && index >= 0)
//	{
//		Vector3 NavPoint = map_tiles[index].transform.position;
//		NavPoint.y += PlayerManager.Instance.transform.localScale.y / 2;
//		
//		PlayerManager.Instance.SetPlayerPosition(GenerateCross(map_tiles[index]));
//		
//		PlayerManager.Instance.StopCoroutine("MovePlayer");
//		PlayerManager.Instance.StartCoroutine("MovePlayer", NavPoint);
//	}
//}
#endregion

#region Old code
//
//namespace TheVandals
//{
//	using System;
//	using System.Collections.Generic;
//	using UnityEngine;
//	using UnityEditor;
//	using System.Collections;
//	
//	public class MapManager: Singleton<MapManager>
//	{
//		public struct SceneElement
//		{
//			public Transform trs;
//			public List<Vector3> vertices;
//			public Rect rect;
//			public float height;
//			
//			public int xMin;
//			public int yMin;			
//			public int xMax;
//			public int yMax;
//			
//			public List<TilesDiscarded> tiles_discarded;
//		}
//		public struct TilesDiscarded
//		{
//			public TileType tt;
//			public int index;
//			public int index_adj;
//			
//			public TilesDiscarded (TileType tt, int index, int index_adj)
//			{
//				this.tt = tt;
//				this.index = index;
//				this.index_adj = index_adj;
//			}
//		}
//		
//		
//		[SerializeField]
//		private Transform trs_Floor;
//		[SerializeField]
//		private GameObject obj_Environment;
//		[SerializeField]
//		private GameObject prefab_Tile;
//		
//		private List<SceneElement> listSceneElements = new List<SceneElement>();
//		private List<TileEntity> map_tiles = new List<TileEntity>();
//		private List<NodeEntity> map_nodes = new List<NodeEntity>();
//		
//		private float mapWidth;
//		private float mapHeight;
//		private float floorHeight;
//		
//		void OnDrawGizmos()
//		{
//			if(Application.isPlaying)
//			{				
//				foreach(TileEntity t in map_tiles)
//					t.DrawTile();
//				
//				Gizmos.color = Color.blue;
//				foreach(TileEntity te in map_tiles)
//				{
//					if(te.tile_type == TileType.Horizontal)
//					{
//						Gizmos.DrawLine(new Vector3(te.rect.xMin, te.transform.position.y, te.rect.yMin),
//						                new Vector3(te.rect.xMax, te.transform.position.y, te.rect.yMin));
//						Gizmos.DrawLine(new Vector3(te.rect.xMin, te.transform.position.y, te.rect.yMax),
//						                new Vector3(te.rect.xMax, te.transform.position.y, te.rect.yMax));
//						Gizmos.DrawLine(new Vector3(te.rect.xMin, te.transform.position.y, te.rect.yMin),
//						                new Vector3(te.rect.xMin, te.transform.position.y, te.rect.yMax));
//						Gizmos.DrawLine(new Vector3(te.rect.xMax, te.transform.position.y, te.rect.yMin),
//						                new Vector3(te.rect.xMax, te.transform.position.y, te.rect.yMax));
//					}
//				}
//				///////////////////////////Show scene elements mesh////////////////////
//				Gizmos.color = Color.yellow;
//				foreach(SceneElement se in listSceneElements)
//				{
//					Vector3 pos = se.trs.position;
//					pos.y = se.height;
//					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMin),
//					                new Vector3(se.rect.xMax, se.height, se.rect.yMin));
//					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMax),
//					                new Vector3(se.rect.xMax, se.height, se.rect.yMax));
//					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMin),
//					                new Vector3(se.rect.xMin, se.height, se.rect.yMax));
//					Gizmos.DrawLine(new Vector3(se.rect.xMax, se.height, se.rect.yMin),
//					                new Vector3(se.rect.xMax, se.height, se.rect.yMax));
//				}
//				
//				foreach(NodeEntity ne in map_nodes)
//				{
//					Gizmos.color = Color.green;
//					//					Gizmos.DrawLine(ne.position, ne.tile_linked[0].transform.position);
//					//					Gizmos.DrawLine(ne.position, ne.tile_linked[1].transform.position);
//					//					Gizmos.color = Color.yellow;
//					//					Gizmos.DrawCube(ne.position, 0.2f * Vector3.one);
//				}
//			}
//		}
//		
//		void Start()
//		{
//			mapWidth = trs_Floor.localScale.x;
//			mapHeight = trs_Floor.localScale.z;
//			floorHeight = trs_Floor.position.y + trs_Floor.localScale.y / 2;
//			
//			if(transform.childCount == 0)
//			{
//				GenerateMapTiles();
//			}
//			else
//			{
//				foreach (Transform child in transform)
//				{
//					map_tiles.Add(child.GetComponent<TileEntity>());
//				}
//			}
//			
//		}
//		
//		private void GenerateMapTiles()
//		{
//			foreach(MeshFilter mesh in obj_Environment.GetComponentsInChildren<MeshFilter>())
//			{
//				SceneElement temp_SceneElement = new SceneElement();
//				GenerateHeightMap(mesh, temp_SceneElement);
//			}
//			obj_Environment.SetActive(false);
//			//////////////////////////////////Generate Horizontal Tiles /////////////////////////////////////////////////////////////////
//			Vector3 tile_position = Vector3.zero;
//			
//			for(int x = 0; x < trs_Floor.localScale.x; x++)
//			{
//				tile_position.x = x - (trs_Floor.localScale.x / 2) + 0.5f; 
//				
//				for(int z = 0; z < trs_Floor.localScale.z; z++)
//				{
//					tile_position.z = z - (trs_Floor.localScale.z / 2) + 0.5f;
//					
//					GameObject tile_obj = Instantiate(prefab_Tile, tile_position, Quaternion.identity) as GameObject;
//					tile_obj.transform.parent = transform;
//					
//					map_tiles.Add(tile_obj.GetComponent<TileEntity>());
//					tile_obj.GetComponent<TileEntity>().SetInit(TileType.Horizontal);
//				}
//			}
//			
//			//////////////////////////////////Generate Tiles Nodes/////////////////////////////////////////////////////////////////
//			/// 
//			//			foreach(TileEntity te in map_tiles)
//			//			{
//			//				float zMax = trs_Floor.position.z + (trs_Floor.localScale.z/2) - (te.size / 2);
//			//				if(te.transform.position.z < zMax)
//			//				{
//			//					if(map_tiles.IndexOf(te) + 1 < map_tiles.Count)
//			//					{
//			//						Vector3 node_position = (te.transform.position + map_tiles[map_tiles.IndexOf(te) + 1].transform.position) / 2;
//			//						map_nodes.Add(new NodeEntity(node_position,true, te, map_tiles[map_tiles.IndexOf(te) + 1]));
//			//					}
//			//				}
//			//
//			//				float xMax = trs_Floor.position.x + (trs_Floor.localScale.x/2) - (te.size / 2);
//			//				if(te.transform.position.x < xMax)
//			//				{
//			//					if(map_tiles.IndexOf(te) + 7 < map_tiles.Count)
//			//					{
//			//						Vector3 node_position = (te.transform.position + map_tiles[map_tiles.IndexOf(te) + 7].transform.position) / 2;
//			//						map_nodes.Add(new NodeEntity(node_position, true, te, map_tiles[map_tiles.IndexOf(te) + 7]));
//			//					}
//			//				}
//			//			}
//			
//			//////////////////////////////////Set Height of Tiles /////////////////////////////////////////////////////////////////	
//			/// 
//			
//			//			foreach(SceneElement se in listSceneElements)
//			//			{
//			//				for(int i = listSceneElements.IndexOf(se); i < listSceneElements.Count; i++)
//			//				{
//			//					////// SceneElement left side
//			//					if(se.xMin > 0 && 
//			//					   listSceneElements[i].xMax == se.xMin - 1 && 
//			//					   listSceneElements[i].yMax >= se.yMin && 
//			//					   listSceneElements[i].yMin <= se.yMax)
//			//					{
//			//						for(int couter = se.yMin; couter <= se.yMax; couter++)
//			//						{
//			//							if( couter >= listSceneElements[i].yMin && couter <= listSceneElements[i].yMax)	
//			//							{
//			//								print ("l: " + se.trs + "case:" + (se.xMin) + "/" + couter+ "    " + listSceneElements[i].trs + "case:" + (se.xMin - 1) + "/" + couter);
//			//								
//			//								int index = se.xMin * (int)(mapHeight) + couter;
//			//								Vector3 pos = map_tiles[index].transform.position;
//			//								float height = se.trs.position.y > map_tiles[index].transform.position.y ? map_tiles[index].transform.position.y : se.trs.position.y;
//			//								print (se.height + "  " +  map_tiles[index].transform.gameObject);
//			//								se.tiles_discarded.Add(new TilesDiscarded(TileType.VerticalL, index, (se.xMin - 1) * (int)(mapHeight) + couter));
//			//							}
//			//						}
//			//					}
//			//					
//			//					if(se.yMin > 0 && 
//			//					   listSceneElements[i].yMax == se.yMin - 1 &&
//			//					   listSceneElements[i].xMax >= se.xMin && 
//			//					   listSceneElements[i].xMin <= se.xMax)
//			//					{
//			//						for(int couter = se.xMin; couter <= se.xMax; couter++)
//			//						{
//			//							if( couter >= listSceneElements[i].xMin && couter <= listSceneElements[i].xMax)
//			//							{
//			//								print ("b: " + se.trs + "case:" + couter + "/" + (se.yMin) + "    " + listSceneElements[i].trs + "case:" + couter + "/" + (se.yMin - 1));
//			////								
//			////								int index = couter * (int)(mapHeight) + se.yMin;
//			////								Vector3 pos = map_tiles[index].transform.position;
//			//								
//			////								se.tiles_discarded.Add(new TilesDiscarded(TileType.VerticalL, index));
//			//							}
//			//						}
//			//					}
//			//					
//			//					if(se.xMax < mapWidth - 1 && 
//			//					   listSceneElements[i].xMin == se.xMax + 1 && 
//			//					   listSceneElements[i].yMax >= se.yMin && 
//			//					   listSceneElements[i].yMin <= se.yMax)
//			//					{
//			//						for(int couter = se.yMin; couter <= se.yMax; couter++)
//			//						{
//			//							if( couter >= listSceneElements[i].yMin && couter <= listSceneElements[i].yMax)								
//			//								print ("r: " + se.trs + "case:" + (se.xMax) + "/" + couter+ "    " + listSceneElements[i].trs + "case:" + (se.xMax + 1) + "/" + couter);
//			//						}
//			//					}
//			//					
//			//					if(se.yMax < mapHeight - 1 && 
//			//					   listSceneElements[i].yMin == se.yMax + 1 && 
//			//					   listSceneElements[i].xMax >= se.xMin && 
//			//					   listSceneElements[i].xMin <= se.xMax)
//			//					{
//			//						for(int couter = se.xMin; couter <= se.xMax; couter++)
//			//						{
//			//							if( couter >= listSceneElements[i].xMin && couter <= listSceneElements[i].xMax)								
//			//								print ("u: " + se.trs + "case:" + couter + "/" + (se.yMax) + "    " + listSceneElements[i].trs + "case:" + couter + "/" + (se.yMax + 1));
//			//						}
//			//					}
//			//				}
//			//			}
//			
//			foreach(SceneElement se in listSceneElements)
//			{
//				//				se.te = new List<TileEntity>();
//				
//				//				print (se.trs + "   " + se.tiles_discarded.FindAll (z => z.tt == TileType.VerticalL && z.index == index).Count);
//				foreach(TilesDiscarded td in se.tiles_discarded)
//				{
//					print(td.index + "   " + td.tt);
//				}
//				for(int x = se.xMin; x <= se.xMax; x++)
//				{
//					for(int y = se.yMin; y <= se.yMax; y++)
//					{
//						int index = x * (int)(mapHeight) + y;
//						
//						if(map_tiles[index].transform.position.y < se.height)
//						{
//							
//							float minHeight = floorHeight;
//							//////////////////////////	RemoveTiles///////////////////////////////////////
//							/// 
//							if(map_tiles[index].transform.position.y > floorHeight)
//							{
//								minHeight = map_tiles[index].transform.position.y;
//							}
//							map_tiles[index].SetTileHeight(se.height);
//							//							print (se.trs + "   " + se.height + "   " + se.tiles_discarded.FindAll (z => z.tt == TileType.VerticalL && z.index == index).Count);
//							for(float h = map_tiles[index].transform.position.y -0.5f; h > minHeight; h--)
//							{
//								if(x == se.xMax && x < mapWidth - 1)
//								{
//									tile_position = map_tiles[index].transform.position + new Vector3(map_tiles[index].size / 2, 0, 0);
//									GenerateVerticalTile(tile_position, new Vector3(0,0,270), h, TileType.VerticalR, se);
//									
//									//									GenerateVerticalNodes(tile_position, new Vector3(0,0,270), h + 0.5f, TileType.VerticalR);
//								}
//								if(x == se.xMin && x > 0 && se.tiles_discarded.FindAll (z => z.tt == TileType.VerticalL && z.index == index).Count == 0)			
//								{
//									tile_position = map_tiles[index].transform.position - new Vector3(map_tiles[index].size / 2, 0, 0);
//									GenerateVerticalTile(tile_position, new Vector3(0,0,90), h, TileType.VerticalL, se);
//									
//									GenerateVerticalNodes(tile_position, new Vector3(0,0,270), h + 0.5f, TileType.VerticalL);
//								}
//								else
//								{
//									//									print ("here" + map_tiles[se.tiles_discarded.Find (z => z.tt == TileType.VerticalL && z.index == index).index_adj].h);
//								}
//								if(y == se.yMax && y < mapHeight - 1)		
//								{
//									tile_position = map_tiles[index].transform.position + new Vector3(0, 0, map_tiles[index].size / 2);
//									GenerateVerticalTile(tile_position, new Vector3(90,0,0), h, TileType.VerticalUp, se);	
//								}
//								
//								if(y == se.yMin && y > 0)		
//								{
//									tile_position = map_tiles[index].transform.position - new Vector3(0, 0, map_tiles[index].size / 2);
//									GenerateVerticalTile(tile_position, new Vector3(270,0,0), h, TileType.VerticalDown, se);
//								}
//							}
//							map_tiles[index].SetInit(TileType.Horizontal);
//						}
//					}
//				}
//			}
//			//////////////////////////////////Generate Nodes Links /////////////////////////////////////////////////////////////////	
//			/// 
//			//			foreach(NodeEntity ne in map_nodes)
//			//			{
//			////				ne.UpdateNodePosition();
//			//			}
//			
//		}
//		
//		//Get RayCasted Tile
//		public Vector3 GetTilePosition(Vector3 position)
//		{
//			//			foreach(TileEntity t in map_tiles)
//			//			{
//			//				if(position.x >= t.node_Left.position.x && 
//			//				   position.x <= t.node_Right.position.x &&
//			//				   position.z >= t.node_Bot.position.z && 
//			//				   position.z <= t.node_Up.position.z)
//			//					return t.transform.position;
//			//			}
//			return Vector3.zero;
//		}
//		
//		public void GenerateVerticalTile(Vector3 tile_position, Vector3 angle, float height, TileType tt, SceneElement se)
//		{
//			tile_position.y = height;
//			GameObject tile_obj = Instantiate(prefab_Tile, tile_position, Quaternion.Euler(angle)) as GameObject;
//			tile_obj.transform.parent = transform;	
//			tile_obj.GetComponent<TileEntity>().SetInit(tt);
//			map_tiles.Add(tile_obj.GetComponent<TileEntity>());
//		}
//		
//		public void GenerateVerticalNodes(Vector3 tile_position, Vector3 angle, float height, TileType tt)
//		{
//			NodeEntity node_Right;
//			NodeEntity node_Left;
//			NodeEntity node_Up;
//			
//			switch (tt) 
//			{
//			case TileType.VerticalR:
//				node_Right = new NodeEntity(new Vector3(tile_position.x, height - 0.5f, tile_position.z + 0.5f));
//				node_Left = new NodeEntity(new Vector3(tile_position.x, height - 0.5f, tile_position.z - 0.5f));
//				
//				map_nodes.Add (node_Right);
//				map_nodes.Add (node_Left);
//				if( height < tile_position.y)
//				{
//					node_Up = new NodeEntity(new Vector3(tile_position.x, height, tile_position.z));
//					map_nodes.Add (node_Up);
//				}
//				break;
//				
//			case TileType.VerticalL:
//				node_Right = new NodeEntity(new Vector3(tile_position.x, height - 0.5f, tile_position.z - 0.5f));
//				node_Left = new NodeEntity(new Vector3(tile_position.x, height - 0.5f, tile_position.z + 0.5f));
//				
//				map_nodes.Add (node_Right);
//				map_nodes.Add (node_Left);
//				if( height < tile_position.y)
//				{
//					node_Up = new NodeEntity(new Vector3(tile_position.x, height, tile_position.z));
//					map_nodes.Add (node_Up);
//				}
//				break;
//				
//			case TileType.VerticalUp:
//				break;
//				
//			case TileType.VerticalDown:
//				break;
//			}
//		}
//		
//		public void GenerateHeightMap(MeshFilter mesh, SceneElement se)
//		{
//			se.trs = mesh.transform;
//			se.vertices = new List<Vector3>();
//			se.height = 0;
//			se.tiles_discarded = new List<TilesDiscarded>();
//			
//			Vector3 initVertice = se.trs.TransformPoint(mesh.mesh.vertices[0]);
//			se.rect = new Rect(initVertice.x,initVertice.z,0,0);
//			
//			foreach(Vector3 vertice in mesh.mesh.vertices)
//			{
//				Vector3 temp_Vertice = se.trs.TransformPoint(vertice);
//				
//				if(!se.vertices.Contains(temp_Vertice))
//				{
//					se.vertices.Add (temp_Vertice);
//					
//					if(se.height< temp_Vertice.y)
//						se.height = temp_Vertice.y;
//					
//					if(se.rect.xMin > temp_Vertice.x)
//						se.rect.xMin = temp_Vertice.x;
//					
//					if(se.rect.xMax < temp_Vertice.x)
//						se.rect.xMax = temp_Vertice.x;
//					
//					if(se.rect.yMin > temp_Vertice.z)
//						se.rect.yMin = temp_Vertice.z;
//					
//					if(se.rect.yMax < temp_Vertice.z)
//						se.rect.yMax = temp_Vertice.z;
//				}
//			}
//			
//			float xMin = Mathf.Clamp(Mathf.Floor(se.rect.xMin + (mapWidth / 2)), 0, mapWidth);
//			float yMin = Mathf.Clamp(Mathf.Floor(se.rect.yMin + (mapHeight / 2)), 0, mapWidth);
//			
//			float xMax = Mathf.Clamp(Mathf.Ceil(se.rect.xMax + (mapWidth / 2) - 1), 0, mapWidth);
//			float yMax = Mathf.Clamp(Mathf.Ceil(se.rect.yMax + (mapHeight / 2)- 1), 0, mapWidth);
//			
//			se.xMin = (int)xMin;
//			se.yMin = (int)yMin;
//			se.xMax = (int)xMax;
//			se.yMax = (int)yMax;
//			
//			listSceneElements.Add(se);
//		}
//	}
//}


#endregion
