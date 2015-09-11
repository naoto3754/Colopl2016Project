using UnityEngine;
using System.Collections;

public class StageManager : Singlton<StageManager>
{
	private int _CurrentID = 0;
    private StageInfomation _CurrentInfo;
    public StageInfomation CurrentInfo
    {
        get { return _CurrentInfo; }
        set { _CurrentInfo = value; }
    }
}
