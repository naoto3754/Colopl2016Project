using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DummyCard : Singlton<DummyCard> {
	[SerializeField]
	private GameObject FoldLines;
	[SerializeField]
	private GameObject GroundLines;
	[SerializeField]
	private GameObject Ladders;
	
	private List<Line> _FoldLine;
	//  public List<Line> FoldLine
	//  {
	//  	get { return _FoldLine; }
	//  }
	
	private List<Line> _GroundLine;
	//  public List<Line> GroundLine
	//  {
	//  	get { return _GroundLine; }
	//  }
	
	private List<CardRect> _Ladder;
	
	public override void OnInitialize()
	{
		_FoldLine = new List<Line>();
		_GroundLine = new List<Line>();
		_Ladder = new List<CardRect>();
		
		foreach(Transform line in FoldLines.transform)
		{
			_FoldLine.Add(new Line(line.position, line.position+line.localScale));
		}
		foreach(Transform line in GroundLines.transform)
		{
			_GroundLine.Add(new Line(line.position, line.position+line.localScale));
		}
		foreach(Transform ladder in Ladders.transform)
		{
			_Ladder.Add(new CardRect(ladder.position,ladder.localScale.x,ladder.localScale.y));
		}
	}
	
	public float CalcAmountOfMovementY(Vector3 charaPos, float deltaY)
	{
		foreach(Line groundline in _GroundLine)
		{
			if(groundline.ThroughLine(charaPos, charaPos+new Vector3(0f, deltaY, 0f)))
			{
				float ret = groundline.points[0].y - charaPos.y;	
				return ret - Mathf.Sign(ret)*0.01f;
			}
		}
		return deltaY;
	}
	
	public float[] CalcAmountOfMovementXZ(Vector3 charaPos, float delta, ref bool moveX)
	{
		foreach(Line foldline in _FoldLine)
		{
			if(foldline.ThroughLine(charaPos, charaPos+new Vector3(delta, 0f, 0f)))
			{
				moveX = !moveX;
				if(!moveX)
					return new float[] {foldline.points[0].x -charaPos.x, -(charaPos.x+delta-foldline.points[0].x)};
				else
					return new float[] {charaPos.x+delta-foldline.points[0].x, -(foldline.points[0].x - charaPos.x)};
			}	
		}
		if(moveX)
			return new float[] {delta , 0f};
		else
			return new float[] {0f , -delta};
	}
	
	public bool CanUseLadder(Vector3 charaPos)
	{
		foreach(CardRect rect in _Ladder)
		{
			if(rect.Contains(charaPos))
				return true;
		}
		return false;
	}
	
	public class Line
	{
		public Vector2[] points = new Vector2[2];
		
		public Line(Vector2 start, Vector2 end)
		{
			points[0] = start;
			points[1] = end;
		}
		
		public bool ThroughLine(Vector2 startpos, Vector2 endpos)
		{
			return Cross(points[1]-points[0], startpos-points[0])*Cross(points[1]-points[0], endpos-points[0]) < 0 &&
				   Cross(endpos-startpos, points[0]-startpos)*Cross(endpos-startpos, points[1]-startpos) < 0;
		}
		
		private float Cross(Vector2 lhs, Vector2 rhs)
		{
			return lhs.x * rhs.y - rhs.x * lhs.y;
		}
	}
	
	public class CardRect
	{
		public Vector2 center;
		public float width;
		public float height;
		public float left{ get { return  center.x - width/2; } }
		public float right{ get { return  center.x + width/2; } }
		public float bottom{ get { return  center.y - height/2; } }
		public float up{ get { return  center.y + height/2; } }

		public CardRect(Vector2 center, float width, float height)
		{
			this.center = center;
			this.width = width;
			this.height = height;
		}
		 
		public bool Contains(Vector2 point)
		{
			Vector2 diff = center - point;
			return Mathf.Abs(diff.x) <= width/2 && Mathf.Abs(diff.y) <= height/2;
		}

	}

}