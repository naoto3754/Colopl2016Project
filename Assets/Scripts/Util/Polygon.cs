using UnityEngine;
using System.Collections;

public class Polygon
{
	protected ColorData color;

	public virtual bool Contains (Vector2 point){
		Debug.LogError ("Don't through this code");
		return false;
	}
}

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
		if (param == null || CharacterController.I.color == param.Color || param.Color == ColorData.NONE) {
			if (param == null || param.EnableCase == StageObjectParameter.EnableFlag.ALWAYS ||
				(param.EnableCase == StageObjectParameter.EnableFlag.IS_TOP && CharacterController.I.IsTopOfWall) ||
				(param.EnableCase == StageObjectParameter.EnableFlag.ISNOT_TOP && CharacterController.I.IsTopOfWall == false))
			{
				return Cross (points [1] - points [0], startpos - points [0]) * Cross (points [1] - points [0], endpos - points [0]) <= 0 &&
					Cross (endpos - startpos, points [0] - startpos) * Cross (endpos - startpos, points [1] - startpos) <= 0;
			}
		}
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

public class Triangle : Polygon
{
	public Vector2[] points = new Vector2[3];

	public Triangle(Vector2 p1, Vector2 p2, Vector2 p3, ColorData c = ColorData.NONE)
	{
		this.points[0] = p1;
		this.points[1] = p2;
		this.points[2] = p3;
		color = c;
	}
	/// <summary>
	/// 点が矩形に含まれているかを判定
	/// </summary>
	public override bool Contains(Vector2 point)
	{
		float z1 = (points[2].x-points[1].x)*(point.y-points[1].y)-(points[2].y-points[1].y)*(point.x-points[1].x);
		float z2 = (points[0].x-points[2].x)*(point.y-points[2].y)-(points[0].y-points[2].y)*(point.x-points[2].x);
		float z3 = (points[1].x-points[0].x)*(point.y-points[0].y)-(points[1].y-points[0].y)*(point.x-points[0].x);

		if (CharacterController.I.color == color || color == ColorData.NONE)
			return (z1>=0 && z2>=0 && z3>=0) || (z1<=0 && z2<=0 && z3<=0);
		else
			return false;
	}
}

public class Rectangle : Polygon
{
	public Vector2 center;
	public float width;
	public float height;
	public float left { get { return center.x - width / 2; } }
	public float right { get { return center.x + width / 2; } }
	public float bottom { get { return center.y - height / 2; } }
	public float up { get { return center.y + height / 2; } }

	public Rectangle(Vector2 center, float width, float height, ColorData c = ColorData.NONE)
	{
		this.center = center;
		this.width = width;
		this.height = height;
		color = c;
	}
		
	/// <summary>
	/// 点が矩形に含まれているかを判定
	/// </summary>
	public override bool Contains(Vector2 point)
	{
		Vector2 diff = center - point;
		if (CharacterController.I.color == color || color == ColorData.NONE)
			return Mathf.Abs(diff.x) <= width / 2 && Mathf.Abs(diff.y) <= height / 2;
		else
			return false;

	}

	public bool IsOverlaped(Rectangle rect)
	{
		return rect.Contains (new Vector2 (left, up)) ||
		rect.Contains (new Vector2 (right, up)) ||
		rect.Contains (new Vector2 (left, bottom)) ||
		rect.Contains (new Vector2 (right, bottom)) ||
		this.Contains (new Vector2 (rect.left, rect.up)) ||
		this.Contains (new Vector2 (rect.right, rect.up)) ||
		this.Contains (new Vector2 (rect.left, rect.bottom)) ||
		this.Contains (new Vector2 (rect.right, rect.bottom));
	}
}
