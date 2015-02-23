using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//skrypt nie kontroluje skąd dokąd są dodawane obiekty
public class SliderScript : MonoBehaviour {

    public List<GameObject> cardsInSlider;
    int mid = 0;
    int prevMid = 0;
    int nextMid = 0;
    public int off = 3;
    public Transform midTransform;
    public bool loadCards = false;
    public bool isCardNumberShown = false;
    public TextMesh cardNumberTM;

    public void SetCards(List<GameObject> value)
    {
        
            if (cardsInSlider != null)
                foreach (GameObject x in cardsInSlider) x.transform.localPosition = Vector3.zero;
            cardsInSlider = value;
            CheckCountInList();
            ResetLayout();
       
    }
    
    void Start()
    {
        if(loadCards)LoadCards(XmlLoader.instance.GetWarCards());
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<CardDragAndDrop>().clicked) {
            Debug.Log("wyciągam karte");
        cardsInSlider.Remove(col.gameObject);
        CheckCountInList();
        ResetLayout();
        Debug.Log(cardsInSlider.Count + "po odjeciu");
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (!col.gameObject.GetComponent<CardDragAndDrop>().clicked) {
            col.gameObject.transform.position = new Vector3(midTransform.position.x, midTransform.position.y, midTransform.position.z-1);
            col.gameObject.GetComponent<CircleCollider2D>().enabled = false;

        if (!cardsInSlider.Contains(col.gameObject))
        {
            this.cardsInSlider.Add(col.gameObject);
            Debug.Log(cardsInSlider.Count + "po dodaniu");
        }
        CheckCountInList();
        ResetLayout();
        }

    }


    void CheckCountInList()
    {
        if (cardsInSlider.Count == 0) mid = -2;
        else if (cardsInSlider.Count < 4)
        {
            mid = -1;
            prevMid = -1;
            nextMid = -1; 
        }
        else
        {
            mid = 1;
            prevMid = 0;
            nextMid = 2;
        }
    }
    void LoadCards(List<GameObject> karty)
    {

        foreach (GameObject x in karty)
        {
            if (cardsInSlider.Count > 3) x.GetComponent<CircleCollider2D>().enabled = false;
            cardsInSlider.Add(x);
        }
        CheckCountInList();
        ResetLayout();
        MoveLeft();
    }
        
   public void ResetLayout()
    {
        if (isCardNumberShown) cardNumberTM.text = "NR: " + (mid+1).ToString();

       // CheckCountInList();
        if (mid == -2) ; else
        if (mid == -1)
        {
            cardsInSlider[0].GetComponent<CircleCollider2D>().enabled = true;
            switch (cardsInSlider.Count)
            {
                case 0:
                    break;
                case 1:
                    cardsInSlider[0].transform.position = new Vector3(midTransform.position.x, midTransform.position.y, cardsInSlider[0].transform.position.z);
                    break;
                case 2:
                     cardsInSlider[0].transform.position = new Vector3(midTransform.position.x , midTransform.position.y, cardsInSlider[0].transform.position.z);
                     cardsInSlider[1].transform.position = new Vector3(midTransform.position.x + off, midTransform.position.y, cardsInSlider[1].transform.position.z);
     
                    break;
                case 3:
                     cardsInSlider[0].transform.position = new Vector3(midTransform.position.x , midTransform.position.y, cardsInSlider[0].transform.position.z);
                     cardsInSlider[1].transform.position = new Vector3(midTransform.position.x + off, midTransform.position.y, cardsInSlider[1].transform.position.z);
                     cardsInSlider[2].transform.position = new Vector3(midTransform.position.x - off, midTransform.position.y, cardsInSlider[2].transform.position.z);
                    break;
            }
        }
        else { 

        cardsInSlider[prevMid].GetComponent<CircleCollider2D>().enabled = false;
        cardsInSlider[nextMid].GetComponent<CircleCollider2D>().enabled = false;
        cardsInSlider[mid].GetComponent<CircleCollider2D>().enabled = true;

        cardsInSlider[prevMid].transform.position = new Vector3(midTransform.position.x - off, midTransform.position.y, cardsInSlider[prevMid].transform.position.z);
        cardsInSlider[mid].transform.position = new Vector3(midTransform.position.x, midTransform.position.y, cardsInSlider[mid].transform.position.z);
        cardsInSlider[nextMid].transform.position = new Vector3(midTransform.position.x + off, midTransform.position.y, cardsInSlider[nextMid].transform.position.z);
        }
       // Debug.Log(cardsInSlider[mid].name);
    }



   public void MoveRight()
    {
        
        if (mid != -1 && mid != -2)
        { 
        prevMid = mid;
        mid = nextMid;
        nextMid = (nextMid + 1) % cardsInSlider.Count;
        }
        ResetLayout();
        Debug.Log(cardsInSlider.Count + " prewMID " + prevMid);
    }

  public  void MoveLeft()
    {
        if (mid != -1 && mid != -2)
        {
            nextMid = mid;
            mid = prevMid;
            prevMid = prevMid - 1;
            if (prevMid < 0) prevMid = cardsInSlider.Count - 1;
        }
        ResetLayout();
    }

}
