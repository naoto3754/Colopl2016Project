using UnityEngine;
using System.Collections;

public class CollectionEvent: EventBase 
{
	protected override void OnAwake()
	{
		base.OnAwake ();
		int chap = StageManager.I.CurrentChapter;
		int book = StageManager.I.CurrentBookID;
		int index = StageManager.I.CurrentStageIndex;

		if (CollectionManager.I [chap, book, index]) {
			this.gameObject.SetActive (false);
			var param = this.GetComponent<StageObjectParameter> ();
			foreach (var obj in param.ObjectsOnStage) {
				obj.SetActive (false);
			}
		}
	}

	protected override void OnEnter()
	{
		int chap = StageManager.I.CurrentChapter;
		int book = StageManager.I.CurrentBookID;
		int index = StageManager.I.CurrentStageIndex;
		CollectionManager.I [chap, book, index] = true;

		base.GetObj (this.gameObject);
		var param = this.GetComponent<StageObjectParameter> ();
		foreach (var obj in param.ObjectsOnStage) {
			base.GetObj (obj);
		}
	}
}
