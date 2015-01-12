using UnityEngine;
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

    public void DebugValues()
    {
        Debug.Log("Card values: \n"+cardName +" ini: "+ initiative + " FiRa: " + fireRate + " Def " + defence + " range: " + range + " type: " + type);
    }


}
