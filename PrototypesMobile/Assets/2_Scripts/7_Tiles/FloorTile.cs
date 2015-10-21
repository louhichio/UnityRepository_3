namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public enum TileState
	{
		Clear,
		PlayerOn,
		EnemyOn,
		EnemyDetect,
		EnemyView
	}

	public class FloorTile : Tile 
	{	
		public TileState tile_state;
		[HideInInspector]
		public SpriteRenderer sr;

		public bool playerLeft = true;
		public bool enemyLeft = true;

		public Rect rect;
		void Update()
		{
			if(enemyCount < 0)
				print (index);
		}
		public override void Initialize (int index)
		{
			Name = "Floor";
			TraversalCost = 1;
			IsTraversable = true;
			ActualBoundaryHeights = new float[]{1f, 1f, 1f, 1f};
			VirtualBoundaryHeights = new float[]{10f, 10f, 10f, 10f};
			CentreUnitOffset =  new Vector3(0, Player.Instance.transform.localScale.y / 2, 0);
			CentreUnitRotation = new Vector3(0, 0, 0);

			this.rect = new Rect(transform.position.x - 0.5f, transform.position.z - 0.5f, 1, 1);
			this.index = index;

			isEnemyOn = false;
			isPlayerOn = false;
			enemyCount = 0;

			isFoVDetect = false;
			isFoVView = false;

			isHide = false;
			isCollectible = false;
			isCaptureOeuvre = false;

			int[] arrayCoordinate = ArrayCoordinateFromPosition(transform.position);
			
			X = arrayCoordinate[0];
			Y = arrayCoordinate[1];
			Z = arrayCoordinate[2];

			sr = GetComponent<SpriteRenderer>();
			sr.enabled = true;			
			sr.color = new Color(0, 0, 0, 0);
		}

		public override void Reset ()
		{
			playerLeft = true;
			enemyLeft = true;			
			if(!sr)
				sr = GetComponent<SpriteRenderer>();

			if(sr.color != Color.yellow)
			{
				sr.color = new Color(0, 0, 0, 0);
				if(!sr.enabled)
				{
					sr.enabled = true;
				}
			}

			isEnemyOn = false;
			isPlayerOn = false;
			enemyCount = 0;

			isFoVDetect = false;
			isFoVView = false;

			this.tile_state = TileState.Clear;
		}

		public override void SetTilesState(TileState tileState)
		{			
			foreach(Tile te in Neighbours)
			{
				if(!Tile.ReferenceEquals(te, null))
				{
					te.SetTileState(tileState);
				}
			}			
		}

		public override void SetTileState(TileState tile_state)
		{
			if(!sr)
				sr = GetComponent<SpriteRenderer>();

			if(sr.color != Color.yellow)
			{
				switch(tile_state)
				{
				case TileState.Clear:
					if(TurnManager.Instance.turnState != TurnState.PlayerTurn)
					{
						sr.color = new Color(1, 1, 1, sr.color.a);
//						sr.enabled = !playerLeft;
						StopAllCoroutines();
						StartCoroutine("FadeRenderer", playerLeft);

						enemyLeft = true;

						if(enemyCount == 0)						
							this.tile_state = playerLeft ? tile_state : TileState.PlayerOn;						
					}
					else
					{
						if(enemyLeft)
						{	
							sr.color = new Color(1, 1, 1, sr.color.a);
//							sr.enabled = false;
							StopAllCoroutines();
							StartCoroutine("FadeRenderer", false);
						}				
						
						this.tile_state = enemyLeft ? tile_state : TileState.EnemyOn;
						playerLeft = true;
					}
					break;
				case TileState.PlayerOn:
					if(enemyLeft)
					{	
						sr.color = new Color(1, 1, 1, sr.color.a);
//						sr.enabled = true;
						StopAllCoroutines();
						StartCoroutine("FadeRenderer", true);
						this.tile_state = tile_state;
					}	
					playerLeft = false;
					break;
				case TileState.EnemyOn:
					sr.color = Color.red;
//					sr.enabled = true;
					
					enemyLeft = false;
					this.tile_state = tile_state;
					break;
				}
			}
			else
			{
				StopAllCoroutines();
				StartCoroutine("FadeRenderer", true);
			}
		}

		private IEnumerator FadeRenderer(bool isFadeIn)
		{
			Color c = sr.color;
			float x = sr.color.a;
			if(isFadeIn)
			{
				while(x < 1)
				{ 				
					c.a = x;
					sr.color = c;
					x += Time.deltaTime * 2;
					yield return null;
				}
				
				c.a = 1;
				sr.color = c;
			}
			else
			{
				while(x > 0)
				{ 				
					c.a = x;
					sr.color = c;
					x -= Time.deltaTime * 2;
					yield return null;
				}
				
				c.a = 0;
				sr.color = c;
			}
		}
	}
}