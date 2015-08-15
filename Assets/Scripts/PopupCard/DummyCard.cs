using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DummyCard : Singlton<DummyCard> {
	private List<CardRect> _CardRects;
	public List<CardRect> CardRects
	{
		get { return _CardRects; }
	}
	
	public override void OnInitialize() 
	{
		_CardRects = new List<CardRect>();
		foreach(Transform child in transform)
		{
			float parentLine = CalcParentLine(child);
			_CardRects.Add(new CardRect(child.position, child.lossyScale.x, child.lossyScale.y,
										parentLine+(child.position.x-parentLine)*2, parentLine));
		}
	}
	
	public bool MoveXaxis(Vector3 charaPos)
	{
		bool ret = true;
		if(charaPos.x > 0f)
			ret = !ret;
		foreach(CardRect rect in _CardRects)
		{
			if(rect.Contains(charaPos)){
				ret = !ret;
				foreach(int fline in rect.foldlines)
				{
					if(charaPos.x < fline)
						ret = !ret;
				}
			}
		}
		return ret;
	}
	
	private float CalcParentLine(Transform child)
	{
		if(Mathf.Abs(child.position.x) < child.lossyScale.x/2)
			return 0f;
		foreach(CardRect rect in _CardRects)
		{
			if(rect.Contains(new Vector2(child.position.x+child.lossyScale.x/2, child.position.y)))
			{
				return rect.left;
			}
			else if(rect.Contains(new Vector2(child.position.x-child.lossyScale.x/2, child.position.y)))
			{
				return rect.right;
			}
		}
			
		Debug.LogError("Invalid Dummy Input");
		return 0f;
	}
}

public class CardRect
{
	public Vector2 center;
	public float width;
	public float height;
	public float[] foldlines = new float[2];
	public float left{ get { return  center.x - width/2; } }
	public float right{ get { return  center.x + width/2; } }
	public float bottom{ get { return  center.y - height/2; } }
	public float up{ get { return  center.y + height/2; } }
	
	public CardRect(Vector2 center, float width, float height, float fline1, float fline2)
	{
		this.center = center;
		this.width = width;
		this.height = height;
		foldlines[0] = fline1;
		foldlines[1] = fline2;
	} 
	
	public bool Contains(Vector2 point)
	{
		Vector2 diff = center - point;
		return Mathf.Abs(diff.x) <= width/2 && Mathf.Abs(diff.y) <= height/2;
	}
}