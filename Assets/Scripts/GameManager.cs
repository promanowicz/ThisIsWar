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
	public List<Player> players; //lista graczy
	public List<Transform> playerBars;
	public Transform playerMarker; //znacznik pokazujący, który gracz aktualnie wykonuje ruch
	public int currPlayerID = 0; //ID aktywnego gracza
	public int turn;
	public Text phaseText;
	//public Button nextTurnButton;
    public int roundNumber;

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

		//nextTurnButton.gameObject.SetActive (false);
    }

	// Use this for initialization
	void Start () 
	{
        roundNumber = 0;

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void NextPlayer () //Przekazanie tury następnemu graczowi
	{
		currPlayerID++;
		currPlayerID %= players.Count;
		playerMarker.position = playerBars [currPlayerID].position;
		Debug.Log ("current player:" + currPlayerID);

		if(gamePhase == Phase.SETUP && currPlayerID == 0)
		{
			gamePhase = Phase.REINFORCE;
			//nextTurnButton.gameObject.SetActive (true);
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
			phaseText.text = gamePhase.ToString ();
			//players[currPlayerID].CreateFogOfWar ();
		}
	}
}
