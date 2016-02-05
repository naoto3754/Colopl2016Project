using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class InGameManager : Singleton<InGameManager> 
{
	bool _MenuIsOpened = false;
	bool _IsPlayingClearAnimation = false;

	[SerializeField]
	GameObject _MenuButton;
	[SerializeField]
	GameObject _HomeButton;
	[SerializeField]
	GameObject _RestartButton;
	[SerializeField]
	Animator _StageClearAnim;
	[SerializeField]
	Image _Circle;

	private void ButtonPushShared()
	{
		InputManager.I.ClearDoubleTapParam ();
	}
	/// <summay>
	/// リスタートする
	/// </summay>
	public void OnRestart()
	{
		if (_IsPlayingClearAnimation)
			return;
		
		ButtonPushShared ();
		StageAnimator.I.RestartStage();
		CloseMenu();
	}
	/// <summay>
	/// ホームに戻る
	/// </summay>
	public void OnReturnHome()
	{
		if (_IsPlayingClearAnimation)
			return;
		
		ButtonPushShared ();
		StageAnimator.I.CloseStage (45, 1f);
		CloseMenu();
	}
	
	public void OnReverse()
	{
		if (_IsPlayingClearAnimation)
			return;
		
		StageAnimator.I.Reverse();
	}
	
	public void OnPause()
	{
		ButtonPushShared ();
		if (!_MenuIsOpened) {
			OpenMenu ();
		} else {
			CloseMenu();
		}
	}

	private void OpenMenu()
	{
		_MenuIsOpened = true;
		Vector3 menuButtonPos = _MenuButton.transform.position;
		var rectTrans = _MenuButton.GetComponent<RectTransform> ();
		float buttonWidth = rectTrans.TransformPoint (rectTrans.rect.max).x - rectTrans.TransformPoint (rectTrans.rect.min).x;
		float moveDelta = buttonWidth * 1.2f;
		_RestartButton.transform.DOMove (menuButtonPos+moveDelta*Vector3.left, 0.3f);
		_HomeButton.transform.DOMove (menuButtonPos+2*moveDelta*Vector3.left, 0.3f);
	}

	private void CloseMenu()
	{
		_MenuIsOpened = false;
		_RestartButton.transform.DOMove (_MenuButton.transform.position, 0.3f);
		_HomeButton.transform.DOMove (_MenuButton.transform.position, 0.3f);
	}

	public override void OnInitialize ()
	{
		base.OnInitialize ();
		Material mat = _Circle.material;
		mat.SetFloat ("_Border1", 0f);
		mat.SetFloat ("_Border2", 0f);
		mat.SetFloat ("_Border3", 0f);
		mat.SetFloat ("_Border4", 0f);
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
		_IsPlayingClearAnimation = true;
		CustomBlur blur = Camera.main.GetComponent<CustomBlur>();
		blur.blurSize = 0f;
		var images = _StageClearAnim.GetComponentsInChildren<Image> ();
		foreach (var image in images) {
			image.DOColorA (1f, 0);
		}
		images [0].color = StageManager.I.CurrentInfo.BackgroundColor;

		Sequence seq = DOTween.Sequence();
		//フェードアウト
		seq.Append( blur.DOBlurSize(4f, FADE_DURATION).SetEase(Ease.OutSine).OnComplete(()=>{
			_StageClearAnim.Play("Clear");
		}) );

		seq.Append ( transform.DOMove (transform.position, TEXTFADE_DURATION).SetDelay(2.0f)
			.OnStart(()=>{
				StartCoroutine(CircleAnimation(10f, StageManager.I.CurrentInfo.BackgroundColor));		
			})
			.OnComplete(()=>{
				_StageClearAnim.Play("Idle");
			}) );

		foreach (var image in images) {
			seq.Join (image.DOColorA (0f, TEXTFADE_DURATION) );
		}

		//フェードイン
		seq.Append( blur.DOBlurSize(0f, FADE_DURATION).SetEase(Ease.OutQuart).OnStart(()=>{
			_IsPlayingClearAnimation = false;
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
		
	private readonly float CIRCLE_WIDTH = 0.6f;
	private readonly float CIRCLE_INTERVAL = 4f;
	IEnumerator CircleAnimation(float time, Color color){
		float max = 0.25f + (CIRCLE_WIDTH*2 + CIRCLE_INTERVAL)/2;
		int frame = 1000;
		Material mat = _Circle.material;
		mat.SetColor ("_MainColor", color);
		for (int n = 0; n < frame; n++) {
			float step = (float)n;
			mat.SetFloat ("_Border1", Mathf.Clamp((max-(CIRCLE_WIDTH*2-CIRCLE_INTERVAL)/2)*step/frame, 0f, 0.25f));
			mat.SetFloat ("_Border2", Mathf.Clamp((max-(CIRCLE_WIDTH-CIRCLE_INTERVAL)/2)*step/frame, 0f, 0.25f));
			mat.SetFloat ("_Border3", Mathf.Clamp((max-(CIRCLE_WIDTH)/2)*step/frame, 0f, 0.25f));
			mat.SetFloat ("_Border4", Mathf.Clamp(max*step/frame, 0f, 0.25f));
			yield return new WaitForSeconds(time/frame*((frame-step+100)/((1+frame)*frame/2+100*frame)));
		}
	}
}
