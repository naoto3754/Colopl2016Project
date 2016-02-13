using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CollectionEvent: EventBase 
{
	[SerializeField]
	bool _OnTop;

	protected override void OnAwake()
	{
		base.OnAwake ();
		_Delta = Vector3.zero;
		int chap = StageManager.I.CurrentChapter;
		int book = StageManager.I.CurrentBookID;
		int index = StageManager.I.CurrentStageIndex;

		if (CollectionManager.I [chap, book, index] == CollectionManager.State.COLLECTED) {
			this.gameObject.SetActive (false);
			var param = this.GetComponent<StageObjectParameter> ();
			foreach (var obj in param.ObjectsOnStage) {
				obj.SetActive (false);
			}
		}
	}

	protected override void OnEnter()
	{
		if (StageManager.I.CurrentController == null)
			return;
		if (StageManager.I.CurrentController.IsTopOfWall != _OnTop)
			return;

		StageManager.I.CurrentController.GetCollection = true;
		AudioManager.I.PlaySE (SEConfig.Tag.GET);
		GetCollectionObj ();
	}

	protected void GetCollectionObj()
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
			_GetSequence.Join (rootsprite.DOColorA (0, 1f).SetEase (Ease.Linear));
			foreach (var sprite in obj.GetComponentsInChildren<SpriteRenderer>()) {
				_GetSequence.Join (sprite.DOColorA (0, 1f).SetEase (Ease.Linear));
			}
			foreach (var anim in obj.GetComponentsInChildren<Animator>()) {
				anim.GetComponent<SpriteRenderer> ().color = rootsprite.color;
				anim.Play ("Effect");
			}
		}

		_GetSequence.Play ();
		this.enabled = false;
	}
}
