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
			float delta = _Speed * Input.GetAxis("Horizontal") / loops;
			
			if(DummyCard.I.MoveXaxis(_DummyCharacter.transform.position))
			{
				_Character.transform.Translate(delta, 0f, 0f);
			}
			else
			{
				_Character.transform.Translate(0f, 0f, -delta);
			}
			
			_DummyCharacter.transform.Translate(delta, 0f, 0f);
		}
	}
}
