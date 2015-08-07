using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {

	private Rigidbody rig;
	void Awake () {
		rig = GetComponent<Rigidbody>();
	}
	
	public void SetVelocity(Vector3 vel)
	{
		rig.velocity = vel;
	}
	
	void OnTriggerEnter()
	{
		transform.GetChild(0).GetComponent<BombFlow>().SetActive(true);
	}
}
