namespace TheVandals
{
	using UnityEngine;
	using System;

	public class TileEntity
	{		
		public int index;
		public Transform trs;
		public Vector3 center;
		public float size;
		public Rect rect;

		public NodeEntity node_Right;
		public NodeEntity node_Left;
		public NodeEntity node_Up;
		public NodeEntity node_Bot;

		public TileEntity (Vector3 center, float size)
		{
			this.center = center;
			this.size = size;

			node_Right = new NodeEntity(center);
			node_Left = new NodeEntity(center);
			node_Up = new NodeEntity(center);
			node_Bot = new NodeEntity(center);

			node_Right.position.x += size / 2;
			node_Left.position.x -= size / 2;
			node_Up.position.z += size / 2;
			node_Bot.position.z -= size / 2;

			rect.center = new Vector2(center.x - 0.5f,center.z - 0.5f);
			rect.size = Vector2.one;
		}

		public void DrawTile()
		{
//			Gizmos.color = Color.red;
//			Gizmos.DrawCube(node_Right.position, 0.2f * Vector3.one);
//			Gizmos.DrawCube(node_Left.position, 0.2f * Vector3.one);
//			Gizmos.DrawCube(node_Up.position, 0.2f * Vector3.one);
//			Gizmos.DrawCube(node_Bot.position, 0.2f * Vector3.one);
			
			Gizmos.color = Color.black;
			Gizmos.DrawCube(center, 0.2f * Vector3.one);
		}
	}
}
