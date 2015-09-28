namespace TheVandals
{
	using UnityEngine;
	using System;

	public enum TileType
	{
		Horizontal,
		VerticalR,
		VerticalL,
		VerticalUp,
		VerticalDown
	}
	public enum TileState
	{
		Clear,
		PlayerOn,
		EnemyOn,
	}

	public class TileObject : MonoBehaviour
	{		
		public TileType tile_type;		
		public TileState tile_state;

		public int index;
		public Vector2 pos;
		public float size;
		public Rect rect;

		private SpriteRenderer sr;

		public bool playerLeft = true;
		public bool enemyLeft = true;

		public float height = 0;
		public bool isActive = true;

		void Start ()
		{
			sr = GetComponent<SpriteRenderer>();

			this.size = 1.0f;
			this.rect = new Rect(transform.position.x - 0.5f, transform.position.z - 0.5f, 1, 1);
//			SetTileState(TileState.Clear);
			if(sr.color != Color.yellow)
			{
				sr.color = Color.white;
				sr.enabled = false;
			}
		}

		public void SetInit(TileType tt, int index, Vector2 pos)
		{
			this.tile_type = tt;
			this.size = 1.0f;
			this.rect = new Rect(transform.position.x - 0.5f, transform.position.z - 0.5f, 1, 1);
			this.index = index;
			this.pos = pos;
		}

		public void SetTileHeight(float height)
		{			
			Vector3 pos = transform.position;
			pos.y = height;
			transform.position = pos;
			
			this.height = height;
		}

		public void SetTileState(TileState tile_state)
		{
			if(sr.color != Color.yellow)
			{
			switch(tile_state)
			{
			case TileState.Clear:
				if(TurnManager.Instance.turnState != TurnState.PlayerTurn)
				{
					sr.color = Color.white;
					sr.enabled = playerLeft ? false : true;
					enemyLeft = true;
				}
				else
				{
					if(enemyLeft)
					{	
						sr.color = Color.white;
						sr.enabled = false;
					}					
					playerLeft = true;
				}
				break;
			case TileState.PlayerOn:
				if(enemyLeft)
				{	
					sr.color = Color.white;
					sr.enabled = true;
				}	
				else
				{
					sr.color = Color.red;
					sr.enabled = true;
				}
				playerLeft = false;
				break;
			case TileState.EnemyOn:
				sr.color = Color.red;
				sr.enabled = true;
				enemyLeft = false;
				break;
			}
			}
			else
			{
				sr.enabled = true;
			}
			this.tile_state = tile_state;
		}

		public void Reset()
		{
			playerLeft = true;
			enemyLeft = true;
			if(sr.color != Color.yellow)
			{
				sr.color = Color.white;
				sr.enabled = false;
			}

			this.tile_state = TileState.Clear;
		}

		public bool IsPlayerOnTile()
		{
			if(PlayerManager.Instance.cross_current.tile_center == this)
				return true;
			return false;
		}
	}
}

#region OldNodes
//namespace TheVandals
//{
//	using UnityEngine;
//	using System;
//	
//	public enum TileType
//	{
//		Horizontal,
//		VerticalR,
//		VerticalL,
//		VerticalUp,
//		VerticalDown
//	}
//	
//	public class TileEntity : MonoBehaviour
//	{		
//		public TileType tile_type;
//		
//		public int index;
//		public float size;
//		public Rect rect;
//		
//		//		public NodeEntity node_Right;
//		//		public NodeEntity node_Left;
//		//		public NodeEntity node_Up;
//		//		public NodeEntity node_Bot;
//		
//		void Start ()
//		{
//			this.size = 1.0f;
//			
//			//			node_Right = new NodeEntity(transform.position);
//			//			node_Left = new NodeEntity(transform.position);
//			//			node_Up = new NodeEntity(transform.position);
//			//			node_Bot = new NodeEntity(transform.position);
//			
//			//			node_Right.position.x += size / 2;
//			//			node_Left.position.x -= size / 2;
//			//			node_Up.position.z += size / 2;
//			//			node_Bot.position.z -= size / 2;
//			
//			rect = new Rect(transform.position.x - 0.5f, transform.position.z - 0.5f, 1, 1);
//		}
//		
//		public void SetInit(TileType tt)
//		{
//			this.tile_type = tt;
//			this.size = 1.0f;
//			
//			//			this.node_Right = new NodeEntity(transform.position);
//			//			this.node_Left = new NodeEntity(transform.position);
//			//			this.node_Up = new NodeEntity(transform.position);
//			//			this.node_Bot = new NodeEntity(transform.position);
//			
//			//			if(tile_type == TileType.Horizontal)
//			//			{
//			//				this.node_Right.position.x += size / 2;
//			//				this.node_Left.position.x -= size / 2;
//			//				this.node_Up.position.z += size / 2;
//			//				this.node_Bot.position.z -= size / 2;
//			
//			this.rect = new Rect(transform.position.x - 0.5f, transform.position.z - 0.5f, 1, 1);
//			//			}
//			//			else if(tile_type == TileType.VerticalR || tile_type == TileType.VerticalL)
//			//			{
//			//				this.node_Right.position.x += size / 2;
//			//				this.node_Left.position.x -= size / 2;
//			//				this.node_Up.position.z += size / 2;
//			//				this.node_Bot.position.z -= size / 2;
//			//
//			//				this.node_Right.position = transform.TransformVector(this.node_Right.position);
//			//				
//			//				this.rect = new Rect(transform.position.x - 0.5f, transform.position.z - 0.5f, 1, 1);
//			//			}
//			//			else
//			//			{
//			//				this.node_Right.position.x += size / 2;
//			//				this.node_Left.position.x -= size / 2;
//			//				this.node_Up.position.z += size / 2;
//			//				this.node_Bot.position.z -= size / 2;
//			//				
//			//				this.node_Right.position = transform.TransformVector(this.node_Right.position);
//			//				this.rect = new Rect(transform.position.x - 0.5f, transform.position.z - 0.5f, 1, 1);
//			//
//			//			}
//			
//		}
//		
//		public void DrawTile()
//		{
//			Gizmos.color = Color.yellow;
//			//			Gizmos.DrawCube(node_Right.position, 0.2f * Vector3.one);
//			//			Gizmos.DrawCube(node_Left.position, 0.2f * Vector3.one);
//			//			Gizmos.DrawCube(node_Up.position, 0.2f * Vector3.one);
//			//			Gizmos.DrawCube(node_Bot.position, 0.2f * Vector3.one);
//			
//			Gizmos.color = Color.black;
//			Gizmos.DrawCube(transform.position, 0.2f * Vector3.one);
//		}
//		
//		public void SetTileHeight(float height)
//		{			
//			Vector3 pos = transform.position;
//			pos.y = height;
//			transform.position = pos;
//		}
//	}
//}

#endregion