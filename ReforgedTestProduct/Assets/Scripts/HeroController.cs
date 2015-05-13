using UnityEngine;
using System.Collections;

public class HeroController : Photon.MonoBehaviour 
{
	Vector3 lastClientClick;
	Vector3 serverCurrentClick;
	bool clicked = false;
	private Unit forgerInfo;
	private UnitMovement uMovement;
	private NavMeshAgent nav;
	private Vector3 movement;
	private Vector3 originalPos;
	private Unit target;
	private Vector3 posReceived = Vector3.zero;
	private Vector3 velReceived = Vector3.zero;
	private float rotReceived = 0f;
	private string serializeCheck = "";

	void Start ()
	{
		forgerInfo = GetComponent<Unit>();
		forgerInfo.Init( "Hero", "Hero", 500.0, 5, 3.4, 54, 2, 1.5f, 0.5f, 16, 2, 15, 2, 5, 3, 0.2f, 0.8f,0.2f,0.5f );
		nav = GetComponent<NavMeshAgent> ();
		uMovement = GetComponent<UnitMovement>();
	}
	
	void Awake ()
	{
		if (PhotonNetwork.isMasterClient) {
			originalPos = transform.position;
		}
		nav = GetComponent<NavMeshAgent> ();
		uMovement = GetComponent<UnitMovement>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(photonView.isMine)
		{
			if(Input.GetMouseButtonDown(1))
			{
				Debug.Log ("Clicked!");
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit; LayerMask mask = 1 << 8;
				Vector3 click = lastClientClick; int hitID = gameObject.GetPhotonView().viewID;

				if (Physics.Raycast (ray, out hit, 200, mask))
				{
					click = hit.point;
					hitID = ClickedCollider( click );
				}
				Debug.Log(hit.point.ToString() + ", " +  hit.collider.name);

				if( lastClientClick != click )
				{
					lastClientClick = click;
					uMovement.C2UMMovementInput(lastClientClick, hitID, "MOVE");
//					if(PhotonNetwork.isMasterClient)
//					{
//						SendMovementInput(click.x, click.y, click.z, hitID);
//					}
//					else if(PhotonNetwork.isNonMasterClientInRoom)
//					{
//						photonView.RPC("SendMovementInput", PhotonTargets.MasterClient, click.x, click.y, click.z, hitID);
//					}
				}
			}
		}

//		if (PhotonNetwork.isMasterClient) 
//		{
//			if (target == null) 
//			{
//				float distance = (serverCurrentClick - transform.position).magnitude;
//				if (serverCurrentClick != Vector3.zero && distance > 1) 
//				{
//					movement = serverCurrentClick;
//				}
//				else 
//				{
//					movement = transform.position;
//				}
//			} 
//			else 
//			{
//				transform.LookAt (target.transform.position);
//				float distance = (transform.position - target.transform.position).magnitude;
//				if (distance < forgerInfo.AttackRange) 
//				{
//					movement = transform.position;
//					clicked = false;
//					Attack();
//				} 
//				else 
//				{
//					movement = target.transform.position;
//				}
//			}
//		}
//
//		nav.destination = movement;
	}

	void Attack()
	{
		if(Time.time >= forgerInfo.NextAttackTime)
		{
			Debug.Log("Hero Attacks!");
			AttackAnimation();
			target.TakeDamage(forgerInfo.Attack);
			forgerInfo.NextAttackTime = Time.time + forgerInfo.AttackSpeed;
		}
	}
	
	void AttackAnimation()
	{
		
	}

	private int ClickedCollider( Vector3 hit )
	{
		Collider[] hitColliders = Physics.OverlapSphere(hit, 0.5f); 
		return FindClosestCollider(hitColliders, hit);
	}
	
