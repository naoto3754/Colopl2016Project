using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectionManager : Singleton<CollectionManager> 
{
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
		set { _CollectionList [index] = value; }
	}
	public State this[int chap, int book, int id]
	{
		get { return _CollectionList [StageManager.CalcStageListIndex(chap, book, id)]; }
		set { _CollectionList [StageManager.CalcStageListIndex(chap, book, id)] = value; }
	}
	/// <summary>
	/// 初期化処理
	/// </summary>
	public override void OnInitialize ()
	{
		base.OnInitialize ();
		_CollectionList = new State[StageManager.I.StageCount];
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
		//読み込み
		List<GameObject> loadList = PlayerPrefsUtility.LoadList<GameObject> ("ListSaveKey");

		var collectionExist = loadList.Find (x => x.Equals (State.COLLECTED));
		if (collectionExist) {

			for (int i = 0; i < _CollectionObjects.Count; i++) {
				var obj = loadList [i];
				obj.SetActive (this [i] == State.COLLECTED);
			}
		} 
		else {
			for (int i = 0; i < _CollectionObjects.Count; i++) {
				var obj = _CollectionObjects [i];
				obj.SetActive (this [i] == State.COLLECTED);
			}
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
		//保存
		PlayerPrefsUtility.SaveList<GameObject> ("ListSaveKey", _CollectionObjects);
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
