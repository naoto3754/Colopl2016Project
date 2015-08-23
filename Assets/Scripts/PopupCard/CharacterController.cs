using UnityEngine;
using System.Collections;

public class CharacterController : Singlton<CharacterController> {
	private GameObject _Character;
	public GameObject Character
	{
		set { _Character = value; }
	}
	[SerializeField]
	private GameObject _DummyCharacter;
	public GameObject DummyCharacter
	{
		get { return _DummyCharacter; }
	}
	
	[SerializeField]
	private float _Speed;
	[SerializeField]
	private float _DropSpeed;
	
	private bool _MoveX = true;
	
	void Update () {
		float deltaHol = Time.deltaTime * _Speed * Input.GetAxis("Horizontal");
		float deltaVer = Time.deltaTime * _Speed * Input.GetAxis("Vertical");
		float deltaDrop = Time.deltaTime * _DropSpeed;
		
		if(!DummyCard.I.CanUseLadder(_DummyCharacter.transform.position))
		{
			deltaVer = -deltaDrop;
		}
			
		Vector3 moveDir = DummyCard.I.CalcAmountOfMovement(_DummyCharacter.transform.position, new Vector2(deltaHol, deltaVer), ref _MoveX);
			
		_Character.transform.Translate(moveDir);
		_DummyCharacter.transform.Translate(moveDir.x+(-moveDir.z), moveDir.y, 0f);
			
	}
}
