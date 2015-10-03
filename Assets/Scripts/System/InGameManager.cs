﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class InGameManager : Singlton<InGameManager> 
{
	readonly float FADEIN_DURATION = 1f;
	readonly float DISPLAY_DURATION = 1.5f;
	readonly float FADEOUT_DURATION = 1f;
	
	bool _NowDisplaying = false;
	
	void FixedUpdate ()
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
			Restart();
		}
		else if(Input.GetKeyDown(KeyCode.H))
		{
			ReturnHome();
		}
	}
	
	/// <summay>
	/// リスタートする
	/// </summay>
	private void Restart()
	{
		Debug.Log("Restart");
	}
	/// <summay>
	/// ホームに戻る
	/// </summay>
	private void ReturnHome()
	{
		Debug.Log("Return Home");
	}
	
	/// <summay>
	/// 辞書表示
	/// </summay>
	public void DisplayDictionary()
	{
		if(gameObject.activeSelf == false)
			return;
		if(_NowDisplaying)
			return;
		
		_NowDisplaying = true;
		BlurOptimized blur = Camera.main.GetComponent<BlurOptimized>();
		blur.enabled = true;
		Sequence seq = DOTween.Sequence();
		//フェードアウト
		seq.Append( blur.DOBlurSize(3f, FADEOUT_DURATION).SetEase(Ease.OutSine).OnStart(() => {
			FadeManager.I.FadeOut(FADEOUT_DURATION);
		}));
		//UI表示
		seq.Append( transform.DOMove(transform.position, DISPLAY_DURATION).OnStart( DisplayText ) );
		//フェードイン
		seq.Append( blur.DOBlurSize(0f, FADEIN_DURATION).SetEase(Ease.InSine).OnStart(() => {
			FadeManager.I.FadeIn(FADEIN_DURATION);
		}));
		foreach(Text text in GetComponentsInChildren<Text>())
		{
			Color c = text.color;
			c.a = 0f;
			seq.Join( text.DOColor(c, FADEIN_DURATION) );
		}
		//終了処理
		seq.OnComplete(() => 
		{
			blur.enabled = false;
			StageCreater.I.IsPlayingAnimation = false;
			_NowDisplaying = false;
		});
		seq.Play();

		// BGM再生
		AudioManager.Instance.PlayBGM (0);
	}
	
	private void DisplayText()
	{
		foreach(Text text in GetComponentsInChildren<Text>())
		{
			Color c = text.color;
			c.a = 1f;
			text.color = c;
		}
	} 
	
}
