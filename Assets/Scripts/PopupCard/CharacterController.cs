using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;

public class CharacterController : Singlton<CharacterController> {
	private GameObject _CharacterX;
	public GameObject CharacterX
	{
		set { _CharacterX = value; }
	}
	private GameObject _CharacterZ;
	public GameObject CharacterZ
	{
		set { _CharacterZ = value; }
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
	[SerializeField]
	public ColorData color;
	
	private bool _MoveX = true;
	private bool _OverFoldLine = false;
	private int _EnterDirection;
	
	void Update () {
		float deltaHol = Time.deltaTime * _Speed * Input.GetAxis("Horizontal");
		float deltaVer = Time.deltaTime * _Speed * Input.GetAxis("Vertical");
		float deltaDrop = Time.deltaTime * _DropSpeed;
		
		if(!DummyCard.I.CanUseLadder(_DummyCharacter.transform.position))
		{
			deltaVer = -deltaDrop;
		}

		List<Vector2> charaPosList = new List<Vector2>(4);
		charaPosList.Add (_DummyCharacter.transform.position + new Vector3(_DummyCharacter.transform.localScale.x/2,0f,0f));
		charaPosList.Add (_DummyCharacter.transform.position);
		charaPosList.Add (_DummyCharacter.transform.position + new Vector3(-_DummyCharacter.transform.localScale.x/2,0f,0f)); 
		charaPosList.Add (_DummyCharacter.transform.position + new Vector3(_DummyCharacter.transform.localScale.x/2,_DummyCharacter.transform.localScale.y * (682/423),0f)); 
		charaPosList.Add (_DummyCharacter.transform.position + new Vector3(-_DummyCharacter.transform.localScale.x/2,_DummyCharacter.transform.localScale.y * (682/423),0f)); 

//		Vector2[] charaPosArray = new Vector2[4];
//		charaPosArray [0] = _DummyCharacter.transform.position + new Vector3 (_DummyCharacter.transform.localScale.x / 2, 0f, 0f);
//		charaPosArray [1] = _DummyCharacter.transform.position + new Vector3 (_DummyCharacter.transform.localScale.x / 2, 0f, 0f);
//		charaPosArray [2] = _DummyCharacter.transform.position + new Vector3 (_DummyCharacter.transform.localScale.x / 2, 0f, 0f);
//		charaPosArray [3] = _DummyCharacter.transform.position + new Vector3 (_DummyCharacter.transform.localScale.x / 2, 0f, 0f); s

		Vector2 moveDir = DummyCard.I.CalcAmountOfMovement(charaPosList, new Vector2(deltaHol, deltaVer));
			
		UpdateCharacterXZPosition(moveDir);
		UpdateCharacterState(moveDir);
	}
	
	private void UpdateCharacterXZPosition(Vector2 moveDir)
	{
		_CharacterX.transform.position += new Vector3(moveDir.x, moveDir.y, 0f);
		_CharacterZ.transform.position += new Vector3(0f, moveDir.y, -moveDir.x);
		_DummyCharacter.transform.Translate(moveDir.x, moveDir.y, 0f);
		
		if(Mathf.Abs(moveDir.x) > 0f)
		{	
			float delta = Mathf.Sign(moveDir.x)*_DummyCharacter.transform.localScale.x/2;
			float foldlineDist = DummyCard.I.CalcFoldLineDistance(_DummyCharacter.transform.position, delta);
			if(Mathf.Abs(foldlineDist) < Mathf.Abs(delta))
			{
				if(_OverFoldLine == false)
				{
					if(_MoveX)
					{
						Vector3 zCharaPos;
						zCharaPos.x = _CharacterX.transform.position.x + foldlineDist;
						zCharaPos.y = _CharacterZ.transform.position.y;
						zCharaPos.z = _CharacterX.transform.position.z + foldlineDist;
						_CharacterZ.transform.position = zCharaPos;
					}
					else
					{
						Vector3 xCharaPos;
						xCharaPos.x = _CharacterZ.transform.position.x - foldlineDist;
						xCharaPos.y = _CharacterX.transform.position.y;
						xCharaPos.z = _CharacterZ.transform.position.z - foldlineDist;
						_CharacterX.transform.position = xCharaPos;
					}
					_EnterDirection = (int)Mathf.Sign(moveDir.x);
					_OverFoldLine = true;
				}
			}
			else if(_OverFoldLine == true)
			{
				if(_EnterDirection == (int)Mathf.Sign(moveDir.x))
				{
					_MoveX = !_MoveX;
				}
				_OverFoldLine = false;
			}
		}
	}
	
	/// <summary>
	/// 移動方向からキャラクターの向きやアニメーションを決定する
	/// </summary>
	private void UpdateCharacterState(Vector2 moveDir)
	{
		//アニメーション
		if(Mathf.Abs(moveDir.x) > 0.01f)
		{
			_CharacterX.GetComponent<Animator>().Play("walk");
			_CharacterZ.GetComponent<Animator>().Play("walk");
		}
		else
		{
			_CharacterX.GetComponent<Animator>().Play("idle");
			_CharacterZ.GetComponent<Animator>().Play("idle");
		}
		//キャラクター向き
		if(moveDir.x > 0f)
		{
			_CharacterX.transform.forward = Vector3.forward;
			_CharacterZ.transform.forward = Vector3.right;
		}
		else if(moveDir.x < 0f)
		{
			_CharacterX.transform.forward = Vector3.back;
			_CharacterZ.transform.forward = Vector3.left;
		}
		//キャラクター部分透過
		UpdateSubTransparent(moveDir);
	}
	
	private void UpdateSubTransparent(Vector2 moveDir)
	{
		int r = 0;
		float delta = _DummyCharacter.transform.localScale.x;
		float foldlineDist = DummyCard.I.CalcFoldLineDistance(_DummyCharacter.transform.position-delta/2*Vector3.right, delta);
		foreach(float x in DummyCard.I.GetSortXCoordList(_DummyCharacter.transform.position.y))
		{
			if(_DummyCharacter.transform.position.x-delta/2 < x)
			{
				if(r == 0) //x方向移動
				{
					if(foldlineDist == delta+1f)
					{
						SetCharacterTransparent(1f,0f,0f,1f);
						return;
					}
					if(moveDir.x > 0f){
						SetCharacterTransparent(foldlineDist/delta,0f,1f,foldlineDist/delta);
						return;
					}
					else if(moveDir.x < 0f)
					{
						SetCharacterTransparent(1f,1f-foldlineDist/delta,1f-foldlineDist/delta,0f);
						return;
					}	
				}
				else //z方向移動
				{
					if(foldlineDist == delta+1f)
					{
						SetCharacterTransparent(0f,1f,1f,0f);
						return;
					}
					if(moveDir.x > 0f){
						SetCharacterTransparent(1f,foldlineDist/delta,foldlineDist/delta,0f);
						return;
					}
					else if(moveDir.x < 0f)
					{
						SetCharacterTransparent(1f-foldlineDist/delta,0f,1f,1f-foldlineDist/delta);
						return;
					}
				}
			}	
			r = (int)Mathf.Repeat(r+1, 2);
		}
		if(foldlineDist == delta+1f)
		{
			SetCharacterTransparent(0f,1f,1f,0f);
			return;
		}
		if(moveDir.x > 0f){
			SetCharacterTransparent(1f,foldlineDist/delta,foldlineDist/delta,0f);
			return;
		}
		else if(moveDir.x < 0f)
		{
			SetCharacterTransparent(1f-foldlineDist/delta,1f,0f,1f-foldlineDist/delta);
			return;
		}
	}
	
	private void SetCharacterTransparent(float xForward, float xBack, float zForward, float zBack)
	{
		_CharacterX.transform.GetChild(0).position = _CharacterX.transform.GetChild(1).position + new Vector3(-0.001f,0f,-0.001f);
		_CharacterZ.transform.GetChild(0).position = _CharacterZ.transform.GetChild(1).position + new Vector3(-0.001f,0f,-0.001f);
		foreach(Material material in _CharacterX.GetComponentsInChildren<Renderer>().Select(x => x.material))
		{
			material.SetFloat("_ForwardThreshold", xForward);
			material.SetFloat("_BackThreshold", xBack);
		}
		foreach(Material material in _CharacterZ.GetComponentsInChildren<Renderer>().Select(x => x.material))
		{
			material.SetFloat("_ForwardThreshold", zForward);
			material.SetFloat("_BackThreshold", zBack);
		}
	}
}
