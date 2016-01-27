using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using TMPro;

/*
 * タイトル画面時の挙動を制御
 */
public class TitleManager : Singleton<TitleManager> 
{
	private readonly float FADE_TIME = 2f;
	bool _IsTapped;

	void Awake()
	{
		foreach(var image in GetComponentsInChildren<Image>())
		{
			image.DOColorA (1f, 0f);
			if (image.material != null && image.material.HasProperty("_MainColor")) {
				image.material.DOMainColorA (1f, 0f);
			}
		}
		foreach (var text in GetComponentsInChildren<TextMeshProUGUI>()) {
			text.DOColorA (1f, 0f);
		}
	}

	void Update()
	{
		if (Application.isShowingSplashScreen)
			return;
		
		if (_IsTapped)
			return;
		
		if(InputManager.I.GetAnyTapDown())
		{
			_IsTapped = true;
			Sequence seq = DOTween.Sequence ();
			foreach(var image in GetComponentsInChildren<Image>())
			{
				seq.Join( image.DOColorA (0,FADE_TIME) );
				if (image.material != null && image.material.HasProperty("_MainColor")) {
					seq.Join( image.material.DOMainColorA (0, FADE_TIME) );
				}
			}
			foreach (var text in GetComponentsInChildren<TextMeshProUGUI>()) {
				seq.Join( text.DOColorA (0, FADE_TIME) );
			}
			seq.OnComplete (() => {
				StateManager.I.GoState(GameState.STAGE_SELECT);	
			});
		}
	}
}
