using UnityEngine;
using System.Collections;

public class ClickingMarkerBehaviour : MonoBehaviour {

	private Color myColor;

	public Color color
	{
		get{return myColor;}
		set{myColor = value;}
	}

	public Vector3 Position
	{
		get{return transform.position;}
		set{transform.position = value;}
	}

	void Start()
	{
	}

	// Update is called once per frame
	void Update () 
	{
		Play();
	}

	private void Play()
	{

	}

}
