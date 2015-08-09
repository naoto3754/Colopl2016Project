using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawManager : Singlton<DrawManager>
{
	[SerializeField]
	private GameObject _DrawUnitPrefab;
	[SerializeField]
	private bool _EraseLine;
	[SerializeField]
	private int _MaxNumberOfDraw;
	[SerializeField]
	private float _DrawIntervalScale;

	private List<GameObject> _DrawUnits;
	private float _DrawInterval;
	private Plane _DrawPlane;
	private Vector3 _PreviousDrawPosition;
	private GameObject _ParentObject;
	
	private int _Cnt;
	
	void Start()
	{
		_DrawUnits = new List<GameObject>();
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
			_ParentObject.layer = LayerMask.NameToLayer("DrawObject");
			GameObject obj = Instantiate(_DrawUnitPrefab,
									     CurrentPosition,
										 Quaternion.identity) as GameObject;
			obj.transform.parent = _ParentObject.transform;
			obj.layer = LayerMask.NameToLayer("DrawObject");
			_DrawUnits.Add(obj);
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
					obj.layer = LayerMask.NameToLayer("DrawObject");
					_DrawUnits.Add(obj);
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
			//  _DrawUnits.Clear();		  
		}
		
		
		else if(InputManager.I.GetTapDown(1))
		{
			_Cnt = 1;
			
			Vector3 CurrentPosition = InputManager.I.GetTappedPointAcrossPlane(_DrawPlane);
			_PreviousDrawPosition = CurrentPosition;
			_ParentObject = new GameObject("DrawUnitParent");
			_ParentObject.layer = LayerMask.NameToLayer("DrawObject");
			GameObject obj = Instantiate(_DrawUnitPrefab,
									     CurrentPosition,
										 Quaternion.identity) as GameObject;
			obj.transform.parent = _ParentObject.transform;
			obj.layer = LayerMask.NameToLayer("DrawObject");
			_DrawUnits.Add(obj);
		}
		else if(InputManager.I.GetTap(1))
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
					obj.layer = LayerMask.NameToLayer("DrawObject");
					_DrawUnits.Add(obj);
				}
				
				_Cnt += loopCnt;
				
				_PreviousDrawPosition = CurrentPosition;
			}
		}
		else if(InputManager.I.GetTapUp(1))
		{
			//  _DrawUnits.Clear();
		}	
		if(_EraseLine){
			if(_DrawUnits.Count > _MaxNumberOfDraw)
			{
				for(int i = 0; i < _DrawUnits.Count - _MaxNumberOfDraw; i++){
					Destroy(_DrawUnits[i]);
				}
				_DrawUnits.RemoveRange(0, _DrawUnits.Count - _MaxNumberOfDraw);
			}
		}
	}
}
