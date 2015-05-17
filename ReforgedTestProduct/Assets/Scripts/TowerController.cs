using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerController : Photon.MonoBehaviour 
{
	//Private Variables
	[SerializeField]
	private Transform towerTransform;
	[SerializeField]
	private Transform towerTop;
	[SerializeField]
	private Transform firePosition;
	[SerializeField]
	private Quaternion desiredRotation;
	private Unit towerInfo;
	private UnitStationary uStationary;
	
	private Texture2D backgroundTexture;
	private Texture2D healthTexture;

	// Use this for initialization
	void Start () 
	{
		towerInfo = GetComponent<Unit>();
		towerInfo.Init( "Tower", "Tower", tag, 1, 3000.0, 0, 0, 100, 2, 1f, 0f, 30, 2, 0, 0, 500, 0, 0, 0, 0, 0, 150, 0, 300, 0 );
		uStationary = GetComponent<UnitStationary>();
		uStationary.U2UMState(UnitStationary.AttackState.ATTACKFIRST);

		towerTransform = gameObject.transform;
		towerTop = towerTransform.FindChild("TowerTop").transform.FindChild("TowerShooter");
		firePosition = towerTop.FindChild("FirePosition");
		
		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();
		
		healthTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		healthTexture.SetPixel (0, 0, Color.green);
		healthTexture.Apply ();
	}
	void Awake ()
	{
		towerInfo = GetComponent<Unit>();
		towerInfo.Init( "Tower", "Tower", tag, 1, 3000.0, 0, 0, 100, 2, 1f, 0f, 30, 2, 0, 0, 500, 0, 0, 0, 0, 0, 150, 0, 300, 0 );
		uStationary = GetComponent<UnitStationary>();
		uStationary.U2UMState(UnitStationary.AttackState.ATTACKFIRST);
		
		towerTransform = gameObject.transform;
		towerTop = towerTransform.FindChild("TowerTop").transform.FindChild("TowerShooter");
		firePosition = towerTop.FindChild("FirePosition");
		
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
			if(uStationary.CurrentState != UnitStationary.AttackState.ATTACKFIRST)
				uStationary.U2UMState(UnitStationary.AttackState.ATTACKFIRST);
		}
		if(PhotonNetwork.isMasterClient && towerInfo.Dead)
		{
			//PhotonNetwork.Destroy(transform.FindChild("TowerTop").gameObject);
		}
	}

	void Attack()
	{
		if(Time.time >= towerInfo.NextAttackTime)
		{
			Debug.Log(towerInfo.UnitName + " Attacks with " + towerInfo.Attack + " attack!");
			if(PhotonNetwork.isMasterClient)
			{
				GameObject shot = PoolManager.Instance.SpawnObject("TowerShot",firePosition.position, firePosition.rotation);
				shot.GetComponent<TargetedProjectileController>().Init(photonView.viewID, uStationary.Target.gameObject.GetPhotonView().viewID, 10f, towerInfo);
				towerInfo.NextAttackTime = Time.time + towerInfo.AttackSpeed;
			}
		}
	}

	void OnGUI ()
	{	
		Vector2 backgroundBarSize = new Vector2 (Screen.width * 0.2f, Screen.height * 0.06f);
		
		Vector3 viewPos = Camera.main.WorldToScreenPoint (this.transform.position + new Vector3 (0, 3f, 0));
		
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
		
		GUI.BeginGroup (new Rect (viewPos.x - backgroundBarWidth / 2, posYHealthBar, backgroundBarWidth, backgroundBarHeight));
		GUI.DrawTexture (new Rect (0, 0, backgroundBarWidth, backgroundBarHeight), backgroundTexture, ScaleMode.StretchToFill);
		
		float healthPercent = (float)(towerInfo.CurrentHealth / towerInfo.MaxHealth);
		GUI.DrawTexture (new Rect (2, 2, innerBarWidth * healthPercent, innerBarHeight), healthTexture, ScaleMode.StretchToFill);
		
		GUI.EndGroup ();
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		SerializeState( stream, info );
		uStationary.SerializeState( stream, info );
	}
	
	public void SerializeState(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext(towerInfo.CurrentHealth);
		} else {
			towerInfo.CurrentHealth = (double)stream.ReceiveNext();
		}
	}
}
