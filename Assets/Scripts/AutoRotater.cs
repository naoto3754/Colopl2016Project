using UnityEngine;
using System.Collections;

public class AutoRotater : MonoBehaviour 
{
	void Update () {
		transform.localEulerAngles += Vector3.forward * Time.deltaTime * 40f;
	}
}
