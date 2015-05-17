using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitStationary : Photon.MonoBehaviour 
{
	public enum AttackState 
	{
		ATTACKGROUP,
		ATTACKFIRST,
		ATTACKDAMAGER
	}
	[SerializeField]
	private AttackState currentState;
	private Unit unitInfo;
	private SightControllerStationary sightCon;
	private Vector3 originalPos;
	private Quaternion pivot;
	private Unit target;
	public List<string> enemyTeams;
	public string team;
	
	public AttackState CurrentState
	{
		get{return currentState;}
		set{currentState = value;}
	}
	public Unit Target
	{
		get{return target;}
		set{target = value;}
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
		sightCon = GetComponentInChildren<SightControllerStationary>();
		pivot = transform.rotation;
		enemyTeams = new List<string> {"RedTeam", "BlueTeam", "Neutrals"};
		team = gameObject.tag;
		enemyTeams.Remove(team);
	}
	
	void Awake ()
	{
		unitInfo = GetComponent<Unit>();
		target = unitInfo;
		sightCon = GetComponentInChildren<SightControllerStationary>();
		pivot = transform.rotation;
		enemyTeams = new List<string> {"RedTeam", "BlueTeam", "Neutrals"};
		team = gameObject.tag;
		enemyTeams.Remove(team);
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
				case (AttackState.ATTACKFIRST): 
					if(hasEnemyTargets())
					{
						if(target)
						{
							if(!target.Dead && target.tag != team)
							{
								if(InRange())
								{
									RotateTowards(target.transform.position);
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
					break;
				case (AttackState.ATTACKGROUP):
					//Placeholder
					break;
				case (AttackState.ATTACKDAMAGER): 
					//Placeholder
					break;
				}
				photonView.RPC("ServerSendRotation", PhotonTargets.OthersBuffered, pivot.x, pivot.y, pivot.z, pivot.w);
			}
			
			transform.rotation = Quaternion.Slerp(transform.rotation, pivot, Time.deltaTime);
		}
	}
	
	public void U2UMState(AttackState state)
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
	void ServerSendRotation (float x, float y, float z, float w)
	{
		pivot = new Quaternion( x, y, z, w );
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
	
	Quaternion RotateTowards( Vector3 rot )
	{
		Vector3 dir = rot - transform.position;
		dir.y = 0;
		return Quaternion.LookRotation(dir);
	}
	
	public bool InRange( )
	{
		
		if( Vector3.Distance(transform.position, target.transform.position) <= unitInfo.AttackRange )
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
			sightCon.EnemyTargets.Remove(t);
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
	
	private AttackState RetrieveState(string state)
	{
		if(state == "ATTACKDAMAGER")
			return AttackState.ATTACKDAMAGER;
		else if(state == "ATTACKGROUP")
			return AttackState.ATTACKGROUP;
		else
			return AttackState.ATTACKFIRST;
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
			stream.SendNext(pivot.x);
			stream.SendNext(pivot.y);
			stream.SendNext(pivot.z);
			stream.SendNext(pivot.w);
			stream.SendNext(currentState.ToString());
		}
		else
		{
			int targetCheck = (int)stream.ReceiveNext();
			if(targetCheck != 0)
				target = RetrieveTarget(targetCheck);
			else
				target = null;
			float x = (float)stream.ReceiveNext();
			float y = (float)stream.ReceiveNext();
			float z = (float)stream.ReceiveNext();
			float w = (float)stream.ReceiveNext();
			pivot = new Quaternion(x,y,z,w);
			currentState = RetrieveState((string)stream.ReceiveNext());
			
		}
	}
}
