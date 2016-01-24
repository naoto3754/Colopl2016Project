using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class RectTransformAutoScaler : MonoBehaviour 
{
	[SerializeField]
	bool _FitScreen = true;
	[SerializeField]
	Vector2 _FitSize;

	void Update () {
		var rectTrans = GetComponent<RectTransform> ();
		float scale = Screen.height/960f;
		rectTrans.sizeDelta = _FitScreen ? new Vector2(Screen.width/scale, Screen.height/scale) : _FitSize;
		rectTrans.localScale = new Vector3 (scale, scale, 1f);
	}
}
