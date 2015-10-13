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
		private Transform trs_Tiles;
		[SerializeField]
		private Transform trs_Links;	

		[SerializeField]
		private GameObject prefab_Tile;		
		[SerializeField]
		private LinkObject prefab_Link;

		public int tile_EndGame_index = -1;

		[HideInInspector]
		public Tile tile_EndGame;
		[SerializeField]
		public GameObject prefab_EndGame_particle;

		private List<SceneElement> listSceneElements = new List<SceneElement>();

		private List<Tile> map_tiles = new List<Tile>();

		private List<LinkObject> map_links = new List<LinkObject>();
		
		private float mapWidth;
		private float mapHeight;
		public bool enableEditorMapTilesGeneration = false;
		public bool eraseTiles = false;
		#endregion

		#region Unity
		void OnDrawGizmos()
		{
			if(enableEditorMapTilesGeneration && trs_Tiles.childCount == 0)
			{			
				transform.position = Vector3.up * (-0.001f);
				mapWidth = trs_Floor.localScale.x;
				mapHeight = trs_Floor.localScale.z;
				
				map_tiles.Clear();
				map_links.Clear();

				GenerateMapTiles();

				if(trs_Links.transform.childCount == 0)
					GenerateLinks();
				else
				{
					foreach (Transform child in trs_Tiles.transform)
						map_links.Add(child.GetComponent<LinkObject>());
				}
				enableEditorMapTilesGeneration = false;
				transform.position = Vector3.zero;
			}

			if(eraseTiles)
			{
				foreach(Transform child in trs_Tiles)
					DestroyImmediate(child.gameObject);
				foreach(Transform child in trs_Links)
					DestroyImmediate(child.gameObject);
				eraseTiles = false;
			}
//			if(Application.isPlaying)
//			{
				///////////////////////////Show Tiles////////////////////
//				foreach(Tile te in map_tiles)
//				{
//					if(te.tile_type == TileType.Horizontal)
//					{						
//						Gizmos.color = Color.white;
//						Gizmos.DrawCube(te.transform.position, 0.2f * Vector3.one);
//
//						Gizmos.color = Color.blue;
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
//			}
		}
		#endregion

		#region Public
		public void Initialise()
		{
			mapWidth = trs_Floor.localScale.x;
			mapHeight = trs_Floor.localScale.z;
			
			if(trs_Tiles.transform.childCount == 0)
				GenerateMapTiles();
			else
			{
				foreach (Transform child in trs_Tiles)
				{
					map_tiles.Add(child.GetComponent<Tile>());
//					map_tiles[map_tiles.Count - 1].SetTileState(TileState.Clear);
				}
			}

			if(trs_Links.transform.childCount == 0)
				GenerateLinks();
			else
			{
				foreach (Transform child in trs_Links)
					map_links.Add(child.GetComponent<LinkObject>());
			}
		}

		public void SetMapTiles()
		{
			if(!Application.isPlaying && trs_Tiles.childCount != 0 && map_tiles.Count == 0)
			{
				foreach (Transform child in trs_Tiles.transform)
					map_tiles.Add(child.GetComponent<Tile>());
			}
			print("SetMapTiles");
		}

		public void SetGameOverTile(Tile tile_player)
		{
			if(int.ReferenceEquals(tile_EndGame_index,null) || tile_EndGame_index < 0 || tile_EndGame_index > map_tiles.Count -1)
			{
				while(Tile.ReferenceEquals(tile_EndGame,null) || tile_EndGame == tile_player)
				{				
					tile_EndGame = map_tiles[UnityEngine.Random.Range(0,map_tiles.Count -1)];
				}
			}else
			{
				tile_EndGame = map_tiles[tile_EndGame_index];
			}

			if(prefab_EndGame_particle)
			{
				GameObject prefab_InScene = Instantiate(prefab_EndGame_particle, tile_EndGame.transform.position, Quaternion.identity) as GameObject;
				prefab_InScene.transform.parent = tile_EndGame.transform;
			}

			tile_EndGame.GetComponent<SpriteRenderer>().color = Color.yellow;
			tile_EndGame.GetComponent<SpriteRenderer>().enabled = true;
		}

		//Get RayCasted Tile
		public Tile InitializeUnit(Vector3 position)
		{			
			if(List<Tile>.ReferenceEquals(map_tiles,null) || map_tiles.Count == 0)
				SetMapTiles();

			foreach(Tile t in map_tiles)
			{				
				if(position.x >= t.transform.position.x - 0.5f && 
				   position.x <= t.transform.position.x + 0.5f &&
				   position.z >= t.transform.position.z - 0.5f && 
				   position.z <= t.transform.position.z + 0.5f)
				{	
					return t;
				}
			}
			return null;
		}		

		public Tile GetTapTilePosition(Vector3 position, Tile tile, int cost)
		{	

			foreach(Tile t in tile.GetTilesWithinCost(cost))//cross.list_tiles
			{
//				print (t);
//				foreach(var n in t.Neighbours)
//					print (n);
				if(!Tile.ReferenceEquals(t,null) &&
				   position.x >= t.transform.position.x - 0.5f && 
				   position.x <= t.transform.position.x + 0.5f &&
				   position.z >= t.transform.position.z - 0.5f && 
				   position.z <= t.transform.position.z + 0.5f)
				{								
//					print (t);
					if(t.index != tile.index)
					{
						return t;
					}					
					break;
				}
			}
			return null;
		}

		public Tile GetSwipeTilePosition(Tile t, FingerGestures.SwipeDirection dir)
		{
			int index = -1;

			switch (dir)
			{
				//Forward
			case FingerGestures.SwipeDirection.Up:				
				if((t.index + mapHeight) < map_tiles.Count )
					index = t.index + (int)mapHeight;
				break;
			case FingerGestures.SwipeDirection.UpperRightDiagonal:
				if((t.index + mapHeight) < map_tiles.Count )
					index = t.index + (int)mapHeight;
				break;
				//Right
			case FingerGestures.SwipeDirection.Right:
				if((t.index - 1) >= 0 &&
				   t.index % mapHeight != 0)
					index = t.index - 1;
				break;
			case FingerGestures.SwipeDirection.LowerRightDiagonal:
				if((t.index - 1) >= 0 &&
				   t.index % mapHeight != 0)
					index = t.index - 1;
				break;
				// Back
			case FingerGestures.SwipeDirection.Down:
				if((t.index - mapHeight) >= 0 )
					index = t.index - (int)mapHeight;
				break;
			case FingerGestures.SwipeDirection.LowerLeftDiagonal:
				if((t.index - mapHeight) >= 0)
					index = t.index - (int)mapHeight;
				break;
				//Left
			case FingerGestures.SwipeDirection.Left:
				if((t.index + 1) < map_tiles.Count &&
				   (t.index + 1) % mapHeight != 0 )
					index = t.index + 1;
				break;
			case FingerGestures.SwipeDirection.UpperLeftDiagonal:
				if((t.index + 1) < map_tiles.Count &&
				   (t.index + 1) % mapHeight != 0 )
					index = t.index + 1;
				break;
			}

			if(index != -1)
			{				
				if(!Tile.ReferenceEquals(map_tiles[index], null) &&
				        t.Neighbours.Contains(map_tiles[index]))
//				if(map_tiles[index].index != t.index && 
//				   cross.list_TileCenter_Links.Find(x => x.tile_LinkStart == map_tiles[index] || x.tile_LinkEnd == map_tiles[index]).isActive)		
					return map_tiles[index];
			}
			return null;
		}

		public List<Tile> GetFloorTile(int[,] tile_position, Tile tile)
		{
			int index;
			int x = (int)(tile.index / mapHeight);
			int y = (int)(tile.index % mapHeight);
			int tilePositionX;
			int tilePositionY;
			List<Tile> list_tiles= new List<Tile>();

			for (int i = 0; i < tile_position.GetLength(0); i += 1) 
			{
				tilePositionX = tile_position[i, 1] + x;
				tilePositionY = tile_position[i, 0] + y;

				index = tilePositionX * (int)(mapHeight) + tilePositionY;

				if(index >= 0 && index < map_tiles.Count)
				{
					list_tiles.Add(map_tiles[index]);
				}
			}
			return list_tiles;

		}

		public void ResetTiles()
		{
			foreach(var t in map_tiles)
				t.Reset();
		}

		public void SetWayPoints(ref List<Tile> l, int[] tiles_index)
		{
			foreach(var index in tiles_index)
			{
				l.Add(map_tiles[index]);
			}
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
			
			for(int x = 0; x < mapWidth; x++)
			{
				tile_position.x = x - (mapWidth / 2) + 0.5f; 
				
				for(int z = 0; z < mapHeight; z++)
				{
					tile_position.z = z - (mapHeight / 2) + 0.5f;
					
					GameObject tile_obj = Instantiate(prefab_Tile, tile_position, Quaternion.Euler(Vector3.right * 90)) as GameObject;
					tile_obj.transform.parent = trs_Tiles.transform;
					tile_obj.name = "Tile_" + map_tiles.Count;

					Tile tile_temp = tile_obj.GetComponent<Tile>();

					map_tiles.Add(tile_temp);
					tile_temp.Initialize(map_tiles.Count - 1);
				}
			}
//			print ("GenerateHorizontalTiles Done");
			
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
							map_tiles[index].SetTileHeight(se.height + 0.001f);
							map_tiles[index].Initialize(index);
						}
					}
				}
			}
