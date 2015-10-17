using UnityEngine;
using System.Collections;

/*
 * タイトル画面時の挙動を制御
 */
public class TitleManager : Singlton<TitleManager> {

	//  private readonly float SHELF_HEIGHT = -102.5f;
	
	void Update()
	{
		if(InputManager.I.GetAnyTapDown())
		{
			StateManager.I.GoState(State.STAGE_SELECT);
		}
	}
}
