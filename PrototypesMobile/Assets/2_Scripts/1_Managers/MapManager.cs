namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System.Collections;

	public class MapManager: Singleton<MapManager>
	{
		[SerializeField]
		private Transform trs_Floor;
		[SerializeField]
		private GameObject obj_Environment;
		[SerializeField]
		private GameObject prefab_Tile;

		private List<TileEntity> map_tiles = new List<TileEntity>();

		public struct SceneElement
		{
			public Transform trs;
			public List<Vector3> vertices;
			public Rect rect;
			public float height;
		}

		private List<SceneElement> listSceneElements = new List<SceneElement>();

		void OnDrawGizmos()
		{
			if(Application.isPlaying)
			{
				foreach(TileEntity t in map_tiles)
					t.DrawTile();

				Gizmos.color = Color.green;
				foreach(TileEntity te in map_tiles)
				{
					Gizmos.DrawLine(new Vector3(te.rect.xMin, te.trs.position.y, te.rect.yMin),
					                new Vector3(te.rect.xMax, te.trs.position.y, te.rect.yMin));
					Gizmos.DrawLine(new Vector3(te.rect.xMin, te.trs.position.y, te.rect.yMax),
					                new Vector3(te.rect.xMax, te.trs.position.y, te.rect.yMax));
					Gizmos.DrawLine(new Vector3(te.rect.xMin, te.trs.position.y, te.rect.yMin),
					                new Vector3(te.rect.xMin, te.trs.position.y, te.rect.yMax));
					Gizmos.DrawLine(new Vector3(te.rect.xMax, te.trs.position.y, te.rect.yMin),
					                new Vector3(te.rect.xMax, te.trs.position.y, te.rect.yMax));
				}

				Gizmos.color = Color.red;
				foreach(SceneElement se in listSceneElements)
				{
					Vector3 pos = se.trs.position;
					pos.y = se.height;
					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMin),
					                new Vector3(se.rect.xMax, se.height, se.rect.yMin));
					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMax),
					                new Vector3(se.rect.xMax, se.height, se.rect.yMax));
					Gizmos.DrawLine(new Vector3(se.rect.xMin, se.height, se.rect.yMin),
					                new Vector3(se.rect.xMin, se.height, se.rect.yMax));
					Gizmos.DrawLine(new Vector3(se.rect.xMax, se.height, se.rect.yMin),
					                new Vector3(se.rect.xMax, se.height, se.rect.yMax));
				}
			}
		}

		void Start()
		{
			if(transform.childCount == 0)
				GenerateMapTiles();

			foreach(MeshFilter mesh in obj_Environment.GetComponentsInChildren<MeshFilter>())
			{
				SceneElement temp_SceneElement = new SceneElement();
				GenerateHeightMap(mesh, temp_SceneElement);
			}
		}

		private void GenerateMapTiles()
		{
			Vector3 tile_position = Vector3.zero; 
			TileEntity tile_temp;

			for(int x = 0; x < trs_Floor.localScale.x; x++)
			{
				tile_position.x = x - (trs_Floor.localScale.x / 2) + 0.5f; 

				for(int z = 0; z < trs_Floor.localScale.z; z++)
				{
					tile_position.z = z - (trs_Floor.localScale.z / 2) + 0.5f;
//					tile_position.y = GetTileHeight(new Rect(z + 0.5f, x + 0.5f, 1, 1));
					
					GameObject tile_obj = Instantiate(prefab_Tile, tile_position, Quaternion.identity) as GameObject;
					tile_obj.transform.parent = transform;

					tile_temp = new TileEntity(tile_position, 1.0f);					
					tile_temp.trs = tile_obj.transform;

					map_tiles.Add(tile_temp);

//					tile_obj.GetComponent<TileEditor>().SetTileEntity(tile_temp);
				}
			}
		}

		//Get RayCasted Tile
		public Vector3 GetTilePosition(Vector3 position)
		{
			foreach(TileEntity t in map_tiles)
			{
				if(position.x >= t.node_Left.position.x && 
				   position.x <= t.node_Right.position.x &&
				   position.z >= t.node_Bot.position.z && 
				   position.z <= t.node_Up.position.z)
					return t.center;
			}
			return Vector3.zero;
		}

		public void GenerateHeightMap(MeshFilter mesh, SceneElement se)
		{
			print (mesh.gameObject);

			se.trs = mesh.transform;
			se.vertices = new List<Vector3>();
			se.height = 0;

			Vector3 initVertice = se.trs.TransformPoint(mesh.mesh.vertices[0]);
			se.rect = new Rect(initVertice.x,initVertice.z,0,0);

			List<TileEntity> toBeChangedTile = new List<TileEntity>();

			foreach(Vector3 vertice in mesh.mesh.vertices)
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

			float mapWidth = trs_Floor.localScale.x;
			float mapHeight = trs_Floor.localScale.z;

			float xMin = Mathf.Clamp(Mathf.Floor(se.rect.xMin + (mapWidth / 2)), 0, mapWidth);
			float yMin = Mathf.Clamp(Mathf.Floor(se.rect.yMin + (mapHeight / 2)), 0, mapWidth);

			float xMax = Mathf.Clamp(Mathf.Ceil(se.rect.xMax + (mapWidth / 2) - 1), 0, mapWidth);
			float yMax = Mathf.Clamp(Mathf.Ceil(se.rect.yMax + (mapHeight / 2)- 1), 0, mapWidth);

			for(int x = (int)xMin; x <= (int)xMax; x++)
			{
				for(int y = (int)yMin; y <= (int)yMax; y++)
				{
					int index = x * (int)(mapHeight) + y;

					map_tiles[index].center.y = se.height;

					Vector3 pos = map_tiles[index].trs.position;
					pos.y = se.height;
					map_tiles[index].trs.position = pos;
				}
			}

			listSceneElements.Add(se);
		}
	}
}

