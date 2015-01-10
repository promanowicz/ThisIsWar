using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public Transform map;

	private bool isPerspective = false;
	private bool isMoving = false;
	private Vector3 startPosition;

	// Use this for initialization
	void Start () 
	{
		startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown (KeyCode.LeftShift))
		{
			isMoving = true;
			if(!isPerspective)
				Camera.main.orthographic = false;
		}

		if(isMoving)
			Move ();
		else
		{
			//RUCH KAMERY
			transform.position += new Vector3 (Input.GetAxis ("Mouse X") * 3  * Time.deltaTime, Input.GetAxis ("Mouse Y") * 3 * Time.deltaTime, 0);

			//OGRANICZENIA
			if(transform.position.x<-6)
				transform.position = new Vector3(-6, transform.position.y,  transform.position.z);
			if(transform.position.x>8.5f)
				transform.position = new Vector3(8.5f, transform.position.y,  transform.position.z);

			if(isPerspective)
			{
				if(transform.position.y<-10)
					transform.position = new Vector3(transform.position.x, -10,  transform.position.z);
				if(transform.position.y>-2)
					transform.position = new Vector3(transform.position.x, -2,  transform.position.z);
			}
			else
			{
				if(transform.position.y<-5.5f)
					transform.position = new Vector3(transform.position.x, -5.5f,  transform.position.z);
				if(transform.position.y>1.5f)
					transform.position = new Vector3(transform.position.x, 1.5f,  transform.position.z);
			}
		}
	}

	void Move ()
	{
		transform.LookAt (map.position);

		if(!isPerspective)
		{
			transform.position = Vector3.Lerp (transform.position, new Vector3(0, -5, -10), 0.05f);
			if(Vector3.Distance (transform.position, new Vector3(0, -5, -10))<0.1)
			{
				isMoving = false;
				isPerspective = true;
				transform.position = new Vector3(0, -5, -10);
			}
		}
		else
		{
			transform.position = Vector3.Lerp (transform.position, startPosition, 0.05f);
			if(Vector3.Distance (transform.position, startPosition)<0.1)
			{
				isMoving = false;
				isPerspective = false;
				Camera.main.orthographic = true;
				transform.position = startPosition;
			}
		}
	}
}
