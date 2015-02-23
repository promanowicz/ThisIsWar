using UnityEngine;
using System.Collections;

public class CardDragAndDrop : MonoBehaviour {

   public bool clicked = false;

    // Update is called once per frame
    void Update()
    {
        if (clicked)
        {
			Camera currentCam;
            //Vector3 x = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(Application.loadedLevelName == "map")
				currentCam = GameObject.Find ("ArmiesSetCamera").GetComponent<Camera>();
			else
				currentCam = Camera.main;
			Vector3 x = currentCam.ScreenToWorldPoint(Input.mousePosition);
            x.z = transform.position.z;
            transform.position = x;
        }
    }

    void OnMouseDown()
    {
       // Debug.Log(this.name + " " + clicked);
        clicked = true;

    }

    void OnMouseUp()
    {
        //Debug.Log(this.name + " " + clicked);
        clicked = false;
    }
}
