using UnityEngine;
using System.Collections;

public class Paper : MonoBehaviour {

	void Start () {
		Material paperMaterial = gameObject.GetComponent<Renderer>().material;
		paperMaterial.mainTextureScale = transform.localScale*0.15f;	
	}
}
