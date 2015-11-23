using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class FadeManager : Singleton<FadeManager> { 
	readonly Color INIT_COLOR = new Color(0,0,0,0);
	
	Image _FadeImage;
	//  bool _IsFading;
	
	public override void OnInitialize()
	{
		_FadeImage = GetComponentInChildren<Image>();
	}
	
	public void FadeIn(float duration)
	{
		//  _IsFading = true;
		Color c = _FadeImage.color;
		c.a = 0f;
		_FadeImage.DOColor(c, duration).OnComplete(() => {
			//  _IsFading = false;
		});
	}
	
	public void FadeOut(float duration)
	{
		//  _IsFading = true;
		_FadeImage.color = INIT_COLOR;
		Color c = _FadeImage.color;
		c.a = 0.7f;
		_FadeImage.DOColor(c, duration);
	}
}