	private int FindClosestCollider( Collider[] list, Vector3 pos )
	{
		float minDist = 500000f; int output = gameObject.GetPhotonView().viewID;
		foreach(Collider c in list)
		{
			float dist = Vector3.Distance(c.transform.position, pos);
			if(dist < minDist && c.tag != "Terrain" )
			{
				output = c.gameObject.GetPhotonView().viewID;
				minDist = dist;
			}
		}
		return output;
	}

//	[RPC]
//	void SendMovementInput (float x, float y, float z, int hitID)
//	{
//		if (hitID != gameObject.GetPhotonView().viewID) {
//			GameObject go = PhotonView.Find(hitID).gameObject;
//			if (go != null) {
//				target = go.GetComponent<Unit> ();
//				
//			}
//		} else {
//			target = null;
//		}
//		serverCurrentClick = new Vector3 (x, y, z);
//		clicked = true;
//		if(PhotonNetwork.isMasterClient)
//		{
//			photonView.RPC("ServerMovementReceive", PhotonTargets.OthersBuffered, x, y, z);
//		}
//	}
//
//	[RPC]
//	void ServerMovementReceive (float x, float y, float z)
//	{
//		movement = new Vector3( x, y, z );
//	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		SerializeState( stream, info );
	}

	public void SerializeState(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			serializeCheck = "I am Writing!";
			Vector3 pos = transform.position;
			float rot = transform.eulerAngles.y;
			Vector3 vel = movement;
			stream.SendNext(pos);
			stream.SendNext(rot);
			stream.SendNext(vel);
		} else {
			serializeCheck = "I am Reading!";
			transform.position = posReceived = (Vector3)stream.ReceiveNext();
			Vector3 rota = transform.eulerAngles;
			rota.y = rotReceived = (float)stream.ReceiveNext();
			transform.eulerAngles = rota;
			movement = velReceived = (Vector3)stream.ReceiveNext();
		}
	}

	void OnGUI ()
	{
		Vector2 backgroundBarSize = new Vector2 (Screen.width * 0.2f, Screen.height * 0.06f);
		Vector3 viewPos = Camera.main.WorldToScreenPoint (this.transform.position + new Vector3 (0, 1.5f, 0));
		float valueZ = viewPos.z;
		if (valueZ < 1) {
			valueZ = 1;
		} else if (valueZ > 4) {
			valueZ = 4;
		}
		float valueToNormalize = Mathf.Abs (1 / (valueZ - 0.5f));
		
		int backgroundBarWidth = (int)(backgroundBarSize.x * valueToNormalize);
		if (backgroundBarWidth % 2 != 0) {
			backgroundBarWidth++;
		}
		float backgroundBarHeight = (int)(backgroundBarSize.y * valueToNormalize);
		if (backgroundBarHeight % 2 != 0) {
			backgroundBarHeight++;
		}
		//float innerBarWidth = backgroundBarWidth - 2 * 2;
		//float innerBarHeight = backgroundBarHeight - 2 * 2;
		float posYHealthBar = Screen.height - viewPos.y - backgroundBarHeight;

		if(photonView.isMine)
		{
			//GUI.Label (new Rect (10, 20, 300, 30), "CurrentState: " + msControl.CurrentState.ToString());
			if(target != null)
				//GUI.Label (new Rect (10, 30, 300, 30), "Target: " + target.Name.ToString());
			GUI.Label (new Rect (10, 10, 300, 30), "Movement: " + movement.ToString());

			GUI.Label(new Rect (viewPos.x - 25, posYHealthBar-40, 200, 25), serializeCheck);
			GUI.Label (new Rect (viewPos.x - 50, posYHealthBar-30, 200, 25), "Stream Pos: " + posReceived.ToString());
			GUI.Label (new Rect (viewPos.x - 25, posYHealthBar-20, 200, 25), "Stream Rot: " + rotReceived);
			GUI.Label (new Rect (viewPos.x - 50, posYHealthBar-10, 200, 25), "Stream Vel: " + velReceived.ToString());
		}
		else
		{
			GUI.Label(new Rect (viewPos.x - 25, posYHealthBar-40, 200, 25), serializeCheck);
			GUI.Label (new Rect (viewPos.x - 50, posYHealthBar-30, 200, 25), "Stream Pos: " + posReceived.ToString());
			GUI.Label (new Rect (viewPos.x - 25, posYHealthBar-20, 200, 25), "Stream Rot: " + rotReceived);
			GUI.Label (new Rect (viewPos.x - 50, posYHealthBar-10, 200, 25), "Stream Vel: " + velReceived.ToString());
		}
	}
}
