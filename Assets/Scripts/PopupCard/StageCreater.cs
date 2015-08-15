using UnityEngine;
using System.Collections;

public class StageCreater : Singlton<StageCreater> {
	[SerializeField]
	private GameObject _Paper;
	[SerializeField]
	private GameObject _Block;
	
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
		Vector3 leftPos = new Vector3(-cardSize.x/4, cardSize.y/2, 0f+zOffset);
		Vector3 rightPos = new Vector3(0f, cardSize.y/2, -cardSize.x/4+zOffset);
		Vector3 scale = new Vector3(cardSize.x/2, cardSize.y, 1f);
		
		GameObject leftPaper = Instantiate(_Paper, leftPos, Quaternion.identity) as GameObject;  
		leftPaper.transform.localScale = scale;
		
		GameObject rightPaper = Instantiate(_Paper, rightPos, Quaternion.identity) as GameObject;  
		rightPaper.transform.localScale = scale;
		rightPaper.transform.Rotate(0f,90f,0f);
		
		foreach(Rect rect in DummyCard.I.CardRects)
		{
			GameObject block = Instantiate(_Block, new Vector3(0f, rect.center.y, 0f+zOffset), Quaternion.identity) as GameObject;
			block.transform.localScale = new Vector3(rect.width/2, rect.height, rect.width/2);
		}
	}
	
	void Update () {
	
	}
}
