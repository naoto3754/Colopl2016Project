using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public static class DoTweenExtension
{
	//BlurOptimized
	public static Tween DOBlurSize(this BlurOptimized target, float endValue, float duration)
	{
		return DOTween.To(()=>target.blurSize , x=>target.blurSize=x, endValue, duration );
	}
	
	//Texture2D
	public static Tween DOColor(this Image target, Color endValue, float duration)
	{
		return DOTween.To(()=>target.color , x=>target.color=x, endValue, duration );
	}

	//Text
	public static Tween DOColor(this Text target, Color endValue, float duration)
	{
		return DOTween.To(()=>target.color , x=>target.color=x, endValue, duration );
	}
	
	//Text
	public static Tween DOOrthographicSize(this Camera target, float endValue, float duration)
	{
		return DOTween.To(()=>target.orthographicSize , x=>target.orthographicSize=x, endValue, duration );
	}
	
	//Material
	public static Tween DoShadowWeight(this Material target, float endValue, float duration)
	{
		return DOTween.To(()=>target.GetFloat("_ShadowWeight") , x=>target.SetFloat("_ShadowWeight", x), endValue, duration );
	}

	//Material
	public static Tween DOMainColor(this Material target, Color endValue, float duration)
	{
		return DOTween.To(()=>target.GetColor("_MainColor") , x=>target.SetColor("_MainColor", x), endValue, duration );
	}

	//Material
	public static Tween DOColor(this SpriteRenderer target, Color endValue, float duration)
	{
		return DOTween.To(()=>target.color , x=>target.color = x, endValue, duration );
	}
}
