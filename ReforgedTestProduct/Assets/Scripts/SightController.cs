using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SightController : MonoBehaviour 
{
	private UnitMovement uMovement;
	private List<string> enemyTeams;
	private string team;
	private List<Unit> enemyTargets;


	public List<Unit> EnemyTargets
	{
		get{return enemyTargets;}
		set{enemyTargets = value;}
	}
	// Use this for initialization
	void Start () 
	{
		uMovement = gameObject.GetComponentInParent<UnitMovement>();
		enemyTeams = uMovement.enemyTeams;
		team = uMovement.team;
		enemyTeams.Remove(team);
		enemyTargets = new List<Unit>();
	}

	void Awake () 
	{
		uMovement = gameObject.GetComponentInParent<UnitMovement>();
		enemyTeams = uMovement.enemyTeams;
		team = uMovement.team;
		enemyTeams.Remove(team);
		enemyTargets = new List<Unit>();
	}

	void OnTriggerEnter (Collider other)
	{
		if( PhotonNetwork.isMasterClient )
		{
			if(enemyTeams.Contains(other.gameObject.tag))
			{
				Unit enemy = other.gameObject.GetComponent<Unit>();
				if(!enemyTargets.Contains(enemy))
				{
					enemyTargets.Add(enemy);
				}
			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if(PhotonNetwork.isMasterClient)
		{
			Unit enemy = other.gameObject.GetComponent<Unit>();
			if(enemyTargets.Contains(enemy))
			{
				Debug.Log("Something's went Right here!");
				enemyTargets.Remove(enemy);
			}
			if(enemy == uMovement.Target)
			{
				Debug.Log("Change enemies!");
				uMovement.Target = null;
				uMovement.FindNewTarget();
			}
		}
	}
}
