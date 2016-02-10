using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;

public class StageAnimator : Singleton<StageAnimator>
{
	public enum ReOpenType
	{
		FIRST_OPEN,
		TO_NEXT,
		RESTART_STAGE,
	}
		
	public readonly float START_ANGLE = 45f;
	public readonly float SHADOW_WEIGHT = 0.8f;

	private Ease OPEN_EASE = Ease.Linear;
	private Ease CLOSE_EASE = Ease.Linear;
	public readonly float ANIMATION_TIME = 1f;

	private Sequence _PrevSequence;
	private Sequence _Sequence;
	private Sequence _Sequence_Step1;
	private Sequence _Sequence_Step2;
	private Sequence _Sequence_Step3;
	private Sequence _Sequence_Step4;

	public bool IsPlayingAnimation {
		get;
		private set;
	}

	public void OpenStage()
	{
		bool existStage = StageManager.I.PrevPaperRoot != null;
		StageRendererDeactivate ();

		if(existStage)
		{
			ClosePrevStage(90f, ANIMATION_TIME);
			ReOpenStage(0f, ANIMATION_TIME, 0.001f, 0f, ReOpenType.TO_NEXT);
		}
		else
		{
			ReOpenStage(START_ANGLE, ANIMATION_TIME, 0.001f, 0f, ReOpenType.FIRST_OPEN);
		}
	}

	public void Reverse(){
		if (StageManager.I.CurrentChapter == 1 && StageManager.I.CurrentBookID < 2)
			return;
		
		if (IsPlayingAnimation)
			return;

		if (StageManager.I.CurrentController == null)
			return;

		if (StageManager.I.IsOnObstacle ())
			return;

		_Sequence = DOTween.Sequence();
		ReOpenStageForReverse(1f, 1f, 0.3f);
	}

	public void RestartStage()
	{
		if (IsPlayingAnimation)
			return;

		StageManager.I.CurrentController.GetCollection = false;
		_Sequence = DOTween.Sequence();
		ReOpenStage(45f, 0.5f, 0.5f, 0.3f, ReOpenType.RESTART_STAGE);
	}

