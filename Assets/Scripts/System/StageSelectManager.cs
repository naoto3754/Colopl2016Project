using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/*
 * ステージセレクト画面時の挙動を制御
 */
public class StageSelectManager : Singlton<StageSelectManager> {
	private readonly float HIGHEST_HEIGHT = -50f;
	private readonly float LOWEST_HEIGHT = 0f;
	//  private readonly Vector3 CAMERA_POSITION = new Vector3(-22.2f, 58.2f, -122.2f);
	//  private readonly Vector3 START_BOOK_OFFSET = new Vector3(0f, 15f, 0f);
	//  private readonly Vector3 DEFAULT_LEFTANCHOR_LOCALPOSITION = new Vector3(0.5f, 0f, 0f);
	//  private readonly Vector3 STAGE_BOOK_SCALE = new Vector3(22f, 36f, 22f);
	//  private readonly Vector3 DEFAULT_BOOK_SCALE = new Vector3(10.5f, 17.85f, 18.64f);

	[SerializeField]
	private GameObject _Shelf;
	
	private bool _ViewContents;
	private bool _IsPlayingAnimation;
	
	public int SelectedChapter
	{
		get; set;
	}
	public int SelectedStageIdx
	{
		get; set;
	}
	
	public override void OnInitialize () 
	{
		
	}
	
	public void Init()
	{	
		Sequence seq = DOTween.Sequence();
		_IsPlayingAnimation = true;
		
		Vector3 toPos = _Shelf.transform.position;
		toPos.y = HIGHEST_HEIGHT; 
		seq.Append(_Shelf.transform.DOMove(toPos, 1.5f) );
		
		seq.OnComplete(() => { _IsPlayingAnimation = false; });
		seq.Play();
	}
	
	void FixedUpdate () {
		UpdateShelfPos();
	}
	
	void UpdateShelfPos()
	{
		Vector2 swipeDir = InputManager.I.GetMoveDelta(0);
		Vector3 shelfPos = _Shelf.transform.position;
		float shelfY = Mathf.Clamp(shelfPos.y + swipeDir.y, HIGHEST_HEIGHT, LOWEST_HEIGHT);
		shelfPos.y = shelfY;
		_Shelf.transform.position = shelfPos;
		
		if(InputManager.I.GetAnyTapDown())
		{
			Destroy(_Shelf);
			StateManager.I.BookForStageCreater = new GameObject();
			GameObject child = new GameObject();
			child.transform.SetParent(StateManager.I.BookForStageCreater.transform);
			StageCreater.I.Book = StateManager.I.BookForStageCreater;
			SelectedStageIdx = 1;
			StateManager.I.GoState(State.INGAME);
		}

	}
	
	private void OpenBook(GameObject tappedObj)
	{
		//  Sequence seq = DOTween.Sequence();
		//  _IsPlayingAnimation = true;
		//  int idx = _Books.IndexOf(tappedObj);
		//  SelectedChapter = idx;
		//  for(int i = 0; i < _Books.Count; i++)
		//  {
		//  	if(i < idx){
		//  		seq.Join( _Books[i].transform.DOMove(_Books[i].transform.position + new Vector3(-30, 0, 30), 1.0f) );
		//  	}else if(i > idx){
		//  		seq.Join( _Books[i].transform.DOMove(_Books[i].transform.position + new Vector3(30, 0, -30), 1.0f) );
		//  	}
		//  }
		//  seq.Join( _Books[idx].transform.DOMove(_Books[idx].transform.parent.position, 0.5f).SetDelay(0.25f) );
		//  seq.Join( _Books[idx].transform.DORotate(315*Vector3.up, 0.5f).SetEase(Ease.OutSine) );
		//  seq.Join( _Books[idx].transform.GetChild(0).DOLocalRotate(-45*Vector3.up, 0.5f).SetEase(Ease.OutSine) );
		//  seq.Join( _Books[idx].transform.GetChild(1).DOLocalRotate(-45*Vector3.up, 0.5f).SetEase(Ease.OutSine) );
		//  seq.Join( _Books[idx].transform.DOScale(STAGE_BOOK_SCALE, 0.5f) );
		
		//  seq.OnComplete(() => 
		//  {
		//  	_IsPlayingAnimation = false;
		//  	_ViewContents = true;
		//  	StageCreater.I.Book = _Books[idx];
		//  	StageManager.I.InstantiateStage(SelectedChapter,0); 
		//  });
		//  seq.Play();
	}
	
	private void CloseBook()
	{
	//  	Sequence seq = DOTween.Sequence();
	//  	_IsPlayingAnimation = true;
	//  	_ViewContents = false;
	//  	StageCreater.I.Clear();
	//  	for(int i = 0; i < _Books.Count; i++)
	//  	{
	//  		seq.Join( _Books[i].transform.DOMove(_BookPosList[i], 1f) );
	//  		seq.Join( _Books[i].transform.DORotate(135*Vector3.up, 1f) );
	//  		seq.Join( _Books[i].transform.GetChild(0).DOLocalRotate(Vector3.zero, 1f) );
	//  		seq.Join( _Books[i].transform.GetChild(0).DOLocalMove(DEFAULT_LEFTANCHOR_LOCALPOSITION, 1f) );
	//  		seq.Join( _Books[i].transform.GetChild(1).DOLocalRotate(Vector3.zero, 1f) );
	//  		seq.Join( _Books[i].transform.DOScale(DEFAULT_BOOK_SCALE, 1f) );
	//  	}
	//  	seq.OnComplete(() => { _IsPlayingAnimation = false; });
	//  	seq.Play();
	}
}
