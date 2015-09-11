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

/*            *
 * Line Class *
 *            */
public class Line
{
    public Vector2[] points = new Vector2[2];
    public StageObjectParameter param;

    public Line(Vector2 start, Vector2 end, StageObjectParameter p)
    {
        points[0] = start;
        points[1] = end;
        param = p;
        //  if(param == null)
        //      this.param = new StageObjectParameter();
    }
    /// <summary>
    /// 線の交差を判定
    /// </summary>
    public bool ThroughLine(Vector2 startpos, Vector2 endpos)
    {
        
        if (param == null || CharacterController.I.color == param.color || param.color == ColorData.NONE)
            return Cross(points[1] - points[0], startpos - points[0]) * Cross(points[1] - points[0], endpos - points[0]) <= 0 &&
                   Cross(endpos - startpos, points[0] - startpos) * Cross(endpos - startpos, points[1] - startpos) <= 0;
        else
            return false;
    }
    /// <summary>
    /// 外積を求める
    /// </summary>
    private float Cross(Vector2 lhs, Vector2 rhs)
    {
        return lhs.x * rhs.y - rhs.x * lhs.y;
    }
    /// <summary>
    /// x座標から対応するy座標を求める
    /// </summary>
    public float LarpYCoord(float x)
    {
        Vector2 leftPoint = points[0].x < points[1].x ? points[0] : points[1];
        Vector2 rightPoint = points[0].x < points[1].x ? points[1] : points[0];
        if (x < leftPoint.x)
            return leftPoint.y;
        else if (x < rightPoint.x)
            return Mathf.Lerp(leftPoint.y, rightPoint.y, (x - leftPoint.x) / (rightPoint.x - leftPoint.x));
        else
            return rightPoint.y;
    }
}

/*                *
 * CardRect Class *
 *                */
public class CardRect
{
    public Vector2 center;
    public float width;
    public float height;
    public float left { get { return center.x - width / 2; } }
    public float right { get { return center.x + width / 2; } }
    public float bottom { get { return center.y - height / 2; } }
    public float up { get { return center.y + height / 2; } }

    ColorData color;

    public CardRect(Vector2 center, float width, float height, ColorData c = ColorData.NONE)
    {
        this.center = center;
        this.width = width;
        this.height = height;
        color = c;
    }
    /// <summary>
    /// 点が矩形に含まれているかを判定
    /// </summary>
    public bool Contains(Vector2 point)
    {
        Vector2 diff = center - point;
        if (CharacterController.I.color == color || color == ColorData.NONE)
            return Mathf.Abs(diff.x) <= width / 2 && Mathf.Abs(diff.y) <= height / 2;
        else
            return false;

    }
}