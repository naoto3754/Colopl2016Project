﻿using UnityEngine;
using System.Collections;

public class StageObjectParameter : MonoBehaviour {
	public ColorData color = ColorData.NONE;
	public bool UseAsDecoration = false;
	public bool TopOfWall = false;
	public bool DontThroughUp = false;
	public bool DontThroughDown = true;
	
	public StageObjectParameter()
	{
		
	}
}
