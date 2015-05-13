using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

	// hero prefabs
	public Transform playerPrefabA;
	public Transform playerPrefabB;
	
	// creep prefabs
	public BaseUnit creepPrefabA;
	public BaseUnit creepPrefabB;
	
	public static bool paused = true;
	public float serverTime;
	
	public List<HeroBehavior> playerScripts = new List<HeroBehavior>();
	private int charNumber = 0;
	public static int players = 0;
	private int playersConnected = 0;
	private NetworkPlayer[] networkPlayers;
	private bool init = false;
	
	void Start() {
		paused = true;
	}
	
	void OnServerInitialized ()
	{
		networkPlayers = new NetworkPlayer[players];
		networkPlayers[playersConnected++] = Network.player;
		if (playersConnected >= players) {
			InitGame();
		}
	}
	
	private void InitGame() {
		paused = false;
		int count = 0;
		init = true;
		foreach(NetworkPlayer player in networkPlayers) {
			SpawnPlayer ("BlueHero", player, new Vector3());
		}
	}
	
	void OnPlayerConnected (NetworkPlayer player)
	{
		networkPlayers[playersConnected++] = player;
		if (playersConnected >= players) {
			InitGame();
		}
	}
	
	void SpawnPlayer (string s, NetworkPlayer player, Vector3 position)
	{
		string tempPlayerString = player.ToString ();
		int playerNumber = Convert.ToInt32 (tempPlayerString);
		Transform newPlayerTransform = (Transform)Network.Instantiate (Resources.Load(s), position, transform.rotation, playerNumber);
		newPlayerTransform.GetComponent<UnitInfo>().Info.CharID = charNumber++;
		playerScripts.Add (newPlayerTransform.GetComponent<HeroBehavior> ());
		NetworkView theNetworkView = newPlayerTransform.GetComponent<NetworkView>();
		theNetworkView.RPC ("SetPlayer", RPCMode.AllBuffered, player);
	}
	
	public void Update ()
	{
		if(Network.isServer)
			serverTime = Time.deltaTime;
	}

}
