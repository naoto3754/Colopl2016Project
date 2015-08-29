using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageCreater : Singlton<StageCreater> {
	public static readonly float OFFSET = 0.02f;
	private float _ZOffset = -50f;
	private GameObject _Root;
	
	[SerializeField]
	private GameObject _Paper;
	[SerializeField]
	private Transform _StageSize;
	private float StageWidth
	{
		get { return _StageSize.localScale.x; }
	}
	private float StageHeight
	{
		get { return _StageSize.localScale.y; }
	}
	
	void Start () {
		_Root = new GameObject("StageRoot");
		InstantiateCharacter();
		//  InstantiateStage();
	}
	
	private void InstantiateCharacter()
	{
		//キャラクターを生成
		//X方向に動くキャラクター
		GameObject character = Instantiate(CharacterController.I.DummyCharacter,
										   CharacterController.I.DummyCharacter.transform.position+new Vector3(-OFFSET,0f,_ZOffset),
										   Quaternion.identity) as GameObject;
		character.transform.parent = _Root.transform;
		character.layer = 0;
		foreach(Transform child in character.transform)
			child.gameObject.layer = 0;
		CharacterController.I.CharacterX = character;
		//Z方向に動くキャラクター
		character = Instantiate(CharacterController.I.DummyCharacter,
								CharacterController.I.DummyCharacter.transform.position+new Vector3(-OFFSET,0f,_ZOffset),
								Quaternion.identity) as GameObject;
		character.transform.Rotate(0f,90f,0f);
		character.transform.parent = _Root.transform;
		character.GetComponent<Renderer>().material.SetColor("_MainColor",new Color(0.8f,0.8f,0.8f));
		character.GetComponent<Renderer>().material.SetFloat("_ForwardThreshold",1f);
		character.layer = 0;
		foreach(Transform child in character.transform)
			child.gameObject.layer = 0;
		CharacterController.I.CharacterZ = character;
	}
	
	private void InstantiateStage()
	{
		//ステージオブジェクト生成
		IEnumerable<float> yCoordList = DummyCard.I.GetSortYCoordList();
		float prevY = yCoordList.First();
		float yOffset = 0f; 
		foreach(float y in yCoordList)
		{
			if(y == yCoordList.First())
				continue;
			bool setX = true;
			float prevX = -StageWidth/2;
			float xOffset = -StageWidth/2, zOffset = _ZOffset;
			foreach(float x in DummyCard.I.GetSortXCoordList((prevY+y)/2))
			{
				GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
				if(setX)
				{
					//  Debug.Log("X : x="+x+", prevX="+prevX);
					paper.transform.position = new Vector3((x-prevX)/2+xOffset,(y-prevY)/2+yOffset,zOffset);
					xOffset += x-prevX;
				}
				else
				{
					//  Debug.Log("Z : x"+x+"prevX"+prevX);
					paper.transform.position = new Vector3(xOffset,(y-prevY)/2+yOffset,-(x-prevX)/2+zOffset);
					paper.transform.forward = Vector3.right;
					zOffset -= x-prevX;
				}
				paper.transform.localScale = new Vector3(x - prevX, y - prevY, 1f);
				setX = !setX;
				prevX = x;
			}
			//  Debug.Log("last");
			GameObject lastPaper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
			lastPaper.transform.position = new Vector3(xOffset,(y-prevY)/2+yOffset,-(StageWidth/2-prevX)/2+zOffset);
			lastPaper.transform.forward = Vector3.right;
			lastPaper.transform.localScale = new Vector3(StageWidth/2 - prevX, y - prevY, 1f);
			yOffset += y - prevY;
			prevY = y;
		}
	}
}
