using UnityEngine;
using UnityEditor;
using System.Collections;

public class MeshEditor : MonoBehaviour {

	public Vector3[] newVertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1)};
	public Vector2[] newUV = new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1)};
	public int[] newTriangles = new int[] {0, 1, 2};

	void Start () 
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
		AssetDatabase.CreateAsset(mesh, "Assets/mesh.asset");
		AssetDatabase.SaveAssets();
		
		//			gameObject.AddComponent<MeshFilter>();
		//			gameObject.AddComponent<MeshRenderer>();
	}

	void Update () {
	
	}
}
