using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShelfCreater : MonoBehaviour {
	readonly float BOTTOM_SPACE = 10f;
    
	float _Width;
	float _Height;
    List<Line> _FoldLines;
    List<Line> _HoleLines;
    GameObject _Root;
    
	[SerializeField]
	Texture _ShelfTexture;
    [SerializeField]
    Vector2 _Offset;
    [SerializeField]
    int _ChapterCount;
    [SerializeField]
    float _SizePerChapter;
    
    [SerializeField]
	Color[] _StageColor;
    [SerializeField]
	GameObject _Shelf2D;
    [SerializeField]
	GameObject FoldLines;
    [SerializeField]
	GameObject Holes;
	[SerializeField]
	GameObject _Paper;
	
	public void Create()
	{
        _Width = _Shelf2D.transform.lossyScale.x;
		_Height = _Shelf2D.transform.lossyScale.y;
        
        _FoldLines = new List<Line>();
        _HoleLines = new List<Line>();
        CreateLineList();
        
		_Root = new GameObject("StageSelectShelf");
        _Root.transform.position = new Vector3(_Offset.x,0f,_Offset.y);
		InstantiatePaper();
        InstantiateBottom();
        InstantiateBackWall();
	}
    
    void CreateLineList()
    {
         //折れ線リスト作成
        foreach (LineRenderer line in FoldLines.GetComponentsInChildren<LineRenderer>())
            _FoldLines.Add(new Line(line.transform.position, line.transform.position + line.transform.lossyScale, null));
        foreach (LineRenderer line in Holes.GetComponentsInChildren<LineRenderer>())
        {
            if (line.transform.lossyScale.x == 0f)
                _HoleLines.Add(new Line(line.transform.position, line.transform.position + line.transform.lossyScale, null));
        }
    } 
	
	/// <summary>
    /// ステージのカード部分をを生成する
    /// </summary>
    void InstantiatePaper()
    {
        Vector3 scale = _Paper.transform.localScale;
        float thickness = scale.z;
        //ステージの紙オブジェクト生成
        IEnumerable<float> yCoordList = GetSortYCoordList();
        float prevY = yCoordList.First();
        float yOffset = 0f;
        foreach (float y in yCoordList)
        {
            if (y == yCoordList.First())
                continue;
            bool setX = true;
            bool duringHole = false;
            float prevX = -_Width / 2;
            float xOffset = -_Width / 2+_Offset.x, zOffset = _Offset.y;
            IEnumerable<XCoord> xCoordList = GetXCoordList((prevY + y) / 2);
            foreach (XCoord xCoord in xCoordList)
            {
                //折れ線の場合
                if(duringHole == false){
                    GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
                    paper.transform.SetParent(_Root.transform);
	                paper.GetComponent<Renderer>().material.mainTexture = _ShelfTexture;
	                paper.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(prevX/_Width+0.5f, prevY/_Height);
	                paper.GetComponent<Renderer>().material.mainTextureScale = new Vector2((xCoord.x-prevX)/_Width, (y-prevY)/_Height);
                    if (setX)
                    {
                        paper.transform.position = new Vector3((xCoord.x - prevX) / 2 + xOffset, (y - prevY) / 2 + yOffset, zOffset + thickness / 2);
                        xOffset += xCoord.x - prevX;
                    }
                    else
                    {
                        paper.transform.position = new Vector3(xOffset + thickness / 2, (y - prevY) / 2 + yOffset, -(xCoord.x - prevX) / 2 + zOffset);
                        paper.transform.forward = Vector3.right;
                        zOffset -= xCoord.x - prevX;
                    }
                    paper.transform.eulerAngles += 180*Vector3.forward;
                    paper.transform.localScale = new Vector3(xCoord.x - prevX-0.001f, y - prevY, thickness);
                    
                    if(xCoord.type == XCoord.Type.FOLD)
                        setX = !setX;
                    else
                        duringHole = true;
                    prevX = xCoord.x;
                }
                //穴の場合
                else 
                {
                    if (setX)
                        xOffset += xCoord.x - prevX;
                    else
                        zOffset -= xCoord.x - prevX;
                    if(xCoord.type == XCoord.Type.FOLD)
                        setX = !setX;
                    else
                        duringHole = false;
                    prevX = xCoord.x;
                }
            }
            yOffset += y - prevY;
            prevY = y;
        }
    }
    
    void InstantiateBottom()
    {
        Vector3 scale = _Paper.transform.localScale;
        float thickness = scale.z;
        
        foreach(int i in Enumerable.Range(0,_ChapterCount))
        {
            int idx = _ChapterCount-i-1;
            GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
            paper.transform.SetParent(_Root.transform);
            if(idx%2 == 0)
            {
                paper.transform.position = new Vector3(_Offset.x-_SizePerChapter/2, 2f+i*_SizePerChapter+BOTTOM_SPACE, _Offset.y-_SizePerChapter/2-2f);
                paper.transform.localScale = new Vector3(_SizePerChapter-4f, thickness, _SizePerChapter-4f);
            }
            else
            {
                paper.transform.position = new Vector3(_Offset.x-_SizePerChapter/2-2f, 2f+i*_SizePerChapter+BOTTOM_SPACE, _Offset.y-_SizePerChapter/2);
                paper.transform.localScale = new Vector3(_SizePerChapter-4f, thickness, _SizePerChapter-4f);
            }
        }
    }
    
    void InstantiateBackWall()
    {
        Vector3 scale = _Paper.transform.localScale;
        float thickness = scale.z;
        
        foreach(int i in Enumerable.Range(0,_ChapterCount))
        {
            int idx = _ChapterCount-i-1;
            GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
            paper.transform.SetParent(_Root.transform);
            paper.transform.position = new Vector3(_Offset.x-_SizePerChapter/2, (i+0.5f)*_SizePerChapter+BOTTOM_SPACE, _Offset.y + thickness/2);
            paper.transform.localScale = new Vector3(_SizePerChapter, _SizePerChapter, thickness);
            paper.GetComponent<Renderer>().material.color = _StageColor[idx];
            //2枚目
            paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
            paper.transform.SetParent(_Root.transform);
            paper.transform.position = new Vector3(_Offset.x + thickness/2, (i+0.5f)*_SizePerChapter+BOTTOM_SPACE, _Offset.y-_SizePerChapter/2);
            paper.transform.localScale = new Vector3(_SizePerChapter, _SizePerChapter, thickness);
            paper.transform.forward = Vector3.right;
            paper.GetComponent<Renderer>().material.color = _StageColor[idx];
        }
        
    }
    
    IEnumerable<float> GetSortYCoordList()
    {
        List<float> retList = new List<float>();
        foreach (Line line in _FoldLines)
        {
            if (retList.Contains(line.points[0].y) == false)
                retList.Add(line.points[0].y);
            if (retList.Contains(line.points[1].y) == false)
                retList.Add(line.points[1].y);
        }
        foreach (Line line in _HoleLines)
        {
            if (retList.Contains(line.points[0].y) == false)
                retList.Add(line.points[0].y);
            if (retList.Contains(line.points[1].y) == false)
                retList.Add(line.points[1].y);
        }

        return retList.OrderBy(x => x);
    }
    
    IEnumerable<XCoord> GetXCoordList(float y)
    {
        List<XCoord> retList = new List<XCoord>();
        retList.Add(new XCoord(_Width/2, XCoord.Type.FOLD));
        foreach (Line line in _FoldLines)
        {
            if (((line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y)) ||
                ((line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y)))
            {
                retList.Add(new XCoord(line.points[0].x, XCoord.Type.FOLD));
            }
        }
        foreach (Line line in _HoleLines)
        {
            if (((line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y)) ||
                ((line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y)))
            {
                retList.Add(new XCoord(line.points[0].x, XCoord.Type.HOLE));
            }
        }

        return retList.OrderBy(xcoord => xcoord.x);
    }
}
