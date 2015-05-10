using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameSetup : MonoBehaviour 
{
	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;
	private float spawnTime;

	void Start ()
	{
		PhotonNetwork.ConnectUsingSettings("0.1");
		spawnTime = 15;
		Debug.Log(spawnTime);
	}


	void Update ()
	{
		if(PhotonNetwork.isMasterClient && Time.time >= spawnTime)
		{
			SpawnMinions();
			spawnTime = spawnTime + 15;
			Debug.Log(spawnTime);
		}
	}

	void OnGUI ()
	{
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
		//Spawn Some Minions
		GameObject min = PhotonNetwork.Instantiate("BlueTeamMinion", new Vector3(20,0,0), Quaternion.identity, 0) as GameObject;
		BaseUnit meleeMinion = new BaseUnit( min.name, min.transform, "Minion", 250.0, 15, 0,50, 9, 0.75f, 0f, 20, 5, 0, 2, 2, 2, 0f, 0f, 0f, 0f);
		min.GetComponent<MinionBehaviour>().UpdateStatistics( meleeMinion, new List<Vector3> {new Vector3(15,0,0), new Vector3(10,0,0), new Vector3(-20,0,0)} );
		Debug.Log (min.name + " " + min.GetPhotonView().viewID);

		min = PhotonNetwork.Instantiate("RedTeamMinion", new Vector3(-20,0,0), Quaternion.identity, 0) as GameObject;
		meleeMinion = new BaseUnit( min.name, min.transform, "Minion", 250.0, 15, 0,50, 9, 0.75f, 0f, 20, 5, 0, 2, 2, 2, 0f, 0f, 0f, 0f);
		min.GetComponent<MinionBehaviour>().UpdateStatistics( meleeMinion, new List<Vector3> {new Vector3(-15,0,0), new Vector3(-10,0,0), new Vector3(20,0,0)} );
		Debug.Log (min.name + " " + min.GetPhotonView().viewID);
	
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}

	void OnJoinedRoom()
	{
		//Spawn Same Player
		GameObject player = PhotonNetwork.Instantiate ("BlueHero", Vector3.up, Quaternion.identity, 0) as GameObject;
		player.GetComponent<HeroBehavior>().StartingStatistics(new Hero( "Hero", player.transform, "Hero", 500.0, 5, 3.4, 54, 2, 1.5f, 0.5f, 16, 2, 15, 2, 5, 3, 0.2f, 0.8f,0.2f,0.5f ));
	}
}
