using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class InGameManager : Singleton<InGameManager> 
{
	bool menu = false;

	[SerializeField]
	GameObject _MenuButton;
	[SerializeField]
	GameObject _HomeButton;
	[SerializeField]
	GameObject _RestartButton;
	[SerializeField]
	Animator _StageClearAnim;

	private void ButtonPushShared()
	{
		InputManager.I.ClearDoubleTapParam ();
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
		if(Input.GetKeyDown(KeyCode.Space)){
			OnReverse();
		}else if(Input.GetKeyDown(KeyCode.H)){
			OnReturnHome();
		}else if(Input.GetKeyDown(KeyCode.R)){
			OnRestart();
		}
	}
		
	readonly float FADE_DURATION = 1f;
	readonly float TEXTFADE_DURATION = 2f;
	/// <summay>
	/// ステージクリア表示
	/// </summay>
	public void DisplayStageClear(int stageIndex, int index)
	{
		BlurOptimized blur = Camera.main.GetComponent<BlurOptimized>();
		blur.blurSize = 0f;
		var images = _StageClearAnim.GetComponentsInChildren<Image> ();
		foreach (var image in images) {
			image.DOColorA (1f, 0);
		}
//		images [0].color = Color.blue;

		Sequence seq = DOTween.Sequence();
		//フェードアウト
		seq.Append( blur.DOBlurSize(3f, FADE_DURATION).SetEase(Ease.OutSine).OnComplete(()=>{
			_StageClearAnim.Play("Clear");
		}) );

		seq.Append ( transform.DOMove (transform.position, 0f).SetDelay(2.0f) );

		foreach (var image in images) {
			seq.Join( image.DOColorA (0f, TEXTFADE_DURATION).OnComplete(()=>{
				_StageClearAnim.Play("Idle");	
			}) );
		}

		//フェードイン
		seq.Append( blur.DOBlurSize(0f, FADE_DURATION).SetEase(Ease.OutQuart).OnStart(()=>{
			if(stageIndex == 2){
				InGameManager.I.OnReturnHome();
			}else{
				int[] indexInfo = StageManager.CalcStageIndexInfo (index + 1);
				StageManager.I.InstantiateStage (indexInfo [0], indexInfo [1], indexInfo [2]);
			}
		}) );

		//終了処理
		seq.Play();
	}
}
