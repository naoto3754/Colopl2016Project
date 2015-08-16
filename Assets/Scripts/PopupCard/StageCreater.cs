using UnityEngine;
using System.Collections;

public class StageCreater : Singlton<StageCreater> {
	[SerializeField]
	private GameObject _Paper;
	
	private float zOffset = -50f;
	
	void Start () {
		GameObject character = Instantiate(CharacterController.I.DummyCharacter,
										   CharacterController.I.DummyCharacter.transform.position+new Vector3(0f,0f,zOffset),
										   Quaternion.identity) as GameObject;
		character.layer = 0;
		foreach(Transform child in character.transform)
			child.gameObject.layer = 0;
		CharacterController.I.Character = character;
		
		Vector3 cardSize = DummyCard.I.gameObject.transform.lossyScale;
		Vector3 pos = new Vector3(-cardSize.x/4, cardSize.y/2, 0f+zOffset);
		Vector3 scale = new Vector3(cardSize.x/2, cardSize.y, 1f);
		//  Create(pos, scale, false);
		
		pos = new Vector3(0f, cardSize.y/2, -cardSize.x/4+zOffset);
		//  Create(pos, scale, true);
		
		foreach(CardRect rect in DummyCard.I.CardRects)
		{
			float xWidth = rect.width/2-(rect.center.x-rect.foldlines[1]);
			float zWidth = rect.width/2+(rect.center.x-rect.foldlines[1]);
			pos = new Vector3(rect.foldlines[1]-xWidth/2, rect.center.y, -zWidth+zOffset);
			scale = new Vector3(xWidth, rect.height, 1f);
			Create(pos, scale, false);
			
			pos = new Vector3(rect.left, rect.center.y, -zWidth/2+zOffset);
			scale = new Vector3(zWidth, rect.height, 1f);
			Create(pos, scale, true);

			if(Mathf.Abs(rect.center.x) < rect.width/2)
			{
				pos = new Vector3(-cardSize.x/4-xWidth/2, rect.center.y, 0+zOffset);
				scale = new Vector3(cardSize.x/2-xWidth, rect.height, 1f);
				Create(pos, scale, false);
			
				pos = new Vector3(0f, rect.center.y, -cardSize.x/4-zWidth/2+zOffset);
				scale = new Vector3(cardSize.x/2-zWidth, rect.height, 1f);
				Create(pos, scale, true);
			}
		}
	}
	
	private void Create(Vector3 pos, Vector3 scale, bool rotate)
	{
		GameObject paper = Instantiate(_Paper, pos, Quaternion.identity) as GameObject;  
		paper.transform.localScale = scale;
		if(rotate)
			paper.transform.Rotate(0f,90f,0f);		
	}
}
