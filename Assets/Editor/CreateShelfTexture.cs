using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class CreateShelfTexture : EditorWindow {

	[MenuItem ("Window/Create Shelf Texture (png)")]
	public static void  Open () {
        CreateSnapShot();
    }
	
	private static void CreateSnapShot()
	{
		Camera camera = GameObject.Find("ShelfDesignCamera").GetComponent<Camera>();
		RenderTexture rt = camera.targetTexture;
		
		Texture2D texture = new Texture2D( rt.width, rt.height, TextureFormat.ARGB32, false, false );
		
		camera.Render();
		
		RenderTexture.active = rt;
		texture.ReadPixels( new Rect( 0, 0, rt.width, rt.height), 0, 0 );
		texture.Apply();
		
		byte [] pngData = texture.EncodeToPNG();

		string filePath = EditorUtility.SaveFilePanel("Save Texture", "", "StageTexture", "png");

		if (filePath.Length > 0) {
  			File.WriteAllBytes(filePath, pngData);
		}
	}
}