//			print ("SetTilesHeight Done");
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
//			print ("GenerateHeightMap Done");
		}

		private void GenerateLinks()
		{			
			for(int x = 0; x < mapWidth; x++)
			{				
				for(int z = 0; z < mapHeight; z++)
				{
					int tile_index = x *(int)(mapHeight) + z;

					// Vertical Links
					if((tile_index + 1) % mapHeight != 0)
					{
						AddLink(map_tiles[tile_index],map_tiles[tile_index + 1], true, tile_index);
					}

					// Horizontal Links
					if(tile_index + mapHeight < map_tiles.Count)
					{		
						AddLink(map_tiles[tile_index], map_tiles[tile_index + (int)mapHeight], false, tile_index);
					}
				}
			}
//			print ("GenerateLinks Done");
		}

		private void AddLink(Tile tile_LinkStart, Tile tile_LinkEnd, bool direction, int tile_index)
		{
			LinkObject link_temp;
			link_temp = Instantiate(prefab_Link) as LinkObject;
			link_temp.transform.parent = trs_Links.transform;		
			link_temp.gameObject.name = direction ? "Link_Vertical_" + map_links.Count : "Link_Horizontal_" + map_links.Count;

			bool isActive = Mathf.Abs(tile_LinkStart.transform.position.y - tile_LinkEnd.transform.position.y) < 1 + 0.1f;
			
			link_temp.Initialize(tile_LinkStart, tile_LinkEnd, isActive, map_links.Count);
			map_links.Add(link_temp);
		}


		#endregion
	}
}

