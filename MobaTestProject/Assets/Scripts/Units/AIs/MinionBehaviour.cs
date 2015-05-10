using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionBehaviour : Photon.MonoBehaviour {

	[SerializeField]
	private List<Vector3> waypoints;
	private BaseUnit minionInfo;
	private MovementControl msControl;
	private string team;

	private Texture2D backgroundTexture;
	private Texture2D healthTexture;

	public List<Vector3> Waypoints
	{
		get{return waypoints;}
		set{waypoints = value;}
	}

	// Use this for initialization
	void Start () 
	{
		msControl = GetComponent<MovementControl>();
		UpdateStatistics(new BaseUnit( "Minion", transform, "Minion", 250.0, 15, 0,50, 9, 0.75f, 0f, 20, 5, 0, 2, 2, 2, 0f, 0f, 0f, 0f), 
		                 new List<Vector3> {new Vector3(15,0,0), new Vector3(10,0,0), new Vector3(-20,0,0)});
		msControl.UpdateStateInfo(MovementControl.MovementState.ATTACKMOVE);
		team = gameObject.tag;

		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();

		healthTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		healthTexture.SetPixel (0, 0, Color.green);
		healthTexture.Apply ();
	}

	void Awake () 
	{
		msControl = GetComponent<MovementControl>();
		msControl.UpdateStateInfo(MovementControl.MovementState.STOP);
		team = gameObject.tag;
		
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
				msControl.UpdateMovementInformation(MovementControl.MovementState.ATTACKMOVE, transform, waypoints[0]);
				if(msControl.OnApproach())
					waypoints.RemoveAt(0);
			}
			else
				msControl.UpdateStateInfo(MovementControl.MovementState.STOPATTACK);

			if(minionInfo.Dead)
			{
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}

	public void UpdateStatistics( BaseUnit unit, List<Vector3> points )
	{
		GetComponent<UnitInfo>().Info = unit;
		minionInfo = GetComponent<UnitInfo>().Info;
		minionInfo.NextAttackTime = Time.time + minionInfo.AttackSpeed;
		waypoints = points;
		msControl = GetComponent<MovementControl>();
		msControl.UpdateStatistics();
	}

	void Attack()
	{
		if(Time.time >= minionInfo.NextAttackTime)
		{
			BaseUnit targetInfo = msControl.AttackTarget.GetComponent<UnitInfo>().Info;
			AttackAnimation();
			targetInfo.TakeDamage(minionInfo.Attack);
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
		
		GUI.BeginGroup (new Rect (viewPos.x - backgroundBarWidth / 2, posYHealthBar, backgroundBarWidth, backgroundBarHeight));
		GUI.DrawTexture (new Rect (0, 0, backgroundBarWidth, backgroundBarHeight), backgroundTexture, ScaleMode.StretchToFill);
		
		float healthPercent = (float)(minionInfo.CurrentHealth / minionInfo.MaxHealth);
		GUI.DrawTexture (new Rect (2, 2, innerBarWidth * healthPercent, innerBarHeight), healthTexture, ScaleMode.StretchToFill);
		
		GUI.EndGroup ();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		SerializeState( stream, info );
		msControl.SerializeState( stream, info );
	}
	
	private void SerializeState( PhotonStream stream, PhotonMessageInfo info )
	{
		if( stream.isWriting )
		{
			stream.SendNext(minionInfo.CurrentHealth);
		}
		else
		{
			minionInfo.CurrentHealth = (double)stream.ReceiveNext();
		}
	}
}
