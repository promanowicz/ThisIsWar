using UnityEngine;
using System.Collections;

public class CardDragAndDrop : MonoBehaviour {

   public bool clicked = false;

    // Update is called once per frame
    void Update()
    {
        if (clicked)
        {
            Vector3 x = Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
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
