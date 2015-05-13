using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	[SerializeField]
	private GameObject hero;
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

	public GameObject Hero
	{
		get{return hero;}
		set{hero = value;}
	}

	// Use this for initialization
	void Start () 
	{
		cameraDown = new Rect(0,0,Screen.width, guiSize);
		cameraUp = new Rect(0, Screen.height - guiSize, Screen.width, guiSize);
		cameraLeft = new Rect(0, 0, guiSize, Screen.height);
		cameraRight = new Rect(Screen.width - guiSize, 0, guiSize, Screen.height);
		heroBehaviour = hero.GetComponent<HeroBehavior>();
		heroMovementControl = hero.GetComponent<MovementControl>();
		clickingMarker = Instantiate(Resources.Load("ClickingMarker"), heroMovementControl.Position, Quaternion.identity) as GameObject;
		clickingMarker.SetActive(false);
		team = "BlueTeam";
	}
	
	// Update is called once per frame
	void Update () 
	{
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
					
					if( hitTarget.gameObject != hero.gameObject )
					{
						heroMovementControl.CurrentState = MovementControl.MovementState.MOVEATTACK;
						heroMovementControl.AttackTarget = hitTarget.transform;
					}
					else
					{
						heroMovementControl.CurrentState = MovementControl.MovementState.ATTACKMOVE;
						heroMovementControl.Target = hit.point;
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
					heroBehaviour.HeroInfo.Abilities[0].Active(hero.transform, heroBehaviour.HeroInfo, hitTarget.transform, hit.point);
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

					if( hitTarget.gameObject != hero.gameObject )
					{
						heroMovementControl.CurrentState = MovementControl.MovementState.MOVEATTACK;
						heroMovementControl.AttackTarget = hitTarget.transform;
					}
					else
					{
						heroMovementControl.CurrentState = MovementControl.MovementState.MOVE;
						heroMovementControl.Target = hit.point;
					}
				}
			}
		}

		//Spells
		if(Input.GetKeyDown(attackMove))
		{
			if(storedInput == attackMove)
				storedInput = "";
			else
				storedInput = attackMove;
		}

		if(Input.GetKeyDown(spell1))
		{
			if(storedInput == "ctrl")
			{
				heroBehaviour.HeroInfo.Abilities[0].LevelUp();
				heroBehaviour.UpdateEffects();
			}
			if(Time.time >= heroBehaviour.HeroInfo.Abilities[0].CurrentCooldown && heroBehaviour.HeroInfo.Abilities[0].SkillLevel > 0 )
			{
				storedInput = spell1;
				Debug.Log ("storedInput: " + storedInput);
			}
		}
		if(Input.GetKeyDown(spell2))
		{
			storedInput = spell2;
			Debug.Log ("storedInput: " + storedInput);
		}
		if(Input.GetKeyDown(spell3))
		{
			storedInput = spell3;
			Debug.Log ("storedInput: " + storedInput);
		}
		if(Input.GetKeyDown(spell4))
		{
			storedInput = spell4;
			Debug.Log ("storedInput: " + storedInput);
		}
		if(Input.GetKeyDown(stop))
		{
			Debug.Log ("storedInput: " + storedInput);
			storedInput = "";
			heroMovementControl.CurrentState = MovementControl.MovementState.STOP;
		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(storedInput.Length > 0)
			{
				Debug.Log("Command cancelled!");
				storedInput = "";
			}
			//else if((MenuOpen)) 
			//{
			//Close Menu
			//}
			else
			{
				Debug.Log("Menu Opened");
				//OpenBasicMenu
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
		if(Input.GetKey(KeyCode.LeftControl))
		{
			storedInput = "ctrl";
		}
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

	private Collider ClickedCollider( Vector3 hit )
	{
		Collider[] hitColliders = Physics.OverlapSphere(hit, 0.5f); 
		return FindClosestCollider(hitColliders, hit);
	}

	private Collider FindClosestCollider( Collider[] list, Vector3 pos )
	{
		float minDist = 500000f; Collider output = hero.GetComponent<Collider>();
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
		clickingMarker.SetActive(true);
		ClickingMarkerBehaviour cmBehaviour = clickingMarker.transform.GetComponent<ClickingMarkerBehaviour>();
		cmBehaviour.Position = v;
		cmBehaviour.color = c;
	}

	void OnGUI( )
	{
		GUI.Label (new Rect (10, Screen.height - 60, 300, 30), "Forger Attack: " + heroBehaviour.HeroInfo.Attack);

		SkillData spell = heroBehaviour.HeroInfo.Abilities[0];
		float cooldown = -1f * (Time.time - spell.CurrentCooldown);
		if(cooldown > 1)
			GUI.Label (new Rect (10, Screen.height - 40, 300, 30), "Test Bolt: Level "+ spell.SkillLevel + ": " + (int)(cooldown));
		else if( cooldown > 0 )
			GUI.Label (new Rect (10, Screen.height - 40, 300, 30), "Test Bolt: Level "+ spell.SkillLevel + ": " + cooldown);
		else
			GUI.Label (new Rect (10, Screen.height - 40, 300, 30), "Test Bolt: Level "+ spell.SkillLevel + ":  Ready");
	}
}
