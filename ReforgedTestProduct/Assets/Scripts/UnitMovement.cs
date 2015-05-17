using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Unit))]
[RequireComponent (typeof (NavMeshAgent))]
public class UnitMovement : Photon.MonoBehaviour 
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
	Vector3 lastClientClick;
	Vector3 serverCurrentClick;
	private Unit unitInfo;
	private SightController sightCon;
	private NavMeshAgent nav;
	private Vector3 movement;
	private Vector3 originalPos;
	private Unit target;
	public List<string> enemyTeams;
	public string team;

	public MovementState CurrentState
	{
		get{return currentState;}
		set{currentState = value;}
	}
	public Unit Target
	{
		get{return target;}
		set{target = value;}
	}
	public Vector3 Movement
	{
		get{return movement;}
		set{movement = value;}
	}
	public Vector3 OriginalPosition
	{
		get{return originalPos;}
		set{originalPos = value;}
	}

	// Use this for initialization
	void Start () 
	{
		unitInfo = GetComponent<Unit>();
		target = unitInfo;
		sightCon = GetComponentInChildren<SightController>();
		nav = GetComponent<NavMeshAgent> ();
		enemyTeams = new List<string> {"RedTeam", "BlueTeam", "Neutrals"};
		team = gameObject.tag;
		enemyTeams.Remove(team);
		movement = transform.position;
	}

	void Awake ()
	{
		unitInfo = GetComponent<Unit>();
		target = unitInfo;
		sightCon = GetComponentInChildren<SightController>();
		nav = GetComponent<NavMeshAgent> ();
		enemyTeams = new List<string> {"RedTeam", "BlueTeam", "Neutrals"};
		team = gameObject.tag;
		enemyTeams.Remove(team);
		movement = transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		if(!unitInfo.Dead)
		{
			if (PhotonNetwork.isMasterClient) 
			{
				switch (currentState)
				{
				case (MovementState.ATTACKMOVE): 
					if(target)
					{
						if(!target.Dead && target.tag != team)
							MoveToAttackTarget();
						else
							FindNewTarget();
					}
					else
					{
						if(FindNewTarget())
							MoveToAttackTarget();
						else
							MoveToPositon();
					}
					break;
				case (MovementState.MOVE):
					MoveToPositon();
					break;
				case (MovementState.MOVEATTACK): 
					if(target)
					{
						if(!target.Dead && target.tag != team)
							MoveToAttackTarget();
						else
							currentState = MovementState.STOPATTACK;
					}
					else
						currentState = MovementState.STOPATTACK;
					break;
				case (MovementState.STOPATTACK):
					if(hasEnemyTargets())
					{
						if(target)
						{
							if(!target.Dead && target.tag != team)
							{
								if(InRange())
								{
									movement = transform.position;
									BroadcastMessage("Attack");
								}
								else
									FindNewTarget();
							}
							else
								FindNewTarget();
						}
						else
							FindNewTarget();
					}
					movement = transform.position;
					break;
				case (MovementState.STOP):
					movement = transform.position;
					break;
				}
				photonView.RPC("ServerSendMovement", PhotonTargets.OthersBuffered, movement.x, movement.y, movement.z);
			}
			
			nav.destination = movement;
		}
	}

	public void U2UMMovementInput(Vector3 click, int hitID, MovementState state)
	{
		if(PhotonNetwork.isMasterClient)
		{
			SendMovementInput(click.x, click.y, click.z, hitID, state.ToString());
		}
		else if(PhotonNetwork.isNonMasterClientInRoom)
		{
			photonView.RPC("SendMovementInput", PhotonTargets.MasterClient, click.x, click.y, click.z, hitID, state.ToString());
		}
	}

	public void U2UMState(MovementState state)
	{
		if(PhotonNetwork.isMasterClient)
		{
			SendState(state.ToString());
		}
		else if(PhotonNetwork.isNonMasterClientInRoom)
		{
			photonView.RPC("SendState", PhotonTargets.MasterClient, state.ToString() );
		}
	}

	[RPC]
	void SendMovementInput (float x, float y, float z, int hitID, string state)
	{
		if(hitID == 0)
			target = null;
		else
			target = RetrieveTarget(hitID);
		currentState = RetrieveState(state);
		serverCurrentClick = new Vector3 (x, y, z);
		if(PhotonNetwork.isMasterClient)
		{
			photonView.RPC("ServerMovementReceive", PhotonTargets.OthersBuffered, x, y, z, hitID, state);
		}
	}
	
	[RPC]
	void ServerMovementReceive (float x, float y, float z, int hitID, string state)
	{
		if(hitID == 0)
			target = null;
		else
			target = RetrieveTarget(hitID);
		currentState = RetrieveState(state);
		movement = new Vector3( x, y, z );
	}

	[RPC]
	void ServerSendMovement (float x, float y, float z)
	{
		movement = new Vector3( x, y, z );
	}

	[RPC]
	void SendState(string state)
	{
		currentState = RetrieveState(state);
		if( PhotonNetwork.isMasterClient )
			photonView.RPC ("UpdateState", PhotonTargets.OthersBuffered, state);
	}
	
	[RPC]
	void SendTarget(int ID)
	{
		target = RetrieveTarget(ID);

		if( PhotonNetwork.isMasterClient )
			photonView.RPC ("UpdateTarget", PhotonTargets.OthersBuffered, ID);
	}

	public bool hasEnemyTargets( )
	{
		if(sightCon.EnemyTargets.Count > 0)
			return true;
		else
			return false;
	}

	void MoveToAttackTarget()
	{
		if (InRange()) 
		{
			movement = transform.position;
			BroadcastMessage("Attack");
		} 
		else
		{
			movement = target.transform.position;
		}
	}

	void MoveToPositon()
	{
		if (serverCurrentClick != Vector3.zero && !OnApproach()) 
		{
			movement = serverCurrentClick;
		}
		else 
		{
			movement = transform.position;
		}
	}
	
	public bool InRange( )
	{
		
		if( Vector3.Distance(transform.position, target.transform.position) <= unitInfo.AttackRange )
			return true;
		else
			return false;
	}

	public bool OnApproach ( )
	{
		if( Vector3.Distance(transform.position, serverCurrentClick) <= 1 )
			return true;
		else
			return false;
	}

	public bool FindNewTarget()
	{
		float minDis = 50000f;
		bool check = false;
		List<Unit> thingsToRemove = new List<Unit>();
		foreach(Unit t in sightCon.EnemyTargets)
		{
			if(t == null || t.Dead)
			{
				thingsToRemove.Add(t);
			}
			else
			{
				float distance = Vector3.Distance(transform.position, t.transform.position);
				if( distance < minDis )
				{
					target = t;
					check = true;
				}
			}
		}
		foreach(Unit t in thingsToRemove)
		{
			Debug.Log ("Removing " + t.UnitName);
			sightCon.EnemyTargets.Remove(t);
		}
		return check;
	}

	private Unit RetrieveTarget(int ID)
	{
		if (ID != gameObject.GetPhotonView().viewID) {
			GameObject go = PhotonView.Find(ID).gameObject;
			if (go != null) {
				return go.GetComponent<Unit>();
			}
			else
			{
				return null;
			}
		} 
		else {
			 return null;
		}
	}

	private MovementState RetrieveState(string state)
	{
		if(state == "ATTACKMOVE")
			return MovementState.ATTACKMOVE;
		else if(state == "MOVE")
			return MovementState.MOVE;
		else if(state == "MOVEATTACK")
			return MovementState.MOVEATTACK;
		else if(state == "STOPATTACK")
			return MovementState.STOPATTACK;
		else
			return MovementState.STOP;
	}

	public void SerializeState(PhotonStream stream, PhotonMessageInfo info)
	{
		if( stream.isWriting )
		{
			int targetCheck;
			if(target == null)
				targetCheck = 0;
			else
				targetCheck = target.transform.GetComponent<PhotonView>().viewID;
			stream.SendNext(targetCheck);
			stream.SendNext(movement);
			stream.SendNext(currentState.ToString());
		}
		else
		{
			int targetCheck = (int)stream.ReceiveNext();
			if(targetCheck != 0)
				target = RetrieveTarget(targetCheck);
			else
				target = null;
			movement = (Vector3)stream.ReceiveNext();
			currentState = RetrieveState((string)stream.ReceiveNext());
			
		}
	}
}
