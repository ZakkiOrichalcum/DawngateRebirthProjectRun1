using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerBehaviour : MonoBehaviour {

	//Private Variables
	[SerializeField]
	private GameObject myProjectile;
	[SerializeField]
	private Transform currentTarget;
	[SerializeField]
	private Transform towerTransform;
	[SerializeField]
	private Transform towerTop;
	[SerializeField]
	private Transform firePosition;
	[SerializeField]
	private List<string> enemyTeams;
	private float nextFireTime;
	private float nextMoveTime;
	private string team;
	private Quaternion desiredRotation;
	private List<Transform> targets = new List<Transform>();
	private BaseUnit towerInfo;
	
	//Public Variables
	public float reloadTime = 1f;
	public float turnSpeed = 5f;
	public float firePauseTime = .25f;
	
	// Use this for initialization
	void Start () 
	{
		nextFireTime = reloadTime;
		towerTransform = gameObject.transform;
		towerTop = towerTransform.FindChild("Tower_Base").transform.FindChild("Tower_Top");
		firePosition = towerTop.FindChild("Fire_Position");
		team = gameObject.tag;
		enemyTeams.Remove(team);
		GetComponent<UnitInfo>().Info = new BaseUnit(gameObject, "Tower", "Tower", 1000.0, 25, 1f, 30, 0, 5, 0, true);
		towerInfo = GetComponent<UnitInfo>().Info;
		Debug.Log(towerInfo.Name +" | current team: " + team +", enemy teams: " + enemyTeams[0]);
	}
	
	void Update () 
	{
		if(currentTarget) 
		{
			Vector3 dir = currentTarget.position - towerTop.position;
			dir.y = 0;
			Quaternion rot = Quaternion.LookRotation(dir);
			towerTop.rotation = Quaternion.Slerp(towerTop.rotation, rot, turnSpeed);

			if(Time.time >= nextFireTime)
			{
				FireProjectile();
			}
		}
		else
		{
			FindNewTarget();
		}

		if(towerInfo.Dead)
		{
			Destroy (gameObject);
		}
	}
	
	void OnTriggerEnter (Collider other)
	{
		if(enemyTeams.Contains(other.gameObject.tag))
		{
			if(!targets.Contains(other.gameObject.transform))
			{
				targets.Add(other.gameObject.transform);
			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if(targets.Contains(other.gameObject.transform))
		{
			Debug.Log("Something's went Right here!");
			targets.Remove(other.gameObject.transform);
		}
		if(other.gameObject.transform == currentTarget)
		{
			Debug.Log("Change Targets!");
			currentTarget = null;
			FindNewTarget();
			nextFireTime = Time.time + (reloadTime * 1.25f);
		}
	}
	
	void FireProjectile()
	{
		nextFireTime = Time.time + reloadTime;

		GameObject projectile = Instantiate(myProjectile, firePosition.position, firePosition.rotation) as GameObject;
		TowerShotBehaviour towershot = projectile.GetComponent<TowerShotBehaviour>();
		towershot.Target = currentTarget;
	}
	
	bool FindNewTarget()
	{
		float minDis = 50000f;
		bool check = false;
		List<Transform> thingsToRemove = new List<Transform>();
		foreach(Transform t in targets)
		{
			if(t == null)
			{
				thingsToRemove.Add(t);
			}
			else{
			float distance = Vector3.Distance(transform.position, t.position);
				if( distance < minDis )
				{
					currentTarget = t;
					check = true;
				}
			}
		}
		foreach(Transform t in thingsToRemove)
			targets.Remove(t);
		if(check)
			nextFireTime = Time.time + reloadTime;
		return check;
	}
}
