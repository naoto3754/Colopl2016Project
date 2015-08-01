using UnityEngine;
using System.Collections;

public class DrawManager : MonoBehaviour 
{
	[SerializeField]
	private GameObject _DrawUnitPrefab;
	[SerializeField]
	private float _DrawIntervalScale;

	private float _DrawInterval;
	private Plane _DrawPlane;
	private Vector3 _PreviousDrawPosition;
	private GameObject _ParentObject;
	
	void Start()
	{
		_DrawInterval = _DrawUnitPrefab.transform.localScale.x*_DrawIntervalScale;
		_DrawPlane = new Plane(new Vector3(0f,0f,0f), new Vector3(-1f,1f,0f), new Vector3(1f,1f,0f));
	}
	
	void Update()
	{
		if(InputManager.I.GetTapDown())
		{
			Vector3 CurrentPosition = InputManager.I.GetTappedPointAcrossPlane(_DrawPlane);
			_PreviousDrawPosition = CurrentPosition;
			_ParentObject = new GameObject("DrawUnitParent");
		}
		else if(InputManager.I.GetTap())
		{
			Vector3 CurrentPosition = InputManager.I.GetTappedPointAcrossPlane(_DrawPlane);
			float distance = Vector3.Distance(CurrentPosition, _PreviousDrawPosition); 
			if(distance > _DrawInterval)
			{
				int loopCnt = (int)(distance / _DrawInterval);
				for(int i = 0; i < loopCnt; i++){
					GameObject obj = Instantiate(_DrawUnitPrefab,
												 Vector3.Lerp(_PreviousDrawPosition, CurrentPosition, (float)i/loopCnt),
												 Quaternion.identity) as GameObject;
					obj.transform.parent = _ParentObject.transform;
				}
				_PreviousDrawPosition = CurrentPosition;
			}
		}
		else if(InputManager.I.GetTapUp())
		{
			_ParentObject.AddComponent<Rigidbody>();
		}	
	}
}
