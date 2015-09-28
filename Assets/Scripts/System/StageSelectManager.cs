using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/*
 * ステージセレクト画面時の挙動を制御
 */
public class StageSelectManager : Singlton<StageSelectManager> {
	private readonly Vector3 CAMERA_POSITION = new Vector3(-22.2f, 58.2f, -122.2f);
	private readonly Vector3 START_BOOK_OFFSET = new Vector3(0f, 15f, 0f);
	private readonly Vector3 DEFAULT_LEFTANCHOR_LOCALPOSITION = new Vector3(0.5f, 0f, 0f);
	private readonly Vector3 STAGE_BOOK_SCALE = new Vector3(22f, 36f, 22f);
	private readonly Vector3 DEFAULT_BOOK_SCALE = new Vector3(10.5f, 17.85f, 18.64f);

	[SerializeField]
	private GameObject _BookRoot;
	private List<GameObject> _Books;
	public List<GameObject> Books
	{
		get { return _Books; }
	}
	private List<Vector3> _BookPosList;
	private List<Vector3> _BookRotList;
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
		_Books = new List<GameObject>();
		foreach(Transform child in _BookRoot.transform)
		{
			_Books.Add(child.gameObject);
		}
		
		_BookPosList = new List<Vector3>();
		_BookRotList = new List<Vector3>();
		foreach(GameObject book in _Books)
		{
			_BookPosList.Add(book.transform.position);
			_BookRotList.Add(book.transform.eulerAngles);
		}
	}
	
	public void Init()
	{
		if(_BookPosList == null)
			OnInitialize();
			
		Sequence seq = DOTween.Sequence();
		_IsPlayingAnimation = true;
		for(int i = 0; i < _Books.Count; i++)
		{
			//初期設定
			_Books[i].SetActive(true);
			_Books[i].transform.position = _BookPosList[i] + START_BOOK_OFFSET;
			_Books[i].transform.localScale = DEFAULT_BOOK_SCALE;
			seq.Join( _Books[i].transform.DOMove(_BookPosList[i], 1f).SetEase(Ease.OutQuart).SetDelay(i*0.2f) );
			foreach(Renderer renderer in _Books[i].GetComponentsInChildren<Renderer>())
			{
				foreach(Material material in renderer.materials)
				{
					Color defaultColor = material.color;
					Color transparent = defaultColor;
					transparent.a = 0f;
					material.color = transparent;
					seq.Join( material.DOColor(defaultColor, 1f).SetEase(Ease.OutQuart) );
				}
			}
		}
		seq.OnComplete(() => { _IsPlayingAnimation = false; });
		seq.Play();
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
		Sequence seq = DOTween.Sequence();
		_IsPlayingAnimation = true;
		int idx = _Books.IndexOf(tappedObj);
		SelectedChapter = idx;
		for(int i = 0; i < _Books.Count; i++)
		{
			if(i < idx){
				seq.Join( _Books[i].transform.DOMove(_Books[i].transform.position + new Vector3(-30, 0, 30), 1.0f) );
			}else if(i > idx){
				seq.Join( _Books[i].transform.DOMove(_Books[i].transform.position + new Vector3(30, 0, -30), 1.0f) );
			}
		}
		seq.Join( _Books[idx].transform.DOMove(_Books[idx].transform.parent.position, 0.5f).SetDelay(0.25f) );
		seq.Join( _Books[idx].transform.DORotate(315*Vector3.up, 0.5f).SetEase(Ease.OutSine) );
		seq.Join( _Books[idx].transform.GetChild(0).DOLocalRotate(-45*Vector3.up, 0.5f).SetEase(Ease.OutSine) );
		seq.Join( _Books[idx].transform.GetChild(1).DOLocalRotate(-45*Vector3.up, 0.5f).SetEase(Ease.OutSine) );
		seq.Join( _Books[idx].transform.DOScale(STAGE_BOOK_SCALE, 0.5f) );
		
		seq.OnComplete(() => 
		{
			_IsPlayingAnimation = false;
			_ViewContents = true;
			StageCreater.I.Book = _Books[idx];
			StageManager.I.InstantiateStage(SelectedChapter,0); 
		});
		seq.Play();
	}
	
	private void CloseBook()
	{
		Sequence seq = DOTween.Sequence();
		_IsPlayingAnimation = true;
		_ViewContents = false;
		StageCreater.I.Clear();
		for(int i = 0; i < _Books.Count; i++)
		{
			seq.Join( _Books[i].transform.DOMove(_BookPosList[i], 1f) );
			seq.Join( _Books[i].transform.DORotate(135*Vector3.up, 1f) );
			seq.Join( _Books[i].transform.GetChild(0).DOLocalRotate(Vector3.zero, 1f) );
			seq.Join( _Books[i].transform.GetChild(0).DOLocalMove(DEFAULT_LEFTANCHOR_LOCALPOSITION, 1f) );
			seq.Join( _Books[i].transform.GetChild(1).DOLocalRotate(Vector3.zero, 1f) );
			seq.Join( _Books[i].transform.DOScale(DEFAULT_BOOK_SCALE, 1f) );
		}
		seq.OnComplete(() => { _IsPlayingAnimation = false; });
		seq.Play();
	}
}
