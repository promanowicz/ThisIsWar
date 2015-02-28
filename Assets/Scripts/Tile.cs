using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public List<Tile> neighbours;
	public GameObject slotPrefab;
	public GameManager gameManager;
	public Player owner;
	public Color tileColor;
	public bool production;
    public bool isHidden;
    public string tileName;

    public CardType restriction;
    public List<Army> armiesInRegion;
    //public List<GameObject> armiesInRegion;

	void OnMouseEnter ()
	{
		if(!GetComponent<SpriteRenderer> ().enabled)
			GetComponent<SpriteRenderer> ().enabled = true;
		GetComponent<SpriteRenderer> ().color = tileColor * Color.grey;
	}

	void OnMouseDown ()
	{
		if(/*gameManager.IfPlayerTurn() && */!owner && CheckNeighbours () && gameManager.gamePhase == Phase.SETUP && production)
		{
			owner = gameManager.players[gameManager.currPlayerID];
			tileColor = owner.playerColor;
			tileColor = new Color(tileColor.r, tileColor.g, tileColor.b, 0.25f);
			GetComponent<SpriteRenderer>().color = tileColor;
			owner.AddTile (this);
			//networkView.RPC("UpdateTile", RPCMode.Others);
			gameManager.NextPlayer ();
		}

	}

	void OnMouseExit ()
	{
		GetComponent<SpriteRenderer>().color = tileColor;
		if(!owner)
			GetComponent<SpriteRenderer>().enabled = false;
		
	}

	// Use this for initialization
	void Start () 
	{
		GetComponent<SpriteRenderer>().enabled = false;

		//TWORZENIE SLOTÓW ARMII
		for(int i = 0; i < 3; i++)
		{
			GameObject slot = (GameObject)Instantiate (slotPrefab);
			slot.transform.parent = transform;
		}
		//pozycja slotu 1.
		transform.GetChild (0).localPosition = new Vector3 (0, 0.3f, -0.01f);
		//pozycja slotu 2.
		transform.GetChild (1).localPosition = new Vector3 (-0.3f, 0, -0.01f);
		//pozycja slotu 3.
		transform.GetChild (2).localPosition = new Vector3 (0.3f, 0, -0.01f);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	bool CheckNeighbours ()
	{
		foreach(Tile n in neighbours)
		{
			if(n.owner)
				return false;
		}
		return true;
	}

    public List<Army> GetArmies()
    {
        return armiesInRegion;
        //List<Army> lista = new List<Army>();
        //foreach (GameObject tmp in armiesInRegion) lista.Add(tmp.GetComponent<Army>());
        //return lista;
    }


    [RPC]  void UpdateTile()
	{
				owner = gameManager.players [gameManager.currPlayerID];
				tileColor = owner.playerColor;
				tileColor = new Color (tileColor.r, tileColor.g, tileColor.b, 0.25f);
				gameManager.NextPlayer ();
	}
}
