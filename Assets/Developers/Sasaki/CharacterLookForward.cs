using UnityEngine;
using System.Collections;

public class CharacterLookForward : MonoBehaviour {

	void OnTriggerStay(Collider col)
	{
		transform.parent.GetComponent<CharacterMover>().Jump();
	}

}
