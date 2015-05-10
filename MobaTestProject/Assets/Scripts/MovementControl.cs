using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (UnitInfo))]
[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (SphereCollider))]
public class MovementControl : Photon.MonoBehaviour 
{
	public enum MovementState 
	{
		ATTACKMOVE,
		MOVE,
		MOVEATTACK,
		STOPATTACK,
		STOP
	}
	[SerializeField]
	private MovementState currentState;
	private SphereCollider attackRadius;
	private NavMeshAgent nav;
	private Vector3 target;
	private Transform attackTarget;
	private BaseUnit unitInfo;
	private List<string> enemyTeams;
	private string team;
	private List<Transform> enemyTargets;
	private int minStop = 1;
	private float syncTime = 0f;
	private float syncDelay = 0f;
	private float lastSyncTime = 0f;
	private Vector3 syncEndPosition;
	private Vector3 syncStartPosition;

	public MovementState CurrentState
	{
		get{return currentState;}
		set{currentState = value;}
	}

	public Vector3 Target
	{
		get{return target;}
		set{target = value;}
	}

	public Transform AttackTarget
	{
		get{return attackTarget;}
		set{attackTarget = value;}
	}

	public Quaternion Rotation
	{
		get{return transform.rotation;}
		set{transform.rotation = value;}
	}

	public Vector3 Position
	{
		get{return transform.position;}
		set{transform.position = value;}
	}

	// Use this for initialization
	void Start () 
	{
		enemyTargets = new List<Transform>();
		nav = GetComponent<NavMeshAgent>();
		attackRadius = gameObject.GetComponent<SphereCollider>();
		attackRadius.isTrigger = true;
		enemyTeams = new List<string> {"RedTeam", "BlueTeam", "Neutrals"};
		team = gameObject.tag;
		enemyTeams.Remove(team);
		syncStartPosition = syncEndPosition = transform.position;
	}

	void Awake () 
	{
		enemyTargets = new List<Transform>();
		nav = GetComponent<NavMeshAgent>();
		attackRadius = gameObject.GetComponent<SphereCollider>();
		attackRadius.isTrigger = true;
		enemyTeams = new List<string> {"RedTeam", "BlueTeam", "Neutrals"};
		team = gameObject.tag;
		enemyTeams.Remove(team);
		syncStartPosition = syncEndPosition = transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		if(photonView.isMine)
		{
			switch (currentState)
			{
			case (MovementState.ATTACKMOVE): 
				if(hasEnemyTargets())
				{
					if(attackTarget)
					{
						if(InRange())
						{
							nav.Stop();
							BroadcastMessage("Attack");
						}
						else
							MoveToAttackTarget();
					}
					else
						FindNewTarget();
				}
				else
					MoveToTarget();
				break;
			case (MovementState.MOVE):
				MoveToTarget();
				break;
			case (MovementState.MOVEATTACK): 

				if(attackTarget)
				{
					if(InRange())
					{
						nav.Stop();
						BroadcastMessage("Attack");
					}
					else
						MoveToAttackTarget();
				}
				else
					UpdateStateInfo(MovementState.STOPATTACK);
				break;
			case (MovementState.STOPATTACK):
				nav.Stop();
				if(hasEnemyTargets())
				{
					if(attackTarget)
					{
						if(InRange())
						{
							nav.Stop();
							BroadcastMessage("Attack");
						}
					}
					else
						FindNewTarget();
				}
				break;
			case (MovementState.STOP):
				nav.Stop();
				break;
			}
		}
		else
		{
			SyncedMovement();
		}
	}

	public void UpdateStatistics( )
	{
		unitInfo = GetComponent<UnitInfo>().Info;

		enemyTargets = new List<Transform>();
		nav = GetComponent<NavMeshAgent>();
		nav.speed = unitInfo.MovementSpeed;
		nav.stoppingDistance = minStop;
		attackRadius = gameObject.GetComponent<SphereCollider>();
		attackRadius.isTrigger = true;
		attackRadius.radius = unitInfo.AttackRange;
		enemyTeams = new List<string> {"RedTeam", "BlueTeam", "Neutrals"};
		team = gameObject.tag;
		enemyTeams.Remove(team);
	}

	public void UpdateMovementInformation(MovementState state, Transform at, Vector3 pos)
	{
		UpdateState(state.ToString());
		UpdateAttackTarget(at.GetComponent<PhotonView>().viewID);
		UpdateTarget(pos);
	}

