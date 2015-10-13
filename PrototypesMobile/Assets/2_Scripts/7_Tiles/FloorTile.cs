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

			int[] arrayCoordinate = ArrayCoordinateFromPosition(transform.position);
			
			X = arrayCoordinate[0];
			Y = arrayCoordinate[1];
			Z = arrayCoordinate[2];

			sr = GetComponent<SpriteRenderer>();
			
			sr.color = Color.white;
			sr.enabled = false;
		}

		public override void Reset ()
		{
			playerLeft = true;
			enemyLeft = true;			
			if(!sr)
				sr = GetComponent<SpriteRenderer>();
			if(sr.color != Color.yellow)
			{
				sr.color = Color.white;
				sr.enabled = false;
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
						sr.color = Color.white;
						sr.enabled = playerLeft ? false : true;
						enemyLeft = true;
						if(enemyCount == 0)						
							this.tile_state = playerLeft ? tile_state : TileState.PlayerOn;						
					}
					else
					{
						if(enemyLeft)
						{	
							sr.color = Color.white;
							sr.enabled = false;
						}				
						
						this.tile_state = enemyLeft ? tile_state : TileState.EnemyOn;
						playerLeft = true;
					}
					break;
				case TileState.PlayerOn:
					if(enemyLeft)
					{	
						sr.color = Color.white;
						sr.enabled = true;
						this.tile_state = tile_state;
					}	
					playerLeft = false;
					break;
				case TileState.EnemyOn:
					sr.color = Color.red;
					sr.enabled = true;
					enemyLeft = false;
					this.tile_state = tile_state;
					break;
				}
			}
			else
			{
				sr.enabled = true;
			}
		}
	}
}