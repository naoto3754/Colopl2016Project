using UnityEngine;
using System.Collections;

public class DrawManager : MonoBehaviour 
{
	[SerializeField]
	private GameObject _DrawUnitPrefab;
	[SerializeField]
	private float _DrawIntervalScale;

	
	private Plane _DrawPlane;
	private Vector3 _PreviousDrawPosition;
	
	void Start()
	{
		_DrawPlane = new Plane(new Vector3(0f,0f,0f), new Vector3(-1f,1f,0f), new Vector3(1f,1f,0f));//(Vector3.back, Vector3.zero);

	}
	
	void Update()
	{
		if(InputManager.I.GetTap())
		{
			Vector3 CurrentPosition = InputManager.I.GetTappedPointAcrossPlane(_DrawPlane);
			if(Vector3.Distance(CurrentPosition, _PreviousDrawPosition)
				> _DrawUnitPrefab.transform.localScale.x*_DrawIntervalScale)
			{
				Instantiate(_DrawUnitPrefab, CurrentPosition, Quaternion.identity);
				_PreviousDrawPosition = CurrentPosition;
			}
		}	
	}
}
