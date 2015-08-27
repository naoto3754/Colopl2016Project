using UnityEngine;
using UnityEditor; 
using System.Collections;
using System.IO;

public class SnapshotStage : EditorWindow
 {
	 
	[MenuItem("Window/Snapshot Stage")]
    static void Open()
    { 
        EditorWindow.GetWindow<SnapshotStage>( "Stapshot Stage" );
    }
	
	void OnGUI()
    {
		if(GUILayout.Button("take shapshot"))
		{
			SaveStageImage();
		}
    }
	
	private void SaveStageImage()
	{
		RenderTexture rt = GameObject.Find("SnapshotCamera").GetComponent<Camera>().targetTexture;
		Texture2D texture = new Texture2D( rt.width, rt.height, TextureFormat.ARGB32, false, false );
		
		Camera.main.Render();
		RenderTexture.active = rt;
		
		texture.ReadPixels( new Rect( 0, 0, rt.width, rt.height), 0, 0 );
		texture.Apply();
		
		byte [] pngData = texture.EncodeToJPG();

		string filePath = EditorUtility.SaveFilePanel("Save Texture", "", texture.name + ".jpg", "jpg");

		if (filePath.Length > 0) {
  			File.WriteAllBytes(filePath, pngData);
		}
		return;
	}
}
