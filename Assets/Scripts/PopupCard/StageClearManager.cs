using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageClearManager : Singleton<StageClearManager> 
{
	[SerializeField]
	private GameObject _BookObjectRoot;
	private List<Book> _BookObjects;

	[SerializeField]
	Sprite _LockIcon;
	[SerializeField]
	Sprite _ClearIcon;

	private State[] _StageClearList;
	public State[] ClearList
	{
		get { return _StageClearList; }
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	public override void OnInitialize ()
	{
		base.OnInitialize ();
		//セーブある場合
		if (PlayerPrefs.HasKey ("ListSaveKey")) {
			var loadList = PlayerPrefsUtility.LoadList<State> ("ListSaveKey");
			_StageClearList = loadList.ToArray<State> ();
		}
		//セーブない場合
		else {
			_StageClearList = new State[StageManager.I.StageCount];
		}

		_BookObjects = new List<Book> ();
		foreach (var book in _BookObjectRoot.GetComponentsInChildren<Book>()) {
			_BookObjects.Add (book);
		}

		SetInitParam ();
		SetBooksSpine ();
	}
	/// <summary>
	/// ステージをクリアした時の処理
	/// </summary>
	public void ClearStage(int index)
	{
		_StageClearList [index] = State.CLEARED;
		int[] info = StageManager.CalcStageIndexInfo (index);
		if (info [2] == 2 && info[1] != 2) {
			_StageClearList [index + 1] = State.PLAYABLE;
			_StageClearList [index + 2] = State.PLAYABLE;
			_StageClearList [index + 3] = State.PLAYABLE;
		}
		SetBooksSpine ();
		Save ();
	}
	/// <summary>
	/// ステージのクリア状況セーブする
	/// </summary>
	private void Save()
	{
		var stageSaveList = _StageClearList.ToList<State> ();
		PlayerPrefsUtility.SaveList<State> ("ListSaveKey", stageSaveList);
	}
	/// <summary>
	/// データ初期化
	/// </summary>
	public void Clear()
	{
		SetInitParam ();
		Save();
	}
	/// <summary>
	/// 初期パラメータをセットする
	/// </summary>
	private void SetInitParam()
	{
		for (int i = 0; i < _StageClearList.Length; i++) {
			int[] info = StageManager.CalcStageIndexInfo (i);
			if(info[0] >= 3)
				_StageClearList [i] = State.UNPLAYABLE;
			else if(info[1] == 0)
				_StageClearList [i] = State.PLAYABLE;
			else
				_StageClearList [i] = State.UNPLAYABLE;
		}
		foreach (var book in _BookObjects) {
			book.bookmark.GetComponent<Renderer> ().enabled = false;
		}
	}
	/// <summary>
	/// Sets the bookmark active.
	/// </summary>
	public void SetBookmarkActive(bool active, int index)
	{
		_BookObjects[index].bookmark.GetComponent<Renderer> ().enabled = active;
	}

	/// <summary>
	/// クリア状況に合わせて本の背表紙を設定する
	/// </summary>
	private void SetBooksSpine()
	{
		for (int i = 0; i < _BookObjects.Count; i ++) {
			State state0 = _StageClearList [i * 3];
			State state2 = _StageClearList [i * 3 + 2];
			bool unplayable = state0 == State.UNPLAYABLE;
			bool displayIcon = state0 == State.UNPLAYABLE || state2 == State.CLEARED;
			unplayable = false;
			displayIcon = false;
			Sprite sprite = unplayable ? _LockIcon : _ClearIcon;

			SetIDText (i, unplayable);
			SetIcon (i, sprite, displayIcon);
			SetCountText (i, !displayIcon);
		}
	}
	/// <summary>
	/// アイコンを設定する
	/// </summary>
	private void SetIcon(int index, Sprite sprite, bool active)
	{
		Book book = _BookObjects[index];
		if (book.icon == null)
			return;

		book.icon.enabled = active;
		if (active == false)
			return;

		book.icon.sprite = sprite;
	}
	/// <summary>
	/// 数字のテキストを設定する
	/// </summary>
	private void SetIDText(int index, bool unplayable)
	{
		Book book = _BookObjects[index];
		if (book.text_id == null)
			return;

		string str = unplayable ? "-" : (book.bookID+1).ToString();
		book.text_id.text = str;
	}
	/// <summary>
	/// クリア数のテキストを設定する
	/// </summary>
	private void SetCountText(int index, bool active)
	{
		Book book = _BookObjects[index];
		if (book.text_count == null)
			return;

		book.text_count.GetComponent<Renderer>().enabled = active;
		if (active == false)
			return;

		int clearCnt = 0;
		for (int n = 0; n < 3; n++) {
			if (_StageClearList [index * 3 + n] == State.CLEARED) {
				clearCnt++;
			}
		}
		book.text_count.text = string.Format ("{0}/3", clearCnt);
	}

	public enum State
	{
		UNPLAYABLE,
		PLAYABLE,
		CLEARED,
	}
}
