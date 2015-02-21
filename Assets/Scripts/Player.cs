﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public Color playerColor;
	public GameObject counterPrefab;
    public List<GameObject> army = new List<GameObject>();
    public List<GameObject> strategyCards;
    public List<GameObject> cardsReceived; //karty otrzymywane na począktu rundy
	public Text pointsText;


	public void CreateArmyCounter (Tile tile)
	{
		//TWORZENIE NOWEGO SLOTU DLA ARMII
		GameObject newSlot = new GameObject ("Army Slot");
		newSlot.AddComponent<Army> ();
		newSlot.transform.position = tile.transform.position;
		newSlot.transform.parent = tile.transform;
		newSlot.transform.localScale = Vector3.one * 0.1f;

		Army newArmy = newSlot.GetComponent<Army> ();
		newArmy.owner = this;
		newArmy.dislocation = tile;
		army.Add (newSlot);

		int points = int.Parse (pointsText.text);
		pointsText.text = (++points).ToString ();
	}

	public void CreateFogOfWar ()
	{
		foreach(Transform child in GameManager.instance.map)
		{
			Tile tile = child.GetComponent<Tile>();
			tile.renderer.material.color = new Color(0.25f,0.25f,0.25f);
			tile.isHidden = true;
		}

		/*foreach(Player p in GameManager.instance.players)
		{
			if(p!=this) p.gameObject.SetActive (false);
			else p.gameObject.SetActive (true);
		}*/

		foreach(GameObject y in army)
		{
            Army a = y.GetComponent<Army>();
			a.dislocation.renderer.material.color = a.dislocation.tileColor * Color.white;
			a.dislocation.isHidden = false;

			foreach(Tile t in a.dislocation.neighbours)
			{
				t.renderer.material.color = t.tileColor * Color.white;
				t.isHidden = false;
			}
		}
	}

    //dodac ograniczenie ilosci pobieranych kart
    List<GameObject> GetWarCardsOnStartUp()
    {
        return XmlLoader.instance.GetWarCards();
    }
}
