using UnityEngine;
using System;
using System.Collections;

/*
 * ゲームの進行を管理する
 */
public class StateManager : Singleton<StateManager> {

	[SerializeField]
	private GameObject _Title;
	[SerializeField]
	private GameObject _StageSelect;
	[SerializeField]
	private GameObject _InGame;
	
	public GameObject BookForStageCreater
	{
		get;
		set;
	} 
	
	private State _CurrentState;
	public State CurrentState
	{
		get { return _CurrentState; }
	}
	
	public override void OnInitialize()
	{
		_CurrentState = State.TITLE;
		SwitchActiveManager(_CurrentState, State.TITLE);
	}
	
	public void GoState(State state)
	{
		State prev = _CurrentState;
		_CurrentState = state;
		SwitchActiveManager(_CurrentState, prev);
	}
	
	private void SwitchActiveManager(State current, State previous)
	{
		foreach(Transform child in _Title.transform)
			child.gameObject.SetActive(false);
		foreach(Transform child in _StageSelect.transform)
			child.gameObject.SetActive(false);
		foreach(Transform child in _InGame.transform)
			child.gameObject.SetActive(false);
		switch(current)
		{
		case State.TITLE:
			foreach(Transform child in _Title.transform)
				child.gameObject.SetActive(true);
			break;
		case State.STAGE_SELECT:
			foreach (Transform child in _StageSelect.transform)
				child.gameObject.SetActive (true);
			if(previous == State.TITLE)
				StageSelectManager.I.InitFromTitle ();
			if(previous == State.INGAME)
				StageSelectManager.I.InitFromInGame ();
			CollectionManager.I.ActivateSprite ();
			break;
		case State.INGAME:
			foreach(Transform child in _InGame.transform)
				child.gameObject.SetActive(true);
			StageManager.I.InstantiateStage(StageSelectManager.I.SelectedChapter, StageSelectManager.I.SelectedBookID, StageSelectManager.I.SelectedStageIdx);
			break;
		}
	}
}

public enum State
{
	TITLE = 0,
	STAGE_SELECT,
	INGAME,
}