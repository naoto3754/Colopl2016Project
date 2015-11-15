using UnityEngine;
using System.Collections;

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
}

public enum StageLineType
{
	FOLD,
	GROUND,
	WALL,
	SLOPE,
}