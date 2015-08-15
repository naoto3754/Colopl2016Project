using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
	[SerializeField]
	private GameObject _Character;
	[SerializeField]
	private GameObject _DummyCharacter;
	[SerializeField]
	private DummyCard _DummyCard;
	[SerializeField]
	private float _Speed;
	
	void Update () {
		int loops = 20;
		for(int i = 0; i < loops; i++)
		{
			float delta = _Speed * Input.GetAxis("Horizontal") / loops;
			
			if(_DummyCard.MoveXaxis(_DummyCharacter.transform.position))
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