	public void UpdateATInfo(Transform at)
	{
		UpdateAttackTarget(at.GetComponent<PhotonView>().viewID);
	}
	public void UpdateTargetInfo(Vector3 pos)
	{
		UpdateTarget(pos);
	}
	public void UpdateStateInfo(MovementState state)
	{
		UpdateState(state.ToString());
	}

	[RPC]
	void UpdateState(string state)
	{
		if(state == "ATTACKMOVE")
			currentState = MovementState.ATTACKMOVE;
		else if(state == "MOVE")
			currentState = MovementState.MOVE;
		else if(state == "MOVEATTACK")
			currentState = MovementState.MOVEATTACK;
		else if(state == "STOPATTACK")
			currentState = MovementState.STOPATTACK;
		else
			currentState = MovementState.STOP;
		if( GetComponent<HeroBehavior>() != null )
			if( photonView.isMine )
				photonView.RPC ("UpdateState", PhotonTargets.OthersBuffered, state);
		else
			if( PhotonNetwork.isMasterClient )
				photonView.RPC ("UpdateState", PhotonTargets.OthersBuffered, state);
	}

	[RPC]
	void UpdateAttackTarget(int ID)
	{
		attackTarget = PhotonView.Find(ID).transform;
		if( unitInfo.Type == "Hero" )
			if( photonView.isMine )
				photonView.RPC ("UpdateAttackTarget", PhotonTargets.OthersBuffered, ID);
		else
			if( PhotonNetwork.isMasterClient )
				photonView.RPC ("UpdateAttackTarget", PhotonTargets.OthersBuffered, ID);
	}

	[RPC]
	void UpdateTarget(Vector3 pos)
	{
		target = pos;
		if( unitInfo.Type == "Hero" )
			if( photonView.isMine )
				photonView.RPC ("UpdateTarget", PhotonTargets.OthersBuffered, pos);
		else
			if( PhotonNetwork.isMasterClient )
				photonView.RPC ("UpdateTarget", PhotonTargets.OthersBuffered, pos);
	}

	private void SyncedMovement()
	{
		if(currentState == MovementState.ATTACKMOVE || currentState == MovementState.MOVEATTACK)
			nav.destination = attackTarget.position;
		else
			nav.destination = syncEndPosition;
	}

	void MoveToTarget()
	{
		nav.destination = target;
	}

	void MoveToAttackTarget()
	{
		nav.destination = attackTarget.position;
	}

	public bool hasEnemyTargets( )
	{
		if(enemyTargets.Count > 0)
			return true;
		else
			return false;
	}

	public bool InRange( )
	{

		if( Vector3.Distance(transform.position, attackTarget.position) <= unitInfo.AttackRange )
			return true;
		else
			return false;
	}

	public bool OnApproach ( )
	{
		if( Vector3.Distance(transform.position, target) <= minStop )
			return true;
		else
			return false;
	}

	void FindNewTarget()
	{
		if(PhotonNetwork.isMasterClient)
		{
			float minDis = 50000f;
			bool check = false;
			List<Transform> thingsToRemove = new List<Transform>();
			foreach(Transform t in enemyTargets)
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
						attackTarget = t;
						check = true;
					}
				}
			}
			foreach(Transform t in thingsToRemove)
				enemyTargets.Remove(t);
			UpdateAttackTarget(attackTarget.GetComponent<PhotonView>().viewID);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if(enemyTeams.Contains(other.gameObject.tag))
		{
			if(!enemyTargets.Contains(other.gameObject.transform))
			{
				enemyTargets.Add(other.gameObject.transform);
			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if(enemyTargets.Contains(other.gameObject.transform))
		{
			Debug.Log("Something's went Right here!");
			enemyTargets.Remove(other.gameObject.transform);
		}
		if(other.gameObject.transform == attackTarget)
		{
			Debug.Log("Change enemies!");
			attackTarget = null;
			FindNewTarget();
		}
	}

	public void SerializeState(PhotonStream stream, PhotonMessageInfo info)
	{
		if( stream.isWriting )
		{
			stream.SendNext(transform.position);
			stream.SendNext(target);
		}
		else
		{
			syncTime = 0f;
			syncDelay = Time.time - lastSyncTime;
			lastSyncTime = Time.time;

			syncEndPosition = (Vector3)stream.ReceiveNext();
			syncStartPosition = transform.position;
			target = (Vector3)stream.ReceiveNext();

		}
	}
}
