using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;
using TMPro;

public static class DoTweenExtension
{
	//BlurOptimized
	public static Tween DOBlurSize(this BlurOptimized target, float endValue, float duration)
	{
		return DOTween.To(()=>target.blurSize , x=>target.blurSize=x, endValue, duration );
	}
	
	//Image
	public static Tween DOColor(this Image target, Color endValue, float duration)
	{
		return DOTween.To(()=>target.color , x=>target.color=x, endValue, duration );
	}

	//Image
	public static Tween DOColorA(this Image target, float endValue, float duration)
	{
		return DOTween.To(()=>target.color.a , x=>{Color c = target.color; c.a = x; target.color = c;}, endValue, duration );
	}

	//Text
	public static Tween DOColor(this Text target, Color endValue, float duration)
	{
		return DOTween.To(()=>target.color , x=>target.color=x, endValue, duration );
	}

	//TextMeshProUGUI
	public static Tween DOColorA(this TextMeshProUGUI target, float endValue, float duration)
	{
		return DOTween.To(()=>target.color.a , x=>{Color c = target.color; c.a = x; target.color = c;}, endValue, duration );
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
	public static Tween DOMainColorA(this Material target, float endValue, float duration)
	{
		return DOTween.To(()=>target.GetColor("_MainColor").a , x=>{Color c = target.GetColor("_MainColor"); c.a = x; target.SetColor("_MainColor", c);}, endValue, duration );
	}

	//SpriteRenderer
	public static Tween DOColor(this SpriteRenderer target, Color endValue, float duration)
	{
		return DOTween.To(()=>target.color , x=>target.color = x, endValue, duration );
	}

	//SpriteRenderer
	public static Tween DOColorA(this SpriteRenderer target, float endValue, float duration)
	{
		return DOTween.To(()=>target.color.a , x=>{Color c = target.color; c.a = x; target.color = c;}, endValue, duration );
	}

	//RectTransform
	public static Tween DOWidth(this RectTransform target, float endValue, float duration)
	{
		return DOTween.To(()=>target.sizeDelta.x , x=>{target.sizeDelta = new Vector2(x, target.sizeDelta.y);}, endValue, duration );
	}
}
