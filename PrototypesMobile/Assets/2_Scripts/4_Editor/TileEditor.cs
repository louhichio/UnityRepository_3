namespace TheVandals
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections;

	public class TileEditor : MonoBehaviour 
	{
		private TileEntity tileEntity;

		public void SetTileEntity(TileEntity tileEntity)
		{
			this.tileEntity = tileEntity;
		}
	}
}
