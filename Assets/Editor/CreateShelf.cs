using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateShelf : EditorWindow {

	[MenuItem ("Window/Create Shelf")]
	public static void  Open () {
        CreateStageSelectShelf();
    }
	
	private static void CreateStageSelectShelf()
	{
		ShelfCreater sc = GameObject.Find("[TMP] ShelfCreater").GetComponent<ShelfCreater>();
		sc.Create();
	}
}
