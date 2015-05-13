using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Unit))]
[RequireComponent (typeof (NavMeshAgent))]
//[RequireComponent (typeof (SphereCollider))]
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
	bool clicked = false;
	private Unit forgerInfo;
	private NavMeshAgent nav;
	private Vector3 movement;
	private Vector3 originalPos;
	private Unit target;

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

	// Use this for initialization
	void Start () 
	{
		forgerInfo = GetComponent<Unit>();
		nav = GetComponent<NavMeshAgent> ();
	}

	void Awake ()
	{
		if (PhotonNetwork.isMasterClient) {
			originalPos = transform.position;
		}
		nav = GetComponent<NavMeshAgent> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (PhotonNetwork.isMasterClient) 
		{
			switch (currentState)
			{
			case (MovementState.ATTACKMOVE): 

				break;
			case (MovementState.MOVE):

				break;
			case (MovementState.MOVEATTACK): 

				break;
			case (MovementState.STOPATTACK):

				break;
			case (MovementState.STOP):
				movement = transform.position;
				break;
			}
			if (target == null) 
			{
				float distance = (serverCurrentClick - transform.position).magnitude;
				if (serverCurrentClick != Vector3.zero && distance > 1) 
				{
					movement = serverCurrentClick;
				}
				else 
				{
					movement = transform.position;
				}
			} 
			else 
			{
				transform.LookAt (target.transform.position);
				float distance = (transform.position - target.transform.position).magnitude;
				if (distance < forgerInfo.AttackRange) 
				{
					movement = transform.position;
					clicked = false;
					BroadcastMessage("Attack");
				} 
				else 
				{
					movement = target.transform.position;
				}
			}
		}
		
		nav.destination = movement;
	}

	public void C2UMMovementInput(Vector3 click, int hitID, string state)
	{
		if(PhotonNetwork.isMasterClient)
		{
			SendMovementInput(click.x, click.y, click.z, hitID, state);
		}
		else if(PhotonNetwork.isNonMasterClientInRoom)
		{
			photonView.RPC("SendMovementInput", PhotonTargets.MasterClient, click.x, click.y, click.z, hitID, state );
		}
	}

	[RPC]
	void SendMovementInput (float x, float y, float z, int hitID, string state)
	{
		target = RetrieveViewObject(hitID);
		currentState = RetrieveState(state);
		serverCurrentClick = new Vector3 (x, y, z);
		clicked = true;
		if(PhotonNetwork.isMasterClient)
		{
			photonView.RPC("ServerMovementReceive", PhotonTargets.OthersBuffered, x, y, z, hitID, state);
		}
	}
	
	[RPC]
	void ServerMovementReceive (float x, float y, float z, int hitID, string state)
	{
		target = RetrieveViewObject(hitID);
		currentState = RetrieveState(state);
		movement = new Vector3( x, y, z );
	}
	
	private Unit RetrieveViewObject(int ID)
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
}
