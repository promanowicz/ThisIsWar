using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    //do testowania
    public Army army1;
    public Army army2;
    public Army army3;
    public byte[] testWynik;

    //zmienne do obsługi bitwy

    //dostarczanie danych:
    public Player _attacker;
    public Tile attackedRegion;

    //obsługa ogólna
    public Text regionText; //nazwa regionu
    public Text roleText;   //rola gracz (atakujący/broniący)
    public Text battleStage;    //etap bitwy + inne info tekstw dole ekarny
    public float fightTime = 1.0f; // czas wykonywania sekwencji w bitwie
    public Vector3 setBackPos = new Vector3(20, 0, 0);
    public bool surrender = false;
    public bool pause = false;
    int currentDefenderArmyFighting = 0; //bierzące armie walączące
    int currentAttackerArmyFighting = 0; //dla atakującegi i broniącego
    bool isAnimRuning = false;
    GameObject[] armiesTmp = new GameObject[6];//zmienna przechowująca kopie pierwszych kart z armii, 
    //do wykorzystania jako obraz armii w lewym(lub prawym) pasku
   // FightResultWrapper result1 = new FightResultWrapper(FightResult.DRAW);   //zmienna na wynik bitwy

    //ustawianie kart atakującego, analogiczne dla gracza A i B
    public Transform[] playerAarmiesPos;    //ustawienie kart wskazujących armie ze zmiennej armies tmp
    public Transform playerAcardPos;        //pozycja karty walczacej
    public Transform playerAsupportPos;     //pozycja karty wspomagającej
    public ParticleSystem playerAparticle1;   //particle dla karty gracza A
    List<CardWar> attackerDrawCards = new List<CardWar>(); // lista kart zremisowanych
    List<Army> attackerArmies;  //armie atakującego gracza
    public GameObject[] playerAbulletHoles;

    //ustawianie kart broniącego analogicznie jak wżej
    public Transform[] playerBarmiesPos;
    public Transform playerBcardPos;
    public Transform playerBsupportPos;
    public ParticleSystem playerBparticle1;
    List<CardWar> defenderDrawCards = new List<CardWar>();
    List<Army> armiesInRegion;  //armie broniącego gracza
    public GameObject[] playerBbulletHoles;


    /*******************************do testowania *********************/
    //funckja ustawia żetony armii (maksymalnie po 3)
    void SetArmiesInPositions()
    {
        int i = 0;
        for (i = 0; i < 3; i++)
        {
            //ustawienie armii broniących i atakujących
            if (i<armiesInRegion.Count)
            {
                armiesTmp[i] = (GameObject)Instantiate(armiesInRegion[i].cardList[0]);  //instancjonowanie "emblematu armii"
                armiesTmp[i].GetComponent<CircleCollider2D>().enabled = false;  
                armiesTmp[i].transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                armiesTmp[i].transform.position = playerBarmiesPos[i].position;     //ustawienie na pozycji obiektu podanego w inspektorze
            }
            //WARNING powtórzony kod
            if (i < attackerArmies.Count)
            {
                armiesTmp[i + 3] = (GameObject)Instantiate(attackerArmies[i].cardList[0]); 
                armiesTmp[i + 3].GetComponent<CircleCollider2D>().enabled = false;
                armiesTmp[i+3].transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                armiesTmp[i + 3].transform.position = playerAarmiesPos[i].position;
            }
        }
    }

    void BulletHolesDisable(){
        for (int i = 0; i < 5; i++)
        {
            playerAbulletHoles[i].SetActive(false);
            playerBbulletHoles[i].SetActive(false);
        }
    }

    //player = "A" or "B"  0<nr<5
    void BulletHolesEnable(int nr, string player)
    {
        if (player == "A")
        {
            playerAbulletHoles[nr].SetActive(true);
        }
        if (player == "B")
        {
            playerBbulletHoles[nr].SetActive(true);
        }
    }

    public void Surrender()
    {
        surrender = true;
    }

    public void Pause()
    {
        pause = true;
    }

    void Start()
    {
        //testowanie
        army1.cardList = XmlLoader.instance.GetWarCards();
        army2.cardList = XmlLoader.instance.GetWarCards();
        army3.cardList = XmlLoader.instance.GetWarCards();
        armiesInRegion = attackedRegion.GetArmies();
        attackerArmies = _attacker.GetArmies();
        SetArmiesInPositions();

       // testWynik = CompareCards(attackerArmies[0].cardList[0].GetComponent<CardWar>(),armiesInRegion[0].cardList[0].GetComponent<CardWar>());
        StartCoroutine(BattleFight());

    }

    //Funkcja porównuje dwie karty i zwraca tablicę przebiegu bitwy przekazywaną następnie do ComparationAnimation
    byte[] CompareCards(CardWar attackerCard, CardWar defenderCard)
    {
        byte[] result = new byte[7]{0,0,0,0,0,0,0}; //pozycje w tablicy to kolejne wyniki porównań statystych zaczynając od wsparcia.
        byte attLittlePoints = 0;
        byte defLittlePoints = 0;

        Debug.Log("Porównuje karty: " + attackerCard.cardName + " z " + defenderCard.cardName);
        //czy karty mogą ze sobą walczyć?
        if (defenderCard.fightsAgainst.Contains(attackerCard.type) && attackerCard.fightsAgainst.Contains(defenderCard.type))
        {
            
            //czy karty leżące na wierzchu talii dają wsparcie. 0 - brak wsparcia, 1 - daje atakującemu, 2 daje broniącemu, 3 - dają obie
            if (attackerDrawCards.Count>0 && attackerDrawCards[0].supportAgainst.Contains(defenderCard.type))
            {
                result[0] += 1;
                attLittlePoints += 1;
            }
            if (defenderDrawCards.Count > 0 && defenderDrawCards[0].supportAgainst.Contains(attackerCard.type))
            {
                result[0] += 2;
                defLittlePoints += 1;
            }

            //porownanie statystyk 0-remis 1 - wygrywa atakujacy 2 - wygrywa obronca
            //inicjatywa
            if (attackerCard.initiative > defenderCard.initiative)
            {
                result[1] = 1;
                attLittlePoints += 1;
            }
            else if (attackerCard.initiative < defenderCard.initiative)
            {
                result[1] = 2;
                defLittlePoints += 1;
            }

            //siła ognia
            if (attackerCard.fireRate > defenderCard.fireRate)
            {
                result[2] = 1;
                attLittlePoints += 1;
            }
            else if (attackerCard.fireRate < defenderCard.fireRate)
            {
                result[2] = 2;
                defLittlePoints += 1;
            }

            //obrona
            if (attackerCard.defence > defenderCard.defence)
            {
                result[3] = 1;
                attLittlePoints += 1;
            }
            else if (attackerCard.defence < defenderCard.defence)
            {
                result[3] = 2;
                defLittlePoints += 1;
            }

            //zasięg
            if (attackerCard.range > defenderCard.range)
            {
                result[4] = 1;
                attLittlePoints += 1;
            }
            else if (attackerCard.range < defenderCard.range)
            {
                result[4] = 2;
                defLittlePoints += 1;
            }

            //technologia
            if (attackerCard.technology > defenderCard.technology)
            {
                result[5] = 1;
                attLittlePoints += 1;
            }
            else if (attackerCard.technology < defenderCard.technology)
            {
                result[5] = 2;
                defLittlePoints += 1;
            }
        }
        if (attLittlePoints > defLittlePoints) result[6] = 1;
        else if (attLittlePoints < defLittlePoints) result[6] = 2;
        else result[6] = 0;

        return result;
    }

    //Funkcja odpowiada za uruchamianie particli w czasie i za wyświetlanie fazy bitwy
    IEnumerator ComparationAnimation(byte[] tab)
    {
        string[] stages = new string[6]{"Wsparcie!","Inicjatywa",
        "Siła Ognia!","Obrona!","Zasięg!","Technologia"};
        for (int i = 0; i < 6; i++)
        {
            if (tab[i] > 0)
            {
                battleStage.text = stages[i];
                if (tab[i] == 1)
                {
                    playerBparticle1.Play();
                    if(i>0)
                    BulletHolesEnable(i-1, "B");
                }
                if (tab[i] == 2)
                {
                    playerAparticle1.Play();
                    if (i > 0)
                    BulletHolesEnable(i - 1, "A");
                }
                if (tab[i] > 2)
                {
                    playerAparticle1.Play();
                    playerBparticle1.Play();
                }
                yield return new WaitForSeconds(fightTime);
            }
            battleStage.text = " ";
        }
        yield return new WaitForSeconds(1f);
        isAnimRuning = false;
    }

    IEnumerator BattleFight()
    {
        //Sprawdzenie ograniczeń regionu
        //sprawdzić czy to była ostatnia runda i dodać
        //co trzecią karte do armii atakującego
        //lerp wjazdu i wyjazdu kart

        //dodać strzały -> BulletHolesDisable(); i enable
        // odsuwać karty po porównaniu

        float zOffset = 1;
        byte[] comparationResult =new byte[7];
        currentAttackerArmyFighting = 0;
        currentDefenderArmyFighting = 0;
        FightResult finalResult = FightResult.DRAW;
        int i = 0;
        int tmp = 0;

        for (i = 0; i < armiesInRegion.Count; i++)
        {
            currentDefenderArmyFighting = i;
            //sprawdzenie czy gracz wciąż ma armie i w nich karty

            //tak długo jak są karty w armiach
            while (armiesInRegion[i].cardList.Count > 0 && attackerArmies[currentAttackerArmyFighting].cardList.Count > 0)
            {
                //ustawienie kart na pozycjach 
                armiesInRegion[i].cardList[0].transform.position = new Vector3(playerBcardPos.position.x, playerBcardPos.position.y, playerBcardPos.position.z-zOffset);
                attackerArmies[currentAttackerArmyFighting].cardList[0].transform.position = new Vector3(playerAcardPos.position.x, playerAcardPos.position.y, playerAcardPos.position.z-zOffset); ;
                //zOffset += 0.5f;

                //porówananie i animacja
                comparationResult = CompareCards(attackerArmies[currentAttackerArmyFighting].cardList[0].GetComponent<CardWar>(),
                    armiesInRegion[i].cardList[0].GetComponent<CardWar>());

                isAnimRuning = true;
                StartCoroutine(ComparationAnimation(comparationResult));
                //czekamy aż zakończy się animacja, ustawia ona isAnimRuning na false
                while (isAnimRuning)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                //Po porównaniu karty są wrzucane do kart zremisowanych dlatego że jeśli jest remis to i tak tam trafią
                //a jeśli wygra któraś ze stron to karty zremisowane strony wygranej wracają do talii z talią zremisowaną wraca tam także karta
                //która ostatnio walczyła

                attackerDrawCards.Insert(0, attackerArmies[currentAttackerArmyFighting].cardList[0].GetComponent<CardWar>());
                attackerArmies[currentAttackerArmyFighting].cardList.RemoveAt(0);
                defenderDrawCards.Insert(0, armiesInRegion[i].cardList[0].GetComponent<CardWar>());
                armiesInRegion[i].cardList.RemoveAt(0);

                if (surrender) comparationResult[6] = 2;


                //jeśli jest remis karty zostają odłozone
                if (comparationResult[6] == 0)
                {
                    Debug.Log("Wynik porownania to remis");
                    battleStage.text = "Równe karty!";
                    yield return new WaitForSeconds(1f);
                }
               //jeżeli wygrał napastnik
                else if (comparationResult[6] == 1)
                {
                    Debug.Log("Wynik porownania to zwycięstwo atakującego");
                    //karty napastnika wracają do talii
                    tmp = attackerDrawCards.Count;
                    for (int t = 0; t < tmp; t++)
                    {
                        attackerDrawCards[0].transform.position = setBackPos;
                        attackerArmies[currentAttackerArmyFighting].cardList.Add(attackerDrawCards[0].transform.gameObject);
                        attackerDrawCards.RemoveAt(0);
                    }
                    //karty obrońcy wracają do "krupiera" 
                    tmp = defenderDrawCards.Count;
                    for (int t = 0; t < tmp; t++)
                    {
                        defenderDrawCards[0].transform.position = setBackPos;
                        XmlLoader.instance.allWarCards.Add(defenderDrawCards[0].transform.gameObject);
                        defenderDrawCards.RemoveAt(0);
                    }
                }
                //Jeśli wygrał obrońca
                else if (comparationResult[6] == 2)
                {
                    Debug.Log("Wynik porownania to zwycięstwo obrońcy");
                    //karty napastnika wracają do krupiera
                    tmp = attackerDrawCards.Count;
                    for (int t = 0; t < tmp; t++)
                    {
                        attackerDrawCards[0].transform.position = setBackPos;
                        XmlLoader.instance.allWarCards.Add(attackerDrawCards[0].transform.gameObject);
                        attackerDrawCards.RemoveAt(0);
                    }
                    //karty obrońcy wracają  do jego talii
                    tmp = defenderDrawCards.Count;
                    for (int t = 0; t < tmp; t++)
                    {
                        defenderDrawCards[0].transform.position = setBackPos;
                        armiesInRegion[i].cardList.Add(defenderDrawCards[0].transform.gameObject);
                        defenderDrawCards.RemoveAt(0);
                    }
                }


                if (surrender)
                {
                    battleStage.text = "Atakujący poddał się";
                    yield break;
                }
                if (pause)
                {
                    yield return new WaitForSeconds(5f);
                    pause = false;
                }

                //wyłączenie kul
                BulletHolesDisable();


                //gdyby skonczyly sie atakujacemu karty w danej armii przejdź do kolejnej armii
                if(attackerArmies[currentAttackerArmyFighting].cardList.Count==0)
                {
                    Debug.Log("Tu powinno być przejście do drugiej armmii" + attackerArmies.Count +" " + currentAttackerArmyFighting);
                    if (attackerArmies.Count > currentAttackerArmyFighting +1) 
                    { 
                        currentAttackerArmyFighting += 1;
                        Debug.Log("Było przejście do drugiej armii");
                    }
                    else
                    {
                        Debug.Log("Atakujący nie ma więcej kart");
                    }
                }
                if (i == 2 && armiesInRegion[i].cardList.Count == 0)
                {
                    Debug.Log("Obrońca nie ma więcej kart");
                }

                //ustawienie kart wsparcia
                SetSupportCards();

                battleStage.text = "Następna karta!";
                yield return new WaitForSeconds(1f);
            }

        }
        //po co ta dekrementacja?
        //tu sie konczy for ;d
        --i;

        //sprawdzenie wyniku bitwy
        if (attackerArmies[currentAttackerArmyFighting].cardList.Count > armiesInRegion[i].cardList.Count)
        {
            finalResult = FightResult.WIN;
            Debug.Log("wygrales " + roleText.text);
           battleStage.text = "Wygrana atakującego!";
           
        }
        else if (attackerArmies[currentAttackerArmyFighting].cardList.Count < armiesInRegion[i].cardList.Count)
        {
            finalResult = FightResult.LOSE;
            battleStage.text = "Wygrana obrońcy!";
        }
        else 
        { 
            finalResult = FightResult.LOSE;
            battleStage.text = "Remis";
        }

        yield return new WaitForSeconds(1f);
    }

    //Funkcje ustawiające karty w celu poprawnego działania powinny być używane wyłącznie w funkcji porównujacej armmie, w bitwie
    //odstawienie kart do wsparcia
    void SetSupportCards()
    {
        Vector3 nextPositionA = new Vector3(playerAsupportPos.position.x, playerAsupportPos.position.y, playerAsupportPos.position.z);
        Vector3 nextPositionB = new Vector3(playerBsupportPos.position.x, playerBsupportPos.position.y, playerBsupportPos.position.z);
        foreach (CardWar x in attackerDrawCards)
        {
            Debug.Log("Ustawiam wsparcie dla gracza atakującego" + attackerDrawCards.Count);
            x.gameObject.transform.position = nextPositionA;
            nextPositionA.x += 0.2f;
            nextPositionA.z += 1.5f;
        }
        foreach (CardWar x in defenderDrawCards)
        {
            Debug.Log("Ustawiam wsparcie dla gracza broniącego" + defenderDrawCards.Count);
            x.gameObject.transform.position = nextPositionB;
            nextPositionB.x += 0.2f;
            nextPositionB.z += 1.5f;
        }
    }
    //odstawienie kart zeby nie przesłaniały walczących kart
    void SetCardsBack(int i)
    {
        attackerArmies[currentAttackerArmyFighting].cardList[0].transform.position = XmlLoader.instance.transform.position;
        armiesInRegion[i].cardList[0].transform.position = XmlLoader.instance.transform.position;
    }
}
