using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class InGameManager : Singlton<InGameManager> 
{
	
	readonly float FADEIN_DURATION = 1f;
	readonly float DISPLAY_DURATION = 1.5f;
	readonly float FADEOUT_DURATION = 1f;
	
	bool _NowDisplaying = false;
	bool _MenuIsOpened = false;
	
	[SerializeField]
	List<Image> _DictionaryLines; 
	[SerializeField]
	List<Text> _DictionaryLabels;
	[SerializeField]
	GameObject _MenuButton;
	
	/// <summay>
	/// リスタートする
	/// </summay>
	public void OnRestart()
	{
		StageCreater.I.RestartStage();
	}
	/// <summay>
	/// ホームに戻る
	/// </summay>
	public void OnReturnHome()
	{
		StageCreater.I.Clear();
		StateManager.I.GoState(State.STAGE_SELECT);
	}
	
	public void OnReverse()
	{
		StageCreater.I.Reverse();
	}
	
	public void OnPressMenu()
	{
		_MenuIsOpened = !_MenuIsOpened;
		if(_MenuIsOpened)
			OpenMenu();
		else
			CloseMenu();
	}
	
	private void OpenMenu()
	{
		Sequence sequence = DOTween.Sequence();
		foreach(Transform button in _MenuButton.transform)
		{
			Vector3 defaultPos = button.position;
			Vector3 defaultScale = button.localScale;
			button.position = _MenuButton.transform.position; 
			button.localScale = Vector3.zero;
			sequence.Join( button.DOMove(defaultPos, 0.1f).SetDelay(0.05f) );
			sequence.Join( button.DOScale(defaultScale, 0.1f) );
			
			button.gameObject.SetActive(true);
		}
		sequence.Play();
		
	}
	
	private void CloseMenu()
	{
		foreach(Transform button in _MenuButton.transform){
			button.gameObject.SetActive(false);
		}
	}
	
	//仮でキーボードの入力とる
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)){
			OnReverse();
		}else if(Input.GetKeyDown(KeyCode.H)){
			OnReturnHome();
		}else if(Input.GetKeyDown(KeyCode.R)){
			OnRestart();
		}
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
		foreach(Text text in _DictionaryLabels)
		{
			Color c = text.color;
			c.a = 0f;
			seq.Join( text.DOColor(c, FADEIN_DURATION) );
		}
		foreach(Image image in _DictionaryLines)
		{
			Color c = image.color;
			c.a = 0f;
			seq.Join( image.DOColor(c, FADEIN_DURATION) );
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
		Sequence seq = DOTween.Sequence();
		foreach(Image image in _DictionaryLines)
		{
			Color c = image.color;
			c.a = 1f;
			image.color = c;
			Vector3 scale = image.transform.localScale;
			scale.x = 0;  
			image.transform.localScale = scale; 
			seq.Join( image.transform.DOScaleX(200, DISPLAY_DURATION/3) );
		}
		seq.Append( transform.DOMove(transform.position, DISPLAY_DURATION/3) );
		foreach(Text text in _DictionaryLabels)
		{		
			Color c = text.color;
			c.a = 1f;
			seq.Join( text.DOColor(c, DISPLAY_DURATION/3) );
		}
		seq.Play();
	} 
}
