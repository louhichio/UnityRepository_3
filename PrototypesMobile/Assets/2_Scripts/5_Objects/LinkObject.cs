namespace TheVandals
{
	using UnityEngine;
	using System;

	public class LinkObject : MonoBehaviour
	{
		public Tile tile_LinkStart;
		public Tile tile_LinkEnd;
		public bool activate;

		[HideInInspector]
		public bool isActive;
		public int index;

		void OnDrawGizmos()
		{			
			if(tile_LinkStart && tile_LinkEnd)
			{
				if(!tile_LinkStart.IsTraversable || !tile_LinkEnd.IsTraversable)
				{
					isActive = false;
				}

				Gizmos.color = isActive ? Color.white : Color.red;
				Gizmos.DrawLine(tile_LinkStart.transform.position, tile_LinkEnd.transform.position);
			}

			if(activate != isActive)
				SetActive();
		}

		public void Initialize (Tile tile_LinkStart, Tile tile_LinkEnd, bool isActive, int index)
		{
			this.tile_LinkStart = tile_LinkStart;
			this.tile_LinkEnd = tile_LinkEnd;
			this.isActive = isActive;
			this.activate = isActive;
			this.index = index;
			transform.position = ((tile_LinkStart.transform.position - tile_LinkEnd.transform.position) / 2) + tile_LinkEnd.transform.position ;
			SetActive();
		}

		private void SetActive()
		{
			isActive = activate;
			if(isActive)
			{
				tile_LinkStart.Neighbours.Add(tile_LinkEnd);
				tile_LinkEnd.Neighbours.Add(tile_LinkStart);
			}
			else
			{
				tile_LinkStart.Neighbours.Remove(tile_LinkEnd);
				tile_LinkEnd.Neighbours.Remove(tile_LinkStart);
			}
		}

		public void SetActive(bool activate)
		{
			this.activate = activate;
			isActive = activate;
			if(isActive)
			{
				tile_LinkStart.Neighbours.Add(tile_LinkEnd);
				tile_LinkEnd.Neighbours.Add(tile_LinkStart);
			}
			else
			{
				tile_LinkStart.Neighbours.Remove(tile_LinkEnd);
				tile_LinkEnd.Neighbours.Remove(tile_LinkStart);
			}
		}
	}
}