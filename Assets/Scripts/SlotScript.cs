﻿using UnityEngine;
using System.Collections;

public class SlotScript : MonoBehaviour 
{
	public bool detailView = false;
	public bool picked = false;

	TextMesh armyCount;

	void OnMouseOver ()
	{
		Debug.Log (gameObject.GetComponent<Army>().owner != null ? ("Army of "+gameObject.GetComponent<Army>().owner) : "Independent region.");
		
		if (Input.GetMouseButtonUp (1) && !detailView) {
			CheckOtherSlots ();
			
			ShowDetails ();
			//StartCoroutine ("ShowDetails");
		}

		if(Input.GetMouseButtonDown (0) && !picked 
		   && gameObject.GetComponent<Army>().owner == GameManager.instance.players[GameManager.instance.currPlayerID])
			picked = true;
	}

	void Update ()
	{
		if(Input.GetMouseButtonDown (1) && detailView)
		{
			detailView = false;
			armyCount.text = "";
			XmlLoader.instance.ReturnCards ();
		}

		if(detailView)
			UpdatePosition ();

		if(picked)
		{
			MoveArmy ();


			if(Input.GetMouseButtonUp (0))
			{
				transform.position = new Vector3(transform.position.x, transform.position.y, -0.01f);
				picked = false;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		Debug.Log (other.gameObject.name);
	}

	public void CreateCollider ()
	{
		gameObject.AddComponent<CircleCollider2D> ();
		gameObject.GetComponent<CircleCollider2D> ().isTrigger = true;
	}

	//Poruszanie armią
	void MoveArmy ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit hit;
		if(Physics.Raycast (ray, out hit))
		{
			if(hit.collider.gameObject.name == "map")
			{
				transform.position = hit.point - new Vector3(0,0,0.1f);
			}
		}
	}

	//Aktualizacja pozycji szczegułowego widoku slotu
	void UpdatePosition ()
	{
		gameObject.GetComponent<Army>().cardList[0].transform.position -= Camera.main.GetComponent<CameraController>().GetPositionDelta ();
	}

	//Wyświetlenie widoku szczegułowego
	void ShowDetails ()
	{
		detailView = true;

		Ray ray = GameManager.instance.detailCam.ScreenPointToRay (Input.mousePosition);
		gameObject.GetComponent<Army>().cardList[0].transform.position = ray.GetPoint (10);

		armyCount = gameObject.GetComponent<Army> ().cardList [0].transform.GetChild (6).GetComponent<TextMesh> ();
		armyCount.text = gameObject.GetComponent<Army> ().cardList.Count.ToString ();
	}

	//Zamknięcie wszystkich widoków szczegółowych
	void CheckOtherSlots ()
	{
		foreach(Transform child in GameManager.instance.map)
		{
			if(child.name == "tile")
			{
				foreach(Transform slot in child)
				{
					if(slot.GetComponent<SlotScript>().detailView)
					{
						//StopCoroutine (slot.GetComponent<SlotScript>().ShowDetails ());
						slot.GetComponent<SlotScript>().detailView = false;
						XmlLoader.instance.ReturnCards ();
						return;
					}
				}
			}
		}
	}
}
