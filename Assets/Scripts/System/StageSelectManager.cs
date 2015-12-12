using UnityEngine;
using System.Collections;
using DG.Tweening;

/*
 * ステージセレクト画面時の挙動を制御
 */
public class StageSelectManager : Singleton<StageSelectManager> 
{
	private readonly float ANIMATION_TIME = 2f;
	private readonly float ZOOM_TIME = 0.5f;
	private readonly float LEAN_TIME = 0.2f;
	private readonly float HIGHEST_HEIGHT = -98f;
	private readonly float LOWEST_HEIGHT = -24.5f;
	private readonly Vector3 BOOK_POS = new Vector3(42.6f, -0.8f, -57.4f);
	private readonly Vector3 STAGE_BOOK_SCALE = new Vector3(22f, 36f, 22f);

	private GameObject _SelectedBook;
	private Vector3 _DefaultCameraPos;
	private float _DefaultCameraScale;

	private State _CurrentState;
	private bool _IsZooming = false;
	private bool _FinishTitle = false;
	private GameObject _PrevSelectedObj;
	
	private Sequence _Sequence;
	
	[SerializeField]
	private GameObject _Shelf;
	
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
	
	public override void OnInitialize()
	{
		_DefaultCameraPos = Camera.main.transform.position;
		_DefaultCameraScale = Camera.main.orthographicSize;
		_Sequence = DOTween.Sequence();	
	}
	
	public void InitFromTitle()
	{	
		_CurrentState = State.FROM_TITLE;
		_Sequence = DOTween.Sequence();		

		_Sequence.Append( _Shelf.transform.DOMoveY(HIGHEST_HEIGHT, 10f).SetEase(Ease.OutCubic) );
		
		_Sequence.OnComplete(() => { 
			_FinishTitle = true;
			_CurrentState = State.SELECT;
		});
		_Sequence.Play();
	}

	public void InitFromInGame()
	{	
		_CurrentState = State.FROM_INGAME;
		_Sequence = DOTween.Sequence();
		Vector3 moveDir = SelectedChapter%2==0 ? Vector3.left : Vector3.back;

		var book = StageManager.I.Book;
		_Sequence.Append( book.transform.DOMove(_SelectedBook.transform.position+8f*moveDir, ANIMATION_TIME).SetDelay(0.5f)
			.OnStart(() => {
				FadeManager.I.ShelfFadeIn (2f);
			}));
		_Sequence.Join( book.transform.DORotate(_SelectedBook.transform.eulerAngles, ANIMATION_TIME) );
		_Sequence.Join( book.transform.DOScale(_SelectedBook.transform.lossyScale, ANIMATION_TIME) );
		_Sequence.Append( book.transform.DOMove(_SelectedBook.transform.position, 0.5f)
			.OnComplete(() => {
				_CurrentState = State.SELECT;
				_SelectedBook.SetActive (true);
				Destroy(book);
			}) );
		_Sequence.Play();
	}
	
	void Update () {
		switch (_CurrentState) {
		case State.FROM_TITLE:
			UpdateAtScroll ();
			break;
		case State.SELECT:
			UpdateAtSelect ();
			break;
		}
	}

	void UpdateAtScroll()
	{
		if(InputManager.I.GetAnyTapDown())
		{
			_Sequence.timeScale *= 5f;
		}
	}

	void UpdateAtSelect()
	{
		if(_FinishTitle == false)
			return;

		UpdateShelfPos();

		if(InputManager.I.GetAnyTapDown())
		{
			GameObject tappedObj = InputManager.I.GetTappedGameObject();
			if(tappedObj != null && tappedObj.GetComponent<Book>() != null)
			{	
				Book bookInfo = tappedObj.GetComponent<Book>();
				if(_IsZooming == false)
				{
					LeanBook(tappedObj);
					_PrevSelectedObj = tappedObj;
					ZoomIn();
				}
				else if(_PrevSelectedObj != null)
				{
					Book prevBookInfo = _PrevSelectedObj.GetComponent<Book>();
					if(prevBookInfo.chapter == bookInfo.chapter && prevBookInfo.bookID == bookInfo.bookID)
					{
						GoIngame(tappedObj);
					}
					else
					{
						ResetBook(_PrevSelectedObj);
						LeanBook(tappedObj);
						_PrevSelectedObj = tappedObj;
						ZoomIn();
					}
				}				
			}
			else if(_IsZooming)
			{
				ResetBook(_PrevSelectedObj);
				ZoomOut();
			}
		}
	}
	
	void UpdateShelfPos()
	{
		if(_IsZooming)
			return;
		
		Vector2 swipeDir = InputManager.I.GetMoveDelta(0);
		Vector3 shelfPos = _Shelf.transform.position;
		float shelfY = Mathf.Clamp(shelfPos.y + swipeDir.y, HIGHEST_HEIGHT, LOWEST_HEIGHT);
		shelfPos.y = shelfY;
		_Shelf.transform.position = shelfPos;
	}
	
