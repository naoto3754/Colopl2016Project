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
			EditorGUILayout.Separator();
			for (int i = 0; i < BGMList.arraySize; i++) {
				EditorGUILayout.PropertyField(BGMList.GetArrayElementAtIndex(i).FindPropertyRelative("tag"), GUIContent.none);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(BGMList.GetArrayElementAtIndex(i).FindPropertyRelative("clip"), GUIContent.none);
				EditorGUILayout.PropertyField(BGMList.GetArrayElementAtIndex(i).FindPropertyRelative("clip_loop"), GUIContent.none);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.PropertyField(BGMList.GetArrayElementAtIndex(i).FindPropertyRelative("volume"), GUIContent.none);
				
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
			EditorGUILayout.Separator();
			for (int i = 0; i < SEList.arraySize; i++) {				
				EditorGUILayout.PropertyField(SEList.GetArrayElementAtIndex(i).FindPropertyRelative("tag"), GUIContent.none);
				EditorGUILayout.PropertyField(SEList.GetArrayElementAtIndex(i).FindPropertyRelative("clip"), GUIContent.none);
				EditorGUILayout.PropertyField(SEList.GetArrayElementAtIndex(i).FindPropertyRelative("volume"), GUIContent.none);
				
				EditorGUILayout.Separator();
			}
			EditorGUI.indentLevel -= 1;
		}
		
		serializedObject.ApplyModifiedProperties();			
	}
}

