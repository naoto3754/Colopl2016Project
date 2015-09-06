using UnityEngine;
using UnityEditor; 
using System.Collections;
using System.IO;

public class PreviewStage : EditorWindow
 {
	 
	[MenuItem("Custom/Preview Stage")]
    static void Open()
    { 
		StageCreater.I.CreateStage(40f,0f);
    }
	
}
