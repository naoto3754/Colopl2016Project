using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageObjectParameter : MonoBehaviour {
	public StageObjectType Type;
	public StageLineType LineType;
	public ColorData Color = ColorData.NONE;
	public bool UseAsDecoration = false;
	public bool TopOfWall = false;
	public bool DontThroughUp = false;
	public bool DontThroughDown = true;
	public EnableFlag EnableCase = EnableFlag.ALWAYS;
	public float HeightWithMaxWidth = 0f;

	public List<GameObject> _ObjectOnStage = new List<GameObject>();
	public List<GameObject> ObjectsOnStage {
		get { return _ObjectOnStage; }
	}

	public enum EnableFlag
	{
		ALWAYS,
		ISNOT_TOP,
		IS_TOP,
	}
}
	
public enum StageObjectType
{
	LINE,
	HOLE,
	LADDER,
	GOAL,
	RECTANGLE,
	TRIANGLE,
	DECORATION,
	TRIANGLE2,
}

public enum StageLineType
{
	FOLD,
	GROUND,
	WALL,
	SLOPE,
	TOP_FOLD,
}