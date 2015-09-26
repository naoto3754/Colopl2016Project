using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class InGameManager : Singlton<InGameManager> 
{
	bool _NowDisplaying = false;
	
	public void DisplayDictionary()
	{
		if(_NowDisplaying)
			return;
		
		_NowDisplaying = true;
		BlurOptimized blur = Camera.main.GetComponent<BlurOptimized>();
		blur.enabled = true;
		Sequence seq = DOTween.Sequence();
		//フェードイン
		seq.Append( blur.DOBlurSize(3f, 1f).SetEase(Ease.OutSine).OnStart(() => {
			FadeManager.I.FadeOut(1f);
		}));
		//UI表示
		seq.Append( transform.DOMove(transform.position, 1.5f));
		//フェードアウト
		seq.Append( blur.DOBlurSize(0f, 1f).SetEase(Ease.InSine).OnStart(() => {
			FadeManager.I.FadeIn(1f);
		}));
		//終了処理
		seq.OnComplete(() => 
		{
			blur.enabled = false;
			StageCreater.I.IsPlayingAnimation = false;
			_NowDisplaying = false;
		});
		seq.Play();
	}
}
