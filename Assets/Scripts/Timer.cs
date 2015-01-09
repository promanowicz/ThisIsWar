using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

	public int minutes = 1;
	public int seconds = 0;
	public Text timeText;

	// Use this for initialization
	void Start () 
	{
		timeText = GetComponent<Text> ();
		StartCoroutine ("CountDown");
	}

	IEnumerator CountDown ()
	{
		while (true)
		{
			if(seconds==0)
			{
				minutes--;
				seconds = 60;
			}
			seconds--;
			if(minutes==0)
			{
				if(seconds>10)
					timeText.color = Color.yellow;
				else if(seconds>0)
					timeText.color = Color.red;
			}
			timeText.text = minutes+":"+seconds.ToString ("D2");
			yield return new WaitForSeconds (1);

			if(minutes==0 && seconds==0)
			{
				ResetTimer ();
				GameManager.instance.NextPlayer ();
			}
		}
	}

	public void ResetTimer ()
	{
		minutes = 2;
		seconds = 0;
		timeText.color = Color.green;
		//GameManager.instance.NextPlayer ();
	}
}
