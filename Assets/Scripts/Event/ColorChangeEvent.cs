using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ColorChangeEvent : EventBase  
{
	[SerializeField]
	ColorData _Color;

	protected override void OnEnter()
	{
		StageManager.I.CurrentController.ChangeColor(_Color);

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
