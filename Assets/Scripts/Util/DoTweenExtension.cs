using UnityEngine;
using System.Collections;
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
	public static Tween DOPixelColor(this Texture2D target, float endValue, float duration)
	{
		return DOTween.To(()=>target.GetPixel(0,0).a , x=>target.SetPixel(0,0,new Color(0f,0f,0f,x)), endValue, duration );
	}
}
