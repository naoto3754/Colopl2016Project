using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class InGameManager : Singleton<InGameManager> 
{
	
//	readonly float FADEIN_DURATION = 1f;
//	readonly float DISPLAY_DURATION = 1.5f;
//	readonly float FADEOUT_DURATION = 1f;
	
//	bool _NowDisplaying = false;
	bool menu = false;

	[SerializeField]
	List<Image> _DictionaryLines; 
	[SerializeField]
	List<Text> _DictionaryLabels;
	[SerializeField]
	GameObject _MenuButton;
	[SerializeField]
	GameObject _HomeButton;
	[SerializeField]
	GameObject _RestartButton;
	[SerializeField]
	Animator _StageClearAnim;

	[SerializeField]
	GameObject _TouchMark;

	private void ButtonPushShared()
	{
		InputManager.I.ClearDoubleTapParam ();
		_TouchMark.SetActive (false);
	}
	/// <summay>
	/// リスタートする
	/// </summay>
	public void OnRestart()
	{
		ButtonPushShared ();
		StageAnimator.I.RestartStage();
		CloseMenu();
	}
	/// <summay>
	/// ホームに戻る
	/// </summay>
	public void OnReturnHome()
	{
		ButtonPushShared ();
		StageAnimator.I.CloseStage (45, 1f);
		CloseMenu();
	}
	
	public void OnReverse()
	{
		StageAnimator.I.Reverse();
	}
	
	public void OnPause()
	{
		ButtonPushShared ();
		if (!menu) {
			OpenMenu ();
		} else {
			CloseMenu();
		}
	}

	private void OpenMenu()
	{
		menu = true;
		Vector3 menuButtonPos = _MenuButton.transform.position;
		var rectTrans = _MenuButton.GetComponent<RectTransform> ();
		float buttonWidth = rectTrans.TransformPoint (rectTrans.rect.max).x - rectTrans.TransformPoint (rectTrans.rect.min).x;
		float moveDelta = buttonWidth * 1.2f;
		_RestartButton.transform.DOMove (menuButtonPos+moveDelta*Vector3.left, 0.3f);
		_HomeButton.transform.DOMove (menuButtonPos+2*moveDelta*Vector3.left, 0.3f);
	}

	private void CloseMenu()
	{
		menu = false;
		_RestartButton.transform.DOMove (_MenuButton.transform.position, 0.3f);
		_HomeButton.transform.DOMove (_MenuButton.transform.position, 0.3f);
	}
	
	//仮でキーボードの入力とる
	void Update()
	{
		if (InputManager.I.GetTapDown (0)) {
			_TouchMark.transform.position = Input.mousePosition;
			_TouchMark.SetActive (true);
		} else if (InputManager.I.GetTapUp (0)) {
			_TouchMark.SetActive (false);
		}

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
//		if (StageManager.I.CurrentStageIndex != 0)
//			return; 
//		if(gameObject.activeSelf == false)
//			return;
//		if(_NowDisplaying)
//			return;
//		
//		_NowDisplaying = true;
//		BlurOptimized blur = Camera.main.GetComponent<BlurOptimized>();
//		blur.enabled = true;
//		Sequence seq = DOTween.Sequence();
//		//フェードアウト
//		seq.Append( blur.DOBlurSize(3f, FADEOUT_DURATION).SetEase(Ease.OutSine).OnStart(() => {
//			FadeManager.I.AllFadeOut(FADEOUT_DURATION);
//		}));
//		//UI表示
//		seq.Append( transform.DOMove(transform.position, DISPLAY_DURATION).OnStart( DisplayText ) );
//		//フェードイン
//		seq.Append( blur.DOBlurSize(0f, FADEIN_DURATION).SetEase(Ease.InSine).OnStart(() => {
//			FadeManager.I.AllFadeIn(FADEIN_DURATION);
//		}));
//		foreach(Text text in _DictionaryLabels)
//		{
//			Color c = text.color;
//			c.a = 0f;
//			seq.Join( text.DOColor(c, FADEIN_DURATION) );
//		}
//		foreach(Image image in _DictionaryLines)
//		{
//			Color c = image.color;
//			c.a = 0f;
//			seq.Join( image.DOColor(c, FADEIN_DURATION) );
//		}
//		//終了処理
//		seq.OnComplete(() => 
//		{
//			blur.enabled = false;
//			_NowDisplaying = false;
//		});
//		seq.Play();
//
//		// BGM再生
//		AudioManager.I.PlayBGM (0);
	}
	
	private void DisplayText()
	{
//		Sequence seq = DOTween.Sequence();
//		foreach(Image image in _DictionaryLines)
//		{
//			Color c = image.color;
//			c.a = 1f;
//			image.color = c;
//			Vector3 scale = image.transform.localScale;
//			scale.x = 0;  
//			image.transform.localScale = scale; 
//			seq.Join( image.transform.DOScaleX(200, DISPLAY_DURATION/3) );
//		}
//		seq.Append( transform.DOMove(transform.position, DISPLAY_DURATION/3) );
//		foreach(Text text in _DictionaryLabels)
//		{		
//			Color c = text.color;
//			c.a = 1f;
//			seq.Join( text.DOColor(c, DISPLAY_DURATION/3) );
//		}
//		seq.Play();
	} 
}
