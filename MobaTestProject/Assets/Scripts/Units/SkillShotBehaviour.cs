using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillShotBehaviour : MonoBehaviour {

	private float missleSpeed;
	private Transform target;
	private double missleDamage;
	private float missleLifetime;
	private string flags;
	private List<string> targetables;

	// Update is called once per frame
	void Update () 
	{
		if(Time.time >= missleLifetime)
		{
			Destroy(gameObject);
		}
		transform.Translate(Vector3.forward * Time.deltaTime * missleSpeed);
	}
	public void StatsSet (float speed, Transform t, double damage, float lifetime, string f, List<string> targetTypes /*Effects[] effects*/)
	{
		missleSpeed = speed; target = t; missleDamage = damage; flags = f; targetables = targetTypes; missleLifetime = Time.time + lifetime;
	}

	void OnTriggerEnter( Collider collision )
	{
		if(targetables.Contains(collision.tag))
		{
			Debug.Log (collision.name);
			TargetHit(collision.gameObject);
		}
	}

	private void TargetHit( GameObject go )
	{
		go.GetComponent<UnitInfo>().Info.TakeDamage(missleDamage);
		Destroy(gameObject);

		//applyEffects
	}
}
