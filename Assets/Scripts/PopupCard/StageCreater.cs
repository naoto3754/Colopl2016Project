using UnityEngine;
using System.Collections;

public class StageCreater : Singlton<StageCreater> {
	//  [SerializeField]
	//  private GameObject _Paper;
	//  [SerializeField]
	//  private GameObject _Ladder;
	//  [SerializeField]
	//  private GameObject _Line;
	
	private float zOffset = -50f;
	private GameObject _Root;
	
	void Start () {
		_Root = new GameObject("StageRoot");
		GameObject character = Instantiate(CharacterController.I.DummyCharacter,
										   CharacterController.I.DummyCharacter.transform.position+new Vector3(-0.02f,0f,zOffset),
										   Quaternion.identity) as GameObject;
		character.transform.parent = _Root.transform;
		character.layer = 0;
		foreach(Transform child in character.transform)
			child.gameObject.layer = 0;
		CharacterController.I.Character = character;
		
	//  	Vector3 cardSize = DummyCard.I.gameObject.transform.lossyScale;
	//  	Vector3 pos;
	//  	Vector3 scale;
		
	//  	foreach(CardRect rect in DummyCard.I.CardRects)
	//  	{
	//  		float xWidth = rect.width/2-(rect.center.x-rect.foldlines[1]);
	//  		float zWidth = rect.width/2+(rect.center.x-rect.foldlines[1]);
	//  		if(rect.foldlines[1] <= 0f)
	//  		{
	//  			pos = new Vector3(rect.foldlines[1]-xWidth/2, rect.center.y, -zWidth+zOffset);
	//  			scale = new Vector3(xWidth, rect.height, 1f);
	//  			CreatePaper(pos, scale, false);
				
	//  			pos = new Vector3(rect.left, rect.center.y, -zWidth/2+zOffset);
	//  			scale = new Vector3(zWidth, rect.height, 1f);
	//  			CreatePaper(pos, scale, true);
	//  		}
	//  		else
	//  		{
	//  			pos = new Vector3(-zWidth, rect.center.y, -rect.foldlines[1]-xWidth/2+zOffset);
	//  			scale = new Vector3(xWidth, rect.height, 1f);
	//  			CreatePaper(pos, scale, true);
				
	//  			pos = new Vector3(-zWidth/2, rect.center.y, -rect.right+zOffset);
	//  			scale = new Vector3(zWidth, rect.height, 1f);
	//  			CreatePaper(pos, scale, false);

		}

	//  		if(Mathf.Abs(rect.center.x) < rect.width/2)
	//  		{
	//  			pos = new Vector3(-cardSize.x/4-xWidth/2, rect.center.y, 0+zOffset);
	//  			scale = new Vector3(cardSize.x/2-xWidth, rect.height, 1f);
	//  			CreatePaper(pos, scale, false);
				
	//  			pos = new Vector3(0f, rect.center.y, -cardSize.x/4-zWidth/2+zOffset);
	//  			scale = new Vector3(cardSize.x/2-zWidth, rect.height, 1f);
	//  			CreatePaper(pos, scale, true);
	//  		}
	//  	}
		
	//  	foreach(CardRect rect in DummyCard.I.Ladders)
	//  	{
	//  		GameObject ladder = Instantiate(_Ladder, 
	//  										new Vector3(Mathf.Min(0.0001f, rect.center.x),rect.center.y,-Mathf.Max(0.0001f, rect.center.x)+zOffset),
	//  										Quaternion.identity) as GameObject;
	//  		ladder.transform.parent = _Root.transform;
	//  		ladder.transform.localScale = new Vector3(rect.width, rect.height, 1f);
	//  		if(rect.center.x > 0f)
	//  			ladder.transform.Rotate(0f,90f,0f);
	//  	}
		
	//  	foreach(CardRect rect in DummyCard.I.Lines)
	//  	{
	//  		GameObject line = Instantiate(_Line, 
	//  										new Vector3(Mathf.Min(-0.0001f, rect.center.x),rect.center.y,-Mathf.Max(0.0001f, rect.center.x)+zOffset),
	//  										Quaternion.identity) as GameObject;
	//  		line.transform.parent = _Root.transform;
	//  		line.transform.localScale = new Vector3(rect.width, rect.height, 1f);
	//  		if(rect.center.x > 0f)
	//  			line.transform.Rotate(0f,90f,0f);
	//  	}
	//  }
	
	//  private void CreatePaper(Vector3 pos, Vector3 scale, bool rotate)
	//  {
	//  	GameObject paper = Instantiate(_Paper, pos, Quaternion.identity) as GameObject;
	//  	paper.transform.parent = _Root.transform;  
	//  	paper.transform.localScale = scale;
	//  	if(rotate)
	//  		paper.transform.Rotate(0f,90f,0f);		
	//  }
}
