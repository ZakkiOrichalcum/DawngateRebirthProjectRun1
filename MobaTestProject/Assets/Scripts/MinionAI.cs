using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionAI : MonoBehaviour {

	[SerializeField]
	private Vector3[] waypoints;
	[SerializeField]
	private string team;
	[SerializeField]
	private List<string> enemyTeams;
	private BaseUnit minionInfo;
	private NavMeshAgent nav;
	private Transform currentTarget;
	private BaseUnit targetInfo;
	private int currentWaypoint;
	private bool targetInSight;
	[SerializeField]
	private List<Transform> enemies;

	public Vector3[] Waypoints
	{
		get{return waypoints;}
		set{waypoints = value;}
	}


	// Use this for initialization
	void Start () 
	{
		nav = GetComponent<NavMeshAgent>();
		GetComponent<UnitInfo>().Info = new BaseUnit(gameObject, "Minion", "Minion", 250.0, 50,0.75f, 20, 0, 2, 2, true);
		minionInfo = GetComponent<UnitInfo>().Info;
		currentWaypoint = 0;
		team = gameObject.tag;
		enemyTeams.Remove(team);
		nav.speed = minionInfo.MovementSpeed;
		nav.stoppingDistance = minionInfo.AttackRange;
		minionInfo.NextAttackTime = Time.time + minionInfo.AttackSpeed;

		if( team == "RedTeam" )
		{
			waypoints = new Vector3[] {new Vector3(-20,0,0), new Vector3(-10,0,0), new Vector3(20,0,0), new Vector3(30,0,0)};
		}
		else
		{
			waypoints = new Vector3[] {new Vector3(20,0,0), new Vector3(10,0,0), new Vector3(-20,0,0), new Vector3(-30,0,0)};
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(currentTarget)
		{
			Debug.DrawRay(transform.position, currentTarget.position - transform.position);
			if(inRange())
			{
				Attack();
			}
			else
			{
				MoveToTarget();
			}
		}
		else if( FindNewTarget() )
		{
			MoveToTarget();
		}
		else
		{
			targetInfo = null;
			MoveDownLane();
		}

		if(minionInfo.Dead)
		{
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if(enemyTeams.Contains(other.gameObject.tag))
		{
			if(!enemies.Contains(other.gameObject.transform))
			{
				enemies.Add(other.gameObject.transform);
			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if(enemies.Contains(other.gameObject.transform))
		{
			Debug.Log("Something's went Right here!");
			enemies.Remove(other.gameObject.transform);
		}
		if(other.gameObject.transform == currentTarget)
		{
			Debug.Log("Change enemies!");
			currentTarget = null;
			FindNewTarget();
		}
	}

	bool FindNewTarget()
	{
		float minDis = 50000f;
		bool check = false;
		List<Transform> thingsToRemove = new List<Transform>();
		foreach(Transform t in enemies)
		{
			if(t == null)
			{
				thingsToRemove.Add(t);
			}
			else
			{
				float distance = Vector3.Distance(transform.position, t.position);
				if( distance < minDis )
				{
					currentTarget = t;
					check = true;
				}
			}
		}
		if(check)
		{
			targetInfo = currentTarget.GetComponent<UnitInfo>().Info;
		}
		foreach(Transform t in thingsToRemove)
			enemies.Remove(t);
		return check;
	}

	bool inRange()
	{
		if( Vector3.Distance(transform.position, currentTarget.position) <= minionInfo.AttackRange)
			return true;
		else
			return false;
	}

	void Attack()
	{
		if(Time.time >= minionInfo.NextAttackTime)
		{
			targetInfo = currentTarget.GetComponent<UnitInfo>().Info;
			AttackAnimation();
			targetInfo.TakeDamage(minionInfo.BaseAttack);
			minionInfo.NextAttackTime = Time.time + minionInfo.AttackSpeed;
		}
	}

	void AttackAnimation()
	{
	}

	void MoveToTarget()
	{
		nav.destination = currentTarget.transform.position;
	}

	void MoveDownLane()
	{
		nav.destination = waypoints[currentWaypoint];

		if(Vector3.Distance(transform.position, waypoints[currentWaypoint]) <= 3)
			currentWaypoint++;
	}
}
