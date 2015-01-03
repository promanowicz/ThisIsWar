using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public List<Tile> neighbours;
	public GameManager gameManager;
	public Transform tileInfo;
	public Player owner;
	public Color tileColor;
	public bool production;
    public bool isHidden;
    public string tileName;

    public CardType restriction;
    public List<Army> armiesInRegion;

	void OnMouseEnter ()
	{
		if(!GetComponent<SpriteRenderer> ().enabled)
			GetComponent<SpriteRenderer> ().enabled = true;
		GetComponent<SpriteRenderer> ().color = tileColor * Color.grey;
	}

	void OnMouseOver ()
	{
		if(gameManager.gamePhase == Phase.MOVE)
		{
			tileInfo.position = new Vector3(Input.mousePosition.x,Input.mousePosition.y);

			if (Input.mousePosition.x > Screen.width / 2)
				tileInfo.GetComponent<RectTransform> ().pivot = new Vector2 (1, tileInfo.GetComponent<RectTransform> ().pivot.y);
			else
				tileInfo.GetComponent<RectTransform> ().pivot = new Vector2 (0, tileInfo.GetComponent<RectTransform> ().pivot.y);

			if (Input.mousePosition.y > Screen.height / 2)
				tileInfo.GetComponent<RectTransform> ().pivot = new Vector2 (tileInfo.GetComponent<RectTransform> ().pivot.x, 1);
			else
				tileInfo.GetComponent<RectTransform> ().pivot = new Vector2 (tileInfo.GetComponent<RectTransform> ().pivot.x, 0);
		}
	}

	void OnMouseDown ()
	{
		if(gameManager.IfPlayerTurn()&&!owner && CheckNeighbours () && gameManager.gamePhase == Phase.SETUP && production)
		{
			owner = gameManager.players[gameManager.currPlayerID];
			tileColor = owner.playerColor;
			tileColor = new Color(tileColor.r, tileColor.g, tileColor.b, 0.25f);
			GetComponent<SpriteRenderer>().color = tileColor;
			owner.CreateArmyCounter (this);
			networkView.RPC("UpdateTile", RPCMode.Others);
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

    [RPC]  void UpdateTile()
	{
				owner = gameManager.players [gameManager.currPlayerID];
				tileColor = owner.playerColor;
				tileColor = new Color (tileColor.r, tileColor.g, tileColor.b, 0.25f);
				gameManager.NextPlayer ();
	}
		
}
