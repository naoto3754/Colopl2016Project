using UnityEngine;
using System.Collections;

public class StageCreater : Singlton<StageCreater> {
	public static readonly float OFFSET = 0.02f;
	private float zOffset = -50f;
	private GameObject _Root;
	
	void Start () {
		//（仮）キャラクターを生成
		_Root = new GameObject("StageRoot");
		GameObject character = Instantiate(CharacterController.I.DummyCharacter,
										   CharacterController.I.DummyCharacter.transform.position+new Vector3(-OFFSET,0f,zOffset),
										   Quaternion.identity) as GameObject;
		character.transform.parent = _Root.transform;
		character.layer = 0;
		foreach(Transform child in character.transform)
			child.gameObject.layer = 0;
		CharacterController.I.CharacterX = character;
		
		character = Instantiate(CharacterController.I.DummyCharacter,
								CharacterController.I.DummyCharacter.transform.position+new Vector3(-OFFSET,0f,zOffset),
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
}
