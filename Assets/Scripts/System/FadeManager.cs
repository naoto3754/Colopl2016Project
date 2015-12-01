using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class FadeManager : Singleton<FadeManager> { 
	readonly Color INIT_COLOR = new Color(0,0,0,0);

	[SerializeField]
	Image _AllFadeImage;
	[SerializeField]
	Image _ShelfFadeImage;
	
	public override void OnInitialize()
	{
	}
	
	public void AllFadeIn(float duration)
	{
		Color c = _AllFadeImage.color;
		c.a = 0f;
		_AllFadeImage.DOColor (c, duration);
	}
	
	public void AllFadeOut(float duration)
	{
		_AllFadeImage.color = INIT_COLOR;
		Color c = _AllFadeImage.color;
		c.a = 0.7f;
		_AllFadeImage.DOColor(c, duration);
	}

	public void ShelfFadeIn(float duration)
	{
		Color start = _ShelfFadeImage.color;
		start.a = 1f;
		_ShelfFadeImage.color = start;

		Color end = _ShelfFadeImage.color;
		end.a = 0f;
		_ShelfFadeImage.DOColor(end, duration);
	}

	public void ShelfFadeOut(float duration)
	{
		Color start = _ShelfFadeImage.color;
		start.a = 0f;
		_ShelfFadeImage.color = start;

		Color end = _ShelfFadeImage.color;
		end.a = 1f;
		_ShelfFadeImage.DOColor(end, duration);
	}

	public void ShelfDoColor(Color c, float duration)
	{
		_ShelfFadeImage.DOColor(c, duration);
	}

	public void SetShelfColor(Color c)
	{
		_ShelfFadeImage.color = c;
	}
}
