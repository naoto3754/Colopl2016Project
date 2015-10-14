using UnityEngine;
using System.Collections;
using DG.Tweening;

/*
 * ステージセレクト画面時の挙動を制御
 */
public class StageSelectManager : Singlton<StageSelectManager> {
	private readonly float ANIMATION_TIME = 2f;
	private readonly float HIGHEST_HEIGHT = -50f;
	private readonly float LOWEST_HEIGHT = 0f;
	private readonly Vector3 BOOK_POS = new Vector3(42.3f, -0.8f, -57.7f);
    private readonly Vector3 BOOK_SCALE = new Vector3(22f, 36f, 22f);

	[SerializeField]
	private GameObject _Shelf;
	
	private bool _ViewContents;
	
	public int SelectedChapter
	{
		get; set;
	}
	public int SelectedBookID
	{
		get; set;
	}
	public int SelectedStageIdx
	{
		get; set;
	}
	
	public void Init()
	{	
		Sequence seq = DOTween.Sequence();
		_ViewContents = false;
		
		_Shelf.SetActive(true);
		Vector3 toPos = _Shelf.transform.position;
		toPos.y = HIGHEST_HEIGHT; 
		seq.Append(_Shelf.transform.DOMove(toPos, 1.5f) );
		
		seq.Play();
	}
	
	void FixedUpdate () {
		UpdateShelfPos();
		
		if(InputManager.I.GetAnyTapDown())
		{
			GameObject tappedObj = InputManager.I.GetTappedGameObject();
			if(tappedObj != null)
			{				
				Book bookInfo = tappedObj.GetComponent<Book>();
				if(bookInfo != null && _ViewContents == false)
				{
					_ViewContents = true;
					GameObject selectedBook = Instantiate(tappedObj, tappedObj.transform.position, tappedObj.transform.rotation) as GameObject;
					_Shelf.SetActive(false);
					StageCreater.I.Book = selectedBook;
					SelectedChapter = bookInfo.chapter;
					SelectedBookID = bookInfo.bookID;
					SelectedStageIdx = 0;
					Sequence sequence = DOTween.Sequence();
					//はじめは本を開く処理もする
						
					sequence.Append( selectedBook.transform.DOMove(BOOK_POS, ANIMATION_TIME) );
					sequence.Join( selectedBook.transform.DORotate((StageCreater.I.START_ANGLE-90)*Vector3.up, ANIMATION_TIME) );
					sequence.Join( selectedBook.transform.DOScale(BOOK_SCALE, ANIMATION_TIME) );
					sequence.OnComplete( () => {
						StateManager.I.GoState(State.INGAME);
					});
					sequence.Play();
				}
			}
		}
	}
	
	void UpdateShelfPos()
	{
		Vector2 swipeDir = InputManager.I.GetMoveDelta(0);
		Vector3 shelfPos = _Shelf.transform.position;
		float shelfY = Mathf.Clamp(shelfPos.y + swipeDir.y, HIGHEST_HEIGHT, LOWEST_HEIGHT);
		shelfPos.y = shelfY;
		_Shelf.transform.position = shelfPos;
	}	
}
