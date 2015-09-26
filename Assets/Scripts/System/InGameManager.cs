using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class InGameManager : Singlton<InGameManager> 
{
	public void DisplayDictionary()
	{
		BlurOptimized blur = Camera.main.GetComponent<BlurOptimized>();
		blur.enabled = true;
		Sequence seq = DOTween.Sequence();
		//フェードイン
		seq.Append( blur.DOBlurSize(3f, 1f).SetEase(Ease.OutSine) );
		//UI表示
		//フェードアウト
		seq.Append( blur.DOBlurSize(0f, 1f).SetEase(Ease.InSine) );
		//終了処理
		seq.OnComplete(() => 
		{
			blur.enabled = false;
			StageCreater.I.IsPlayingAnimation = false;
		});
		seq.Play();
	}
}
