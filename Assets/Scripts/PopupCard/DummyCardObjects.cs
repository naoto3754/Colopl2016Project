using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DummyCardObjects : Singlton<DummyCardObjects>
{
    
    [SerializeField]
    private GameObject FoldLines;
    [SerializeField]
    private GameObject StageComponent;

    private List<Line> _FoldLine;
    private List<Line> _GroundLine;
    private List<CardRect> _Ladder;
    private List<Line> _Slope;
    private List<Line> _Wall;
    private List<GameObject> _Decoration;
    public List<GameObject> Decoration
    {
        get { return _Decoration; }
    }

    public override void OnInitialize()
    {
        _FoldLine = new List<Line>();
        _GroundLine = new List<Line>();
        _Ladder = new List<CardRect>();
        _Slope = new List<Line>();
        _Wall = new List<Line>();
        _Decoration = new List<GameObject>();

        foreach (Transform line in FoldLines.transform)
            _FoldLine.Add(new Line(line.position, line.position + line.lossyScale, null));
        foreach (LineRenderer renderer in StageComponent.GetComponentsInChildren<LineRenderer>())
        {
            Vector3 linePos = renderer.transform.position;
            Vector3 lineScale = renderer.transform.lossyScale;
            
            StageObjectParameter param = null;
            if (renderer.GetComponent<StageObjectParameter>() != null)
            {
                param = renderer.GetComponent<StageObjectParameter>();
                if(renderer.GetComponent<StageObjectParameter>().UseAsDecoration)
                    _Decoration.Add(renderer.gameObject);
            }
            
            if (lineScale.y == 0f)
                _GroundLine.Add(new Line(linePos, linePos + lineScale, param));
            else if (lineScale.x == 0f)
                _Wall.Add(new Line(linePos, linePos + lineScale, param));
            else
                _Slope.Add(new Line(linePos, linePos + lineScale, param));
        }
        foreach (Ladder ladder in StageComponent.GetComponentsInChildren<Ladder>())
        {
            _Ladder.Add(new CardRect(ladder.transform.position,
                                     ladder.transform.lossyScale.x,
                                     ladder.transform.lossyScale.y,
                                     ladder.GetComponent<StageObjectParameter>().color));
        }
        foreach (SpriteRenderer renderer in StageComponent.GetComponentsInChildren<SpriteRenderer>())
        {
            _Decoration.Add(renderer.gameObject);
        }
    }
    /// <summary>
    /// 移動量を計算
    /// </summary>
    public Vector2 CalcAmountOfMovement(Vector2 delta)
    {
        Vector2 retVec = Vector3.zero;
        retVec = delta;
        //地面との交差判定
        retVec = CalcGroundIntersection(retVec);
        //坂道との交差判定
        retVec = CalcSlopeIntersection(retVec);
        //壁との交差判定
        retVec = CalcWallIntersection(retVec);
        //再度、地面との交差判定
        retVec = CalcGroundIntersection(retVec);
        
        return retVec;
    }
    /// <summary>
    /// 地面との交点から移動量を計算する
    /// </summary>
    private Vector2 CalcGroundIntersection(Vector2 delta)
    {
        foreach (Line groundline in _GroundLine)
        {
            Vector2 charaPos;
            //下に移動してる時
            if(delta.y < 0f)
            {
                if(groundline.param.DontThroughDown == false)
                    continue;
                //左下
                charaPos = CharacterController.CharaParam.BottomLeft;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
                //右下
                charaPos = CharacterController.CharaParam.BottomRight;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
            }
            //上に移動してる時
            else if(delta.y > 0f)
            {
                //飛び出た部分の上に乗った判定
                if(groundline.param.TopOfWall)
                {
                    charaPos = CharacterController.CharaParam.Bottom;
                    if (groundline.ThroughLine(charaPos, charaPos + delta))
                        CharacterController.I.IsTopOfWall = true;
                }
                
                if(groundline.param.DontThroughUp == false)
                    continue;
                //左上
                charaPos = CharacterController.CharaParam.TopLeft;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
                //右上
                charaPos = CharacterController.CharaParam.TopRight;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
            }
        }
        return delta;
    }
    private Vector2 CalcDistanceToGround(Vector2 delta, float charaY, float groundY)
    {
        float distance = groundY - charaY;
        delta.y = distance - Mathf.Sign(distance) * 0.01f;
        return delta;
    }
    /// <summary>
    /// 壁との交点から移動量を計算する
    /// </summary>
    private Vector2 CalcWallIntersection(Vector2 delta)
    {
        foreach (Line wall in _Wall)
        {
            Vector2 charaPos;
            //下
            charaPos = CharacterController.CharaParam.Bottom;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
            //左
            charaPos = CharacterController.CharaParam.Left;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
            //右
            charaPos = CharacterController.CharaParam.Right;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
            //左上
            charaPos = CharacterController.CharaParam.TopLeft;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
            //右上
            charaPos = CharacterController.CharaParam.TopRight;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
        }
        return delta;
    }
    private Vector2 CalcDistanceToWall(Vector2 delta, float charaX, float wallX)
    {
        float distance = wallX - charaX;
        delta.x = distance - Mathf.Sign(distance) * 0.01f;
        return delta;
    }
    /// <summary>
    /// 坂道との交点から移動量を計算する
    /// </summary>
    private Vector2 CalcSlopeIntersection(Vector2 delta)
    {
        foreach (Line slope in _Slope)
        {
            Vector2 charaPos = CharacterController.CharaParam.Bottom;
            if (slope.ThroughLine(charaPos, charaPos + delta))
            {
                float ret = slope.LarpYCoord(charaPos.x + delta.x) - charaPos.y;
                delta.y = ret + 0.01f;
                break;
            }
        }
        return delta;
    }
    
    /// <summary>
    /// 与えられた距離内にある折り目までの距離を返す
    /// </summary>
    public float CalcFoldLineDistance(Vector2 pos, float delta)
    {
        if(CharacterController.I.IsTopOfWall)
            pos -= 0.02f * Vector2.up;
        float ret = delta + Mathf.Sign(delta) * 1f;
        foreach (Line foldline in _FoldLine)
        {
            if (foldline.ThroughLine(pos, pos + delta * Vector2.right))
            {
                ret = foldline.points[0].x - pos.x;
                break;
            }
        }
        return ret;
    }
    /// <summary>
    /// はしごの矩形に含まれているか判定
    /// </summary>
    public bool CanUseLadder(Vector3 charaPos)
    {
        foreach (CardRect rect in _Ladder)
        {
            if (rect.Contains(charaPos))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 自分の真下に飛び出ている部分の上面があるかどうか
    /// </summary>
    public bool OnTopOfWall()
    {
        foreach (Line groundline in _GroundLine)
        {
            if(groundline.param.TopOfWall)
            {
                Vector2 charaPos = CharacterController.CharaParam.BottomLeft;
                if (groundline.ThroughLine(charaPos, charaPos - 0.4f*Vector2.up))
                    return true;
                charaPos = CharacterController.CharaParam.BottomRight;
                if (groundline.ThroughLine(charaPos, charaPos - 0.4f*Vector2.up))
                    return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 折り目のy座標をソートした配列を取得
    /// </summary>
    public IEnumerable<float> GetSortYCoordList()
    {
        List<float> retList = new List<float>();
        foreach (Line line in _FoldLine)
        {
            if (retList.Contains(line.points[0].y) == false)
                retList.Add(line.points[0].y);
            if (retList.Contains(line.points[1].y) == false)
                retList.Add(line.points[1].y);
        }

        return retList.OrderBy(x => x);
    }
    /// <summary>
    /// 引数のy座標を含む折り目のx座標をソートした配列を取得
    /// </summary>
    public IEnumerable<float> GetSortXCoordList(float y)
    {
        if(CharacterController.I.IsTopOfWall)
            y -= 0.02f;
        List<float> retList = new List<float>();
        foreach (Line line in _FoldLine)
        {
            if (((line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y)) ||
                ((line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y)))
            {
                retList.Add(line.points[0].x);
            }
        }

        return retList.OrderBy(x => x);
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