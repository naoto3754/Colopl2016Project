﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageClearManager : Singleton<StageClearManager> 
{
	[SerializeField]
	private GameObject _BookObjectRoot;
	private List<Book> _BookObjects;
	[SerializeField]
	private Color[] _OriginalColors;

	private State[] _StageClearList;
	public State[] ClearList
	{
		get { return _StageClearList; }
	}

	public override void OnInitialize ()
	{
		base.OnInitialize ();
		_StageClearList = new State[StageManager.I.StageCount];
		_BookObjects = new List<Book> ();
		foreach (var book in _BookObjectRoot.GetComponentsInChildren<Book>()) {
			_BookObjects.Add (book);
		}

		SetInitParam ();
		SetBooksColor ();
		SetBooksText ();
	}

	public void ClearStage(int index)
	{
		_StageClearList [index] = State.CLEARED;
		int[] info = StageManager.CalcStageIndexInfo (index);
		if (info [2] == 2) {
			_StageClearList [index + 1] = State.PLAYABLE;
			_StageClearList [index + 2] = State.PLAYABLE;
			_StageClearList [index + 3] = State.PLAYABLE;
		}
		SetBooksColor ();
		SetBooksText ();
	}

	private void Save()
	{
		
	}

	public void Clear()
	{
		SetInitParam ();
		Save();
	}

	private void SetInitParam()
	{
		for (int i = 0; i < _StageClearList.Length; i++) {
			int[] info = StageManager.CalcStageIndexInfo (i);
			if(info[1] == 0)
				_StageClearList [i] = State.PLAYABLE;
			else
				_StageClearList [i] = State.UNPLAYABLE;
		}
	}

	private void SetBooksColor()
	{
		for (int i = 0; i < _BookObjects.Count; i ++) {
			SetBookColor (_BookObjects [i].anchorL, i);
			SetBookColor (_BookObjects [i].anchorR, i);
		}
	}

	private void SetBooksText()
	{
		for (int i = 0; i < _BookObjects.Count; i ++) {
			if (_BookObjects [i].text == null)
				continue;
			
			int clearCnt = 0;
			for (int n = 0; n < 3; n++) {
				if (_StageClearList [i * 3 + n] == State.CLEARED) {
					clearCnt++;
				}
			}
			_BookObjects [i].text.text = string.Format ("{0}/3", clearCnt);
		}
	}

	private void SetBookColor(Transform target, int index)
	{
		bool unplayable = _StageClearList [index * 3] == State.UNPLAYABLE;
		bool isCleared = _StageClearList [index * 3] == State.CLEARED
			&& _StageClearList [index * 3 + 1] == State.CLEARED
			&& _StageClearList [index * 3 + 2] == State.CLEARED;
		Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
		foreach (var renderer in renderers) {
			foreach(var material in renderer.materials){
				Color color = material.name == "BookPaper (Instance)" ? Color.white : _OriginalColors [index];
				if (unplayable) {
					material.color = color*0.5f;
				} else if (isCleared) {
					material.color = color*1.1f;
				} else {
					material.color = color;
				}
			}
		}
	}

	public enum State
	{
		UNPLAYABLE,
		PLAYABLE,
		CLEARED,
	}
}
