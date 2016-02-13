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
	
	private GameState _CurrentState;
	public GameState CurrentState
	{
		get { return _CurrentState; }
	}
	
	public override void OnInitialize()
	{
		_CurrentState = GameState.TITLE;
		SwitchActiveManager(_CurrentState, GameState.TITLE);
	}
	
	public void GoState(GameState state)
	{
		GameState prev = _CurrentState;
		_CurrentState = state;
		SwitchActiveManager(_CurrentState, prev);
	}
	
	private void SwitchActiveManager(GameState current, GameState previous)
	{
		foreach(Transform child in _Title.transform)
			child.gameObject.SetActive(false);
		foreach(Transform child in _StageSelect.transform)
			child.gameObject.SetActive(false);
		foreach(Transform child in _InGame.transform)
			child.gameObject.SetActive(false);
		switch(current)
		{
		case GameState.TITLE:
			foreach(Transform child in _Title.transform)
				child.gameObject.SetActive(true);
			break;
		case GameState.STAGE_SELECT:
			foreach (Transform child in _StageSelect.transform)
				child.gameObject.SetActive (true);
			if(previous == GameState.TITLE)
				StageSelectManager.I.InitFromTitle ();
			if(previous == GameState.INGAME)
				StageSelectManager.I.InitFromInGame ();
			CollectionManager.I.ActivateSprite ();
			AudioManager.I.StopBGM (true);
			break;
		case GameState.INGAME:
			switch (StageSelectManager.I.SelectedChapter) {
			case 1:
				AudioManager.I.PlayBGM (BGMConfig.Tag.CHAPTER1);
				break;
			case 2:
				AudioManager.I.PlayBGM (BGMConfig.Tag.CHAPTER2);
				break;
			default:
				AudioManager.I.PlayBGM (BGMConfig.Tag.CHAPTER1);
				break;
			}
			foreach(Transform child in _InGame.transform)
				child.gameObject.SetActive(true);
			StageManager.I.InstantiateStage(StageSelectManager.I.SelectedChapter, StageSelectManager.I.SelectedBookID, StageSelectManager.I.SelectedStageIdx);
			break;
		}
	}
}

public enum GameState
{
	TITLE = 0,
	STAGE_SELECT,
	INGAME,
}