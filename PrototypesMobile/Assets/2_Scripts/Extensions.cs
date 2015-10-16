using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{

	public static void SetAlpha(this Color col, float alpha)
	{
		Color c = new Color();
		c = col;
		c.a = alpha;
		col = c;
	}

	public static IEnumerator FadeIn(this Color col)
	{
		Color c = col;
		float x = col.a;
		while( x < 1)
		{ 				
			c.a = x;
			col = c;
			x += Time.deltaTime;
			yield return null;
		}		
		
		c.a = 1;
		col = c;
	}

	public static IEnumerator FadeOut(this Color col)
	{
		Color c = col;
		float x = col.a;
		while( x > 0)
		{ 				
			c.a = x;
			col = c;
			x -= Time.deltaTime;
			yield return null;
		}
		
		c.a = 0;
		col = c;
	}
}