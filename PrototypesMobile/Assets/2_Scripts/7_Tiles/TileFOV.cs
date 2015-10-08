namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public class TileFOV : MonoBehaviour 
	{
		public int index;
		public SpriteRenderer sr;
		public TileState tile_state;

		public void Initialise()
		{
			sr = GetComponent<SpriteRenderer>();
//			sr.enabled = false;
		}
		public void SetTile()
		{
			SetTileState(TileState.Clear);
		}
		public void SetTile(Tile tile, TileState ts)
		{
			index = tile.index;
			SetTilePosition(tile.transform.position);
			SetTileState(ts);
		}

		public void SetTilePosition(Vector3 pos)
		{
			pos.y += 0.001f;
			transform.position = pos;
		}

		public void SetTileState(TileState ts)
		{
			if(tile_state != ts)
			{
				switch(ts)
				{
				case TileState.Clear:
					sr.enabled = false ;	
					break;
				case TileState.EnemyDetect:
					sr.color = new Color(1,0,0,0.75f);
					sr.enabled = true;
					break;
				case TileState.EnemyView:
					sr.color = new Color(1,0,0,0.35f);
					sr.enabled = true;
					break;
				}			
				tile_state = ts;
			}
		}
	}
}