	/// <summary>
	/// ステージを閉じる
	/// </summary>
	public void CloseStage(float angle, float closetime)
	{
		if (IsPlayingAnimation)
			return;

		_Sequence = DOTween.Sequence ();
		int chap = StageManager.I.CurrentChapter;
		int bookID = StageManager.I.CurrentBookID;
		int stageIdx = StageManager.I.CurrentStageIndex;
		bool bookmarkActive = StageClearManager.I.IsSuspended ((chap-1)*3+bookID);
		float time = bookmarkActive ? 1f : 0f;
		StageClearManager.I.SetBookmarkActive (bookmarkActive, (chap-1)*3+bookID);
		StageManager.I.Book.bookmark.GetComponent<Renderer> ().enabled = bookmarkActive;
		_Sequence.Append( StageManager.I.Book.bookmark.DOLocalMoveY(Constants.BOOKMARK_LOCALY,time) );

		_Sequence.Append( StageManager.I.PaperRoot.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_Sequence.Join( StageManager.I.DecoRoot.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_Sequence.Join( StageManager.I.BackRootL.transform.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_Sequence.Join( StageManager.I.BackRootR.transform.DORotate(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_Sequence.Join( StageManager.I.Book.anchorL.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_Sequence.Join( StageManager.I.Book.anchorR.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );

		PushCloseStage(closetime);
		_Sequence.OnComplete(() => { 
			StageManager.I.Clear();
			IsPlayingAnimation = false; 
			StateManager.I.GoState(GameState.STAGE_SELECT);
			AudioManager.I.PlaySE (SEConfig.Tag.CLOSE);
		});
		_Sequence.Play();
	}
	/// <summary>
	/// ステージを閉じて開く
	/// </summary>
	public void ClosePrevStage(float angle, float closetime)
	{
		closetime *= 0.95f;
		_PrevSequence = DOTween.Sequence ();
		float thickness = StageCreater.THICKNESS*2;
		StageManager.I.PrevPaperRoot.transform.position += new Vector3(-thickness, 0, thickness);
		StageManager.I.PrevDecoRoot.transform.position += new Vector3(-thickness, 0, thickness);
		StageManager.I.PrevBackRootL.transform.position += new Vector3(-thickness, 0, thickness);
		StageManager.I.PrevBackRootR.transform.position += new Vector3(-thickness, 0, thickness);
		_PrevSequence.Append( StageManager.I.PrevPaperRoot.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_PrevSequence.Join( StageManager.I.PrevDecoRoot.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_PrevSequence.Join( StageManager.I.PrevBackRootL.transform.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_PrevSequence.Join( StageManager.I.PrevBackRootR.transform.DORotate(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );

		PushCloseStage(closetime, true);
		_PrevSequence.OnComplete(() => { 
			StageManager.DestroyObject(StageManager.I.PrevPaperRoot);
			StageManager.DestroyObject(StageManager.I.PrevDecoRoot);
			StageManager.DestroyObject(StageManager.I.PrevBackRootL);
			StageManager.DestroyObject(StageManager.I.PrevBackRootR);
			StageManager.DestroyObject(StageManager.I.PrevStageRoot);
			IsPlayingAnimation = false; 
		});
		_PrevSequence.Play();
		AudioManager.I.PlaySE (SEConfig.Tag.TURN_OVER);

		FadeManager.I.DoShelfColor (StageManager.I.CurrentInfo.BackgroundColor, closetime);
	}
	/// <summary>
	/// ステージを閉じて開く
	/// </summary>
	public void ReOpenStage(float angle, float opentime, float closetime, float waittime, ReOpenType type)
	{
		_Sequence = DOTween.Sequence();
		_Sequence.OnStart(() => {
			if(type == ReOpenType.FIRST_OPEN || type == ReOpenType.TO_NEXT)
				StageRendererActivate();
		});
		_Sequence.Append( StageManager.I.PaperRoot.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_Sequence.Join( StageManager.I.DecoRoot.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_Sequence.Join( StageManager.I.BackRootL.transform.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		_Sequence.Join( StageManager.I.BackRootR.transform.DORotate(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
		switch(type)
		{    
		case ReOpenType.FIRST_OPEN:
		case ReOpenType.RESTART_STAGE:
			_Sequence.Join( StageManager.I.Book.anchorL.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
			_Sequence.Join( StageManager.I.Book.anchorR.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
			break;
		}
		PushCloseStage(closetime);
		_Sequence.Append (transform.DOMove (transform.position, 0f).OnStart (() => {
			if (type == ReOpenType.RESTART_STAGE)
			{
				StageManager.I.CurrentController.HideCharacter();
				StageManager.I.CurrentController.SetInitParam();
				foreach(var eventBase in StageManager.I.CurrentInfo.gameObject.GetComponentsInChildren<EventBase>()){
					eventBase.Reset();
				}
				AudioManager.I.PlaySE (SEConfig.Tag.CLOSE);
			}
		}));
		_Sequence.Append( StageManager.I.PaperRoot.transform.DOBlendableRotateBy(-angle*Vector3.up, opentime).SetEase(OPEN_EASE).SetDelay(waittime));
		_Sequence.Join( StageManager.I.DecoRoot.transform.DOBlendableRotateBy(-angle*Vector3.up, opentime).SetEase(OPEN_EASE) );
		_Sequence.Join( StageManager.I.BackRootL.transform.DORotate(0*Vector3.up, opentime).SetEase(OPEN_EASE) );
		_Sequence.Join( StageManager.I.BackRootR.transform.DORotate(0*Vector3.up, opentime).SetEase(OPEN_EASE) );
		//はじめは本を開く処理もする
		switch(type)
		{    
		case ReOpenType.FIRST_OPEN:
		case ReOpenType.RESTART_STAGE:
			_Sequence.Join( StageManager.I.Book.anchorL.DORotate(0*Vector3.up, opentime).SetEase(OPEN_EASE) );
			_Sequence.Join( StageManager.I.Book.anchorR.DORotate(-90*Vector3.up, opentime).SetEase(OPEN_EASE) );
			break;
		}
		PushOpenStage(opentime, type);
		_Sequence.Play();
	}
	/// <summary>
	/// 凹凸を押し出しながらステージを開く
	/// </summary>
	public void PushOpenStage(float time, ReOpenType type)
	{
		IsPlayingAnimation = true;
		OpenChildren (StageManager.I.PaperRoot, time);
		OpenChildren (StageManager.I.DecoRoot, time);

		_Sequence.OnComplete(() => {
			List<Transform> paperRoot = new List<Transform>(StageManager.I.PaperRoot.transform.childCount);
			foreach (Transform child in StageManager.I.PaperRoot.transform)
				paperRoot.Add(child);
			foreach (Transform tmp in paperRoot)
			{
				tmp.GetChild(0).SetParent(StageManager.I.PaperRoot.transform);
				DestroyImmediate(tmp.gameObject);
			}
			List<Transform> decoRoot = new List<Transform>(StageManager.I.DecoRoot.transform.childCount);
			foreach (Transform child in StageManager.I.DecoRoot.transform)
				decoRoot.Add(child);
			foreach (Transform tmp in decoRoot)
			{
				tmp.GetChild(0).SetParent(StageManager.I.DecoRoot.transform);
				DestroyImmediate(tmp.gameObject);
			}
			switch(type)
			{    
			case ReOpenType.RESTART_STAGE:
				StageManager.I.CurrentController.UpdateDummyCharacterPosition(Vector2.right);
				StageManager.I.CurrentController.UpdateCharacterState(Vector2.right);
				break;
			case ReOpenType.FIRST_OPEN:
				StageManager.I.Book.bookmark.DOLocalMoveY(Constants.BOOKMARK_LOCALY+1f,1f);
				break;
			}
			IsPlayingAnimation = false;
		});
	}

	private void OpenChildren(GameObject root, float time)
	{
		foreach (Transform tmpAnchor in root.transform)
		{
			bool dirX = tmpAnchor.GetChild(0).tag != "ZSideComponent";
			if(dirX)
			{
				_Sequence.Join( tmpAnchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(OPEN_EASE) );
			}
			else
			{
				Transform child = tmpAnchor.transform.GetChild(0); 
				_Sequence.Join( tmpAnchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(OPEN_EASE)
					.OnUpdate(() =>{
						Vector3 angle = child.eulerAngles;
						angle.y = StageManager.I.PaperRoot.transform.eulerAngles.y+90f;
						child.eulerAngles = angle;
					}) );
			}
		}
	}

	/// <summary>
	/// 凹凸を押し出しながらステージを閉じる
	/// </summary>
	public void PushCloseStage(float time, bool previous = false)
	{
		IsPlayingAnimation = true;

		Sequence targetSequence = previous ? _PrevSequence : _Sequence;
		GameObject paperRoot = previous ? StageManager.I.PrevPaperRoot : StageManager.I.PaperRoot;
		GameObject decoRoot = previous ? StageManager.I.PrevDecoRoot : StageManager.I.DecoRoot;

		List<Transform> paperChildren = new List<Transform>(paperRoot.transform.childCount);
		foreach (Transform child in paperRoot.transform)
			paperChildren.Add(child);

		foreach (Transform stageObj in paperChildren)
		{
			CloseChild (targetSequence, stageObj, paperRoot, new Vector3(StageManager.I.Offset.x, 0f, stageObj.position.z), time);
		}

		List<Transform> decoChildren = new List<Transform>(decoRoot.transform.childCount);
		foreach (Transform child in decoRoot.transform)
			decoChildren.Add(child);

		foreach (Transform stageObj in decoChildren)
		{
			CloseChild (targetSequence, stageObj, decoRoot, new Vector3(StageManager.I.Offset.x-StageCreater.OFFSET, 0f, stageObj.position.z), time);
		}
	}

	private void CloseChild(Sequence sequence, Transform target, GameObject root, Vector3 anchorPos, float time)
	{
		bool dirX = target.tag != "ZSideComponent";

		GameObject anchor = new GameObject("TmpAnchor");
		anchor.transform.SetParent(root.transform);
		anchor.transform.position = anchorPos;
		target.SetParent(anchor.transform);
		if(!dirX)
		{ 
			sequence.Join( anchor.transform.DOBlendableRotateBy(-90*Vector3.up, time).SetEase(CLOSE_EASE)
				.OnUpdate(() =>{
					Vector3 angle = anchor.transform.GetChild(0).eulerAngles;
					angle.y = root.transform.eulerAngles.y+90f;
					anchor.transform.GetChild(0).eulerAngles = angle;
				}) );
		}
		else
		{
			sequence.Join( anchor.transform.DOBlendableRotateBy(-90*Vector3.up, time).SetEase(CLOSE_EASE) );
		}
	}

	private void SwapCharacter()
	{
		StageManager.I.CurrentController.SwapCharacter();
		StageManager.I.CurrentController.Bottom = StageManager.I.CurrentController.DestBottom;
	}
	/// <summary>
	/// ステージを閉じて開く
	/// </summary>
	public void ReOpenStageForReverse(float opentime, float closetime, float waittime)
	{
		StageManager.I.CurrentController.SetAnimationTimeScale (0f);
		IsPlayingAnimation = true;
		//ステップ1(180度開く)
		_Sequence_Step1 = DOTween.Sequence();
		_Sequence_Step1.Append( StageManager.I.PaperRoot.transform.DOBlendableRotateBy(-45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
		_Sequence_Step1.Join( StageManager.I.DecoRoot.transform.DOBlendableRotateBy(-45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
		_Sequence_Step1.Join( StageManager.I.BackRootL.transform.DORotate(45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
		_Sequence_Step1.Join( StageManager.I.BackRootR.transform.DORotate(-45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
		_Sequence_Step1.Join( StageManager.I.Book.anchorL.DORotate(45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
		_Sequence_Step1.Join( StageManager.I.Book.anchorR.DORotate(-135*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
		ReverseAnimationStep1(closetime*1/3);
		_Sequence_Step1.OnComplete(() => {
			_Sequence_Step2 = DOTween.Sequence();
			_Sequence_Step2.Append( transform.DOMove(transform.position, 0f) );
			_Sequence_Step2.Join( StageManager.I.BackRootL.transform.DORotate(-45*Vector3.up, closetime*2/3).SetEase(CLOSE_EASE) );
			_Sequence_Step2.Join( StageManager.I.BackRootR.transform.DORotate(45*Vector3.up, closetime*2/3).SetEase(CLOSE_EASE) );
			_Sequence_Step2.Join( StageManager.I.Book.anchorL.DORotate(-45*Vector3.up, closetime*2/3).SetEase(CLOSE_EASE) );
			_Sequence_Step2.Join( StageManager.I.Book.anchorR.DORotate(-45*Vector3.up, closetime*2/3).SetEase(CLOSE_EASE) );
			ReverseAnimationStep2(closetime*2/3);
			_Sequence_Step2.OnComplete(() => {
				SwapCharacter();
				_Sequence_Step3 = DOTween.Sequence();
				_Sequence_Step3.Append( transform.DOMove(transform.position, opentime*2/3).SetEase(OPEN_EASE).SetDelay(waittime));
				_Sequence_Step3.Join( StageManager.I.BackRootL.transform.DORotate(45*Vector3.up, opentime*2/3).SetEase(OPEN_EASE) );
				_Sequence_Step3.Join( StageManager.I.BackRootR.transform.DORotate(-45*Vector3.up, opentime*2/3).SetEase(OPEN_EASE) );
				_Sequence_Step3.Join( StageManager.I.Book.anchorL.DORotate(45*Vector3.up, opentime*2/3).SetEase(OPEN_EASE) );
				_Sequence_Step3.Join( StageManager.I.Book.anchorR.DORotate(-135*Vector3.up, opentime*2/3).SetEase(OPEN_EASE) );
				ReverseAnimationStep3(opentime*2/3);
				AudioManager.I.PlaySE (SEConfig.Tag.CLOSE);
				_Sequence_Step3.OnComplete(() => {
					_Sequence_Step4 = DOTween.Sequence();
					_Sequence_Step4.Append( StageManager.I.PaperRoot.transform.DOBlendableRotateBy(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
					_Sequence_Step4.Join( StageManager.I.DecoRoot.transform.DOBlendableRotateBy(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
					_Sequence_Step4.Join( StageManager.I.BackRootL.transform.DORotate(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
					_Sequence_Step4.Join( StageManager.I.BackRootR.transform.DORotate(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
					_Sequence_Step4.Join( StageManager.I.Book.anchorL.DORotate(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
					_Sequence_Step4.Join( StageManager.I.Book.anchorR.DORotate(-90*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
					ReverseAnimationStep4(opentime*1/3);
					_Sequence_Step4.Play();
				});
				_Sequence_Step3.Play();
			});
			_Sequence_Step2.Play();
		});
		_Sequence_Step1.Play();
	}
	public void ReverseAnimationStep1(float time)
	{
		List<Transform> paperChildren = new List<Transform>(StageManager.I.PaperRoot.transform.childCount);
		foreach (Transform child in StageManager.I.PaperRoot.transform)
			paperChildren.Add(child);
		foreach (Transform stageObj in paperChildren)
		{
			CloseChildStep1 (stageObj, StageManager.I.PaperRoot,
				new Vector3 (StageManager.I.Offset.x + StageCreater.THICKNESS / 2, 0f, stageObj.position.z), time);
		}

		List<Transform> decoChildren = new List<Transform>(StageManager.I.PaperRoot.transform.childCount);
		foreach (Transform child in StageManager.I.DecoRoot.transform)
			decoChildren.Add(child);
		foreach (Transform stageObj in decoChildren)
		{
			CloseChildStep1 (stageObj, StageManager.I.DecoRoot, 
				new Vector3 (StageManager.I.Offset.x - StageCreater.OFFSET, 0f, stageObj.position.z), time);
		}
	}
	private void CloseChildStep1(Transform target, GameObject root, Vector3 anchorPos, float time)
	{
		bool dirX = target.tag != "ZSideComponent";

		GameObject anchor = new GameObject("TmpAnchor1");
		anchor.transform.SetParent(root.transform);
		anchor.transform.position = anchorPos;
		target.SetParent(anchor.transform);
		if(!dirX)
		{ 
			_Sequence_Step1.Join( anchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(CLOSE_EASE)
				.OnUpdate(() =>{
					Vector3 angle = anchor.transform.GetChild(0).eulerAngles;
					angle.y = root.transform.eulerAngles.y+90f;
					anchor.transform.GetChild(0).eulerAngles = angle;
				}) );
		}
		else
		{
			_Sequence_Step1.Join( anchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(CLOSE_EASE) );
		}
		if (target.childCount > 0) {
			var renderer = target.transform.GetChild (0).GetComponent<Renderer> ();
			if (renderer != null && renderer.material.HasProperty("_ShadowWeight")) {
				_Sequence_Step4.Join (renderer.material.DoShadowWeight (0f, time));
			}   
		}
	}

	public void ReverseAnimationStep2(float time)
	{
		List<Transform> paperChildren = new List<Transform>(StageManager.I.PaperRoot.transform.childCount);
		foreach (Transform child in StageManager.I.PaperRoot.transform)
			paperChildren.Add(child);
		foreach (Transform tmpAnchor in paperChildren)
		{            
			CloseChildStep2 (tmpAnchor, new Vector3(StageManager.I.Offset.x, 0f, StageManager.I.Offset.z), time);
		}

		List<Transform> decoChildren = new List<Transform>(StageManager.I.DecoRoot.transform.childCount);
		foreach (Transform child in StageManager.I.DecoRoot.transform)
			decoChildren.Add(child);
		foreach (Transform tmpAnchor in decoChildren)
		{            
			CloseChildStep2 (tmpAnchor, new Vector3(StageManager.I.Offset.x-StageCreater.OFFSET, 0f, StageManager.I.Offset.z-StageCreater.OFFSET), time);
		}
	}
	private void CloseChildStep2(Transform target, Vector3 anchorPos, float time)
	{
		Transform stageObj = target.GetChild(0);
		Vector3 pos = stageObj.transform.position;
		var tmPro = stageObj.GetComponent<TextMeshPro> ();
		Vector3 scale = tmPro == null ? stageObj.transform.lossyScale : tmPro.bounds.size;
		var renderer = stageObj.GetComponent<Renderer> ();
		var material = renderer == null ? null : renderer.material;
		bool useOffset = material != null && material.HasProperty (Constants.M_FORWARD_THRES) && material.HasProperty (Constants.M_BACK_THRES);
		float offset;
		if (useOffset){
			if(tmPro == null){
				offset = material.GetFloat (Constants.M_BACK_THRES) * scale.x/2 - (1 - material.GetFloat (Constants.M_FORWARD_THRES)) * scale.x/2;
			}else{
				offset = (material.GetFloat (Constants.M_BACK_THRES)+scale.x/2)/2 - (scale.x/2 - material.GetFloat (Constants.M_FORWARD_THRES))/2;
			}
		}else{
			offset = 0f;
		}
		float sign = Mathf.Sign( pos.x-(StageManager.I.Offset.x-StageManager.I.CurrentInfo.StageWidth/2) - (pos.z-StageManager.I.Offset.z) - StageManager.I.CurrentInfo.StageWidth/2 + offset);

		GameObject anchor = new GameObject("TmpAnchor2");
		anchor.transform.position = anchorPos;
		anchor.transform.SetParent(target);
		stageObj.SetParent(anchor.transform);
		if(sign < 0)
		{
			anchor.tag = StageCreater.X_TAG_NAME;
			_Sequence_Step2.Join( anchor.transform.DOBlendableRotateBy(-90*Vector3.up, time).SetEase(CLOSE_EASE) );
			if(renderer != null && material.name == "ThickPaper (Instance)")
			{
				_Sequence_Step2.Join( material.DOColor(material.color*SHADOW_WEIGHT, time) );
			}
		}
		else
		{
			anchor.tag = StageCreater.Z_TAG_NAME;
			_Sequence_Step2.Join( anchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(CLOSE_EASE) );
		}
	}

	public void ReverseAnimationStep3(float time)
	{
		foreach (Transform tmpAnchor in StageManager.I.PaperRoot.transform)
		{         
			OpenChildStep3 (tmpAnchor, time);
		}
		foreach (Transform tmpAnchor in StageManager.I.DecoRoot.transform)
		{         
			OpenChildStep3 (tmpAnchor, time);
		}
	}
	private void OpenChildStep3(Transform target, float time)
	{
		Transform stageObj = target.GetChild(0).GetChild(0);
		Transform anchor = target.GetChild(0);
		bool sign = anchor.tag != StageCreater.Z_TAG_NAME;  

		if(sign)
		{
			_Sequence_Step3.Join( anchor.transform.DOBlendableRotateBy(0*Vector3.up, time).SetEase(OPEN_EASE) );
			if(stageObj.GetComponent<Renderer>() != null && stageObj.GetComponent<Renderer>().material.name == "ThickPaper (Instance)")
			{
				Material mat = stageObj.GetComponent<Renderer>().material;
				_Sequence_Step3.Join( mat.DOColor(mat.color/SHADOW_WEIGHT, time) );
			}
		}
		else
		{                
			_Sequence_Step3.Join( anchor.transform.DOBlendableRotateBy(0*Vector3.up, time).SetEase(OPEN_EASE) );
		} 
	}

	public void ReverseAnimationStep4(float time)
	{
		foreach (Transform tmpAnchor in StageManager.I.PaperRoot.transform)
		{
			OpenChildStep4 (tmpAnchor, time);
		}
		foreach (Transform tmpAnchor in StageManager.I.DecoRoot.transform)
		{
			OpenChildStep4 (tmpAnchor, time);
		}
		_Sequence_Step4.OnComplete(() => {
			List<Transform> paperAnchors = new List<Transform>(StageManager.I.PaperRoot.transform.childCount);
			foreach (Transform tmpAnchor in StageManager.I.PaperRoot.transform)
				paperAnchors.Add(tmpAnchor);
			foreach (Transform tmpAnchor in paperAnchors)
			{
				Transform stageObj = tmpAnchor.GetChild(0).GetChild(0);
				stageObj.SetParent(StageManager.I.PaperRoot.transform);
				DestroyImmediate(tmpAnchor.gameObject);
			}
			List<Transform> decoAnchors = new List<Transform>(StageManager.I.DecoRoot.transform.childCount);
			foreach (Transform tmpAnchor in StageManager.I.DecoRoot.transform)
				decoAnchors.Add(tmpAnchor);
			foreach (Transform tmpAnchor in decoAnchors)
			{
				Transform stageObj = tmpAnchor.GetChild(0).GetChild(0);
				stageObj.SetParent(StageManager.I.DecoRoot.transform);
				DestroyImmediate(tmpAnchor.gameObject);
			}
			IsPlayingAnimation = false;
			StageManager.I.CurrentController.UpdateCharacterState(StageManager.I.CurrentController.TowardPositive ? Vector2.left : Vector2.right);
			StageManager.I.CurrentController.SetAnimationTimeScale (1f);
		});
	}
	private void OpenChildStep4(Transform target, float time)
	{
		Transform stageObj = target.GetChild(0).GetChild(0); 
		bool dirX = stageObj.tag != "ZSideComponent";
		if(dirX)
		{
			_Sequence_Step4.Join( target.transform.DOBlendableRotateBy(-45*Vector3.up, time).SetEase(OPEN_EASE) );
		}
		else
		{ 
			_Sequence_Step4.Join( target.transform.DOBlendableRotateBy(-45*Vector3.up, time).SetEase(OPEN_EASE)
				.OnUpdate(() =>{
					Vector3 angle = stageObj.eulerAngles;
					angle.y = StageManager.I.PaperRoot.transform.eulerAngles.y+90f;
					stageObj.eulerAngles = angle;
				}) );
		}
		if (stageObj.childCount > 0) {
			var renderer = stageObj.transform.GetChild (0).GetComponent<Renderer> ();
			if (renderer != null && renderer.material.HasProperty("_ShadowWeight")) {
				_Sequence_Step4.Join (renderer.material.DoShadowWeight (1f, time));
			}   
		}
	}

	private void StageRendererDeactivate(){
		foreach(Renderer renderer in StageManager.I.PaperRoot.GetComponentsInChildren<Renderer>())
		{
			if(renderer.enabled == false)
				renderer.gameObject.SetActive(false);
			renderer.enabled = false;
		}
		foreach(Renderer renderer in StageManager.I.DecoRoot.GetComponentsInChildren<Renderer>())
		{
			if(renderer.enabled == false)
				renderer.gameObject.SetActive(false);
			renderer.enabled = false;
		}
		foreach(Renderer renderer in StageManager.I.BackRootL.GetComponentsInChildren<Renderer>())
			renderer.enabled = false;
		foreach(Renderer renderer in StageManager.I.BackRootR.GetComponentsInChildren<Renderer>())
			renderer.enabled = false;
	}
	private void StageRendererActivate(){
		foreach(Renderer renderer in StageManager.I.PaperRoot.GetComponentsInChildren<Renderer>())
			renderer.enabled = true;
		foreach(Renderer renderer in StageManager.I.DecoRoot.GetComponentsInChildren<Renderer>())
			renderer.enabled = true;
		foreach(Renderer renderer in StageManager.I.BackRootL.GetComponentsInChildren<Renderer>())
			renderer.enabled = true;
		foreach(Renderer renderer in StageManager.I.BackRootR.GetComponentsInChildren<Renderer>())
			renderer.enabled = true;
	}
}
