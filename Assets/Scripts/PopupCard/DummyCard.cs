using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DummyCard : Singlton<DummyCard> {
	[SerializeField]
	private GameObject FoldLines;
	
	private List<Line> _FoldLine;
	public List<Line> FoldLine
	{
		get { return _FoldLine; }
	}
	
	public override void OnInitialize()
	{
		_FoldLine = new List<Line>();
		foreach(Transform line in FoldLines.transform)
		{
			_FoldLine.Add(new Line(line.position, line.position+Vector3.up*line.localScale.y));
		}
	}
	
	public float[] CalcAmountOfMovement(Vector3 charaPos, float delta, ref bool moveX)
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
	
//  	private List<CardRect> _CardRects;
//  	public List<CardRect> CardRects
//  	{
//  		get { return _CardRects; }
//  	}
	
//  	private List<CardRect> _Ladders;
//  	public List<CardRect> Ladders
//  	{
//  		get { return _Ladders; }
//  	}
	
//  	private List<CardRect> _Lines;
//  	public List<CardRect> Lines
//  	{
//  		get { return _Lines; }
//  	}
	
//  	public override void OnInitialize() 
//  	{
//  		_CardRects = new List<CardRect>();
//  		_Ladders = new List<CardRect>();
//  		_Lines = new List<CardRect>();
//  		foreach(Transform child in transform)
//  		{
//  			switch(child.gameObject.tag)
//  			{
//  			case "FoldPaper":
//  				float parentLine = CalcParentLine(child);
//  				_CardRects.Add(new CardRect(child.position, child.lossyScale.x, child.lossyScale.y,
//  											parentLine+(child.position.x-parentLine)*2, parentLine));
//  				break;
//  			case "Ladder":
//  				_Ladders.Add(new CardRect(child.position, child.lossyScale.x, child.lossyScale.y));
//  				break;
//  			case "Line":
//  				_Lines.Add(new CardRect(child.position, child.lossyScale.x, child.lossyScale.y));
//  				break;
//  			}
//  		}
//  	}
	
//  	public bool MoveXaxis(Vector3 charaPos)
//  	{
//  		bool ret = true;
//  		if(charaPos.x > 0f)
//  			ret = !ret;
//  		foreach(CardRect rect in _CardRects)
//  		{
//  			if(rect.Contains(charaPos)){
//  				ret = !ret;
//  				foreach(int fline in rect.foldlines)
//  				{
//  					if(charaPos.x < fline)
//  						ret = !ret;
//  				}
//  			}
//  		}
//  		return ret;
//  	}
	
//  	public bool CanUseLadder(Vector3 charaPos, float delta)
//  	{
//  		foreach(CardRect rect in _Ladders)
//  		{
//  			if(rect.Contains(charaPos+delta*Vector3.up))
//  			{
//  				return true;
//  			}
//  		}
//  		return false;
//  	}
	
//  	public bool IsGrounded(Vector3 charaPos, float delta)
//  	{
//  		if(charaPos.y-delta < 0f)
//  			return true;
//  		foreach(CardRect rect in _CardRects)
//  		{
//  			if(rect.Contains(charaPos-delta*Vector3.up))
//  			{
//  				return true;
//  			}
//  		}
//  		foreach(CardRect rect in _Lines)
//  		{
//  			if(rect.Contains(charaPos-delta*Vector3.up))
//  			{
//  				return true;
//  			}
//  		}
//  		return false;
//  	}
	
//  	private float CalcParentLine(Transform child)
//  	{
//  		if(Mathf.Abs(child.position.x) < child.lossyScale.x/2)
//  			return 0f;
//  		foreach(CardRect rect in _CardRects)
//  		{
//  			if(rect.Contains(new Vector2(child.position.x+child.lossyScale.x/2, child.position.y)))
//  			{
//  				return rect.left;
//  			}
//  			else if(rect.Contains(new Vector2(child.position.x-child.lossyScale.x/2, child.position.y)))
//  			{
//  				return rect.right;
//  			}
//  		}
			
//  		Debug.LogError("Invalid Dummy Input");
//  		return 0f;
//  	}
//  }

//  public class CardRect
//  {
//  	public Vector2 center;
//  	public float width;
//  	public float height;
//  	public float[] foldlines = new float[2];
//  	public float left{ get { return  center.x - width/2; } }
//  	public float right{ get { return  center.x + width/2; } }
//  	public float bottom{ get { return  center.y - height/2; } }
//  	public float up{ get { return  center.y + height/2; } }
	
//  	public CardRect(Vector2 center, float width, float height, float fline1 = 0f, float fline2 = 0f)
//  	{
//  		this.center = center;
//  		this.width = width;
//  		this.height = height;
//  		foldlines[0] = fline1;
//  		foldlines[1] = fline2;
//  	} 
	
//  	public bool Contains(Vector2 point)
//  	{
//  		Vector2 diff = center - point;
//  		return Mathf.Abs(diff.x) <= width/2 && Mathf.Abs(diff.y) <= height/2;
//  	}
}