using UnityEngine;
using System.Collections;
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
			GameObject tappedObj = InputManager.I.GetTappedGameObject();
			if(tappedObj != null && tappedObj.GetComponent<Book>() != null)
			{
				Book bookInfo = tappedObj.GetComponent<Book>();
				Debug.Log("Chapter = "+bookInfo.chapter);
				Debug.Log("Book ID = "+bookInfo.bookID);
				Destroy(_Shelf);
				StateManager.I.BookForStageCreater = new GameObject();
				GameObject child = new GameObject();
				child.transform.SetParent(StateManager.I.BookForStageCreater.transform);
				StageCreater.I.Book = StateManager.I.BookForStageCreater;
				SelectedStageIdx = 1;
				StateManager.I.GoState(State.INGAME);
			}
		}

	}	
}
