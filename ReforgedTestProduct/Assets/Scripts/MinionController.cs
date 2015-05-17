using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionController : Photon.MonoBehaviour 
{
	private Unit minionInfo;
	private UnitMovement uMovement;
	private PoolManager poolManager;

	private Texture2D backgroundTexture;
	private Texture2D healthTexture;

	[SerializeField]
	private List<Vector3> waypoints;

	public List<Vector3> Waypoints
	{
		get{return waypoints;}
		set{waypoints = value;}
	}

	// Use this for initialization
	void Start () 
	{
		minionInfo = GetComponent<Unit>();

		uMovement = GetComponent<UnitMovement>();
		poolManager = PoolManager.Instance;

		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();
		
		healthTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		healthTexture.SetPixel (0, 0, Color.green);
		healthTexture.Apply ();
	}

	void Awake ()
	{
		minionInfo = GetComponent<Unit>();
		uMovement = GetComponent<UnitMovement>();
		poolManager = PoolManager.Instance;

		minionInfo.Init( "Minion", "Minion", tag, 1, 250.0, 15, 0, 50, 9, 0.75f, 0f, 20, 5, 0, 2, 200, 2, 0f, 0f, 0f, 0f, 150, 0, 300, 0 );
		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();
		
		healthTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		healthTexture.SetPixel (0, 0, Color.green);
		healthTexture.Apply ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(PhotonNetwork.isMasterClient)
		{
			if( waypoints.Count > 0 )
			{
				uMovement.U2UMMovementInput(waypoints[0], gameObject.GetPhotonView().viewID, UnitMovement.MovementState.ATTACKMOVE);
				if(uMovement.OnApproach())
					waypoints.RemoveAt(0);
			}
			else
				uMovement.U2UMState(UnitMovement.MovementState.STOPATTACK);
			
			if(minionInfo.Dead)
			{
				PoolManager.Instance.DespawnObject(gameObject);
			}
		}
	}

	void Attack()
	{
		if(Time.time >= minionInfo.NextAttackTime)
		{
			Debug.Log(minionInfo.UnitName + " Attacks with " + minionInfo.Attack + " attack!");
			if(minionInfo.AttackRange > 3.25)
			{
				if(minionInfo.Type == "Scout")
				{
					if(PhotonNetwork.isMasterClient)
					{
						GameObject shot = poolManager.SpawnObject("ScoutShot",transform.position, transform.rotation);
						shot.GetComponent<TargetedProjectileController>().Init(photonView.viewID, uMovement.Target.gameObject.GetPhotonView().viewID, 12f, minionInfo);
					}
				}
				else if(minionInfo.Type == "Wizard")
				{
					if(PhotonNetwork.isMasterClient)
					{
						GameObject shot = poolManager.SpawnObject("WizardShot",transform.position, transform.rotation);
						shot.GetComponent<TargetedProjectileController>().Init(photonView.viewID, uMovement.Target.gameObject.GetPhotonView().viewID, 8f, minionInfo);
					}
				}
			}
			else
			{
				AttackAnimation();
				uMovement.Target.TakeDamage(minionInfo);
			}
			minionInfo.NextAttackTime = Time.time + minionInfo.AttackSpeed;
		}
	}
	
	void AttackAnimation()
	{
		
	}

	void OnGUI ()
	{
		Vector2 backgroundBarSize = new Vector2 (Screen.width * 0.2f, Screen.height * 0.06f);
		
		Vector3 viewPos = Camera.main.WorldToScreenPoint (this.transform.position + new Vector3 (0, 1f, 0));
		
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
		
		float innerBarWidth = backgroundBarWidth - 2 * 2;
		float innerBarHeight = backgroundBarHeight - 2 * 2;
		
		float posYHealthBar = Screen.height - viewPos.y - backgroundBarHeight;

		GUI.Label (new Rect (viewPos.x - 30, posYHealthBar-20, 200, 25), "State: " + uMovement.CurrentState.ToString());
		if(uMovement.Target == null)
			GUI.Label (new Rect (viewPos.x - 30, posYHealthBar-40, 200, 25), "None : --");
		else
			GUI.Label (new Rect (viewPos.x - 30, posYHealthBar-40, 200, 25), uMovement.Target.UnitName + " : " + uMovement.Target.Dead);

		GUI.BeginGroup (new Rect (viewPos.x - backgroundBarWidth / 2, posYHealthBar, backgroundBarWidth, backgroundBarHeight));
		GUI.DrawTexture (new Rect (0, 0, backgroundBarWidth, backgroundBarHeight), backgroundTexture, ScaleMode.StretchToFill);
		
		float healthPercent = (float)(minionInfo.CurrentHealth / minionInfo.MaxHealth);
		GUI.DrawTexture (new Rect (2, 2, innerBarWidth * healthPercent, innerBarHeight), healthTexture, ScaleMode.StretchToFill);
		
		GUI.EndGroup ();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		SerializeState( stream, info );
		uMovement.SerializeState( stream, info );
	}
	
	public void SerializeState(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext(minionInfo.CurrentHealth);
		} else {
			minionInfo.CurrentHealth = (double)stream.ReceiveNext();
		}
	}
}
