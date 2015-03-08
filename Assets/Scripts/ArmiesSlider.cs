using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ArmiesSlider : MonoBehaviour {

    public List<Army> playerArmies;
    public GameObject player;
  
    public SliderScript InArmySlider;
    public TextMesh textObj;
    string napis = "Numer Armii \n";
    string numerArmii = "";
    int currentArmy = 0;

   // public Army a1, a2, a3;
  //  public GameObject text;
    void Awake()
    {

     //   textObj = text.GetComponent<Text>();

    }
  
	// Use this for initialization
	void Start () {
        //zmiana ktora powinna usunąć 1 zbędną armię
        //zmiana 26.02 po konfie
        GetPlayerArmies();
        //InArmySlider.ColliderEnableChange(false);


        //do testowania wczytywanie armii z objectow
        //playerArmies = new List<Army>();
        //playerArmies.Add(a1);
        //playerArmies[0].cardList = XmlLoader.instance.GetWarCards();
        //playerArmies.Add(a2);
        //playerArmies[1].cardList = XmlLoader.instance.GetWarCards();
        //playerArmies.Add(a3);
        //playerArmies[2].cardList = XmlLoader.instance.GetWarCards();
        //ShowArmyCards();
        //textObj.text = napis + numerArmii;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        textObj.text = napis + numerArmii;

		if(Input.GetKeyDown ("return"))
		{
			GameManager.instance.NextPlayer ();
			Timer.instance.ResetTimer ();
		}
	}

   public void GetPlayerArmies()
    {    
        Player temp = player.GetComponent<Player>();
        foreach (GameObject x in temp.army) playerArmies.Add(x.GetComponent<Army>());

       //zmiana 26.02 po konfie
        ShowArmyCards();
    }

    void ShowArmyCards()
    {
        if (playerArmies.Count <= 0 || playerArmies==null) numerArmii = "brak armii";
        else
        {
            numerArmii = currentArmy.ToString();
            InArmySlider.SetCards(playerArmies[currentArmy].cardList);
            //InArmySlider.ColliderEnableChange(true);
        }
    }

    public void MoveRight()
    {
        currentArmy += 1;
        if (currentArmy >= playerArmies.Count ) currentArmy = 0;
        ShowArmyCards();

    }

    public void MoveLeft()
    {
        currentArmy -= 1;
        if (currentArmy < 0 ) currentArmy = playerArmies.Count-1;
        ShowArmyCards();
    }

}
