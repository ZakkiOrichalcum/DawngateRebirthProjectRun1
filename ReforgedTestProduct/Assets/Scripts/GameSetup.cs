using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameSetup : Photon.MonoBehaviour 
{
	public PoolManager poolManager;

	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;
	private float spawnTime;
	private float spawnIncrement = 30f;
	private Vector3 blueSpawnArea = new Vector3(20,0,0);
	private Vector3 redSpawnArea = new Vector3(-20,0,0);
	private List<Vector3> blueWaypoints = new List<Vector3> {new Vector3(15,0,0), new Vector3(10,0,0), new Vector3(-20,0,0)};
	private List<Vector3> redWaypoints = new List<Vector3> {new Vector3(15,0,0), new Vector3(10,0,0), new Vector3(-20,0,0)};
	private int minionLevel;
	private int minionNumber;
	
	void Start ()
	{
		PhotonNetwork.ConnectUsingSettings("0.1");
		spawnTime = 15;
		Debug.Log(spawnTime);
		minionLevel = 1;
		poolManager = PoolManager.Instance;
		minionNumber = 0;
	}
	
	
	void Update ()
	{
		if(PhotonNetwork.isMasterClient && Time.time >= spawnTime)
		{
			SpawnMinions();
			spawnTime = spawnTime + spawnIncrement;
			Debug.Log(spawnTime);
		}
	}
	
	void OnGUI ()
	{
		GUI.Label(new Rect(Screen.width/2, 10, 100, 50), Time.time.ToString());

		if( !PhotonNetwork.connected )
		{
			GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString());
		}
		else if( PhotonNetwork.room == null )
		{
			//Create Room
			if( GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
			{
				PhotonNetwork.CreateRoom(roomName + Guid.NewGuid().ToString("N"), true, true, 10);
			}
			
			//Join Room
			if( roomsList != null )
			{
				for( int i = 0; i < roomsList.Length; i++ )
				{
					if( GUI.Button (new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].name))
					{
						PhotonNetwork.JoinRoom(roomsList[i].name);
					}
				}
			}
		}
	}
	
	private void SpawnMinions()
	{
		//Spawn Blue Minions
		SpawnMinion(new Vector3(blueSpawnArea.x, blueSpawnArea.y, blueSpawnArea.z + 1.5f) , blueWaypoints, 
		            "BlueTeamMeleeMinion", "Melee", "BlueTeam", minionLevel, 450.0, 8, 0, 12, 0.5, 1f, 0f, 0, 0.5, 0, 0.25, 200, 3, 0f, 0f, 0f, 0f, 58, 4.72, 12, 0.07 );
		SpawnMinion(new Vector3(blueSpawnArea.x, blueSpawnArea.y, blueSpawnArea.z - 1.5f) , blueWaypoints, 
		            "BlueTeamMeleeMinion", "Melee", "BlueTeam", minionLevel, 450.0, 8, 0, 12, 0.5, 1f, 0f, 0, 0.5, 0, 0.25, 200, 3, 0f, 0f, 0f, 0f, 58, 4.72, 12, 0.07 );

		SpawnMinion(new Vector3(blueSpawnArea.x - 1.5f, blueSpawnArea.y, blueSpawnArea.z + 1.5f) , blueWaypoints, 
		            "BlueTeamScoutMinion", "Scout", "BlueTeam", minionLevel, 300.0, 7, 0, 14, 0.85, 1f, 0f, 0, 0.35, 0, 0.5, 325, 3, 0f, 0f, 0f, 0f, 42, 4.32, 10, 0.07 );
		SpawnMinion(new Vector3(blueSpawnArea.x - 1.5f, blueSpawnArea.y, blueSpawnArea.z - 1.5f) , blueWaypoints, 
		            "BlueTeamScoutMinion", "Scout", "BlueTeam", minionLevel, 300.0, 7, 0, 14, 0.85, 1f, 0f, 0, 0.35, 0, 0.5, 325, 3, 0f, 0f, 0f, 0f, 42, 4.32, 10, 0.07 );

		SpawnMinion(new Vector3(blueSpawnArea.x + 1.5f, blueSpawnArea.y, blueSpawnArea.z) , blueWaypoints, 
		            "BlueTeamSiegeMinion", "Siege", "BlueTeam", minionLevel, 740.0, 40, 0, 20, 1.2, 0.75f, 0f, 18, 3, 0, 0.25, 225, 3, 0f, 0f, 0f, 0f, 85, 5.3, 45, 0.07 );

		SpawnMinion(new Vector3(blueSpawnArea.x + 3f, blueSpawnArea.y, blueSpawnArea.z + 1.5f) , blueWaypoints, 
		            "BlueTeamWizardMinion", "Wizard", "BlueTeam", minionLevel, 280.0, 6, 0, 15, 1, 0.75f, 0f, 0, 0.28, 0, 0.5, 400, 3, 0f, 0f, 0f, 0f, 28, 2.4, 9, 0.07 );
		SpawnMinion(new Vector3(blueSpawnArea.x + 3f, blueSpawnArea.y, blueSpawnArea.z - 1.5f) , blueWaypoints, 
		            "BlueTeamWizardMinion", "Wizard", "BlueTeam", minionLevel, 280.0, 6, 0, 15, 1, 0.75f, 0f, 0, 0.28, 0, 0.5, 400, 3, 0f, 0f, 0f, 0f, 28, 2.4, 9, 0.07 );


		//Spawn Red Minions
		SpawnMinion(new Vector3(redSpawnArea.x, redSpawnArea.y, redSpawnArea.z + 1.5f) , redWaypoints, 
		            "RedTeamMeleeMinion", "Melee", "RedTeam", minionLevel, 450.0, 8, 0, 12, 0.5, 1f, 0f, 0, 0.5, 0, 0.25, 200, 3, 0f, 0f, 0f, 0f, 58, 4.72, 12, 0.07 );
		SpawnMinion(new Vector3(redSpawnArea.x, redSpawnArea.y, redSpawnArea.z - 1.5f) , redWaypoints, 
		            "RedTeamMeleeMinion", "Melee", "RedTeam", minionLevel, 450.0, 8, 0, 12, 0.5, 1f, 0f, 0, 0.5, 0, 0.25, 200, 3, 0f, 0f, 0f, 0f, 58, 4.72, 12, 0.07 );
		
		SpawnMinion(new Vector3(redSpawnArea.x + 1.5f, redSpawnArea.y, redSpawnArea.z + 1.5f) , redWaypoints, 
		            "RedTeamScoutMinion", "Scout", "RedTeam", minionLevel, 300.0, 7, 0, 14, 0.85, 1f, 0f, 0, 0.35, 0, 0.5, 325, 3, 0f, 0f, 0f, 0f, 42, 4.32, 10, 0.07 );
		SpawnMinion(new Vector3(redSpawnArea.x + 1.5f, redSpawnArea.y, redSpawnArea.z - 1.5f) , redWaypoints, 
		            "RedTeamScoutMinion", "Scout", "RedTeam", minionLevel, 300.0, 7, 0, 14, 0.85, 1f, 0f, 0, 0.35, 0, 0.5, 325, 3, 0f, 0f, 0f, 0f, 42, 4.32, 10, 0.07 );
		
		SpawnMinion(new Vector3(redSpawnArea.x - 1.5f, redSpawnArea.y, redSpawnArea.z) , redWaypoints, 
		            "RedTeamSiegeMinion", "Siege", "RedTeam", minionLevel, 740.0, 40, 0, 20, 1.2, 0.75f, 0f, 18, 3, 0, 0.25, 225, 3, 0f, 0f, 0f, 0f, 85, 5.3, 45, 0.07 );
		
		SpawnMinion(new Vector3(redSpawnArea.x - 3f, redSpawnArea.y, redSpawnArea.z + 1.5f) , redWaypoints, 
		            "RedTeamWizardMinion", "Wizard", "RedTeam", minionLevel, 280.0, 6, 0, 15, 1, 0.75f, 0f, 0, 0.28, 0, 0.5, 400, 3, 0f, 0f, 0f, 0f, 28, 2.4, 9, 0.07 );
		SpawnMinion(new Vector3(redSpawnArea.x - 3f, redSpawnArea.y, redSpawnArea.z - 1.5f) , redWaypoints, 
		            "RedTeamWizardMinion", "Wizard", "RedTeam", minionLevel, 280.0, 6, 0, 15, 1, 0.75f, 0f, 0, 0.28, 0, 0.5, 400, 3, 0f, 0f, 0f, 0f, 28, 2.4, 9, 0.07 );
		
	}

	private void SpawnMinion( Vector3 pos, List<Vector3> points, string n, string t, string tm, int l, double hp, int hpPer, double hpRegen, int a, double aPer, float atkspd, float atkspdPer, int arm, double armPer, 
	                         int mr, double mrPer, int atkrnge, int ms, float pwrRatio, float cdRatio, float asRatio, float msRatio, double x, double xPer, double m, double mPer )
	{
		Debug.Log("Spawn Log! of " + n + minionNumber);
		GameObject min = poolManager.SpawnObject(n, pos, Quaternion.identity);
		min.GetComponent<Unit>().Init( n + minionNumber++, t, tm, l, hp, hpPer, hpRegen, a, aPer, atkspd, atkspdPer, arm, armPer, mr, mrPer, atkrnge, ms, pwrRatio, cdRatio, asRatio, msRatio, x, xPer, m, mPer);
		min.GetComponent<MinionController>().Waypoints = points;
		Debug.Log (min.name + " " + min.GetPhotonView().viewID);
	}
	
	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}
	
	void OnJoinedRoom()
	{
		//Spawn Same Player
		GameObject player = poolManager.SpawnObject("BluePlayer", Vector3.up, Quaternion.identity);
		player.GetComponent<Unit>().Init( "Hero", "Hero", "BlueTeam", 1, 500.0, 5, 3.4, 54, 2, 1.5f, 0.5f, 16, 2, 15, 2, 200, 3, 0.2f, 0.8f,0.2f,0.5f, 150, 0, 300, 0 );
	}
}