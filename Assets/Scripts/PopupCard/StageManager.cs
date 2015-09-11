using UnityEngine;
using System.Collections;

public class StageManager : Singlton<StageManager>
{
	private int _CurrentID = 0;
    private DummyCardObjects _CurrentObjects;
    public DummyCardObjects CurrentObjects
    {
        get { return _CurrentObjects; }
        set { _CurrentObjects = value; }
    }
}
