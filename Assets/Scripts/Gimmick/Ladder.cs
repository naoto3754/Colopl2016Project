using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{
    public static float MovementLimit;
    
    private Rectangle _Rect;
    private bool _PreviousBool;

    void Awake()
    {
        //当たり判定作成
        var param = GetComponent<StageObjectParameter>();
        _Rect = new Rectangle(transform.position, transform.lossyScale.x, transform.localScale.y, param.Color);
    }
    
    void FixedUpdate()
    {
		if(StageManager.I.CurrentController != null && StageManager.I.CurrentController.IsTopOfWall==false && _Rect.Contains(StageManager.I.CurrentController.Bottom))
        {
            _PreviousBool = true;
			StageManager.I.CurrentController.CanUseLadder = true;
			MovementLimit = _Rect.up - StageManager.I.CurrentController.Bottom.y;
        }
        else
        {
            if(_PreviousBool == true)
				StageManager.I.CurrentController.CanUseLadder = false;
            else
				StageManager.I.CurrentController.CanUseLadder = StageManager.I.CurrentController.CanUseLadder || false;
                
            _PreviousBool = false;
        }
    }
}
