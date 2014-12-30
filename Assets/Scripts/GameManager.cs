using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum Phase
{
	SETUP,
	REINFORCE,
	MOVE,
	BATTLE
}

public class GameManager : MonoBehaviour {

	public Transform map;
	public Phase gamePhase = Phase.SETUP;
	public List<Player> players;
	public int currPlayerID = 0;
	public int turn;
	public Text phaseText;
	public Button nextTurnButton;
    public int roundNumber;
	public Image playerMarker; //znacznik pokazujący, który gracz aktualnie wykonuje ruch

    public List<CardWar> allWarCards { get; set; }
    public List<CardWar> allStrategyCards { get; set; }
    public static GameManager instance
    {
        get;
        private set;
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else Debug.Log("GameManager już istnieje!!!");

		nextTurnButton.gameObject.SetActive (false);
    }

	// Use this for initialization
	void Start () 
	{
        roundNumber = 0;
		playerMarker.color = players[currPlayerID].playerColor;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if(Input.GetKeyDown (KeyCode.Space))
		   //NextPlayer ();
	}

	public void NextPlayer ()
	{
		currPlayerID++;
		currPlayerID %= players.Count;
		playerMarker.color = players[currPlayerID].playerColor;
		Debug.Log ("current player:" + currPlayerID);

		if(gamePhase == Phase.SETUP && currPlayerID == 0)
		{
			gamePhase = Phase.REINFORCE;
			nextTurnButton.gameObject.SetActive (true);
		}

		else if(gamePhase == Phase.REINFORCE && currPlayerID == 0)
			gamePhase = Phase.MOVE;

		else if(gamePhase == Phase.MOVE && currPlayerID == 0)
		{
			gamePhase = Phase.REINFORCE;
			turn++;
		}

		if(gamePhase != Phase.SETUP)
		{
			phaseText.text = "TURN "+turn+": "+gamePhase.ToString ();
			//players[currPlayerID].CreateFogOfWar ();
		}
	}
}
