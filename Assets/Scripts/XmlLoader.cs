using UnityEngine;
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


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Debug.Log("XML LOADER JUŻ ISTNIEJE!");

    }

    void Start(){
        ReadFromXML();
    }
    //pobiera karty z bazy, zwraca je i usuwa własne referencje z tablicy do nich
    //wciąż jest możliwe odwołanie do kart, bo są dziecmi loadera
    public List<GameObject> GetWarCards()
    {
        List<GameObject> tmp = allWarCards.GetRange(0,5);//+GameManager.instance.roundNumber);
        allWarCards.RemoveRange(0, 5 );//+ GameManager.instance.roundNumber);

        return tmp;
    }
    //funkcja wczytująca dane z XML i zapisująca gameobjecty do publicznej listy kart;
    void ReadFromXML()
    {
        //zmienne do wczytywania kart
        CardType _type=CardType.TANK;
        string _cardName="", _description="";
        int _initiative = 0, _fireRate = 0, _defence = 0, _range = 0, _technology = 0;
        string spriteName;
        List<CardType> _fightsAgainst = new List<CardType>();
        List<CardType> _supportAgainst = new List<CardType>();
        GameObject karta;

        XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
        xmlDoc.LoadXml(jednostkiXML.text); // load the file.
        XmlNodeList VehicleList = xmlDoc.GetElementsByTagName("ID"); // array of the level nodes.

        Debug.Log(VehicleList.Count);


        int x = 0;
        //przegladanie xml'a po id
        foreach (XmlNode vehicleID in VehicleList)
        {
            //pobranie wszystkich podwęzłów dla pojedyńczych ID
            XmlNodeList data = vehicleID.ChildNodes;

            //Pobranie typu i nazwy spritea
            switch(vehicleID.ParentNode.Name){
                 case "czolgi": _type=CardType.TANK;
                        break;
                 case "helikoptery": _type=CardType.HELICOPTER;
                        break;
                 case "samoloty": _type=CardType.FIGHTER;
                        break;
                 case "bombowce": _type=CardType.BOMBER;
                        break;
                 case "statki": _type=CardType.SHIP;
                        break;
                 case "lodzie_podwodne": _type=CardType.SUBMARINE;
                        break;
                default: Debug.Log("Nie rozpoznałem typu jednostki przy wczytywaniu z XML");
                        break;
            }
            spriteName = vehicleID.Attributes["NAME"].Value; 
           // Debug.Log("vehicleID.Name: "+ vehicleID.Attributes["NAME"].Value);

            //odczyt danych z pól i przypisanie do zmiennych tymczasowych
            foreach(XmlNode attrib in data){
              //  Debug.Log(attrib.InnerText);
                switch (attrib.Name)
                {
                    case "nazwa": _cardName = attrib.InnerText;
                        break;

                    case "atak":
                        _fightsAgainst = ParseStringToCardType(attrib.InnerText);
                            break;

                    case "wsparcie":
                         _supportAgainst = ParseStringToCardType(attrib.InnerText);
                        break;

                    case "inicjatywa": _initiative = ParseStringToInt(attrib.InnerText);
                        break;

                    case "sila_ognia": _fireRate = ParseStringToInt(attrib.InnerText);
                        break;

                    case "obrona": _defence = ParseStringToInt(attrib.InnerText);
                        break;

                    case "zasieg": _range = ParseStringToInt(attrib.InnerText);
                        break;

                    case "technologia": _technology = ParseStringToInt(attrib.InnerText);
                        break;

                    default:
                        Debug.Log("Nie rozpoznałem nazwy attrybutu przy wczytywaniu z XML");
                        break;
                        /*
                    case "":
                        break;
                        */
                }        
            }
            //instancjonowanie obiektu z przypisaniem 
            karta = (GameObject)Instantiate(prefabKartyWojennej, transform.position, transform.rotation);
            karta.GetComponent<CardWar>().SetCardValues(_cardName, _description, _initiative, _fireRate, _defence, _range
                                    , _technology, _type, _fightsAgainst, _supportAgainst);
         //   karta.GetComponent<CardWar>().DebugValues();
            karta.name = _cardName;
            karta.transform.parent = transform;

//DODAC FUNKCJE SZUKAJACA OBRAZKA W VEHICLE IMAGES
//sprawdzić wspierane i atakowane!!!
      //      if(spriteName==vehicleImages[0].name)
            karta.transform.Find("image").GetComponent<SpriteRenderer>().sprite = vehicleImages[x];
            x = (x + 1) % vehicleImages.Length;

            //dodanie karty do card holdera
            allWarCards.Add(karta);
        }

        Debug.Log("Wczytano " + allWarCards.Count + " kart wojennych");
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
}
