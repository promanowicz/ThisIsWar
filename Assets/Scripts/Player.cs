using UnityEngine;
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
	public Text powerText;

	//ODŚWIERZANIE SPRITE'ÓW SLOTÓW I POTĘGI GRACZA
	public void RefreshSlots ()
	{
		int power = 0;

		foreach(GameObject a in army)
		{
			if(a.GetComponent<Army>().cardList.Count > 0)
			{
				Sprite newSprite = a.GetComponent<Army>().cardList[0].GetComponent<SpriteRenderer>().sprite;
				a.gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
			}

			power+=a.GetComponent<Army>().cardList.Count;
		}

		powerText.text = power.ToString ();
	}

	public void AddTile (Tile tile)
	{
		foreach(Transform slot in tile.transform)
		{
			Army newArmy = slot.GetComponent<Army> ();
			newArmy.owner = this;
			newArmy.dislocation = tile;
			army.Add (slot.gameObject);
		}
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

    public List<Army> GetArmies()
    {
        List<Army> lista = new List<Army>();
        foreach(GameObject tmp in army)lista.Add(tmp.GetComponent<Army>());
        return lista;
    }
    //dodac ograniczenie ilosci pobieranych kart
    List<GameObject> GetWarCardsOnStartUp()
    {
        return XmlLoader.instance.GetWarCards();
    }
}
