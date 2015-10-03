using UnityEngine;
using UnityEditor;

	
[CustomEditor(typeof(AudioManager))]
public class ListTesterInspector : Editor 
{
	public override void OnInspectorGUI () 
	{
		serializedObject.Update();
		//BGMList表示
		SerializedProperty BGMList = serializedObject.FindProperty("BGMList");
		EditorGUILayout.PropertyField(BGMList);
		if(BGMList.isExpanded){
			EditorGUI.indentLevel += 1;
			EditorGUILayout.PropertyField(BGMList.FindPropertyRelative("Array.size"));			
			for (int i = 0; i < BGMList.arraySize; i++) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(BGMList.GetArrayElementAtIndex(i).FindPropertyRelative("Title"), GUIContent.none);
				EditorGUILayout.PropertyField(BGMList.GetArrayElementAtIndex(i).FindPropertyRelative("Clip"), GUIContent.none);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(BGMList.GetArrayElementAtIndex(i).FindPropertyRelative("Volume"), GUIContent.none);
				
				EditorGUILayout.Separator();
				
			}
			EditorGUI.indentLevel -= 1;
		}
		//SEList表示
		SerializedProperty SEList = serializedObject.FindProperty("SEList");
		EditorGUILayout.PropertyField(SEList);
		EditorGUI.indentLevel += 1;
		if(SEList.isExpanded){
			EditorGUILayout.PropertyField(SEList.FindPropertyRelative("Array.size"));
			for (int i = 0; i < SEList.arraySize; i++) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(SEList.GetArrayElementAtIndex(i).FindPropertyRelative("Title"), GUIContent.none);
				EditorGUILayout.PropertyField(SEList.GetArrayElementAtIndex(i).FindPropertyRelative("Clip"), GUIContent.none);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(SEList.GetArrayElementAtIndex(i).FindPropertyRelative("Volume"), GUIContent.none);
				
				EditorGUILayout.Separator();
			}
			EditorGUI.indentLevel -= 1;
		}
		
		serializedObject.ApplyModifiedProperties();			
	}
}

