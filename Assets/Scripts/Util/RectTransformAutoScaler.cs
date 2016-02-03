using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class RectTransformAutoScaler : MonoBehaviour 
{
	[SerializeField]
	bool _FitScreen = true;
	[SerializeField]
	bool _UseRate = false;
	[SerializeField]
	float _Rate;
	[SerializeField]
	Vector2 _FitSize;
	[SerializeField]
	Vector2 _Offset;

	void Update () {
		var rectTrans = GetComponent<RectTransform> ();
		float scale = Screen.height/960f;
		var rateSize = _UseRate ? _FitSize*(_Rate*Screen.width/scale)/_FitSize.x : _FitSize;
		rectTrans.sizeDelta = _FitScreen ? new Vector2(Screen.width/scale, Screen.height/scale) : rateSize;
		rectTrans.localScale = new Vector3 (scale, scale, 1f);

		rectTrans.localPosition = new Vector3 (_Offset.x*Screen.width, _Offset.y*Screen.height,0);
	}
}
