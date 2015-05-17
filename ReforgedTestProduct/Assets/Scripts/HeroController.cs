using UnityEngine;
using System.Collections;

public class HeroController : Photon.MonoBehaviour 
{
	Vector3 lastClientClick;
	Vector3 serverCurrentClick;
	private Unit forgerInfo;
	private UnitMovement uMovement;
	private Vector3 originalPos;
	private Vector3 posReceived = Vector3.zero;
	private float rotReceived = 0f;
	private string serializeCheck = "";

	//key Codes
//	private string attackMove = "a";
//	private string stop = "s";
//	private string ability1 = "q";
//	private string ability2 = "w";
//	private string ability3 = "e";
//	private string ability4 = "r";
	private string lockOnToggle = "y";

	//Camera Controls
	[SerializeField]
	private Transform playerCamera;
	private bool LockedOn = false;
	private bool PermLockOn = true;
	private float cameraSpeed = 0.5f;
	private Rect cameraDown;
	private Rect cameraUp;
	private Rect cameraLeft;
	private Rect cameraRight;
	private float guiSize = 100f;

	void Start ()
	{
		forgerInfo = GetComponent<Unit>();
		forgerInfo.Init( "Hero", "Hero", tag, 1, 500.0, 5, 3.4, 54, 2, 1.5f, 0.5f, 16, 2, 15, 2, 200, 3, 0.2f, 0.8f,0.2f,0.5f, 150, 0, 300, 0 );
		uMovement = GetComponent<UnitMovement>();

		playerCamera = Camera.main.transform;
		cameraDown = new Rect(0,0,Screen.width, guiSize);
		cameraUp = new Rect(0, Screen.height - guiSize, Screen.width, guiSize);
		cameraLeft = new Rect(0, 0, guiSize, Screen.height);
		cameraRight = new Rect(Screen.width - guiSize, 0, guiSize, Screen.height);
	}
	
	void Awake ()
	{
		uMovement = GetComponent<UnitMovement>();
		if (PhotonNetwork.isMasterClient) {
			uMovement.OriginalPosition = transform.position;
		}
		forgerInfo = GetComponent<Unit>();

		playerCamera = Camera.main.transform;
		cameraDown = new Rect(0,0,Screen.width, guiSize);
		cameraUp = new Rect(0, Screen.height - guiSize, Screen.width, guiSize);
		cameraLeft = new Rect(0, 0, guiSize, Screen.height);
		cameraRight = new Rect(Screen.width - guiSize, 0, guiSize, Screen.height);
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
				RaycastHit hit; LayerMask mask = 1 << 11;
				Vector3 click = lastClientClick; int hitID = gameObject.GetPhotonView().viewID;
				GameObject hitGO = gameObject;

				if (Physics.Raycast (ray, out hit, 200, mask))
				{
					click = hit.point;
					hitGO = ClickedCollider( click );
				}
				if( lastClientClick != click )
				{
					lastClientClick = click;
					hitID = hitGO.GetPhotonView().viewID;
					if(hitID != photonView.viewID && hitGO.tag != gameObject.tag)
						uMovement.U2UMMovementInput(lastClientClick, hitID, UnitMovement.MovementState.MOVEATTACK);
					else
						uMovement.U2UMMovementInput(lastClientClick, hitID, UnitMovement.MovementState.MOVE);
				}
			}
			if(Input.GetKey(KeyCode.Space))
			{
				LockedOn = true;
			}
			else
			{
				LockedOn = false;
			}
			if(Input.GetKeyDown(lockOnToggle))
			{
				if(PermLockOn)
					PermLockOn = false;
				else
					PermLockOn = true;
			}

//			if(Input.GetKey(KeyCode.LeftControl))
//			{
//				storedInput = "ctrl";
//			}

			//CameraControls
			if(LockedOn || PermLockOn)
			{
				playerCamera.transform.position = new Vector3(transform.position.x, playerCamera.transform.position.y,transform.position.z - 10);
			}
			else
			{
				if(cameraDown.Contains(Input.mousePosition))
					playerCamera.transform.Translate (0f, 0f, -cameraSpeed, Space.World);
				if(cameraUp.Contains(Input.mousePosition))
					playerCamera.transform.Translate (0f, 0f, cameraSpeed, Space.World);
				if(cameraLeft.Contains(Input.mousePosition))
					playerCamera.transform.Translate (-cameraSpeed, 0f, 0f, Space.World);
				if(cameraRight.Contains(Input.mousePosition))
					playerCamera.transform.Translate (cameraSpeed, 0f, 0f, Space.World);
			}
		}
	}

	void Attack()
	{
		if(Time.time >= forgerInfo.NextAttackTime)
		{
			Debug.Log("Hero Attacks with " + forgerInfo.Attack + " attack!");
			AttackAnimation();
			uMovement.Target.TakeDamage(forgerInfo);
			forgerInfo.NextAttackTime = Time.time + forgerInfo.AttackSpeed;
		}
	}
	
	void AttackAnimation()
	{
		
	}

	private GameObject ClickedCollider( Vector3 hit )
	{
		LayerMask mask = 1 << 8;
		Collider[] hitColliders = Physics.OverlapSphere(hit, 0.5f, mask); 
		return FindClosestCollider(hitColliders, hit);
	}
	
	private GameObject FindClosestCollider( Collider[] list, Vector3 pos )
	{
		float minDist = 500000f; GameObject output = gameObject;
		foreach(Collider c in list)
		{
			float dist = Vector3.Distance(c.transform.position, pos);
			if(dist < minDist && c.tag != "Terrain" )
			{
				output = c.gameObject;
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
			GUI.Label (new Rect (Screen.width - 100, 20, 300, 30), "Ping: " + PhotonNetwork.GetPing());
			GUI.Label (new Rect (10, 20, 300, 30), "CurrentState: " + uMovement.CurrentState.ToString());
			if(uMovement.Target != null)
				GUI.Label (new Rect (10, 30, 300, 30), "Target: " + uMovement.Target.UnitName);
			GUI.Label (new Rect (10, 10, 300, 30), "Movement: " + uMovement.Movement.ToString());

			GUI.Label(new Rect (viewPos.x - 25, posYHealthBar-40, 200, 25), serializeCheck);
			GUI.Label (new Rect (viewPos.x - 50, posYHealthBar-30, 200, 25), "Stream Pos: " + posReceived.ToString());
			GUI.Label (new Rect (viewPos.x - 25, posYHealthBar-20, 200, 25), "Stream Rot: " + rotReceived);
		}
		else
		{
			GUI.Label(new Rect (viewPos.x - 25, posYHealthBar-40, 200, 25), serializeCheck);
			GUI.Label (new Rect (viewPos.x - 50, posYHealthBar-30, 200, 25), "Stream Pos: " + posReceived.ToString());
			GUI.Label (new Rect (viewPos.x - 25, posYHealthBar-20, 200, 25), "Stream Rot: " + rotReceived);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		SerializeState( stream, info );
		uMovement.SerializeState( stream, info );
	}
	
	public void SerializeState(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			serializeCheck = "I am Writing!";
			Vector3 pos = transform.position;
			float rot = transform.eulerAngles.y;
			stream.SendNext(pos);
			stream.SendNext(rot);
		} else {
			serializeCheck = "I am Reading!";
			transform.position = posReceived = (Vector3)stream.ReceiveNext();
			Vector3 rota = transform.eulerAngles;
			rota.y = rotReceived = (float)stream.ReceiveNext();
			transform.eulerAngles = rota;
		}
	}
}
