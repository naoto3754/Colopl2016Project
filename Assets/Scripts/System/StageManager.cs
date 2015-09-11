using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageManager : Singlton<StageManager>
{
    private readonly string STAGE_DIR = "Stage";

    private GameObject[] _Stages;
    private StageInfomation _CurrentInfo;
    public StageInfomation CurrentInfo
    {
        get { return _CurrentInfo; }
        set { _CurrentInfo = value; }
    }

    public override void OnInitialize()
    {
        GameObject[] stages = Resources.LoadAll<GameObject>(STAGE_DIR);
        _Stages = new GameObject[stages.Length];
        foreach (GameObject stage in stages)
        {
            StageInfomation info = stage.GetComponent<StageInfomation>();
            if (info.StageID < 0 || info.StageID >= stages.Length)
                Debug.LogError("Invalid Stage ID");
            if (_Stages[info.StageID] != null)
                Debug.LogError("Duplicate Stage ID");
            _Stages[info.StageID] = stage;
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
        //壁との交差判定
        retVec = CalcWallIntersection(retVec);
        //坂道との交差判定
        retVec = CalcSlopeIntersection(retVec);
        //再度、地面との交差判定
        retVec = CalcGroundIntersection(retVec);

        return retVec;
    }
    /// <summary>
    /// 地面との交点から移動量を計算する
    /// </summary>
    private Vector2 CalcGroundIntersection(Vector2 delta)
    {
        foreach (Line groundline in _CurrentInfo.GroundLine)
        {
            Vector2 charaPos;
            //下に移動してる時
            if (delta.y < 0f)
            {
                if (groundline.param.DontThroughDown == false)
                    continue;
                //下
                charaPos = CharacterController.CharaParam.Bottom;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
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
            else if (delta.y > 0f)
            {
                //飛び出た部分の上に乗った判定
                if (groundline.param.TopOfWall)
                {
                    charaPos = CharacterController.CharaParam.Bottom;
                    if (groundline.ThroughLine(charaPos, charaPos + delta))
                        CharacterController.I.IsTopOfWall = true;
                }

                if (groundline.param.DontThroughUp == false)
                    continue;
                //上
                charaPos = CharacterController.CharaParam.Top;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
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
        foreach (Line groundLine in _CurrentInfo.GroundLine)
        {
            Vector2 charaLeftPos = CharacterController.CharaParam.BottomLeft;
            Vector2 charaRightPos = CharacterController.CharaParam.BottomRight;
            Vector3 delta3D = delta;
            bool IntersectTopLeft = groundLine.ThroughLine(CharacterController.CharaParam.Left + delta3D,
                                                           CharacterController.CharaParam.TopLeft + delta3D);
            bool IntersectTopRight = groundLine.ThroughLine(CharacterController.CharaParam.Right + delta3D,
                                                            CharacterController.CharaParam.TopRight + delta3D);
            bool IntersectBottom = groundLine.ThroughLine(CharacterController.CharaParam.Center + delta3D,
                                                          CharacterController.CharaParam.Bottom + delta3D);            
            if (IntersectTopLeft && IntersectTopRight == false)
            {
                return new Vector2(Mathf.Max(groundLine.points[0].x, groundLine.points[1].x) - charaLeftPos.x + 0.01f,
                                   delta.y);
            }
            if (IntersectTopRight && IntersectTopLeft == false)
            {
                return new Vector2(Mathf.Min(groundLine.points[0].x, groundLine.points[1].x) - charaRightPos.x - 0.01f,
                                   delta.y);
            }
            if (IntersectBottom && Mathf.Abs(delta.y) < 0.001f)
            {
                return new Vector2(delta.x, groundLine.points[0].y - charaLeftPos.y + 0.01f);
            }
        }
        foreach (Line wall in _CurrentInfo.Wall)
        {
            Vector2 charaPos;
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
            //下
            charaPos = CharacterController.CharaParam.Bottom;
            if (wall.ThroughLine(charaPos, charaPos + delta) && Mathf.Abs(delta.y) < 0.001f)
                return new Vector2(delta.x, Mathf.Max(wall.points[0].y, wall.points[1].y) - charaPos.y + 0.01f);
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
        foreach (Line slope in _CurrentInfo.Slope)
        {
            Vector2 charaPos = CharacterController.CharaParam.Bottom;
            if (slope.ThroughLine(charaPos, charaPos + delta))
            {
                float ret = slope.LarpYCoord(charaPos.x + delta.x) - charaPos.y;
                delta.y = ret + 0.01f;
                Vector2 ground = CalcGroundIntersection(delta);
                delta *= ground.y / delta.y;
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
        if (CharacterController.I.IsTopOfWall)
            pos -= 0.02f * Vector2.up;
        float ret = delta + Mathf.Sign(delta) * 1f;
        foreach (Line foldline in _CurrentInfo.FoldLine)
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
        foreach (CardRect rect in _CurrentInfo.Ladder)
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
        foreach (Line groundline in _CurrentInfo.GroundLine)
        {
            if (groundline.param.TopOfWall)
            {
                Vector2 charaPos = CharacterController.CharaParam.BottomLeft;
                if (groundline.ThroughLine(charaPos, charaPos - 0.4f * Vector2.up))
                    return true;
                charaPos = CharacterController.CharaParam.BottomRight;
                if (groundline.ThroughLine(charaPos, charaPos - 0.4f * Vector2.up))
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
        foreach (Line line in _CurrentInfo.FoldLine)
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
        if (CharacterController.I.IsTopOfWall)
            y -= 0.02f;
        List<float> retList = new List<float>();
        foreach (Line line in _CurrentInfo.FoldLine)
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