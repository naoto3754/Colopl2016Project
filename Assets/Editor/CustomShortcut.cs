using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using TMPro;

public class CustomShotcut : EditorWindow
{
	[MenuItem("Custom/Delete Save Data")]
	static void DeleteSaveData()
	{
		PlayerPrefs.DeleteAll ();
	}

	[MenuItem("Custom/ActiveToggle %e")]
	static void ApplyAndActiveToggle()
	{
		var obj = Selection.objects[0] as GameObject;
		if(obj != null)
		{
			CommonExecuteMenuItem("GameObject/Apply Changes To Prefab");
			var root = FindRoot(obj.transform);
			root.SetActive(!root.activeSelf);
		}
	}

	static void CommonExecuteMenuItem(string iStr)
	{
		EditorApplication.ExecuteMenuItem(iStr);
	}

	static GameObject FindRoot(Transform trans)
	{
		if(trans.parent == null)
			return trans.gameObject;
		else
			return FindRoot(trans.parent);
	}

	[MenuItem("Custom/Select All TextMeshPro %t")]
	static void SelectAllTextMeshPro()
	{
		var tmproObjs = GameObject.FindObjectsOfType<CollectionEvent> ().Select (x => x.gameObject);
		Selection.objects = tmproObjs.ToArray ();
	}

	[MenuItem("Custom/Select All Book Anchor %l")]
	static void SelectAllBookAnchor()
	{
		var books = GameObject.FindObjectsOfType<Book> ();
		List<GameObject> objs = new List<GameObject> ();
		foreach (var book in books) {
			objs.Add (book.gameObject);

//			objs.Add (book.anchorL.gameObject);
//			objs.Add (book.anchorR.gameObject);
		}
		Selection.objects = objs.ToArray ();
	}
}
