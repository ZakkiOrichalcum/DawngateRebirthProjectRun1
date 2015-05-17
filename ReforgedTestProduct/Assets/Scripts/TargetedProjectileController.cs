using UnityEngine;
using System.Collections;

public class TargetedProjectileController : Photon.MonoBehaviour {

	private Unit myOwner;
	[SerializeField]
	private Transform target;
	[SerializeField]
	private float mySpeed = 10;
	private Unit mySource;

	//private Effects[] myEffects;

	// Update is called once per frame
	void Update () 
	{
		if(target)
		{
			Vector3 dir = target.position - gameObject.transform.position;
			gameObject.transform.rotation = Quaternion.LookRotation(dir);
			transform.Translate(Vector3.forward * Time.deltaTime * mySpeed);
			if(Vector3.Distance(target.position, transform.position) <= 0.2)
			{
				if(PhotonNetwork.isMasterClient)
				{
					TargetHit();
				}
			}
		}
		else
		{
			if(PhotonNetwork.isMasterClient)
			{
				PoolManager.Instance.DespawnObject(gameObject);
			}
		}
	}

	public void Init(int own, int t, float s, Unit d /*, Effects[] e*/)
	{
		myOwner = PhotonView.Find (own).GetComponent<Unit>();
		target = PhotonView.Find(t).transform;
		mySpeed = s;
		mySource = d;
		photonView.RPC("SendProjectileInit", PhotonTargets.Others, t, s);
	}

	[RPC]
	void SendProjectileInit(int t, float s)
	{
		target = PhotonView.Find(t).transform;
		mySpeed = s;
	}

	void TargetHit()
	{
		target.GetComponent<Unit>().TakeDamage(mySource);	
		PoolManager.Instance.DespawnObject(gameObject);
	}
}
