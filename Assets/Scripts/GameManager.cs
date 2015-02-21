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

	//public NetworkManager netManager;

	public GameObject reinforcementPrefab;
	public Transform map;
	public Phase gamePhase = Phase.SETUP;
	public List<Player> players; //lista graczy
	public List<Transform> playerBars;
	public Transform playerMarker; //znacznik pokazujący, który gracz aktualnie wykonuje ruch
	public int currPlayerID = 0; //ID aktywnego gracza
	public int turn;
	public Text phaseText;
	//public Button nextTurnButton;
    //public int roundNumber;
	public int baseCardNo = 5;

    public List<CardWar> allWarCards { get; set; }
    public List<CardWar> allStrategyCards { get; set; }
    public static GameManager instance
    {
        get;
        private set;
    }

	private bool reinforcementIsOpen = false;
	private GameObject reinforcementScreen;

    void Awake()
    {
        if (instance == null) instance = this;
        else Debug.Log("GameManager już istnieje!!!");

		//nextTurnButton.gameObject.SetActive (false);
    }

	// Use this for initialization
	void Start () 
	{
        //roundNumber = 0;

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void NextPlayer () //Przekazanie tury następnemu graczowi
	{
		Timer.instance.ResetTimer ();

		currPlayerID++;
		currPlayerID %= players.Count;
		playerMarker.position = playerBars [currPlayerID].position;
		Debug.Log ("current player:" + currPlayerID);

		if(gamePhase == Phase.SETUP && currPlayerID == 0)
		{
			gamePhase = Phase.REINFORCE;
			turn++;
			//TWORZENIE ARMII DLA TERYTORIÓW NIEZALEŻNYCH
			SetupArmies (XmlLoader.instance.allWarCards);
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

		if(gamePhase == Phase.REINFORCE)
		{
			if(reinforcementIsOpen)
			{
				reinforcementIsOpen = false;
				Destroy (reinforcementScreen);
			}

			reinforcementScreen = (GameObject)Instantiate (reinforcementPrefab);
			ArmiesSlider armiesSlider = reinforcementScreen.transform.GetChild (3).GetComponent<ArmiesSlider>();
			armiesSlider.player = players[currPlayerID].gameObject;
			armiesSlider.GetPlayerArmies ();
			reinforcementIsOpen = true;
		}
	}

	/*public bool IfPlayerTurn()
	{
			return currPlayerID == netManager.GetPlayerID();
	}*/

	//TWORZENIE ARMII DLA TERYTORIÓW NIEZALEŻNYCH
	void SetupArmies (List<GameObject> cards)
	{
		foreach(Transform child in map)
		{
			if(child.name == "tile")
			{
				if(child.GetComponent<Tile>().owner == null)
				{

					//LOSOWANIE KART DLA ARMII
					for(int i = 0; i < baseCardNo; i++)
					{
						GameObject newCard = cards[Random.Range (0, cards.Count)];
						child.GetChild (0).GetComponent<Army>().cardList.Add (newCard);
					}

					Sprite newSprite = child.GetChild (0).GetComponent<Army>().cardList[0].GetComponent<SpriteRenderer>().sprite;
					child.GetChild (0).GetComponent<SpriteRenderer>().sprite = newSprite;
				}
			}
		}
	}
}
