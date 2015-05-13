using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroBehavior : Photon.MonoBehaviour 
{

	public NetworkPlayer theOwner;
	Vector3 lastClientClick;
	Vector3 serverCurrentClick;
	bool clicked = false;

	private MovementControl msControl;
	private BaseUnit heroInfo;

	private Texture2D backgroundTexture;
	private Texture2D healthTexture;
	private float regenTimer;

	private Vector3 originalPos;

	[SerializeField]
	private string storedInput;
	private HeroBehavior heroBehaviour;
	private MovementControl heroMovementControl;
	private string team;
	private GameObject clickingMarker;
	private float guiSize = 100f;
	
	//key Codes
	private string attackMove = "a";
	private string stop = "s";
	private string spell1 = "q";
	private string spell2 = "w";
	private string spell3 = "e";
	private string spell4 = "r";
	private string lockOnToggle = "y";
	
	//Camera Controls
	[SerializeField]
	private Transform playerCamera;
	private bool LockedOn = false;
	private bool PermLockOn = false;
	private float cameraSpeed = 0.5f;
	private Rect cameraDown;
	private Rect cameraUp;
	private Rect cameraLeft;
	private Rect cameraRight;

	public BaseUnit HeroInfo
	{
		get{return heroInfo;}
	}


	// Use this for initialization
	void Start () 
	{
		heroInfo = GetComponent<UnitInfo>().Info;
		msControl = GetComponent<MovementControl>();
		msControl.CurrentState = MovementControl.MovementState.MOVE;

		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();

		healthTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		healthTexture.SetPixel (0, 0, Color.green);
		healthTexture.Apply ();

		playerCamera = Camera.main.transform;
		msControl.Target = transform.position;
		regenTimer = Time.time;
		cameraDown = new Rect(0,0,Screen.width, guiSize);
		cameraUp = new Rect(0, Screen.height - guiSize, Screen.width, guiSize);
		cameraLeft = new Rect(0, 0, guiSize, Screen.height);
		cameraRight = new Rect(Screen.width - guiSize, 0, guiSize, Screen.height);;
		heroMovementControl = GetComponent<MovementControl>();
		clickingMarker = Instantiate(Resources.Load("ClickingMarker"), heroMovementControl.Position, Quaternion.identity) as GameObject;
		clickingMarker.SetActive(false);
		team = transform.tag;

		StartingStatistics(new Hero( "Hero", gameObject.transform, "Hero", 500.0, 5, 3.4, 54, 2, 1.5f, 0.5f, 16, 2, 15, 2, 5, 3, 0.2f, 0.8f,0.2f,0.5f ));
	}

	void Awake ()
	{
		heroInfo = GetComponent<UnitInfo>().Info;
		msControl = GetComponent<MovementControl>();
		msControl.CurrentState = MovementControl.MovementState.MOVE;
		
		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();
		
		healthTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		healthTexture.SetPixel (0, 0, Color.green);
		healthTexture.Apply ();
		
		playerCamera = Camera.main.transform;
		//msControl.Target = transform.position;
		regenTimer = Time.time;
		cameraDown = new Rect(0,0,Screen.width, guiSize);
		cameraUp = new Rect(0, Screen.height - guiSize, Screen.width, guiSize);
		cameraLeft = new Rect(0, 0, guiSize, Screen.height);
		cameraRight = new Rect(Screen.width - guiSize, 0, guiSize, Screen.height);;
		heroMovementControl = GetComponent<MovementControl>();
		clickingMarker = Instantiate(Resources.Load("ClickingMarker"), heroMovementControl.Position, Quaternion.identity) as GameObject;
		clickingMarker.SetActive(false);
		team = transform.tag;
	}

	void Update()
	{
		if( photonView.isMine )
		{
			if(Time.time >= regenTimer)
			{
				heroInfo.RegenAndPassives();
				regenTimer += 1f;
			}
			if(Input.GetMouseButtonDown(0))
			{
				if(storedInput == attackMove)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit; LayerMask mask = 1 << 8;
					
					if (Physics.Raycast (ray, out hit, 200, mask))
					{
						Click(hit.point, Color.red);
						Collider hitTarget = ClickedCollider( hit.point );
						
						if( hitTarget.gameObject != gameObject )
						{
							heroMovementControl.UpdateMovementInformation(MovementControl.MovementState.MOVEATTACK, hitTarget.transform, hit.point);
						}
						else
						{
							heroMovementControl.UpdateMovementInformation(MovementControl.MovementState.ATTACKMOVE, hitTarget.transform, hit.point);
						}
					}
				}
				else if( storedInput == spell1 )
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit; LayerMask mask = 1 << 8;
					
					if (Physics.Raycast (ray, out hit, 200, mask))
					{
						Click(hit.point, Color.red);
						Collider hitTarget = ClickedCollider( hit.point );
						HeroInfo.Abilities[0].Active(transform, HeroInfo, hitTarget.transform, hit.point);
					}
				}
				storedInput = "";
			}
			if(Input.GetMouseButtonDown(1))
			{
				if(storedInput.Length > 0)
				{
					Debug.Log("Command Canceled!");
					storedInput = "";
				}
				else
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit; LayerMask mask = 1 << 8;
					
					if (Physics.Raycast (ray, out hit, 200, mask))
					{
						Collider hitTarget = ClickedCollider( hit.point );
						
						if( hitTarget.gameObject != gameObject )
						{
							heroMovementControl.UpdateMovementInformation(MovementControl.MovementState.MOVEATTACK, hitTarget.transform, hit.point);
						}
						else
						{
							heroMovementControl.UpdateMovementInformation(MovementControl.MovementState.MOVE, hitTarget.transform, hit.point);
						}
					}
				}
			}
			
			//Spells
