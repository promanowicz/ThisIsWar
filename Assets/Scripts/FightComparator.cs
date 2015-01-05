using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//wynik walki ukazany z perspektywy atakującego
// jeśl gracz atakujacy przegrał to wygrał obrońca
public enum FightResult
{
    WIN,
    DRAW,
    LOSE
}

//NA ZWIĘKSZENIE WYDAJNOSCI:
//przy usuwaniu kart nalezy zamieniać karte pozycją z ostatnią kartą i usunąć ją z końca
//Usuwanie karty z początku powoduje konieczność przesunięcia całej kolejki
//http://stackoverflow.com/questions/18142872/how-to-remove-an-object-from-listt-in-c-sharp-and-return-the-removed-object

public class FightComparator : MonoBehaviour
{
    public GameObject _attacker;
    public GameObject attackedRegion;
    public Army army1;
    public Army army2;


    public Transform[] playerAarmies;
    public Transform[] playerBarmies;
    GameObject[] armiesTmp = new GameObject[6];
    public Transform playerAcardPos;
    public Transform playerBcardPos;
    public Transform playerAsupportPos;
    public Transform playerBsupprtPos;
    public Text regionText;
    public Text attDefText;
    public Text battleStage;

    //w tych dwóch zmiennych przechowywane będą karty odłożone gdy nie mogły ze sobą walczyć
    List<CardWar> attackerDrawCards = new List<CardWar>();
    List<CardWar> defenderDrawCards = new List<CardWar>();
    Army attacker;
    List<Army> armiesInRegion;

    void Start()
    {
        _attacker.GetComponent<Army>().cardList = XmlLoader.instance.GetWarCards();
        army1.cardList = XmlLoader.instance.GetWarCards();
        army2.cardList = XmlLoader.instance.GetWarCards();
        attackedRegion.GetComponent<Tile>().armiesInRegion.Add(army1);
        attackedRegion.GetComponent<Tile>().armiesInRegion.Add(army2);
        
        GetArmies();
		Czekaj ();
    }
    IEnumerator GetArmies()
    {
        attacker = _attacker.GetComponent<Army>();
        armiesInRegion = attackedRegion.GetComponent<Tile>().armiesInRegion;
        regionText.text = "Region: " + attackedRegion.GetComponent<Tile>().tileName;

        if (_attacker == null || armiesInRegion == null)
        {
            Debug.LogError("brak armi do porównania - nie prawidłowe obiekty podane jako parametry funkcji CompareArmies \n" +
                           " Podaj obiekt zawierający skrypt Army , oraz drugi zawierający skrypt Tile");
        }
        yield return new WaitForSeconds(5f);
        
       

    }

