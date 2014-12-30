using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Army : MonoBehaviour {

    public List<GameObject> cardList=new List<GameObject>(); 
    public Player owner{ get; set; }
    public Tile dislocation{ get; set; }
}
