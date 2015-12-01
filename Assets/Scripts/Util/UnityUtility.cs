using UnityEngine;
using System.Collections;
using System.Linq;

public static class UnityUtility
{
	public static void SwapGameObject(GameObject obj1, GameObject obj2)
	{
		Transform tmpparent = obj1.transform.parent;
		Vector3 tmppos = obj1.transform.position;
		float forward1 = obj1.GetComponentInChildren<Renderer> ().material.GetFloat (Constants.M_FORWARD_THRES);
		float back1 = obj1.GetComponentInChildren<Renderer> ().material.GetFloat (Constants.M_BACK_THRES);
		float forward2 = obj2.GetComponentInChildren<Renderer> ().material.GetFloat (Constants.M_FORWARD_THRES);
		float back2 = obj2.GetComponentInChildren<Renderer> ().material.GetFloat (Constants.M_BACK_THRES);

		obj1.transform.SetParent (obj2.transform.parent);
		obj1.transform.position = obj2.transform.position;
		foreach (var material in obj1.GetComponentsInChildren<Renderer> ().Select(x => x.material)) {
			material.SetFloat (Constants.M_FORWARD_THRES, forward2);
			material.SetFloat (Constants.M_BACK_THRES, back2);
		}

		obj2.transform.SetParent( tmpparent );
		obj2.transform.position = tmppos;
		foreach (var material in obj2.GetComponentsInChildren<Renderer> ().Select(x => x.material)) {
			material.SetFloat (Constants.M_FORWARD_THRES, forward1);
			material.SetFloat (Constants.M_BACK_THRES, back1);
		}
	}



	public static void CopyGameObjectParam(GameObject src, GameObject dest)
	{
		dest.transform.position = src.transform.position;
		dest.transform.rotation = src.transform.rotation;
	}
}
