using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class FadeManager : Singlton<FadeManager> { 
	readonly Color INIT_COLOR = new Color(0,0,0,0);
	
	Image _FadeImage;
	bool _IsFading;
	//  Color _TexColor;
	
	public override void OnInitialize()
	{
		_FadeImage = GetComponentInChildren<Image>();
	}
	
	void Update()
	{
	}
	
	public void FadeIn(float duration)
	{
		_IsFading = true;
		Color c = _FadeImage.color;
		c.a = 0f;
		_FadeImage.DOColor(c, duration).OnComplete(() => {
			_IsFading = false;
		});
	}
	
	public void FadeOut(float duration)
	{
		_IsFading = true;
		_FadeImage.color = INIT_COLOR;
		Color c = _FadeImage.color;
		c.a = 0.7f;
		_FadeImage.DOColor(c, duration);
	}
}
