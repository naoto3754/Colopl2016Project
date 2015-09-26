using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public static class DoTweenExtension
{
	public static Tween DOBlurSize(this BlurOptimized target, float endValue, float duration)
	{
		return DOTween.To(()=>target.blurSize , x=>target.blurSize=x, endValue, duration );
	}
}
