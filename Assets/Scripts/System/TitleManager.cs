using UnityEngine;
using System.Collections;

/*
 * タイトル画面時の挙動を制御
 */
public class TitleManager : Singleton<TitleManager> {

	void Start()
	{
		StateManager.I.GoState(GameState.STAGE_SELECT);
	}
	
//	void Update()
//	{
//		if(InputManager.I.GetAnyTapDown())
//		{
//			StateManager.I.GoState(GameState.STAGE_SELECT);
//		}
//	}
}
