using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NexusBehaviour : MonoBehaviour {

	[SerializeField]
	private List<Vector3> centerLaneWaypoints;
	[SerializeField]
	private int waveSize = 1;
	[SerializeField]
	private Transform centerSpawnPoint;
	private float individualSpawnTime = 5f;
	private float spawnTime = 30f;
	private float nextSpawnTime;
	private string team;
	private BaseUnit nexusInfo;

	private Texture2D backgroundTexture;
	private Texture2D healthTexture;

	//private BaseUnit minionInfo = new BaseUnit("Minion", "Minion", 400.0, 12, 1.5f, 20, 0, 2, 2);

	// Use this for initialization
	void Start () 
	{
		team = gameObject.tag;
		GetComponent<UnitInfo>().Info = new BaseUnit(team +" Nexus", transform, "Nexus", 1000.0, 0, 0, 0, 0, 0f, 0f, 20, 0, 0, 0, 0, 0, 0f, 0f, 0f, 0f);
		nexusInfo = GetComponent<UnitInfo>().Info;
		nextSpawnTime = Time.time + spawnTime/5;

		backgroundTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		backgroundTexture.SetPixel (0, 0, Color.black);
		backgroundTexture.Apply ();

		healthTexture = new Texture2D (1, 1, TextureFormat.RGB24, false);
		healthTexture.SetPixel (0, 0, Color.green);
		healthTexture.Apply ();

		if( team == "RedTeam" )
		{
			centerLaneWaypoints = new List<Vector3> {new Vector3(-15,0,0), new Vector3(-10,0,0), new Vector3(20,0,0)};
		}
		else
		{
			centerLaneWaypoints = new List<Vector3> {new Vector3(15,0,0), new Vector3(10,0,0), new Vector3(-20,0,0)};
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time >= nextSpawnTime)
		{
			SpawnWave();
		}
	}

	void SpawnWave()
	{
		for(int i = 0; i < waveSize; i++)
		{
			string minion = team+"Minion";
			GameObject min = Instantiate(Resources.Load(minion), centerSpawnPoint.position, centerSpawnPoint.rotation) as GameObject;
			BaseUnit meleeMinion = new BaseUnit( "Minion", min.transform, "Minion", 250.0, 15, 0,50, 9, 0.75f, 0f, 20, 5, 0, 2, 2, 2, 0f, 0f, 0f, 0f);
			min.GetComponent<MinionBehaviour>().UpdateStatistics( meleeMinion, centerLaneWaypoints );
		}

		nextSpawnTime = Time.time + spawnTime;
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
		
		float innerBarWidth = backgroundBarWidth - 2 * 2;
		float innerBarHeight = backgroundBarHeight - 2 * 2;
		
		float posYHealthBar = Screen.height - viewPos.y - backgroundBarHeight;
		
		GUI.BeginGroup (new Rect (viewPos.x - backgroundBarWidth / 2, posYHealthBar, backgroundBarWidth, backgroundBarHeight));
		GUI.DrawTexture (new Rect (0, 0, backgroundBarWidth, backgroundBarHeight), backgroundTexture, ScaleMode.StretchToFill);
		
		float healthPercent = (float)(nexusInfo.CurrentHealth / nexusInfo.MaxHealth);
		GUI.DrawTexture (new Rect (2, 2, innerBarWidth * healthPercent, innerBarHeight), healthTexture, ScaleMode.StretchToFill);
		
		GUI.EndGroup ();
	}
}
