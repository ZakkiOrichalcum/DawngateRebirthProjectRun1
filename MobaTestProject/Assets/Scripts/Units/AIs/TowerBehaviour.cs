using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerBehaviour : MonoBehaviour {

	//Private Variables
	[SerializeField]
	private GameObject myProjectile;
	[SerializeField]
	private Transform currentTarget;
	[SerializeField]
	private Transform towerTransform;
	[SerializeField]
	private Transform towerTop;
	[SerializeField]
	private Transform firePosition;
	[SerializeField]
	private List<string> enemyTeams;
	private float nextFireTime;
	private float nextMoveTime;
	private string team;
	private Quaternion desiredRotation;
	private List<Transform> targets = new List<Transform>();
	private BaseUnit towerInfo;

	private Texture2D backgroundTexture;
	private Texture2D healthTexture;
	
	//Public Variables
	public float reloadTime = 1f;
	public float turnSpeed = 5f;
	public float firePauseTime = .25f;
	
	// Use this for initialization
	void Start () 
	{
		nextFireTime = reloadTime;
		towerTransform = gameObject.transform;
		towerTop = towerTransform.FindChild("Tower_Base").transform.FindChild("Tower_Top");
		firePosition = towerTop.FindChild("Fire_Position");
		team = gameObject.tag;
		enemyTeams.Remove(team);
		GetComponent<UnitInfo>().Info = new BaseUnit("Tower", transform, "Tower", 1000.0, 0, 0, 25, 2, 1f, 0f, 30, 2, 0, 0, 5, 0, 0, 0, 0, 0);
		towerInfo = GetComponent<UnitInfo>().Info;
		Debug.Log(towerInfo.Name +" | current team: " + team +", enemy teams: " + enemyTeams[0]);

		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();

		healthTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		healthTexture.SetPixel (0, 0, Color.green);
		healthTexture.Apply ();
	}
	
	void Update () 
	{
		if(currentTarget) 
		{
			Vector3 dir = currentTarget.position - towerTop.position;
			dir.y = 0;
			Quaternion rot = Quaternion.LookRotation(dir);
			towerTop.rotation = Quaternion.Slerp(towerTop.rotation, rot, turnSpeed);

			if(Time.time >= nextFireTime)
			{
				FireProjectile();
			}
		}
		else
		{
			FindNewTarget();
		}

		if(towerInfo.Dead)
		{
			Destroy (gameObject);
		}
	}
	
	void OnTriggerEnter (Collider other)
	{
		if(enemyTeams.Contains(other.gameObject.tag))
		{
			if(!targets.Contains(other.gameObject.transform))
			{
				targets.Add(other.gameObject.transform);
			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if(targets.Contains(other.gameObject.transform))
		{
			Debug.Log("Something's went Right here!");
			targets.Remove(other.gameObject.transform);
		}
		if(other.gameObject.transform == currentTarget)
		{
			Debug.Log("Change Targets!");
			currentTarget = null;
			FindNewTarget();
			nextFireTime = Time.time + (reloadTime * 1.25f);
		}
	}
	
	void FireProjectile()
	{
		nextFireTime = Time.time + reloadTime;

		GameObject projectile = Instantiate(myProjectile, firePosition.position, firePosition.rotation) as GameObject;
		TowerShotBehaviour towershot = projectile.GetComponent<TowerShotBehaviour>();
		towershot.Target = currentTarget;
	}
	
	bool FindNewTarget()
	{
		float minDis = 50000f;
		bool check = false;
		List<Transform> thingsToRemove = new List<Transform>();
		foreach(Transform t in targets)
		{
			if(t == null)
			{
				thingsToRemove.Add(t);
			}
			else{
			float distance = Vector3.Distance(transform.position, t.position);
				if( distance < minDis )
				{
					currentTarget = t;
					check = true;
				}
			}
		}
		foreach(Transform t in thingsToRemove)
			targets.Remove(t);
		if(check)
			nextFireTime = Time.time + reloadTime;
		return check;
	}

	void OnGUI ()
	{	
		if (GameMaster.paused)
			return;

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
}
