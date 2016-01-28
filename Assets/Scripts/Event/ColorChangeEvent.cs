using UnityEngine;
using System.Collections;

public class ColorChangeEvent : EventBase  
{
	[SerializeField]
	ColorData _ToColor;
	[SerializeField]
	Color _Color;
	[SerializeField]
	bool _AlwaysEnable;
	[SerializeField]
	bool _OnTop;

	protected override void OnEnter()
	{
		if (_IsGetted)
			return;
		if (StageManager.I.CurrentController == null)
			return;
		if (!(_AlwaysEnable || StageManager.I.CurrentController.IsTopOfWall == _OnTop))
			return;

		StageManager.I.CurrentController.ChangeColor(_ToColor, _Color);
		base.GetObj ();
	}
}
