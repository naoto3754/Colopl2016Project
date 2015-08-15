using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DummyCard : MonoBehaviour {
	private List<Rect> _CardRects;
	
	void Awake () {
		_CardRects = new List<Rect>();
		foreach(Transform child in transform)
		{
			_CardRects.Add(new Rect(child.localPosition.x - child.lossyScale.x/2, child.localPosition.y - child.lossyScale.y/2,
									child.lossyScale.x, child.lossyScale.y));
		}
	}
	
	public bool MoveXaxis(Vector3 charaPos)
	{
		bool ret = true;
		if(charaPos.x > 0f)
			ret = !ret;
		foreach(Rect rect in _CardRects)
		{
			if(rect.Contains(charaPos)){
				ret = !ret;
			}
		}
		return ret;
	}
}

//  public class CardRect
//  {
//  	public float center;
//  	public float width;
//  	public float height;
	
//  	public CardRect(float center, float width, float height)
//  	{
//  		this.center = center;
//  		this.width = width;
//  		this.height = height;
//  	} 
//  }
