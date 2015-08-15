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
	
	void Update () {
		int loops = 20;
		for(int i = 0; i < loops; i++)
		{
			float deltaHol = _Speed * Input.GetAxis("Horizontal") / loops;
			float deltaVer = _Speed * Input.GetAxis("Vertical") / loops;
			
			if(DummyCard.I.MoveXaxis(_DummyCharacter.transform.position))
			{
				_Character.transform.Translate(deltaHol, deltaVer, 0f);
			}
			else
			{
				_Character.transform.Translate(0f, deltaVer, -deltaHol);
			}
			
			_DummyCharacter.transform.Translate(deltaHol, deltaVer, 0f);
		}
	}
}
