using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EventBase : MonoBehaviour {
	[SerializeField]
	protected bool _EnableOnCharacter = true;
//	[SerializeField]
//	protected bool _EnableOnTouch;

	private bool _IsGetted;
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

		_IsGetted = false;
		this.enabled = true;

		var rootsprite = this.GetComponent<SpriteRenderer>();
		Color rc = rootsprite.color;
		rc.a = 0;
		rootsprite.color = rc;
		foreach (var sprite in this.GetComponentsInChildren<SpriteRenderer>()) {
			Color c = sprite.color;
			c.a = 0;
			sprite.color = c;
		}

		transform.position -= 4*Vector3.up;
		var param = this.GetComponent<StageObjectParameter> ();
		foreach (var obj in param.ObjectsOnStage) {
			var stagesprite = this.GetComponent<SpriteRenderer>();
			Color sc = stagesprite.color;
			sc.a = 0;
			stagesprite.color = sc;
			foreach (var sprite in this.GetComponentsInChildren<SpriteRenderer>()) {
				Color c = sprite.color;
				c.a = 0;
				sprite.color = c;
			}	
			obj.transform.position -= 4*Vector3.up;
		}
	}

	protected void GetObj(GameObject obj)
	{
		_IsGetted = true;
		Vector3 angle = obj.transform.localEulerAngles;
		angle.z += 360;

		var rootsprite = obj.GetComponent<SpriteRenderer>();
		Color rc = rootsprite.color;
		rc.a = 0;
		rootsprite.DOColor (rc, 1f).SetEase(Ease.InQuad);
		foreach (var sprite in obj.GetComponentsInChildren<SpriteRenderer>()) {
			Color c = sprite.color;
			c.a = 0;
			sprite.DOColor (c, 1f).SetEase(Ease.InQuad);
		}

		obj.transform.DOLocalRotate (angle, 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear);		
		obj.transform.DOMoveY (this.transform.position.y+4f, 1f).OnComplete(() =>{
			if(obj.GetComponent<EventBase>() != null)
				obj.GetComponent<EventBase>().enabled = false;
		});
	}
}
