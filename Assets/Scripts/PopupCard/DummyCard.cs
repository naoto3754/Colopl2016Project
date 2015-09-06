using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DummyCard : Singlton<DummyCard> {
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
		
		foreach(Transform line in FoldLines.transform)
			_FoldLine.Add(new Line(line.position, line.position+line.localScale));
		foreach(LineRenderer renderer in StageComponent.GetComponentsInChildren<LineRenderer>())
		{
			Vector3 linePos = renderer.transform.position;
			Vector3 lineScale = renderer.transform.localScale;
			if(lineScale.y == 0f)
				_GroundLine.Add(new Line(linePos, linePos+lineScale, renderer.GetComponent<StageObjectParameter>().color));
			else if(lineScale.x == 0f)
				_Wall.Add(new Line(linePos, linePos+lineScale, renderer.GetComponent<StageObjectParameter>().color));
			else
				_Slope.Add(new Line(linePos, linePos+lineScale, renderer.GetComponent<StageObjectParameter>().color));
			
			if(renderer.GetComponent<StageObjectParameter>() != null &&
			   renderer.GetComponent<StageObjectParameter>().UseAsDecoration)
			   _Decoration.Add(renderer.gameObject);
		}
		foreach(Ladder ladder in StageComponent.GetComponentsInChildren<Ladder>())
		{
			_Ladder.Add(new CardRect(ladder.transform.position, 
									 ladder.transform.localScale.x,
									 ladder.transform.localScale.y,
									 ladder.GetComponent<StageObjectParameter>().color));
		}
		foreach(SpriteRenderer renderer in StageComponent.GetComponentsInChildren<SpriteRenderer>())
		{
			_Decoration.Add(renderer.gameObject);
		}
		//  foreach(Transform ladder in Ladders.transform)
		//  	_Ladder.Add(new CardRect(ladder.position, ladder.localScale.x,ladder.localScale.y, ladder.GetComponent<StageObjectParameter>().color));
	}
	/// <summary>
	/// 移動量を計算
	/// </summary>
	public Vector2 CalcAmountOfMovement(List<Vector2> charaPosList, Vector2 delta)
	{
		Vector2 retVec = Vector3.zero;
		retVec.x = delta.x;
		retVec.y = delta.y;
		foreach(Line groundline in _GroundLine)
		{
			foreach(Vector2 charaPos in charaPosList)
			{
				if(groundline.ThroughLine(charaPos, charaPos+delta))
				{
					float ret = groundline.points[0].y - charaPos.y;
					delta.y = ret - Mathf.Sign(ret)*0.01f;	
					retVec.y = delta.y;
					break;
				}
			}
		}
		foreach(Line slope in _Slope)
		{
			foreach(Vector2 charaPos in charaPosList)
			{
				if(slope.ThroughLine(charaPos, charaPos+delta))
				{
					float ret = slope.LarpYCoord(charaPos.x+delta.x) - charaPos.y;
					delta.y = ret + 0.01f;	
					retVec.y = delta.y;
					break;
				}
			}
		}
		if(Mathf.Abs(delta.x) > 0f)
		{
			foreach(Line wall in _Wall)
			{
				foreach(Vector2 charaPos in charaPosList)
				{
					if(wall.ThroughLine(charaPos, charaPos + delta))
					{
						float ret = wall.points[0].x - charaPos.x;
						delta.x = ret - Mathf.Sign(ret)*0.01f;
						retVec.x = delta.x;
						break;
					}
				}
			}
		}
		return retVec;
	}
	/// <summary>
	/// 与えられた距離内にある折り目までの距離を返す
	/// </summary>
	public float CalcFoldLineDistance(Vector2 charaPos, float delta)
	{
		float ret = delta+Mathf.Sign(delta)*1f;
		foreach(Line foldline in _FoldLine)
		{
			if(foldline.ThroughLine(charaPos, charaPos+delta*Vector2.right))
			{
				ret = foldline.points[0].x -charaPos.x;
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
		foreach(CardRect rect in _Ladder)
		{
			if(rect.Contains(charaPos))
				return true;
		}
		return false;
	}
	
	/// <summary>
	/// 折り目のy座標をソートした配列を取得
	/// </summary>
	public IEnumerable<float> GetSortYCoordList()
	{
		List<float> retList = new List<float>();
		foreach(Line line in _FoldLine)
		{
			if(retList.Contains(line.points[0].y) == false)
				retList.Add(line.points[0].y);
			if(retList.Contains(line.points[1].y) == false)
				retList.Add(line.points[1].y);
		}
		
		return retList.OrderBy(x => x);
	}
	/// <summary>
	/// 引数のy座標を含む折り目のx座標をソートした配列を取得
	/// </summary>
	public IEnumerable<float> GetSortXCoordList(float y)
	{
		List<float> retList = new List<float>();
		foreach(Line line in _FoldLine)
		{
			if( ( (line.points[0].y - y) <= 0f && 0f < (line.points[1].y - y) ) ||
				( (line.points[0].y - y) >= 0f && 0f > (line.points[1].y - y) ) )
			{
				retList.Add(line.points[0].x);
			}
		}
		
		return retList.OrderBy(x => x);
	}
	
	/*            *
	 * Line Class *
	 *            */
	public class Line
	{
		public Vector2[] points = new Vector2[2];

		ColorData color;
		
		public Line(Vector2 start, Vector2 end, ColorData c = ColorData.NONE)
		{
			points[0] = start;
			points[1] = end;
			color = c;
		}
		/// <summary>
		/// 線の交差を判定
		/// </summary>
		public bool ThroughLine(Vector2 startpos, Vector2 endpos)
		{
			if(CharacterController.I.color == color || color == ColorData.NONE)
				return Cross(points[1]-points[0], startpos-points[0])*Cross(points[1]-points[0], endpos-points[0]) < 0 &&
					   Cross(endpos-startpos, points[0]-startpos)*Cross(endpos-startpos, points[1]-startpos) < 0;
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
			if(x < leftPoint.x)
				return leftPoint.y;
			else if(x < rightPoint.x)
				return Mathf.Lerp(leftPoint.y,rightPoint.y,(x-leftPoint.x)/(rightPoint.x-leftPoint.x));
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
		public float left{ get { return  center.x - width/2; } }
		public float right{ get { return  center.x + width/2; } }
		public float bottom{ get { return  center.y - height/2; } }
		public float up{ get { return  center.y + height/2; } }

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
				return Mathf.Abs (diff.x) <= width / 2 && Mathf.Abs (diff.y) <= height / 2;
			else
				return false;

		}

	}

}