    void Czekaj()
    {
        for (int i = 0; i < attacker.cardList.Count && i < 3; i++)
            if (attacker.cardList[i].GetComponent<CardWar>().type != attackedRegion.GetComponent<Tile>().restriction)
            {
                Debug.Log("Armia nie spełnia wymagań");
                battleStage.text = "Armia atakująca nie spełnia wymagań";
            }
        
       
    }
    //wynik metody ukazany z perspektywy atakującego
    public FightResult CompareArmies(GameObject _attacker, GameObject attackedRegion)
    {
        FightResult cmpResult;
        //Sprawdzenie czy napastnik posiada w swojej talii
        //karty ułożone tak aby można było najechać region
        for (int i = 0; i < attacker.cardList.Count && i < 3; i++)
            if (attacker.cardList[i].GetComponent<CardWar>().type != attackedRegion.GetComponent<Tile>().restriction)
            {
                Debug.Log("Armia nie spełnia wymagań");
                return FightResult.DRAW;
            }

        //napastnik musi zmierzyć się ze wszystkimi armiami w regionie
        // OBROŃCA POWINIEN MIEĆ OPCJĘ WYBORU KTÓRE ARMIE W JAKIJ KOLEJNOSCI SIĘ BRONIĄ
        for (int i = 0; i < armiesInRegion.Count; i++)
        {
            //do póki napastnik ma karty lub armie w regione maja karty
            while (attacker.cardList.Count > 0 || armiesInRegion[i].cardList.Count > 0)
            {
                //wynik porówniania kart: pierwszej karty w armii atakującej, i pierwszej karty w aktualnej armii broniącej z uwzględnieniem wsparcia;
                cmpResult = CompareCards(attacker.cardList[0].GetComponent<CardWar>(), armiesInRegion[i].cardList[0].GetComponent<CardWar>(), attackerDrawCards[0], defenderDrawCards[0]);

                //Po porównaniu karty są wrzucane do kart zremisowanych dlatego że jeśli jest remis to i tak tam trafią
                //a jeśli wygra któraś ze stron to karty zremisowane strony wygranej wracają do talii, z talią zremisowaną wraca tam także karta 
                //która ostatnio walczyła
                attackerDrawCards.Insert(0, attacker.cardList[0].GetComponent<CardWar>());
                attacker.cardList.RemoveAt(0);

                defenderDrawCards.Insert(0, armiesInRegion[i].cardList[0].GetComponent<CardWar>());
                armiesInRegion[i].cardList.RemoveAt(0);

                //jeśli jest remis karty zostają odłozone
                if (cmpResult == FightResult.DRAW)
                {
                    Debug.Log("Wynik porownania to remis");

                }
                //sprawdzić czy to była ostatnia runda i dodać
                //co trzecią karte do armii atakującego

                //jeżeli wygrał napastnik
                else if (cmpResult == FightResult.WIN)
                {
                    //karty napastnika wracają do talii
                    for (int t = 0; t < attackerDrawCards.Count; t++)
                    {
                        attacker.cardList.Add(attackerDrawCards[0].transform.parent.gameObject);
                        attackerDrawCards.RemoveAt(0);
                    }

                    //karty obrońcy wracają do "krupiera" 
                    for (int t = 0; t < defenderDrawCards.Count; t++)
                    {
                        XmlLoader.instance.allWarCards.Add(defenderDrawCards[0].transform.parent.gameObject);
                        defenderDrawCards.RemoveAt(0);
                    }
                }
                //Jeśli wygrał obrońca
                else if (cmpResult == FightResult.LOSE)
                {
                    //karty napastnika wracają do krupiera
                    for (int t = 0; t < attackerDrawCards.Count; t++)
                    {
                        XmlLoader.instance.allWarCards.Add(attackerDrawCards[0].transform.parent.gameObject);
                        attackerDrawCards.RemoveAt(0);
                    }

                    //karty obrońcy wracają  do jego talii
                    for (int t = 0; t < defenderDrawCards.Count; t++)
                    {
                        armiesInRegion[i].cardList.Add(defenderDrawCards[0].transform.parent.gameObject);
                        defenderDrawCards.RemoveAt(0);
                    }
                }

            }

        }

        return FightResult.WIN;
    }

    //metoda porównuje dwie karty pierwszy argument to karta gracza atakujacego
    private static FightResult CompareCards(CardWar attackerCard, CardWar defenderCard, CardWar attackerSupport, CardWar defenderSupport)
    {
        //male punkty walki między graczami
        byte attLilPoints = 0;
        byte defLilPoints = 0;
        //jesli karty mogą ze soba walczyć
        //czy obie muszą walczyć ze sobą,, czy wystarczy zeby tylko jedna karta mogła atakaowac drugą?
        if (defenderCard.fightsAgainst.Contains(attackerCard.type) || attackerCard.fightsAgainst.Contains(defenderCard.type))
        {
            Debug.Log("Porównuje karty: " + attackerCard.cardName + " z " + defenderCard.cardName);
            //porównanie pięciu statystyk i dodawanie punktow
            if (attackerCard.initiative > defenderCard.initiative) attLilPoints += 1;
            else if (attackerCard.initiative < defenderCard.initiative) defLilPoints += 1;

            if (attackerCard.fireRate > defenderCard.fireRate) attLilPoints += 1;
            else if (attackerCard.fireRate < defenderCard.fireRate) defLilPoints += 1;

            if (attackerCard.defence > defenderCard.defence) attLilPoints += 1;
            else if (attackerCard.defence < defenderCard.defence) defLilPoints += 1;

            if (attackerCard.range > defenderCard.range) attLilPoints += 1;
            else if (attackerCard.range < defenderCard.range) defLilPoints += 1;

            if (attackerCard.technology > defenderCard.technology) attLilPoints += 1;
            else if (attackerCard.technology < defenderCard.technology) defLilPoints += 1;

            //uwzględnienie wsparcia, jesli jest
            if (attackerSupport != null && attackerSupport.supportAgainst.Contains(defenderCard.type)) attLilPoints += 1;
            if (defenderSupport != null && defenderSupport.supportAgainst.Contains(attackerCard.type)) defLilPoints += 1;

            if (attLilPoints > defLilPoints) return FightResult.WIN;
            else if (attLilPoints < defLilPoints) return FightResult.LOSE;
            else return FightResult.DRAW;
        }
        else
        {
            return FightResult.DRAW;
        }
    }

}
