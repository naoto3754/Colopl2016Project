using UnityEngine;
using System.Collections;

public class Paper : MonoBehaviour {

	void Start () {
		//均一にテクスチャを貼る
		Material paperMaterial = gameObject.GetComponent<Renderer>().material;
		paperMaterial.mainTextureScale = transform.localScale*0.15f;	
	}
}
