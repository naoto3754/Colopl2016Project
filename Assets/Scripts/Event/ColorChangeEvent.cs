using UnityEngine;
using System.Collections;

public class ColorChangeEvent : EventBase  
{
	[SerializeField]
	ColorData _ToColor;
	[SerializeField]
	Color _Color;
	[SerializeField]
	bool _OnTop;

	protected override void OnEnter()
	{
		if (StageManager.I.CurrentController == null)
			return;
		if (StageManager.I.CurrentController.IsTopOfWall != _OnTop)
			return;

		StageManager.I.CurrentController.ChangeColor(_ToColor, _Color);

		base.GetObj (this.gameObject);
		var param = this.GetComponent<StageObjectParameter> ();
		foreach (var obj in param.ObjectsOnStage) {
			base.GetObj (obj);
		}
	}
}
