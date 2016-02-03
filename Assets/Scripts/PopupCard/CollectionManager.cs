using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CollectionManager : Singleton<CollectionManager> 
{
	private readonly string SAVE_KEY = "CollectionList";

	[SerializeField]
	private GameObject CollectionObjectRoot;
	private List<GameObject> _CollectionObjects;

	/// <summary>
	/// コレクションしているかのリスト
	/// </summary>
	private State[] _CollectionList;
	public State this[int index]
	{
		get { return _CollectionList [index]; }
	}
	public State this[int chap, int book, int id]
	{
		get { return _CollectionList [StageManager.CalcStageListIndex(chap, book, id)]; }
	}
	/// <summary>
	/// 初期化処理
	/// </summary>
	public override void OnInitialize ()
	{
		base.OnInitialize ();
		//セーブある場合
		if (PlayerPrefs.HasKey (SAVE_KEY)) {
			var loadList = PlayerPrefsUtility.LoadList<State> (SAVE_KEY);
			_CollectionList = loadList.ToArray<State> ();
		}
		//セーブない場合
		else {
			_CollectionList = new State[StageManager.I.StageCount];
		}

		_CollectionObjects = new List<GameObject> ();
		foreach (var item in CollectionObjectRoot.GetComponentsInChildren<SpriteRenderer>()) {
			if(item.name.Contains("Collection"))
			_CollectionObjects.Add (item.gameObject);
		}
		ActivateSprite ();
	}
	/// <summary>
	/// コレクション状況によって本棚のコレクションを表示する
	/// </summary>
	public void ActivateSprite()
	{
		for (int i = 0; i < _CollectionObjects.Count; i++) {
			var obj = _CollectionObjects [i];
			obj.SetActive (this [i] == State.COLLECTED);
		}
	}
	/// <summary>
	/// コレクションしたときに呼ばれる
	/// </summary>
	/// <param name="index">ステージのIndex</param>
	public void Collect(int index)
	{
		_CollectionList [index] = State.COLLECTED;
		Save ();
	}
	/// <summary>
	/// コレクションしたときに呼ばれる
	/// </summary>
	public void Collect(int chap, int book, int id)
	{
		_CollectionList [StageManager.CalcStageListIndex(chap, book, id)] = State.COLLECTED;
		Save ();
	}
	/// <summary>
	/// コレクション状況をセーブする
	/// </summary>
	private void Save()
	{
		var collectionSaveList = _CollectionList.ToList<State> ();
		PlayerPrefsUtility.SaveList<State> (SAVE_KEY, collectionSaveList);
	}
	/// <summary>
	/// データ初期化
	/// </summary>
	public void Clear()
	{
		for (int i = 0; i < _CollectionList.Length; i++) {
			_CollectionList [i] = State.UNCOLLECTED;
		}
		Save();
	}

	public enum State
	{
		UNCOLLECTED,
		COLLECTED,
	}
}
