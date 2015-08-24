using UnityEngine;
using System.Collections;

public class StageCreater : Singlton<StageCreater> {
	private float zOffset = -50f;
	private GameObject _Root;
	
	void Start () {
		//（仮）キャラクターを生成
		_Root = new GameObject("StageRoot");
		GameObject character = Instantiate(CharacterController.I.DummyCharacter,
										   CharacterController.I.DummyCharacter.transform.position+new Vector3(-0.02f,0f,zOffset),
										   Quaternion.identity) as GameObject;
		character.transform.parent = _Root.transform;
		character.layer = 0;
		foreach(Transform child in character.transform)
			child.gameObject.layer = 0;
		CharacterController.I.Character = character;
	}
}
