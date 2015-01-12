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
    public ParticleSystem playerAcardPosPart;
    public ParticleSystem playerBcardPosPart;
    public Transform playerBcardPos;
    public Transform playerAsupportPos;
    public Transform playerBsupportPos;
    public Text regionText;
    public Text attDefText;
    public Text battleStage;

    public int armyFighting = 0;
    public float fightTime = 1.0f;
    //w tych dwóch zmiennych przechowywane będą karty odłożone gdy nie mogły ze sobą walczyć
    List<CardWar> attackerDrawCards = new List<CardWar>();
    List<CardWar> defenderDrawCards = new List<CardWar>();
    Army attacker;
    List<Army> armiesInRegion;

    FightResultWrapper result = new FightResultWrapper(FightResult.DRAW);
    void Start()
    {
        _attacker.GetComponent<Army>().cardList = XmlLoader.instance.GetWarCards();
        army1.cardList = XmlLoader.instance.GetWarCards();
        army2.cardList = XmlLoader.instance.GetWarCards();
        attackedRegion.GetComponent<Tile>().armiesInRegion.Add(army1);
        attackedRegion.GetComponent<Tile>().armiesInRegion.Add(army2);

        
        GetArmies();
        StartCoroutine(CompareArmies(result));

    }

    void SetArmiesInPos()
    {
        int i=0;
        foreach (Army x in armiesInRegion)
        {

            armiesTmp[i] = (GameObject)Instantiate(x.cardList[0]);
            armiesTmp[i].transform.position = playerBarmies[i].position;
            i++;
            if(i==3)break;
        }
        armiesTmp[i] = (GameObject)Instantiate(attacker.cardList[0]);
        armiesTmp[i].transform.position = playerAarmies[0].position;

    }

    void SetCardsInPos()
    {
        //yield return new WaitForSeconds(2.0F);
        if(attacker.cardList.Count>0)
        attacker.cardList[0].transform.position = playerAcardPos.position;

        if (armiesInRegion[armyFighting].GetComponent<Army>().cardList.Count>0)
        armiesInRegion[armyFighting].GetComponent<Army>().cardList[0].transform.position = playerBcardPos.position;

        if(attackerDrawCards.Count>0)
            attackerDrawCards[0].transform.position=playerAsupportPos.position;
        if(defenderDrawCards.Count>0)
            defenderDrawCards[0].transform.position=playerBsupportPos.position;
		
    }

    void GetArmies()
    {
        attacker = _attacker.GetComponent<Army>();
        armiesInRegion = attackedRegion.GetComponent<Tile>().armiesInRegion;
        regionText.text = "Region: " + attackedRegion.GetComponent<Tile>().tileName;

        if (_attacker == null || armiesInRegion == null)
        {
            Debug.LogError("brak armi do porównania - nie prawidłowe obiekty podane jako parametry funkcji CompareArmies \n" +
                           " Podaj obiekt zawierający skrypt Army , oraz drugi zawierający skrypt Tile");
        }
        
        
        for (int i = 0; i < attacker.cardList.Count && i < 3; i++)
            if (attacker.cardList[i].GetComponent<CardWar>().type != attackedRegion.GetComponent<Tile>().restriction)
            {
                Debug.Log("Armia nie spełnia wymagań");
                battleStage.text = "Armia atakująca nie spełnia wymagań";
            }

        SetArmiesInPos();
        SetCardsInPos();
    }

    
    //wynik metody ukazany z perspektywy atakującego
    public IEnumerator CompareArmies(FightResultWrapper result)
    {
        FightResult cmpResult=FightResult.DRAW;
        BoolWrapper finished = new BoolWrapper(false);
        FightResultWrapper wynik = new FightResultWrapper(FightResult.DRAW);
        CardWar tmp1 = null;
        CardWar tmp2 = null;

        yield return new WaitForSeconds(3.0f);
        //napastnik musi zmierzyć się ze wszystkimi armiami w regionie
        // OBROŃCA POWINIEN MIEĆ OPCJĘ WYBORU KTÓRE ARMIE W JAKIJ KOLEJNOSCI SIĘ BRONIĄ
        for (int i = 0; i < armiesInRegion.Count; i++)
        {
            if (attacker.cardList.Count == 0)
            {
                Debug.Log("FOR:");
                battleStage.text = "Attacker Lost!";
                break;
            }
            //do póki napastnik ma karty lub armie w regione maja karty
            while (attacker.cardList.Count > 0 || armiesInRegion[i].cardList.Count > 0)
            {
                Debug.Log("attacker cards: "+ attacker.cardList.Count);
                Debug.Log("defender cards: " + armiesInRegion[i].cardList.Count);
                if (attacker.cardList.Count == 0)
                {
                    battleStage.text = "Attacker Lost!"; 
                    break;
                }


                //wynik porówniania kart: pierwszej karty w armii atakującej, i pierwszej karty w aktualnej armii broniącej z uwzględnieniem wsparcia;
                if (attackerDrawCards.Count > 0) tmp1 = attackerDrawCards[0];
                else tmp1 = null;
                if (defenderDrawCards.Count > 0) tmp2 = defenderDrawCards[0];
                else tmp2 = null;

                battleStage.text = "Następna karta!";
                yield return new WaitForSeconds(1.0f);

                StartCoroutine(CompareCards(attacker.cardList[0].GetComponent<CardWar>(), armiesInRegion[0].GetComponent<Army>().cardList[0].GetComponent<CardWar>(), tmp1, tmp2, wynik, finished));

                yield return new WaitForSeconds(fightTime*8+0.5f);
                
                Debug.Log("wynik:" + wynik.ToString());
                cmpResult = wynik.Value;

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

                SetCardsInPos();
                

            }

        }

        Debug.Log("End of WAR!");
    }

    //metoda porównuje dwie karty pierwszy argument to karta gracza atakujacego
    private IEnumerator CompareCards(CardWar attackerCard, CardWar defenderCard, CardWar attackerSupport, CardWar defenderSupport,FightResultWrapper result,BoolWrapper finished)
    {
        
        //male punkty walki między graczami
        byte attLilPoints = 0;
        byte defLilPoints = 0;
        //jesli karty mogą ze soba walczyć
        //czy obie muszą walczyć ze sobą,, czy wystarczy zeby tylko jedna karta mogła atakaowac drugą?
        battleStage.text = "WALKA!";
        yield return new WaitForSeconds(fightTime);

        if (defenderCard.fightsAgainst.Contains(attackerCard.type) || attackerCard.fightsAgainst.Contains(defenderCard.type))
        {
            Debug.Log("Porównuje karty: " + attackerCard.cardName + " z " + defenderCard.cardName);

            battleStage.text = "Walka trwa: Wsparcie!";
            //uwzględnienie wsparcia, jesli jest
            if (attackerSupport != null && attackerSupport.supportAgainst.Contains(defenderCard.type))
            {
                attLilPoints += 1;
                playerBcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }
            if (defenderSupport != null && defenderSupport.supportAgainst.Contains(attackerCard.type))
            {
                defLilPoints += 1;
                playerAcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }

            //porównanie pięciu statystyk i dodawanie punktow
            battleStage.text = "Walka trwa: Inicjatywa";
            if (attackerCard.initiative > defenderCard.initiative)
            {
                attLilPoints += 1;
                playerBcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }
            else if (attackerCard.initiative < defenderCard.initiative)
            {
                defLilPoints += 1;
                playerAcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }

            battleStage.text = "Walka trwa: Siła Ognia";
            if (attackerCard.fireRate > defenderCard.fireRate)
            {
                attLilPoints += 1;
                playerBcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }
            else if (attackerCard.fireRate < defenderCard.fireRate)
            {
                defLilPoints += 1;
                playerAcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }

            battleStage.text = "Walka trwa: Obrona";
            if (attackerCard.defence > defenderCard.defence)
            {
                attLilPoints += 1;
                playerBcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }
            else if (attackerCard.defence < defenderCard.defence)
            {
                defLilPoints += 1;
                playerAcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }

            battleStage.text = "Walka trwa: Zasięg";
            if (attackerCard.range > defenderCard.range)
            {
                attLilPoints += 1;
                playerBcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }
            else if (attackerCard.range < defenderCard.range)
            {
                defLilPoints += 1;
                playerAcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }

            battleStage.text = "Walka trwa: Technologia";
            if (attackerCard.technology > defenderCard.technology)
            {
                attLilPoints += 1;
                playerBcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }
            else if (attackerCard.technology < defenderCard.technology)
            {
                defLilPoints += 1;
                playerAcardPosPart.Play();
                yield return new WaitForSeconds(fightTime);
            }

         

            if (attLilPoints > defLilPoints) result.Value = FightResult.WIN;
            else if (attLilPoints < defLilPoints) result.Value = FightResult.LOSE;
            else result.Value = FightResult.DRAW;
        }
        else
        {
            result.Value = FightResult.DRAW;
            playerAcardPosPart.Play();
            playerBcardPosPart.Play();
        }
        finished.Value = true;
    }

}

public class BoolWrapper
{
    public bool Value { get; set; }
    public BoolWrapper(bool value) { this.Value = value; }
    public override string ToString(){
        return Value.ToString();
    }
}

public class FightResultWrapper
{
    public FightResult Value { get; set; }
    public FightResultWrapper(FightResult value) { this.Value = value; }
    public override string ToString()
    {
        return Value.ToString();
    }
}
