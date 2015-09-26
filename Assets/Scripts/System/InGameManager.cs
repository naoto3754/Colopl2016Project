using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class InGameManager : Singlton<InGameManager> 
{
	public void DisplayDictionary()
	{
		BlurOptimized blur = Camera.main.GetComponent<BlurOptimized>();
		blur.enabled = true;
		StageCreater.I.IsPlayingAnimation = false;
	}
}
