using UnityEngine;
using System.Collections;

public class SliderLeftScript : MonoBehaviour {

    public GameObject slider;

    SliderScript skrypt;
    ArmiesSlider skrypt2;
	// Use this for initialization
	void Start () {
        skrypt = slider.GetComponent<SliderScript>();
        if(skrypt==null)skrypt2 = slider.GetComponent<ArmiesSlider>();
	}

    void OnMouseDown()
    {
        if (skrypt != null)
        {
            
            if (this.tag == "MoveLeft")
                skrypt.MoveLeft();
            if (this.tag == "MoveRight")
                skrypt.MoveRight();
        }
        else if (skrypt2 != null)
        {
            
            if (this.tag == "MoveLeft")
                skrypt2.MoveLeft();
            if (this.tag == "MoveRight")
                skrypt2.MoveRight();
        }
    }
}
