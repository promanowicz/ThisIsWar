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
		transform.LookAt (map.position);

		if(Input.GetKeyDown (KeyCode.LeftShift))
		{
			isMoving = true;
			if(!isPerspective)
				Camera.main.orthographic = false;
		}

		if(isMoving)
			Move ();
	}

	void Move ()
	{
		if(!isPerspective)
		{
			transform.position = Vector3.Lerp (transform.position, new Vector3(0, -10, -20), 0.05f);
			if(Vector3.Distance (transform.position, new Vector3(0, -10, -20))<0.1)
			{
				isMoving = false;
				isPerspective = true;
				transform.position = new Vector3(0, -10, -20);
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
