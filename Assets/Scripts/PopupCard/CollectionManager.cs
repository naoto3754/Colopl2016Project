using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectionManager : Singleton<CollectionManager> 
{
	[SerializeField]
	private GameObject CollectionObjectRoot;
	private List<GameObject> _CollectionObjects;

	private bool[] _CollectionList;
	public bool this[int index]
	{
		get { return _CollectionList [index]; }
		set { _CollectionList [index] = value; }
	}
	public bool this[int chap, int book, int id]
	{
		get { return _CollectionList [StageManager.CalcStageListIndex(chap, book, id)]; }
		set { _CollectionList [StageManager.CalcStageListIndex(chap, book, id)] = value; }
	}

	public override void OnInitialize ()
	{
		base.OnInitialize ();
		_CollectionList = new bool[StageManager.I.StageCount];
		_CollectionObjects = new List<GameObject> ();
		foreach (var item in CollectionObjectRoot.GetComponentsInChildren<SpriteRenderer>()) {
			_CollectionObjects.Add (item.gameObject);
		}
		ActivateSprite ();
	}

	public void ActivateSprite()
	{
		for (int i = 0; i < _CollectionObjects.Count; i++) {
			var obj = _CollectionObjects [i];
			obj.SetActive (this[i]);
		}
	}

	public void Collect(int index)
	{
		_CollectionList [index] = true;
		Save ();
	}

	public void Collect(int chap, int book, int id)
	{
		_CollectionList [StageManager.CalcStageListIndex(chap, book, id)] = true;
		Save ();
	}

	private void Save()
	{
		
	}

	public void Clear()
	{
		for (int i = 0; i < _CollectionList.Length; i++) {
			_CollectionList [i] = false;

		}
		Save();
	}
}
