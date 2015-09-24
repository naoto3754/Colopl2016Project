using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/*
 * ステージセレクト画面時の挙動を制御
 */
public class StageSelectManager : Singlton<StageSelectManager> {
	private readonly Vector3 DEFAULT_LEFTANCHOR_LOCALPOSITION = new Vector3(0.506f, 0.78254f, 0f); 


	[SerializeField]
	private List<GameObject> _Books;
	public List<GameObject> Books
	{
		get { return _Books; }
	}
	private List<Vector3> _BookPosList;
	private List<Vector3> _BookRotList;
	private List<Vector3> _BookScaleList;
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
	
	public override void OnInitialize () {
		_BookPosList = new List<Vector3>();
		_BookRotList = new List<Vector3>();
		_BookScaleList=new List<Vector3>();
		foreach(GameObject book in _Books)
		{
			_BookPosList.Add(book.transform.position);
			_BookRotList.Add(book.transform.eulerAngles);
			_BookScaleList.Add(book.transform.localScale);
		}
	}
	
	public void Init()
	{
		if(_BookPosList == null)
			OnInitialize();
		for(int i = 0; i < _Books.Count; i++)
		{
			_Books[i].SetActive(true);
			_Books[i].transform.position = _BookPosList[i] + 100*Vector3.up;
			_Books[i].transform.localScale *= 0.5f;
			_Books[i].transform.DOMove(_BookPosList[i], 1f).SetEase(Ease.OutBounce).SetDelay(i*0.2f);
		}
	}
	
	void FixedUpdate () {
		UpdateBookAnimation();
	}
	
	void UpdateBookAnimation()
	{
		if(InputManager.I.GetAnyTapDown())
		{
			if(_IsPlayingAnimation == false)
			{
				//本に触ったらアニメーションさせる
				GameObject tappedObj = InputManager.I.GetTappedGameObject();
					
				if(tappedObj != null && _ViewContents == false)
				{
					OpenBook(tappedObj);
				}
				
				//目次表示時
				if(_ViewContents)
				{
					if(tappedObj != null && tappedObj.GetComponent<StageIndex>() != null)
					{
						SelectedStageIdx = tappedObj.GetComponent<StageIndex>().stageIdx;
						StateManager.I.GoState(State.INGAME);
					}
					else
						CloseBook();
				}
			}
		}
	}
	
	private void OpenBook(GameObject tappedObj)
	{
		_IsPlayingAnimation = true;
		int idx = _Books.IndexOf(tappedObj);
		SelectedChapter = idx;
		for(int i = 0; i < _Books.Count; i++)
		{
			if(i < idx){
				_Books[i].transform.DOMove(_Books[i].transform.position + new Vector3(-30, 0, 30), 1.0f);
			}else if(i == idx){
				_Books[i].transform.DOMove(_Books[i].transform.parent.position, 0.5f).SetDelay(0.25f);
				_Books[i].transform.DORotate(315*Vector3.up, 0.5f).SetEase(Ease.OutSine).SetDelay(0.25f);
				_Books[i].transform.GetChild(0).DOLocalRotate(-45*Vector3.up, 0.5f).SetEase(Ease.OutSine).SetDelay(0.25f);
				_Books[i].transform.GetChild(1).DOLocalRotate(-45*Vector3.up, 0.5f).SetEase(Ease.OutSine).SetDelay(0.25f);
				_Books[i].transform.DOScale(_Books[i].transform.localScale*2, 0.5f).SetDelay(0.25f)
					.OnComplete(() => {
						_IsPlayingAnimation = false;
						_ViewContents = true;
						StageCreater.I.Book = _Books[idx];
						StageManager.I.InstantiateStage(SelectedChapter,0);
					});
			}else{
				_Books[i].transform.DOMove(_Books[i].transform.position + new Vector3(30, 0, -30), 1.0f);
			}
		}
	}
	
	private void CloseBook()
	{
		_IsPlayingAnimation = true;
		_ViewContents = false;
		StageCreater.I.Clear();
		for(int i = 0; i < _Books.Count; i++)
		{
			_Books[i].transform.DOMove(_BookPosList[i], 1f);
			_Books[i].transform.DORotate(135*Vector3.up, 1f);
			_Books[i].transform.GetChild(0).DOLocalRotate(Vector3.zero, 1f);
			_Books[i].transform.GetChild(0).DOLocalMove(DEFAULT_LEFTANCHOR_LOCALPOSITION, 1f);
			_Books[i].transform.GetChild(1).DOLocalRotate(Vector3.zero, 1f);
			_Books[i].transform.DOScale(_BookScaleList[i] * 0.5f, 1f)
				.OnComplete(() => {
					_IsPlayingAnimation = false;
				});
		}
	}
}
