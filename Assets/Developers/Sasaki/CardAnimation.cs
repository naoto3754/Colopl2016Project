using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CardAnimation : MonoBehaviour {

	void Start () {
		foreach(GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
		{
			if(obj.GetComponent<Rigidbody>() != null)
				obj.GetComponent<Rigidbody>().isKinematic = true;
		}
		transform.Rotate(-90f,0f,0f);	
	}
	
	void Update () {
		transform.DORotate(Vector3.right*90f, 2f).SetEase(Ease.OutQuad).OnComplete(OnFinish);
	}
	
	void OnFinish()
	{
		foreach(GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
		{
			if(obj.GetComponent<Rigidbody>() != null)
				obj.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
