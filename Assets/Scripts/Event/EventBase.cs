using UnityEngine;
using System.Collections;

public class EventBase : MonoBehaviour {
	[SerializeField]
	protected bool _EnableOnCharacter;
//	[SerializeField]
//	protected bool _EnableOnTouch;

	protected Rectangle _Rect;
	private bool _IsStaying;

	void Awake()
	{
		_Rect = new Rectangle (transform.position, transform.lossyScale.x, transform.lossyScale.y);
	}

	void Update()
	{
		if (_EnableOnCharacter) {
			if (CharacterController.I == null)
				return;
			
			if (_Rect.Contains (CharacterController.CharaParam.Bottom))
				Enter ();
			else
				Exit ();
		}

//		if (_EnableOnTouch) {
//			
//		}
	}

	private void Enter()
	{
		if (_IsStaying) {
			OnStay ();
		} else {
			_IsStaying = true;
			OnEnter ();
		}
	}

	private void Exit()
	{
		if (_IsStaying) {
			_IsStaying = false;
			OnExit ();
		}
	}

	protected virtual void OnEnter()
	{
	}

	protected virtual void OnStay()
	{
	}

	protected virtual void OnExit()
	{
	}
}
