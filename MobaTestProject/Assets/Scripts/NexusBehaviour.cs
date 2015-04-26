using UnityEngine;
using System.Collections;

public class NexusBehaviour : MonoBehaviour {

	[SerializeField]
	private Transform[] centerLaneWaypoints;
	[SerializeField]
	private int waveSize = 1;
	[SerializeField]
	private Transform centerSpawnPoint;
	[SerializeField]
	private Transform meleeMinion;

	private float individualSpawnTime = 5f;
	private float spawnTime = 30f;
	private float nextSpawnTime;
	private string team;
	//private BaseUnit minionInfo = new BaseUnit("Minion", "Minion", 400.0, 12, 1.5f, 20, 0, 2, 2);

	// Use this for initialization
	void Start () 
	{
		team = gameObject.tag;
		GetComponent<UnitInfo>().Info = new BaseUnit(gameObject, team +" Nexus", "Nexus", 1000, 0,0f,20,0,0,0, true);
		nextSpawnTime = Time.time + spawnTime/5;
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
			Instantiate(Resources.Load(minion), centerSpawnPoint.position, centerSpawnPoint.rotation);
		}

		nextSpawnTime = Time.time + spawnTime;
	}
}