	void ZoomIn()
	{			
		_IsZooming = true;
		_Sequence = DOTween.Sequence();
		
		int chapter = _PrevSelectedObj.GetComponent<Book>().chapter;
		_Sequence.Append( Camera.main.DOOrthographicSize(18f, ZOOM_TIME) );
		
		Vector3 pos = _DefaultCameraPos;
		pos.y = Camera.main.transform.position.y;
		pos += chapter%2 == 0 ? new Vector3(-2f, 0f, 2f) : new Vector3(2f, 0f, -2f);
		
		_Sequence.Join( Camera.main.transform.DOMove(pos, ZOOM_TIME) );
		_Sequence.Join( _Shelf.transform.DOMoveY(-83.3f+17f*(chapter-1), ZOOM_TIME) );
		
		_Sequence.Play();
	}
	
	void ZoomOut()
	{
		_Sequence = DOTween.Sequence();
		
		_Sequence.Append( Camera.main.DOOrthographicSize( _DefaultCameraScale, ZOOM_TIME) );
		Vector3 pos = _DefaultCameraPos;
		//  pos.y = Camera.main.transform.position.y;
		_Sequence.Join( Camera.main.transform.DOMove(pos, ZOOM_TIME) );
		if(_Shelf.transform.position.y < HIGHEST_HEIGHT)
			_Sequence.Join( _Shelf.transform.DOMoveY(HIGHEST_HEIGHT, ZOOM_TIME) );
		if(_Shelf.transform.position.y > LOWEST_HEIGHT)
			_Sequence.Join( _Shelf.transform.DOMoveY(LOWEST_HEIGHT, ZOOM_TIME) );
		_Sequence.OnComplete(() => { _IsZooming = false; });
		_Sequence.Play();
	}
	
	void LeanBook(GameObject book)
	{
		_Sequence = DOTween.Sequence();
		
		Vector3 angle = book.transform.parent.eulerAngles;
		angle.z = -15f;
		_Sequence.Append( book.transform.parent.DORotate(angle, LEAN_TIME) );
		_Sequence.Play();
	}
	
	void ResetBook(GameObject book)
	{
		_Sequence = DOTween.Sequence();
		
		Vector3 angle = book.transform.parent.eulerAngles;
		angle.z = 0f;
		_Sequence.Append( book.transform.parent.DORotate(angle, LEAN_TIME) );
		_Sequence.Play();
	}
	
	void GoIngame(GameObject tappedObj)
	{
		_CurrentState = State.TO_INGAME;
		_Sequence.Kill();

		_IsZooming = false;
		Camera.main.DOOrthographicSize(_DefaultCameraScale, ZOOM_TIME);
		Camera.main.transform.DOMove(_DefaultCameraPos, ZOOM_TIME);
		ResetBook (tappedObj);

		_SelectedBook = tappedObj;
		GameObject selectedBook = Instantiate(tappedObj, tappedObj.transform.position, tappedObj.transform.rotation) as GameObject;
		StageManager.I.Book = selectedBook;
		tappedObj.SetActive (false);
		SelectedChapter = tappedObj.GetComponent<Book>().chapter;
		SelectedBookID = tappedObj.GetComponent<Book>().bookID;
		SelectedStageIdx = 0;
		int stgIndex = StageManager.CalcStageListIndex (SelectedChapter, SelectedBookID, SelectedStageIdx);
		
		_Sequence = DOTween.Sequence();
		Vector3 moveDir = SelectedChapter%2==0 ? Vector3.left : Vector3.back;
		_Sequence.Append (selectedBook.transform.DOMove (selectedBook.transform.position + 8f * moveDir, 0.5f)
			.OnComplete (() => {
				//TODO:色決める
				FadeManager.I.SetShelfColor (StageManager.I.Stages[stgIndex].GetComponent<StageInfomation>().BackgroundColor);
				FadeManager.I.ShelfFadeOut (2f);
			}));
		_Sequence.Append( selectedBook.transform.DOMove(BOOK_POS, ANIMATION_TIME) );
		_Sequence.Join( selectedBook.transform.DORotate((StageAnimator.I.START_ANGLE-90)*Vector3.up, ANIMATION_TIME) );
		_Sequence.Join( selectedBook.transform.DOScale(STAGE_BOOK_SCALE, ANIMATION_TIME) );
		_Sequence.OnComplete( () => {
			StateManager.I.GoState(GameState.INGAME);
		});
		_Sequence.Play();
	}

	private enum State{
		FROM_TITLE,
		SELECT,
		TO_INGAME,
		FROM_INGAME,
	}
}
