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
					//TWORZENIE NOWEGO SLOTU ARMII
					GameObject newSlot = new GameObject ("Army Slot");
					newSlot.AddComponent<Army> ();
					newSlot.transform.position = child.transform.position;
					newSlot.transform.parent = child.transform;
					newSlot.transform.localScale = Vector3.one * 0.1f;

					//LOSOWANIE KART DLA ARMII
					for(int i = 0; i < baseCardNo; i++)
					{
						GameObject newCard = cards[Random.Range (0, cards.Count)];
						newSlot.GetComponent<Army>().cardList.Add (newCard);
					}

					Sprite newSprite = newSlot.GetComponent<Army>().cardList[0].GetComponent<SpriteRenderer>().sprite;
					newSlot.AddComponent<SpriteRenderer>().sprite = newSprite;
				}
			}
		}
	}
}
