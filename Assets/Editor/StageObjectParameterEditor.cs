﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageObjectParameter))]
[CanEditMultipleObjects]
public class StageObjectParameterEditor : Editor 
{
	SerializedProperty typeProp;
	SerializedProperty lineProp;
	SerializedProperty colorProp;
	SerializedProperty useProp;
	SerializedProperty topProp;
	SerializedProperty upProp;
	SerializedProperty downProp;
	SerializedProperty fallProp;
	SerializedProperty enableProp;
	SerializedProperty heightProp;

	void OnEnable()
	{
		typeProp = serializedObject.FindProperty("Type");
		lineProp = serializedObject.FindProperty("LineType");
		colorProp = serializedObject.FindProperty("Color");
		useProp = serializedObject.FindProperty("UseAsDecoration");
		topProp = serializedObject.FindProperty("TopOfWall");
		upProp = serializedObject.FindProperty("DontThroughUp");
		downProp = serializedObject.FindProperty("DontThroughDown");
		fallProp = serializedObject.FindProperty("CanFallOff");
		enableProp = serializedObject.FindProperty("EnableCase");
		heightProp = serializedObject.FindProperty("HeightWithMaxWidth");
	}

	public override void OnInspectorGUI () 
	{
		serializedObject.Update();
		// 対象となるオブジェクト
		EditorGUILayout.PropertyField(typeProp);

		var param = target as StageObjectParameter;
		SetFlag (param);

		if (showLine)
			EditorGUILayout.PropertyField(lineProp);
		if (showColor)
			EditorGUILayout.PropertyField (colorProp);
		if(showUse)
			EditorGUILayout.PropertyField (useProp);
		if(showTop)
			EditorGUILayout.PropertyField (topProp);
		if(showUp)
			EditorGUILayout.PropertyField (upProp);
		if(showDown)
			EditorGUILayout.PropertyField (downProp);
		if(showFall)
			EditorGUILayout.PropertyField (fallProp);
		if(showCase)
			EditorGUILayout.PropertyField (enableProp);
		if(showUse && useProp.boolValue)
			EditorGUILayout.PropertyField (heightProp);

		serializedObject.ApplyModifiedProperties();		
	}

	/*1*/
	bool showLine = true;
	/*2*/
	bool showColor = true;
	/*3*/
	bool showUse = true;
	/*4*/
	bool showTop = true;
	/*5*/
	bool showUp = true;
	/*6*/
	bool showDown = true;
	/*7*/
	bool showFall = true;
	/*8*/
	bool showCase = true;

	private void SetFlag(StageObjectParameter param)
	{
		switch (param.Type) {
		case StageObjectType.LINE:
			switch (param.LineType) {
			case StageLineType.FOLD:
			case StageLineType.TOP_FOLD:
				AssignFlag (true, false, false, false, false, false, false, false);
				break;
			case StageLineType.GROUND:
				AssignFlag (true, true, true, true, true, true, true, true);
				break;
			case StageLineType.SLOPE:
			case StageLineType.WALL:
				AssignFlag (true, true, true, false, false, false, false, true);
				break;
			}
			break;
		case StageObjectType.LADDER:
			AssignFlag (false, true, true, false, false, false, false, false);
			break;
		case StageObjectType.GOAL:
		case StageObjectType.DECORATION:
			AssignFlag (false, false, true, false, false, false, false, false);
			break;
		case StageObjectType.RECTANGLE:
			AssignFlag (false, true, true, false, false, false, false, false);
			break;
		case StageObjectType.TRIANGLE:
		case StageObjectType.TRIANGLE2:
		case StageObjectType.TRIANGLE3:
		case StageObjectType.TRIANGLE4:
			AssignFlag (false, true, true, false, false, false, false, false);
			break;
		case StageObjectType.HOLE:
			AssignFlag (false, false, false, false, false, false, false, false);
			break;
		}
	}

	private void AssignFlag(bool f1, bool f2, bool f3, bool f4, bool f5, bool f6, bool f7, bool f8)
	{
		showLine = f1;
		showColor = f2;
		showUse = f3;
		showTop = f4;
		showUp = f5;
		showDown = f6;
		showFall = f7;
		showCase = f8;
	}
}

