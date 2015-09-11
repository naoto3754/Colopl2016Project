using UnityEngine;
using UnityEditor; 
using System.Collections;
using System.IO;

public class PreviewStage : EditorWindow
 {
	[MenuItem("Custom/Preview Stage")]
    static void Open()
    { 
		foreach(GameObject stage in Selection.gameObjects)
		{
			if(stage.GetComponent<StageInfomation>() != null)
			{
				StageManager.I.CurrentInfo = stage.GetComponent<StageInfomation>();
				break;
			}	
		}
		StageCreater.I.CreateStage(40f,0f,true);
    }
	
}
