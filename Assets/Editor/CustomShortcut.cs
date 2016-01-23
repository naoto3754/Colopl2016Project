using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using TMPro;

public class CustomShotcut : EditorWindow
{
	[MenuItem("KeyRemap/ActiveToggle %e")]
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


	static GameObject FindRoot(Transform trans)
	{
		if(trans.parent == null)
			return trans.gameObject;
		else
			return FindRoot(trans.parent);
	}

	[MenuItem("KeyRemap/Select All TextMeshPro %t")]
	static void SelectAllTextMeshPro()
	{
		var tmproObjs = GameObject.FindObjectsOfType<TextMeshPro> ().Select(x=>x.gameObject);
		Selection.objects = tmproObjs.ToArray ();
	}

    static void CommonExecuteMenuItem(string iStr)
    {
        EditorApplication.ExecuteMenuItem(iStr);
	}
}
