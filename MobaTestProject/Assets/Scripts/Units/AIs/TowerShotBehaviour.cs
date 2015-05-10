using UnityEngine;
using System.Collections;

public class TowerShotBehaviour : MonoBehaviour {

	[SerializeField]
	private Transform target;
	[SerializeField]
	private float mySpeed = 10;

	public Transform Target
	{
		get{return target;}
		set{target = value;}
	}

	// Update is called once per frame
	void Update () 
	{
		Vector3 dir = Target.position - gameObject.transform.position;
		gameObject.transform.rotation = Quaternion.LookRotation(dir);
		transform.Translate(Vector3.forward * Time.deltaTime * mySpeed);
		if(Vector3.Distance(Target.position, transform.position) <= 0.2)
			TargetHit();
	}

	void TargetHit()
	{
		Target.GetComponent<UnitInfo>().Info.TakeDamage(100.0);

		Destroy(gameObject);

	}
}
