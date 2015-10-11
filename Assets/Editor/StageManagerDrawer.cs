using UnityEngine;
using UnityEditor;

	
[CustomEditor(typeof(StageManager))]
public class StageManagerInspector : Editor 
{
	bool[] bookfoldout = new bool[15]; 
	
	public override void OnInspectorGUI () 
	{
		serializedObject.Update();
		//BGMList表示
		SerializedProperty stageList = serializedObject.FindProperty("_Stages");
		EditorGUILayout.PropertyField(stageList);
		if(stageList.isExpanded){
			EditorGUILayout.PropertyField(stageList.FindPropertyRelative("Array.size"));
			EditorGUI.indentLevel += 1;
			for (int chap = 0; chap < stageList.arraySize/9; chap++) {
				for (int book = 0; book < 3; book++) {
					for (int idx = 0; idx < 3; idx++) {						
						if(book == 0 && idx == 0){
							EditorGUILayout.Space();
							EditorGUILayout.LabelField("---------- Chapter "+(chap+1).ToString()+" ----------");							
						}
						if(idx == 0){
							bookfoldout[chap*3+book] = EditorGUILayout.Foldout(bookfoldout[chap*3+book] , "Book "+book.ToString());
							EditorGUI.indentLevel += 1;
						}
						if(bookfoldout[chap*3+book])
							EditorGUILayout.PropertyField(stageList.GetArrayElementAtIndex(chap*9+book*3+idx), GUIContent.none);
						
						if(idx == 2)
							EditorGUI.indentLevel -= 1;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		
		serializedObject.ApplyModifiedProperties();			
	}
}

