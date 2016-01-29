using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;

public class FlashTMPro : MonoBehaviour 
{
	TextMeshProUGUI _TMPro;
	float _Delta = -0.03f;
	void Awake () 
	{
		_TMPro = GetComponent<TextMeshProUGUI> ();
	}

	void Update()
	{
		Color c = _TMPro.color;
		c.a += _Delta*(1.2f-c.a);
		if (c.a < 0.2f)
			_Delta = Mathf.Abs (_Delta);
		if (c.a > 1.0f)
			_Delta = -Mathf.Abs (_Delta);
		_TMPro.color = c;
	}
}
