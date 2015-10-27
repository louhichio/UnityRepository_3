using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartScene : MonoBehaviour {

	public Image img_Start;
	public Text textStart;

	public void Play()
	{
		img_Start.enabled = false;
		textStart.enabled = true;
		Application.LoadLevel(1);
	}
}
