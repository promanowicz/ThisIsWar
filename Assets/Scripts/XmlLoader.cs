﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class XmlLoader : MonoBehaviour {

    public static XmlLoader instance = null;
    public TextAsset jednostkiXML;
    public GameObject prefabKartyWojennej;
    public List<GameObject> allWarCards = new List<GameObject>();
    public Sprite[] vehicleImages;
    public Sprite defaultSprite;
    public int shuffleNumber = 150;
    float transOffset = 0;
    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else Debug.Log("XML LOADER JUŻ ISTNIEJE!");
        LoadSprites();
        ReadFromXML();
    }


    void Start(){
        
    }
    //pobiera karty z bazy, zwraca je i usuwa własne referencje z tablicy do nich
    //wciąż jest możliwe odwołanie do kart, bo są dziecmi loadera
    public List<GameObject> GetWarCards()
    {
        List<GameObject> tmp=new List<GameObject>();
        int x = 5; 
        if(x<allWarCards.Count)
        tmp = allWarCards.GetRange(0,x);//+GameManager.instance.roundNumber);
        allWarCards.RemoveRange(0, x );//+ GameManager.instance.roundNumber);

        return tmp;
    }
    //funkcja wczytująca dane z XML i zapisująca gameobjecty do publicznej listy kart;
    void ReadFromXML()
    {
        //zmienne do wczytywania kart
        CardType _type=CardType.TANK;
        string _cardName="", _description="";
        int _initiative = 0, _fireRate = 0, _defence = 0, _range = 0, _technology = 0;
        string spriteName="default";
        List<CardType> _fightsAgainst = new List<CardType>();
        List<CardType> _supportAgainst = new List<CardType>();
        GameObject karta;

        XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
        xmlDoc.LoadXml(jednostkiXML.text); // load the file.
        XmlNodeList VehicleList = xmlDoc.GetElementsByTagName("dane"); // array of the level nodes.

    
        Debug.Log("Znaleziono "+VehicleList.Count+" pojazdow");


        Debug.Log("Rozpoczynam wczytywanie danych");
        //przegladanie xml'a po id
        foreach (XmlNode vehicleID in VehicleList)
        {
            //pobranie wszystkich podwęzłów dla pojedyńczych ID
            XmlNodeList data = vehicleID.ChildNodes;

            //odczyt danych z pól i przypisanie do zmiennych tymczasowych
            foreach (XmlNode attrib in data)
            {
                Debug.Log(attrib.Name + " " +attrib.InnerText);

                switch (attrib.Name)
                {
                    case "type":
                        switch (attrib.InnerText)
                        {
                            case "tank": _type = CardType.TANK;
                                break;
                            case "helicopter": _type = CardType.HELICOPTER;
                                break;
                            case "fighter": _type = CardType.FIGHTER;
                                break;
                            case "bomber": _type = CardType.BOMBER;
                                break;
                            case "naval": _type = CardType.SHIP;
                                break;
                            case "submarine": _type = CardType.SUBMARINE;
                                break;
                            default: Debug.Log("Nie rozpoznałem typu jednostki przy wczytywaniu z XML");
                                break;
                        }
                        break;

                    case "name": _cardName = attrib.InnerText;
                        break;

                    case "png": spriteName = attrib.InnerText;
                        break;

                    case "initiative": _initiative = ParseStringToInt(attrib.InnerText);
                        break;

                    case "firepower": _fireRate = ParseStringToInt(attrib.InnerText);
                        break;

                    case "defense": _defence = ParseStringToInt(attrib.InnerText);
                        break;

                    case "range": _range = ParseStringToInt(attrib.InnerText);
                        break;

                    case "technology": _technology = ParseStringToInt(attrib.InnerText);
                        break;

                    //sekcja wczytywania ataku:

                    case "vstank": if (attrib.InnerText == "1") _fightsAgainst.Add(CardType.TANK);
                        break;
                    case "vshelicopter": if (attrib.InnerText == "1") _fightsAgainst.Add(CardType.HELICOPTER);
                        break;
                    case "vsfighter": if (attrib.InnerText == "1") _fightsAgainst.Add(CardType.FIGHTER);
                        break;
                    case "vsbomber": if (attrib.InnerText == "1") _fightsAgainst.Add(CardType.BOMBER);
                        break;
                    case "vsnaval": if (attrib.InnerText == "1") _fightsAgainst.Add(CardType.SHIP);
                        break;
                    case "vssubmarine": if (attrib.InnerText == "1") _fightsAgainst.Add(CardType.SUBMARINE);
                        break;

                    //sekcja wczytywania wsparcia:

                    case "supporttank": if (attrib.InnerText == "1") _supportAgainst.Add(CardType.TANK);
                        break;
                    case "supporthelicopter": if (attrib.InnerText == "1") _supportAgainst.Add(CardType.HELICOPTER);
                        break;
                    case "supportfighter": if (attrib.InnerText == "1") _supportAgainst.Add(CardType.FIGHTER);
                        break;
                    case "supportbomber": if (attrib.InnerText == "1") _supportAgainst.Add(CardType.BOMBER);
                        break;
                    case "supportnaval": if (attrib.InnerText == "1") _supportAgainst.Add(CardType.SHIP);
                        break;
                    case "supportsubmarine": if (attrib.InnerText == "1") _supportAgainst.Add(CardType.SUBMARINE);
                        break;

                    case "Unit_x0020_type_x0020__x0028_not_x0020_used_x0029_": _description = attrib.InnerText;
                        break;
                    default:
                        Debug.Log("Nie rozpoznałem nazwy attrybutu przy wczytywaniu z XML");
                        break;
                }
            }
            
            //instancjonowanie obiektu z przypisaniem 
            karta = (GameObject)Instantiate(prefabKartyWojennej, new Vector3(transform.position.x+transOffset,transform.position.y,transform.position.z), transform.rotation);
            transOffset += 3.1f*transform.localScale.x;
            karta.GetComponent<CardWar>().SetCardValues(_cardName, _description, _initiative, _fireRate, _defence, _range
                                    , _technology, _type, _fightsAgainst, _supportAgainst);
            //   karta.GetComponent<CardWar>().DebugValues();
            karta.name = _cardName;
            karta.transform.parent = transform;

            karta.GetComponent<SpriteRenderer>().sprite = FindSprite(spriteName);
            //dodanie karty do card holdera
            allWarCards.Add(karta);
            _fightsAgainst = new List<CardType>();
            _supportAgainst = new List<CardType>();

        }

        ShuffleCards();
        Debug.Log("Wczytano " + allWarCards.Count + " kart wojennych");
    }

    Sprite FindSprite(string name)
    {
        for (int i = 0; i < vehicleImages.Length; i++)
        {
            if (vehicleImages[i].name == name) return vehicleImages[i];
        }
        return defaultSprite;
    }

    void LoadSprites()
    {
        Debug.Log("Wczytuję spritey z resource");
        string path = "CardImages";
        vehicleImages = Resources.LoadAll<Sprite>(path);
        Debug.Log("Zakonczylem wczytywac spritey");
    }

    List<CardType> ParseStringToCardType(string tmp)
    {
        string[] tmpTab;
        List<CardType> lst = new List<CardType>();

        tmpTab = tmp.Split(null);
        for (int i = 0; i < tmpTab.Length; i++)
        {
            switch (tmpTab[i])
            {
                case "czolgi": lst.Add(CardType.TANK);
                    break;
                case "helikoptery": lst.Add(CardType.HELICOPTER);
                    break;
                case "samoloty": lst.Add(CardType.FIGHTER);
                    break;
                case "bombowce": lst.Add(CardType.BOMBER);
                    break;
                case "statki": lst.Add(CardType.SHIP);
                    break;
                case "lodzie_podwodne": lst.Add(CardType.SUBMARINE);
                    break;
            }
        }
        return lst;
    }

    public void ShuffleCards()
    {
        GameObject tmp;
        int pos = 0;
        for (int i = 0; i < shuffleNumber; i++)
        {
            pos = Random.Range(0, allWarCards.Count - 1);
            tmp = allWarCards[0];
            allWarCards.RemoveAt(0);
            allWarCards.Insert(pos, tmp);
        }
    }
    int ParseStringToInt(string tmp)
    {
        if (tmp.Equals(""))
        {
            Debug.Log("Pusta wartość przy parsowaniu! (FromXML?)");
            return 0;
        }
        if (tmp.Equals("globalny")) return 1000;
        else
        return int.Parse(tmp);
    }

	//DO USUWANIA KART Z EKRANU
	public void ReturnCards ()
	{
		float offset = 0;
		foreach (Transform card in transform)
		{
			card.localPosition = new Vector3(offset, 0, 0);
			offset+=3.1f;
		}
	}
}
