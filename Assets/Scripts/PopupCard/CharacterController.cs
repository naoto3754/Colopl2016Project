﻿using UnityEngine;
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
	
	void Update () {
		int loops = 50;
		for(int i = 0; i < loops; i++)
		{
			float deltaHol = Time.deltaTime * _Speed * Input.GetAxis("Horizontal") / loops;
			float deltaVer = Time.deltaTime * _Speed * Input.GetAxis("Vertical") / loops;
			float deltaDrop = Time.deltaTime * _DropSpeed / loops;
			
			if(DummyCard.I.CanUseLadder(_DummyCharacter.transform.position, deltaVer))
			{
				_Character.transform.Translate(0f, deltaVer, 0f);
				_DummyCharacter.transform.Translate(0f, deltaVer, 0f);
			}
			else if(!DummyCard.I.IsGrounded(_DummyCharacter.transform.position, deltaDrop))
			{
				_Character.transform.Translate(0f, -deltaDrop, 0f);
				_DummyCharacter.transform.Translate(0f, -deltaDrop, 0f);
			}
			
			if(DummyCard.I.MoveXaxis(_DummyCharacter.transform.position))
				_Character.transform.Translate(deltaHol, 0f, 0f);
			else
				_Character.transform.Translate(0f, 0f, -deltaHol);
			_DummyCharacter.transform.Translate(deltaHol, 0f, 0f);
			
		}
	}
}
