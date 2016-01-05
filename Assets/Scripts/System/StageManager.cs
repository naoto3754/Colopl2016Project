using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageManager : Singleton<StageManager>
{

    [SerializeField]
    private List<GameObject> _Stages;
	public List<GameObject> Stages
	{
		get { return _Stages; }
	}
	public int StageCount
	{
		get { return _Stages.Count; }
	}

    private int _CurrentChapter;
    public int CurrentChapter
    {
        get { return _CurrentChapter; }
    }
    private int _CurrentBookID;
    public int CurrentBookID
    {
        get { return _CurrentBookID; }
    }
    private int _CurrentStageIndex;
    public int CurrentStageIndex
    {
        get { return _CurrentStageIndex; }
    }

	public StageInfomation CurrentInfo{
		get; set;
	}    
	public Book Book {
		get; set;
	}
	public Vector3 Offset {
		get; set;
	}
	public CustomCharaController CurrentController {
		get; set;
	}
	public GameObject PrevStageRoot {
		get; set;
	}
	public GameObject StageRoot {
		get; set;
	}
	public GameObject PrevPaperRoot {
		get; set;
	}
	public GameObject PaperRoot {
		get; set;
	}
	public GameObject PrevDecoRoot {
		get; set;
	}
	public GameObject DecoRoot {
		get; set;
	}
	public GameObject BackRootL{
		get; set;
	}
	public GameObject BackRootR{
		get; set;
	}
	public GameObject PrevBackRootL{
		get; set;
	}
	public GameObject PrevBackRootR{
		get; set;
	}
		    
    /// <summary>
    /// ステージ生成
    /// </summary>
    public void InstantiateStage(int chapter, int bookID, int index)
    {
        
        if(chapter < 1 && 5 < chapter)
            Debug.LogError("Invalid chapter");
        if(bookID < 0 && 2 < bookID)
            Debug.LogError("Invalid bookID");
        if(index < 0 && 2 < index)
            Debug.LogError("Invalid stage index");
        
        ////仮実装
        _CurrentChapter = chapter;
        _CurrentBookID = bookID;
        _CurrentStageIndex = index;
        //ダミーカードをInstantiateすると、ダミーカードのAwakeでステージ情報を更新し、ステージ生成まで行う
        Instantiate(_Stages[ CalcStageListIndex(chapter, bookID, index) ]);
    }
    
    public static int CalcStageListIndex(int chapter, int bookID, int index)
    {
        chapter -= 1;
        int bookCnt = 3;
        int stageCnt = 3;       
        return chapter*bookCnt*stageCnt + bookID * stageCnt + index;
    }
    /// <summary>
    /// 0:chapter 1:bookID 2:index
    /// </summary>
    public static int[] CalcStageIndexInfo(int index)
    {
        int[] ret = new int[3];
        int bookCnt = 3;
        int stageCnt = 3;
        ret[0] = index / (bookCnt*stageCnt) + 1;
        index = index % (bookCnt*stageCnt);
        ret[1] = index / stageCnt;
        ret[2] = index % stageCnt;
        return ret;
    }

    private bool _JumpUp;
    /// <summary>
    /// 移動量を計算
    /// </summary>
    public Vector2 CalcAmountOfMovement(Vector2 delta)
    {
        _JumpUp = false;
        Vector2 retVec = delta;
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
        foreach (Line groundline in CurrentInfo.GroundLine)
        {
            Vector2 charaPos;
            //下に移動してる時
            if (delta.y < 0f)
            {
                if (groundline.param.DontThroughDown == false)
                    continue;
                //下
                charaPos = CurrentController.Bottom;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
                //左下
				charaPos = CurrentController.BottomLeft;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
                //右下
				charaPos = CurrentController.BottomRight;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
            }
            //上に移動してる時
            else if (delta.y > 0f)
            {
                //飛び出た部分の上に乗った判定
                if (groundline.param.TopOfWall && _JumpUp == false)
                {
					charaPos = CurrentController.Bottom;
                    if (groundline.ThroughLine(charaPos, charaPos + delta))
                    {
						CurrentController.IsTopOfWall = true;
                    }
                }

                if (groundline.param.DontThroughUp == false)
                    continue;
                //上
                charaPos = CurrentController.Top;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
                //左上
                charaPos = CurrentController.TopLeft;
                if (groundline.ThroughLine(charaPos, charaPos + delta))
                    return CalcDistanceToGround(delta, charaPos.y, groundline.points[0].y);
                //右上
                charaPos = CurrentController.TopRight;
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
        foreach (Line groundLine in CurrentInfo.GroundLine)
        {
            Vector2 charaLeftPos = CurrentController.BottomLeft;
            Vector2 charaRightPos = CurrentController.BottomRight;
            Vector3 delta3D = delta;
            bool IntersectTopLeft = groundLine.ThroughLine(CurrentController.Left + delta3D,
                                                           CurrentController.TopLeft + delta3D);
            bool IntersectTopRight = groundLine.ThroughLine(CurrentController.Right + delta3D,
                                                            CurrentController.TopRight + delta3D);
            bool IntersectBottomLeft = groundLine.ThroughLine(CurrentController.Left + delta3D,
                                                          CurrentController.BottomLeft + delta3D);
            bool IntersectBottomRight = groundLine.ThroughLine(CurrentController.Right + delta3D,
                                                          CurrentController.BottomRight + delta3D);
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
            if ( (IntersectBottomLeft && !IntersectBottomRight || !IntersectBottomLeft&& IntersectBottomRight) && Mathf.Abs(delta.y) < 0.001f)
            {
                _JumpUp = true;   
                return new Vector2(delta.x, groundLine.points[0].y - charaLeftPos.y + 0.01f);
            }
        }
        foreach (Line wall in CurrentInfo.Wall)
        {
            Vector2 charaPos;
            //左
            charaPos = CurrentController.Left;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
            //右
            charaPos = CurrentController.Right;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
            //左上
            charaPos = CurrentController.TopLeft;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
            //右上
            charaPos = CurrentController.TopRight;
            if (wall.ThroughLine(charaPos, charaPos + delta))
                return CalcDistanceToWall(delta, charaPos.x, wall.points[0].x);
            //下
            charaPos = CurrentController.Bottom;
            if (wall.ThroughLine(charaPos, charaPos + delta) && Mathf.Abs(delta.y) < 0.001f)
            {
                _JumpUp = true;
                return new Vector2(delta.x, Mathf.Max(wall.points[0].y, wall.points[1].y) - charaPos.y + 0.01f);
            }
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
        foreach (Line slope in CurrentInfo.Slope)
        {
            Vector2 charaPos = CurrentController.Bottom;
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
    public float CalcFoldLineDistance(Vector2 pos, float delta, bool isTop)
    {
		float ret = delta + Mathf.Sign(delta) * 1f;

        if(isTop)
        {
			pos -= 0.05f * Vector2.up;
			foreach (Line foldline in CurrentInfo.TopFoldLine) {
				if (foldline.ThroughLine (pos, pos + delta * Vector2.right)) {
					ret = foldline.points [0].x - pos.x;
					break;
				}
			}
        }
        foreach (Line foldline in CurrentInfo.FoldLine)
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
    /// 自分の真下に飛び出ている部分の上面があるかどうか
    /// </summary>
    public bool OnTopOfWall(Vector2 bottomL, Vector2 bottomR)
    {
        foreach (Line groundline in CurrentInfo.GroundLine)
        {
            if (groundline.param.TopOfWall)
            {
                if (groundline.ThroughLine(bottomL + 0.01f * Vector2.up, bottomL - 0.4f * Vector2.up))
                    return true;
                if (groundline.ThroughLine(bottomR + 0.01f * Vector2.up, bottomR - 0.4f * Vector2.up))
                    return true;
            }
        }
        return false;
    }
	/// <summary>
	/// 下に降りることができるか
	/// </summary>
	public bool CanFall()
	{
		foreach (Line groundline in CurrentInfo.GroundLine)
		{
			if (groundline.param.TopOfWall == false)
			{
				Vector2 charaPos = CurrentController.BottomLeft;
				if (groundline.ThroughLine(charaPos + 0.01f * Vector2.up, charaPos - 0.4f * Vector2.up))
					return false;
				charaPos = CurrentController.BottomRight;
				if (groundline.ThroughLine(charaPos + 0.01f * Vector2.up, charaPos - 0.4f * Vector2.up))
					return false;
			}
		}
		return true;
	}
	/// <summary>
	/// 下に降りることができるか
	/// </summary>
	public bool OnJumpOffLine()
	{
		bool ret = false;
		foreach (Line groundline in CurrentInfo.GroundLine)
		{
			if (groundline.param.CanFallOff)
			{
				Vector2 charaPos = CurrentController.BottomLeft;
				if (groundline.ThroughLine(charaPos + 0.01f * Vector2.up, charaPos - 0.4f * Vector2.up))
					ret = true;
				charaPos = CurrentController.BottomRight;
				if (groundline.ThroughLine(charaPos + 0.01f * Vector2.up, charaPos - 0.4f * Vector2.up))
					ret = true;
			}
			else if (groundline.param.DontThroughDown) 
			{
				Vector2 charaPos = CurrentController.BottomLeft;
				if (groundline.ThroughLine(charaPos + 0.01f * Vector2.up, charaPos - 0.4f * Vector2.up))
					return false;
				charaPos = CurrentController.BottomRight;
				if (groundline.ThroughLine(charaPos + 0.01f * Vector2.up, charaPos - 0.4f * Vector2.up))
					return false;
			}
		}
		return ret;
	}
    /// <summary>
    /// 折り目のy座標をソートした配列を取得
    /// </summary>
    public IEnumerable<float> GetSortYCoordList()
    {
        List<float> retList = new List<float>();
        foreach (Line line in CurrentInfo.FoldLine)
        {
            if (retList.Contains(line.points[0].y) == false)
                retList.Add(line.points[0].y);
            if (retList.Contains(line.points[1].y) == false)
                retList.Add(line.points[1].y);
        }
        foreach (Line line in CurrentInfo.HoleLine)
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
    public IEnumerable<float> GetFoldXCoordList(float y, bool isTop)
    {   
		List<float> retList = new List<float>();
		retList.Add(CurrentInfo.StageWidth/2);

		if(isTop)
		{
			y -= 0.05f;
			foreach (Line line in CurrentInfo.TopFoldLine) {
				if (((line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y)) ||
					((line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y)))
				{
					retList.Add(line.points[0].x);
				}
			}
		}

        foreach (Line line in CurrentInfo.FoldLine)
        {
            if (((line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y)) ||
                ((line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y)))
            {
                retList.Add(line.points[0].x);
            }
        }

        return retList.OrderBy(x => x);
    }
    /// <summary>
    /// 引数のy座標を含む折り目と穴のx座標をソートした配列を取得
    /// </summary>
    public IEnumerable<XCoord> GetXCoordList(float y, bool isTop)
    {
		List<XCoord> retList = new List<XCoord>();
		retList.Add(new XCoord(CurrentInfo.StageWidth/2, XCoord.Type.FOLD));

		if(isTop)
		{
			y -= 0.05f;
			foreach (Line line in CurrentInfo.TopFoldLine) {
				if (((line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y)) ||
					((line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y)))
				{
					retList.Add(new XCoord(line.points[0].x, XCoord.Type.FOLD));
				}
			}
		}

        foreach (Line line in CurrentInfo.FoldLine)
        {
            if (((line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y)) ||
                ((line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y)))
            {
                retList.Add(new XCoord(line.points[0].x, XCoord.Type.FOLD));
            }
        }
        foreach (Line line in CurrentInfo.HoleLine)
        {
            if (((line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y)) ||
                ((line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y)))
            {
                retList.Add(new XCoord(line.points[0].x, XCoord.Type.HOLE));
            }
        }
        if(retList.Select(xcoord => xcoord.x).Contains(0f) == false)
        {
            retList.Add(new XCoord(0f, XCoord.Type.NONE));
        }

        return retList.OrderBy(xcoord => xcoord.x);
    }

	public bool IsOnObstacle()
	{
		if (CurrentController == null)
			return false;

		foreach (var obstacle in CurrentInfo.Obstacle) {
			if (obstacle.color != CurrentController.color)
				continue;

			if (obstacle.IsOverlaped (CurrentController.DummyCharaRect)) {
				return true;
			}
		}
		return false;
	}

	public void Clear()
	{
		CurrentController = null;
		DestroyObject (CurrentInfo.gameObject);
		CurrentInfo = null;
		DestroyObject (PaperRoot);
		DestroyObject (PrevPaperRoot);
		DestroyObject (DecoRoot);
		DestroyObject (PrevDecoRoot);
		DestroyObject (PrevPaperRoot);
		DestroyObject (BackRootL);
		DestroyObject (PrevBackRootL);
		DestroyObject (BackRootR);
		DestroyObject (PrevBackRootR);
		DestroyObject (StageRoot);
		DestroyObject (PrevStageRoot);
	}

	public static void DestroyObject(GameObject obj){
		if (obj != null) {
			Destroy (obj);
			obj = null;
		}
	}
}