//			if(Input.GetKeyDown(attackMove))
//			{
//				if(storedInput == attackMove)
//					storedInput = "";
//				else
//					storedInput = attackMove;
//			}
//			
//			if(Input.GetKeyDown(spell1))
//			{
//				if(storedInput == "ctrl")
//				{
//					heroInfo.Abilities[0].LevelUp();
//					UpdateEffects();
//				}
//				if(Time.time >= heroInfo.Abilities[0].CurrentCooldown && heroInfo.Abilities[0].SkillLevel > 0 )
//				{
//					storedInput = spell1;
//					Debug.Log ("storedInput: " + storedInput);
//				}
//			}
//			if(Input.GetKeyDown(spell2))
//			{
//				storedInput = spell2;
//				Debug.Log ("storedInput: " + storedInput);
//			}
//			if(Input.GetKeyDown(spell3))
//			{
//				storedInput = spell3;
//				Debug.Log ("storedInput: " + storedInput);
//			}
//			if(Input.GetKeyDown(spell4))
//			{
//				storedInput = spell4;
//				Debug.Log ("storedInput: " + storedInput);
//			}
//			if(Input.GetKeyDown(stop))
//			{
//				Debug.Log ("storedInput: " + storedInput);
//				storedInput = "";
//				heroMovementControl.UpdateMovementInformation(MovementControl.MovementState.STOP, transform, transform.position);
//			}
//			if(Input.GetKeyDown(KeyCode.Escape))
//			{
//				if(storedInput.Length > 0)
//				{
//					Debug.Log("Command cancelled!");
//					storedInput = "";
//				}
//				//else if((MenuOpen)) 
//				//{
//				//Close Menu
//				//}
//				else
//				{
//					Debug.Log("Menu Opened");
//					//OpenBasicMenu
//				}
//			}
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
				playerCamera.transform.position = new Vector3(heroMovementControl.Position.x, playerCamera.transform.position.y, heroMovementControl.Position.z - 10);
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

	public void StartingStatistics( Hero info )
	{
		GetComponent<UnitInfo>().Info =  info;
		heroInfo = GetComponent<UnitInfo>().Info;
		heroInfo.Abilities = new SkillData[] {new TestBolt()};
		heroInfo.NextAttackTime = Time.time;
		msControl = GetComponent<MovementControl>();
		msControl.UpdateStatistics();
		UpdateEffects();
	}

	public void UpdateEffects( )
	{
		foreach( SkillData ability in heroInfo.Abilities )
		{
			if(ability.PTypes != SkillData.PassiveTypes.NONE && ability.SkillLevel > 0)
			{
				ability.Passive(heroInfo, transform);
			}
		}
		heroInfo.StatisticCalculation();
	}

	void Attack()
	{
		if(Time.time >= heroInfo.NextAttackTime)
		{
			Debug.Log("Hero Attacks!");
			BaseUnit targetInfo = msControl.AttackTarget.GetComponent<UnitInfo>().Info;
			AttackAnimation();
			targetInfo.TakeDamage(heroInfo.Attack);
			heroInfo.NextAttackTime = Time.time + heroInfo.AttackSpeed;
		}
	}
	
	void AttackAnimation()
	{

	}

	private Collider ClickedCollider( Vector3 hit )
	{
		Collider[] hitColliders = Physics.OverlapSphere(hit, 0.5f); 
		return FindClosestCollider(hitColliders, hit);
	}
	
	private Collider FindClosestCollider( Collider[] list, Vector3 pos )
	{
		float minDist = 500000f; Collider output = GetComponent<Collider>();
		foreach(Collider c in list)
		{
			float dist = Vector3.Distance(c.transform.position, pos);
			if(dist < minDist && c.tag != team && c.tag != "Ground" )
			{
				output = c;
				minDist = dist;
			}
		}
		return output;
	}
	
	private void Click(Vector3 v, Color c)
	{

	}

	// Health Bar (all) and level (hero only)
	void OnGUI ()
	{	
		if( photonView.isMine )
		{
			GUI.Label (new Rect (10, 20, 300, 30), "CurrentState: " + msControl.CurrentState.ToString());
			if(msControl.Target != null)
				GUI.Label (new Rect (10, 40, 300, 30), "Target: " + msControl.Target.ToString());
			if(msControl.AttackTarget != null)
				GUI.Label (new Rect (10, 60, 300, 30), "Attack Target: " + msControl.AttackTarget.name);
			GUI.Label (new Rect (10, Screen.height - 60, 300, 30), "Forger Attack: " + HeroInfo.Attack);
			
			SkillData spell = HeroInfo.Abilities[0];
			float cooldown = -1f * (Time.time - spell.CurrentCooldown);
			if(cooldown > 1)
				GUI.Label (new Rect (10, Screen.height - 40, 300, 30), "Test Bolt: Level "+ spell.SkillLevel + ": " + (int)(cooldown));
			else if( cooldown > 0 )
				GUI.Label (new Rect (10, Screen.height - 40, 300, 30), "Test Bolt: Level "+ spell.SkillLevel + ": " + cooldown);
			else
				GUI.Label (new Rect (10, Screen.height - 40, 300, 30), "Test Bolt: Level "+ spell.SkillLevel + ":  Ready");
		}

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
		
		float innerBarWidth = backgroundBarWidth - 2 * 2;
		float innerBarHeight = backgroundBarHeight - 2 * 2;
		
		float posYHealthBar = Screen.height - viewPos.y - backgroundBarHeight;
		
		GUI.BeginGroup (new Rect (viewPos.x - backgroundBarWidth / 2, posYHealthBar, backgroundBarWidth, backgroundBarHeight));
		GUI.DrawTexture (new Rect (0, 0, backgroundBarWidth, backgroundBarHeight), backgroundTexture, ScaleMode.StretchToFill);
		
		float healthPercent = (float)(heroInfo.CurrentHealth / heroInfo.MaxHealth);
		GUI.DrawTexture (new Rect (2, 2, innerBarWidth * healthPercent, innerBarHeight), healthTexture, ScaleMode.StretchToFill);
		
		GUI.EndGroup ();

		GUI.Label(new Rect (viewPos.x - 50, posYHealthBar - 23, 100, 25), "Level " + heroInfo.Level);	
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
			stream.SendNext(heroInfo.CurrentHealth);
			//Send Items
		}
		else
		{
			heroInfo.CurrentHealth = (double)stream.ReceiveNext();
			//Send Items
		}
	}
}
