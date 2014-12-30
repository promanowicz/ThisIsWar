using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliderScript : MonoBehaviour {

    public List<GameObject> cardsInSlider;
    int mid = 0;
    int prevMid = 0;
    int nextMid = 0;
    public int off = 3;
    public Transform midTransform;

    
    void Start()
    {
        LoadCards(XmlLoader.instance.GetWarCards());
    }

    void LoadCards(List<GameObject> karty)
    {
        foreach (GameObject x in karty)
        {
            x.GetComponent<BoxCollider2D>().enabled = false;
            cardsInSlider.Add(x);
        }
        if (cardsInSlider.Count == 1)
        {
            mid = 1;
            prevMid = 0;
        }
        else
        {
            mid = 2;
            prevMid = 1;
            nextMid = 3;
        }
        ResetLayout();
       

    }

    void ResetLayout()
    {
        cardsInSlider[prevMid].GetComponent<BoxCollider2D>().enabled = false;
        cardsInSlider[nextMid].GetComponent<BoxCollider2D>().enabled = false;
        cardsInSlider[mid].GetComponent<BoxCollider2D>().enabled = true;

        cardsInSlider[prevMid].transform.position = new Vector3(midTransform.position.x - off, midTransform.position.y, cardsInSlider[prevMid].transform.position.z);
        cardsInSlider[mid].transform.position = new Vector3(midTransform.position.x , midTransform.position.y, cardsInSlider[mid].transform.position.z);
        cardsInSlider[nextMid].transform.position = new Vector3(midTransform.position.x + off, midTransform.position.y, cardsInSlider[nextMid].transform.position.z);

        Debug.Log(cardsInSlider[mid].name);
    }



   public void MoveRight()
    {
        prevMid = mid;
        mid = nextMid;
        nextMid = (nextMid + 1) % cardsInSlider.Count;
        ResetLayout();
      
    }

  public  void MoveLeft()
    {
        nextMid = mid;
        mid = prevMid;
        prevMid = prevMid - 1;
        if (prevMid < 0) prevMid = cardsInSlider.Count - 1;    
        ResetLayout();
    }

}
