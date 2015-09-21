using UnityEngine;
using System;
using System.Collections;

/*
 * ゲームの進行を管理する
 */
public class StateManager : Singlton<StateManager> {

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
		SwitchActiveManager(_CurrentState);
	}
	
	public void GoNextState()
	{
		_CurrentState = (State)Mathf.Clamp((int)_CurrentState+1, 0, Enum.GetValues(typeof(State)).Length);
		SwitchActiveManager(_CurrentState);
	}
	
	public void GoPrevioiusState()
	{
		_CurrentState = (State)Mathf.Clamp((int)_CurrentState-1, 0, Enum.GetValues(typeof(State)).Length);
		SwitchActiveManager(_CurrentState);
	}
	
	public void GoState(State state)
	{
		_CurrentState = state;
		SwitchActiveManager(_CurrentState);
	}
	
	private void SwitchActiveManager(State current)
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
			foreach(Transform child in _StageSelect.transform)
				child.gameObject.SetActive(true);
			StageSelectManager.I.Init();
			break;
		case State.INGAME:
			foreach(Transform child in _InGame.transform)
				child.gameObject.SetActive(true);
			StageCreater.I.Book = BookForStageCreater;
			StageManager.I.InstantiateStage(0);
			break;
		}
	}

	public enum State
	{
		TITLE = 0,
		STAGE_SELECT,
		INGAME,
	}
}
