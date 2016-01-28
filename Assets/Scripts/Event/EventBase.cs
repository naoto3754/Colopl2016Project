using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EventBase : MonoBehaviour {
	[SerializeField]
	protected bool _EnableOnCharacter = true;
//	[SerializeField]
//	protected bool _EnableOnTouch;

	private Sequence _GetSequence;
	protected bool _IsGetted;
	protected Rectangle _Rect;
	private bool _IsStaying;
	public Vector3 DefalutPos {
		get;
		private set;
	}
	public Quaternion DefalutRot {
		get;
		private set;
	}

	void Awake()
	{
		OnAwake ();
	}

	protected virtual void OnAwake()
	{
		DefalutPos = transform.position;
		DefalutRot = transform.rotation;
		_Rect = new Rectangle (transform.position, transform.lossyScale.x, transform.lossyScale.y);
	}

	void Update()
	{
		if (StageAnimator.I != null && StageAnimator.I.IsPlayingAnimation)
			return;

		if (_EnableOnCharacter) {
			if (StageManager.I.CurrentController != null && _Rect.IsOverlaped(StageManager.I.CurrentController.CharaRect))
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

	public void Reset()
	{	
		if (_IsGetted == false)
			return;

		_GetSequence.Complete();
		_IsGetted = false;
		this.enabled = true;

		var param = this.GetComponent<StageObjectParameter> ();
		foreach (var obj in param.ObjectsOnStage) {
			var stagesprite = obj.GetComponent<SpriteRenderer>();
			stagesprite.DOColorA (1,0);
			foreach (var sprite in obj.GetComponentsInChildren<SpriteRenderer>()) {
				sprite.DOColorA (1,0);
			}	
			obj.transform.position -= 4*Vector3.up;
		}
	}

	protected void GetObj()
	{
		if (_IsGetted)
			return;
		
		_IsGetted = true;
		var param = this.GetComponent<StageObjectParameter> ();
		foreach (var obj in param.ObjectsOnStage) {
			
			Vector3 angle = obj.transform.localEulerAngles;
			angle.z += 360;
			_GetSequence = DOTween.Sequence ();

			var rootsprite = obj.GetComponent<SpriteRenderer> ();
			_GetSequence.Join (rootsprite.DOColorA (0, 1f).SetEase (Ease.InQuad));
			foreach (var sprite in obj.GetComponentsInChildren<SpriteRenderer>()) {
				_GetSequence.Join (sprite.DOColorA (0, 1f).SetEase (Ease.InQuad));
			}

			_GetSequence.Join (obj.transform.DOLocalRotate (angle, 1f, RotateMode.FastBeyond360).SetEase (Ease.Linear));
			_GetSequence.Join ( obj.transform.DOMoveY (this.transform.position.y + 4f, 1f) );
		}

		_GetSequence.Play ();
		this.enabled = false;
	}
}
