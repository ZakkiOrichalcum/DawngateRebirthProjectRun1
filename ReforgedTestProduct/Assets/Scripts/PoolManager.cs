using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : Photon.MonoBehaviour 
{
	//Courtesy of Damien Delmarle of unity3d forums

	[HideInInspector]
	public List<int> SpawnedObjects = new List<int> ();
	[HideInInspector]
	public List<GameObject> PooledObjects = new List<GameObject> ();
	
	
	void OnJoinedRoom()
	{
		if(isMaster)
			BuildPool ();
	}
	
	public virtual void BuildPool()
	{
		
	}
	void Awake()
	{
		GetInstance ();
	}
	public static PoolManager instance;
	
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static PoolManager Instance
	{
		get
		{
			return instance;
		}
	}
	void GetInstance()
	{
		instance = this;
	}

	void SendPlayerCurrentSpawnedAndPooled(PhotonPlayer playerTarget)
	{
		photonView.RPC ("ReceiveSpawnedObjectList", playerTarget, SpawnedObjects.ToArray ());
		List<int> pooledIDs = new List<int> ();
		foreach (GameObject obj in PooledObjects)
		{
			pooledIDs.Add (FindView (obj));
		}
		photonView.RPC ("ReceivePooledObjectList", playerTarget, pooledIDs.ToArray ());
	}
	
	[RPC]
	public void ReceiveSpawnedObjectList(int[] toSpawn)
	{
		Debug.Log ("From Master Server: <color=green>"+toSpawn.Length+"  Units spawned</color>");
		foreach (int activeThis in toSpawn)
		{
			FindGO (activeThis).SetActive (true);  
		}
	}
	
	[RPC]
	public void ReceivePooledObjectList(int[] pooled)
	{
		foreach(int obj in pooled)
		{
			FindGO (obj).transform.parent = transform;
			PooledObjects.Add (FindGO (obj));
		}
	}
	
	/// <summary>
	/// Despawns the object. Must be called only from master server
	/// </summary>
	/// <param name="obj">Object.</param>
	public virtual void DespawnObject(GameObject obj)
	{
		if (!isMaster)
			return;
		
		int objectID = obj.GetPhotonView ().viewID;
		photonView.RPC ("RPC_DespawnObject", PhotonTargets.AllBufferedViaServer, objectID);
		//[[Updating the list]]
		if(SpawnedObjects.Contains (objectID))
			SpawnedObjects.Remove (objectID);  
	}
	
	public virtual GameObject SpawnObject(string obj, Vector3 spawnPosition, Quaternion spawnRotation)
	{
		if (!isMaster)
			return null;
		GameObject NextObjectFound = NextToSpawn (obj); // check null?
		if (!NextObjectFound)
		{
			Debug.Log ("<color=red>Not Enough Object in the Pool</color>");
			return null;
		}
		int objectID = NextObjectFound.GetPhotonView ().viewID;

		NextObjectFound.transform.position = spawnPosition;
		NextObjectFound.transform.rotation = spawnRotation;
		NextObjectFound.SetActive (true);

		photonView.RPC ("RPC_SpawnObject", PhotonTargets.OthersBuffered, spawnPosition, objectID, spawnRotation);
		//[[Updating the list]]
		SpawnedObjects.Add (objectID);  
		return NextObjectFound;
	}
	
	
	[RPC]
	public void RPC_DespawnObject(int ourObjectID)
	{
		FindGO (ourObjectID).SetActive (false);
		FindGO (ourObjectID).transform.position = new Vector3(0, -4, 0);
		
	}
	
	[RPC]
	public void RPC_SpawnObject(Vector3 spawnPos, int ourObjectID, Quaternion spawnRot)
	{
		Debug.Log ("SpawningObjectS");
		FindGO (ourObjectID).transform.position = spawnPos;
		FindGO (ourObjectID).transform.rotation = spawnRot;
		FindGO (ourObjectID).SetActive (true);
	}
	
	/// <summary>
	/// create unit as scene object, add it to pooled list, and immediatly disable it.
	/// </summary>
	/// <returns>The unit.</returns>
	public GameObject PoolUnit(string UnitName)
	{
		GameObject newUnit = PhotonNetwork.Instantiate
			(
				UnitName,
				new Vector3(0, -4, 0),
				Quaternion.identity,
				0
				);
		//[[Immediately disable the object in the pool]]
		newUnit.SetActive(false);

		newUnit.transform.parent = transform;
		PooledObjects.Add (newUnit);
		return newUnit;
	}
	#region PhotonEvents
	void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (isMaster)
			SendPlayerCurrentSpawnedAndPooled (newPlayer);
	}
	void OnMasterClientSwitched()
	{
		if (isMaster)
		{Debug.Log ("you are now MasterClient");
			SpawnedObjects.Clear ();
			for(int i =0; i < PooledObjects.Count; i++)
			{
				if(PooledObjects[i].activeSelf)
					SpawnedObjects.Add (PooledObjects[i].GetPhotonView().viewID);
			}
		}
	}
	#endregion

	#region helper
	private GameObject FindGO(int view_ID)
	{
		PhotonView v = PhotonView.Find (view_ID);
		if (v.viewID != 0)
			return v.gameObject;
		else
			return null;
	}
	
	private int FindView(GameObject go)
	{
		return go.GetPhotonView ().viewID;
	}
	
	public bool isMine
	{
		get
		{
			return photonView.isMine;
		}
	}
	
	public bool isMaster
	{
		get
		{
			return PhotonNetwork.isMasterClient;
		}
	}
	
	public bool IsSpawned(GameObject _o)
	{
		return SpawnedObjects.Contains (FindView (_o));
	}
	
	public bool SpawnedLeft()
	{
		return SpawnedObjects.Count > 0;
	}
	
	private GameObject NextToSpawn(string _type)
	{
		Debug.Log ("PooledObjects Count: " + PooledObjects.Count);
		int i = 0;
		for (i = 0; i < PooledObjects.Count; i++)
		{
			if(PooledObjects[i].name == _type+"(Clone)" && !PooledObjects[i].activeSelf)
			{
				Debug.Log ("Return the actual object");
				return PooledObjects[i];
			}
		}

		//TODO Case: we dont have enough units in our pool
		GameObject go = PoolUnit(_type) as GameObject;
		return go;
	}
	
	private GameObject NextToDespawn(string _type)
	{
		GameObject go;
		for (int i = 0; i < SpawnedObjects.Count; i++)
		{
			go = FindGO(SpawnedObjects[i]);
			if(go.name == _type+"(Clone)" && !go.activeSelf)
				return go;
		}
		return null;
	}
	#endregion
}