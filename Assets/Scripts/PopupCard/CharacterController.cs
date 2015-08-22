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
		
		float deltaY = DummyCard.I.CalcAmountOfMovementY(_DummyCharacter.transform.position, deltaVer);
		if(DummyCard.I.CanUseLadder(_DummyCharacter.transform.position)){
			_Character.transform.Translate(0f, deltaY, 0f);
			_DummyCharacter.transform.Translate(0f, deltaY, 0f);
		}
		else
		{
			deltaY = DummyCard.I.CalcAmountOfMovementY(_DummyCharacter.transform.position, -deltaDrop);
			_Character.transform.Translate(0f, deltaY, 0f);
			_DummyCharacter.transform.Translate(0f, deltaY, 0f);
		}	
			
		float[] moveDir = DummyCard.I.CalcAmountOfMovementXZ(_DummyCharacter.transform.position, deltaHol, ref _MoveX);
			
		_Character.transform.Translate(moveDir[0], 0f, moveDir[1]);
		_DummyCharacter.transform.Translate(deltaHol, 0f, 0f);
			
	}
}
