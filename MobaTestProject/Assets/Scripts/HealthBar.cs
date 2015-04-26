using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour 
{
	private Transform healthBar;

	void Start ()
	{
		healthBar = gameObject.transform;
	}
	
	// Health between [0.0f,1.0f] == (currentHealth / totalHealth)
	public void SetHealthVisual(float healthNormalized){
		healthBar.transform.localScale = new Vector3( healthNormalized,
		                                             healthBar.transform.localScale.y,
		                                             healthBar.transform.localScale.z);
	}
}
