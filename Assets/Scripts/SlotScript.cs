using UnityEngine;
using System.Collections;

public class SlotScript : MonoBehaviour 
{
	void OnMouseOver ()
	{
		Debug.Log (gameObject.GetComponent<Army>().owner != null ? ("Army of "+gameObject.GetComponent<Army>().owner) : "Independent region.");
	}

	public void CreateCollider ()
	{
		gameObject.AddComponent<CircleCollider2D> ();
	}
}
