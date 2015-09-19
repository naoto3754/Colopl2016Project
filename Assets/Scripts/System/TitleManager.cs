using UnityEngine;
using System.Collections;

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
