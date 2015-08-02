using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

	void OnTriggerEnter()
	{
		StartCoroutine(goal());
	}
	
	private IEnumerator goal()
	{
		transform.GetChild(0).GetComponent<TextMesh>().text = "Clear";
		gameObject.GetComponent<Renderer>().enabled = false;
		
		yield return new WaitForSeconds(2f);
		
		Application.LoadLevel("FreeDraw");	
	}
}
