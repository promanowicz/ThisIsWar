using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class testingScript : MonoBehaviour {

    public List<CardWar> testList;
    public bool sortuj = false;
	// Use this for initialization
	void Start () {
        //List<GameObject> list  = XmlLoader.instance.GetWarCards();
        //foreach ( GameObject x in list){
        //    testList.Add(x.GetComponent<CardWar>());
        //    Debug.Log(x.GetComponent<CardWar>().TotalValue());
        //}
        
	}
	
	// Update is called once per frame
	void Update () {
        if (sortuj)
        {
            XmlLoader.instance.ShuffleCards();
            //testList.Sort(CardWar.CompareCardWar);
            //Debug.Log("POSORTOWANE!!!!!!!::::::::::::");
            //foreach (CardWar x in testList) Debug.Log(x.TotalValue());
            sortuj = false;
        }
	}
}
