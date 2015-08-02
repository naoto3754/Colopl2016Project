using UnityEngine;
using System.Collections;

public class BreakDrawObject : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		Destroy(col.gameObject);
	}
	
}
