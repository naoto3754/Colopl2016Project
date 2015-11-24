using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CollectionEvent: EventBase 
{
	protected override void OnEnter()
	{
		int chap = StageManager.I.CurrentChapter;
		int book = StageManager.I.CurrentBookID;
		int index = StageManager.I.CurrentStageIndex;
		CollectionManager.I [chap, book, index] = true;

		GetObj (this.gameObject);
		var param = this.GetComponent<StageObjectParameter> ();
		foreach (var obj in param.ObjectsOnStage) {
			GetObj (obj);
		}
	}

	private void GetObj(GameObject obj)
	{
		obj.transform.DORotate (4*360*Vector3.up, 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		obj.transform.DOMoveY (this.transform.position.y+4f, 1f).OnComplete(() =>{
			Destroy(obj);
		});
	}
}
