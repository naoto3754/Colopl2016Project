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
	//  private readonly Vector3 CAMERA_POSITION = new Vector3(-22.2f, 58.2f, -122.2f);
	//  private readonly Vector3 START_BOOK_OFFSET = new Vector3(0f, 15f, 0f);
	//  private readonly Vector3 DEFAULT_LEFTANCHOR_LOCALPOSITION = new Vector3(0.5f, 0f, 0f);
	//  private readonly Vector3 DEFAULT_BOOK_SCALE = new Vector3(10.5f, 17.85f, 18.64f);

	[SerializeField]
	private GameObject _Shelf;
	
	private bool _ViewContents;
	
	public int SelectedChapter
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
					SelectedChapter = 0;
					SelectedStageIdx = 1;
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
