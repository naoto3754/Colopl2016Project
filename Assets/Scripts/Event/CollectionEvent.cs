using UnityEngine;
using System.Collections;

public class CollectionEvent: EventBase 
{
	[SerializeField]
	bool _OnTop;

	protected override void OnAwake()
	{
		base.OnAwake ();
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
		base.GetObj ();
	}
}
