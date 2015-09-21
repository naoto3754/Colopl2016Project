using UnityEngine;
using System.Collections;

/*
 * タイトル画面時の挙動を制御
 */
public class TitleManager : Singlton<TitleManager> {

	public void Init() {}
	
	void FixedUpdate()
	{
		if(InputManager.I.GetAnyTapDown())
		{
			StateManager.I.GoState(StateManager.State.STAGE_SELECT);
		}
	}
}
