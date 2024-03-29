﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//info o skrypcie
// skrypt ogarnia jedynie mechanike karty
// nie ogarnia natomiast wizualizacji karty
// mimo że posiada spriterenderer

//utworzenie karty z prefabem CardWarPrefab
        //GameObject x = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
        //x.GetComponent<CardWar>().SetCardValues(100, 100, 100, 100, CardType.TANK);
        //x.GetComponent<CardWar>().DebugValues();


public enum CardType
{
    TANK,
    HELICOPTER,
    FIGHTER,
    SUBMARINE,
    SHIP,
    BOMBER,
   // NUKE
}

public class CardWar : MonoBehaviour {

    public TextMesh car;
    public TextMesh ini;
    public TextMesh fir;
    public TextMesh def;
    public TextMesh ran;
    public TextMesh tec;

    public string cardName;
    public string description;
    public int initiative;
    public int fireRate;
    public int defence;
    public int range;
    public int technology;
    public CardType type;

    public List<CardType> fightsAgainst;
    public List<CardType> supportAgainst;
	
    public void SetCardValues(int _initiative, int _fireRate, int _defence, int _range, int _technology, CardType _type)
    {
        initiative = _initiative;
        fireRate = _fireRate;
        defence = _defence;
        range = _range;
        technology = _technology;
        type = _type;
    }

    public void SetCardValues(string _cardName, string _description, int _initiative,
        int _fireRate, int _defence, int _range, int _technology,
        CardType _type, List<CardType> _fightsAgainst, List<CardType> _supportAgainst)
    {
        cardName = _cardName;
        description = _description;
        initiative = _initiative;
        fireRate = _fireRate;
        defence = _defence;
        range = _range;
        technology = _technology;
        type = _type;
        fightsAgainst = _fightsAgainst;
        supportAgainst = _supportAgainst;
        LoadIntoTextMesh();
    }

    void LoadIntoTextMesh()
    {
        car.text = cardName;
        ini.text = initiative.ToString();
        fir.text = fireRate.ToString();
        def.text = defence.ToString();
        ran.text = range.ToString();
        tec.text = technology.ToString();
    }
    public void SetSprite(Sprite newSprite)
    {
        this.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    public int TotalValue()
    {
        return initiative + fireRate + defence + technology + range;
    }

    public static int CompareCardWar(CardWar x, CardWar y) 
    {
        if (x == null)
        {
            if (y == null)
            {
                // If x is null and y is null, they're 
                // equal.  
                return 0;
            }
            else
            {
                // If x is null and y is not null, y 
                // is greater.  
                return -1;
            }
        }
        else
        {
            // If x is not null... 
            if (y == null)
            // ...and y is null, x is greater.
            {
                return 1;
            }
            else
            {
                // ...and y is not null, compare the  power of cards
              //  return x.TotalValue().CompareTo(y.TotalValue());
            return y.TotalValue().CompareTo(x.TotalValue()); //przy użyciu funkcji list.SORT() zwracana jest lista od największej wartosci karty
            }
        }
    }
    public void DebugValues()
    {
        Debug.Log("Card values: \n"+cardName +" ini: "+ initiative + " FiRa: " + fireRate + " Def " + defence + " range: " + range + " type: " + type);
    }


}
