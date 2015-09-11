using UnityEngine;
using System.Collections;

public class StageManager : Singlton<StageManager>
{
	private int _CurrentID = 0;
    private StageInfomation _CurrentObjects;
    public StageInfomation CurrentObjects
    {
        get { return _CurrentObjects; }
        set { _CurrentObjects = value; }
    }
}
