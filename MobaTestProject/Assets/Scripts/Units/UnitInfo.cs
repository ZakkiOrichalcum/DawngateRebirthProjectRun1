using UnityEngine;
using System.Collections;

public class UnitInfo : MonoBehaviour {

	[SerializeField]
	private BaseUnit info;

	public BaseUnit Info
	{
		get{return info;}
		set{info = value;}
	}
}
