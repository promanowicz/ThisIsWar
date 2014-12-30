using UnityEngine;
using System.Collections;

public class SliderLeftScript : MonoBehaviour {

    public GameObject slider;
    SliderScript skrypt;
	// Use this for initialization
	void Start () {
        skrypt = slider.GetComponent<SliderScript>();
	}

    void OnMouseDown()
    {
        if(this.tag=="MoveLeft")
        skrypt.MoveLeft();
        if (this.tag == "MoveRight")
            skrypt.MoveRight();
    }
}
