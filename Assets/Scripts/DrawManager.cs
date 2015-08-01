using UnityEngine;
using System.Collections;

public class DrawManager : Singlton<DrawManager>
{
	[SerializeField]
	private GameObject _DrawUnitPrefab;
	[SerializeField]
	private float _DrawIntervalScale;

	private float _DrawInterval;
	private Plane _DrawPlane;
	private Vector3 _PreviousDrawPosition;
	private GameObject _ParentObject;
	
	private int _Cnt;
	
	void Start()
	{
		_DrawInterval = _DrawUnitPrefab.transform.localScale.x*_DrawIntervalScale;
		_DrawPlane = new Plane(new Vector3(0f,0f,0f), new Vector3(-1f,1f,0f), new Vector3(1f,1f,0f));
	}
	
	void Update()
	{
		if(InputManager.I.GetTapDown())
		{
			_Cnt = 1;
			
			Vector3 CurrentPosition = InputManager.I.GetTappedPointAcrossPlane(_DrawPlane);
			_PreviousDrawPosition = CurrentPosition;
			_ParentObject = new GameObject("DrawUnitParent");
			GameObject obj = Instantiate(_DrawUnitPrefab,
									     CurrentPosition,
										 Quaternion.identity) as GameObject;
			obj.transform.parent = _ParentObject.transform;
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
				
				_Cnt += loopCnt;
				
				_PreviousDrawPosition = CurrentPosition;
			}
		}
		else if(InputManager.I.GetTapUp())
		{
			Rigidbody rig = _ParentObject.AddComponent<Rigidbody>();
			rig.constraints = RigidbodyConstraints.FreezeRotationX| 
							  RigidbodyConstraints.FreezeRotationY| 
							  RigidbodyConstraints.FreezePositionZ;
			rig.mass = _Cnt;
							  
		}	
	}
}
