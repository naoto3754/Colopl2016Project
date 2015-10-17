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
}
