using UnityEngine;
using System.Collections;

public class InputManager : Singlton<InputManager>
{
	//定数
	public const bool SWIPEMODE_TIME_LIMIT = true;
	public const bool SWIPEMODE_NO_LIMIT = false;
	private const int COUNT_MOUSE_BUTTON_TYPE = 3;
	
	//seriarize field
	[SerializeField]
	private float SwipeValidTime = 1.0f;
	
	//構造体
	struct SwipeInitalInfo{
		public bool validSwipe;
		public float swipeTime;
		public Vector2 touchPos;
		
		public void Start(Vector2 pos)
		{			
			validSwipe = true;
			swipeTime = 0f;
			touchPos = pos;
		}
		
		public void SetInvalid()
		{			
			validSwipe = false;
			swipeTime = Mathf.Infinity;
			touchPos = Vector2.zero;
		}
	}
	
	//tmp
	SwipeInitalInfo swipeInfo = new SwipeInitalInfo();
	
	/// <summary>
	/// 対応するインデックスにおけるタップ開始を取得
	/// </summary>
	public bool GetTapDown(int index = 0)
	{
#if UNITY_EDITOR
		return Input.GetMouseButtonDown(index);
#elif UNITY_IOS || UNITY_ANDROID
		if(Input.touchCount <= index){
			if(index != 0)
				Debug.Log("[InputManager.GetTapDown] Invalid Touch Index : "+index);
			return false;
		}
		return Input.GetTouch(index).phase == TouchPhase.Began;
#else
		return Input.GetMouseButtonDown(index);
#endif
	}
	/// <summary>
	/// 対応するインデックスにおけるタップ継続を取得
	/// </summary>
	public bool GetTap(int index = 0)
	{
#if UNITY_EDITOR
		return Input.GetMouseButton(index);
#elif UNITY_IOS || UNITY_ANDROID
		if(Input.touchCount <= index){
			if(index != 0)
				Debug.Log("[InputManager.GetTap] Invalid Touch Index : "+index);
			return false;
		}
		return Input.GetTouch(index).phase == TouchPhase.Moved || Input.GetTouch(index).phase == TouchPhase.Stationary;
#else
		return Input.GetMouseButton(index);
#endif
	}
	/// <summary>
	/// 対応するインデックスにおけるタップ終了を取得
	/// </summary>
	public bool GetTapUp(int index = 0)
	{
#if UNITY_EDITOR
		return Input.GetMouseButtonUp(index);
#elif UNITY_IOS || UNITY_ANDROID
		if(Input.touchCount <= index){
			if(index != 0)
				Debug.Log("[InputManager.GetTapUp] Invalid Touch Index : "+index);
			return false;
		}
		return Input.GetTouch(index).phase == TouchPhase.Ended;
#else
		return Input.GetMouseButtonUp(index);
#endif
	}
	// <summary>
	/// 対応するインデックスにおけるタップ場所を取得
	/// </summary>
	public Vector2 GetTapPos(int index = 0)
	{
#if UNITY_EDITOR
		return new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
#elif UNITY_IOS || UNITY_ANDROID
		if(Input.touchCount <= index){
			if(index != 0)
				Debug.Log("[InputManager.GetTapPos] Invalid Touch Index : "+index);
			return Vector2.zero;
		}
		return new Vector2(Input.GetTouch(index).position.x/Screen.width, Input.GetTouch(index).position.y/Screen.height);;
#else
		return new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
#endif
	}
	/// <summary>
	/// 任意のタップ開始を取得
	/// </summary>
	public bool GetAnyTapDown()
	{
#if UNITY_EDITOR
		for(int i = 0; i < COUNT_MOUSE_BUTTON_TYPE; i++)
			if(Input.GetMouseButtonDown(i))
				return true;
		return false;
#elif UNITY_IOS || UNITY_ANDROID
		foreach(Touch touch in Input.touches)
			if(touch.phase == TouchPhase.Began)
				return true;
		return false;
#else
		for(int i = 0; i < COUNT_MOUSE_BUTTON_TYPE; i++)
			if(Input.GetMouseButtonDown(i))
				return true;
		return false;
#endif
	}
	/// <summary>
	/// 任意のタップ継続を取得
	/// </summary>
	public bool GetAnyTap()
	{
#if UNITY_EDITOR
		for(int i = 0; i < COUNT_MOUSE_BUTTON_TYPE; i++)
			if(Input.GetMouseButton(i))
				return true;
		return false;
#elif UNITY_IOS || UNITY_ANDROID
		foreach(Touch touch in Input.touches)
			if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
				return true;
		return false;
#else
		for(int i = 0; i < COUNT_MOUSE_BUTTON_TYPE; i++)
			if(Input.GetMouseButton(i))
				return true;
		return false;
#endif
	}
	/// <summary>
	/// 任意のタップ終了を取得
	/// </summary>
	public bool GetAnyTapUp()
	{
#if UNITY_EDITOR
		for(int i = 0; i < COUNT_MOUSE_BUTTON_TYPE; i++)
			if(Input.GetMouseButtonUp(i))
				return true;
		return false;
#elif UNITY_IOS || UNITY_ANDROID
		foreach(Touch touch in Input.touches)
			if(touch.phase == TouchPhase.Ended)
				return true;
		return false;
#else
		for(int i = 0; i < COUNT_MOUSE_BUTTON_TYPE; i++)
			if(Input.GetMouseButtonUp(i))
				return true;
		return false;
#endif
	}
	/// <summary>
	/// 対応するインデックスにおけるタップ情報が指すGameObjectを取得
	/// </summary>
	public GameObject GetTappedGameObject(int index = 0, float maxDistance = Mathf.Infinity)
	{
		Ray ray;
		RaycastHit hit;
#if UNITY_EDITOR
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID
		if(Input.touchCount <= index){
			if(index != 0)
				Debug.Log("[InputManager.GetTappedGameObject] Invalid Touch Index : "+index);
			return null;
		}
		ray = Camera.main.ScreenPointToRay(Input.GetTouch(index).position);
#else
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif
		GameObject selectedObject = null;
		if (Physics.Raycast(ray, out hit, maxDistance)) {
			selectedObject = hit.collider.gameObject;
			return selectedObject;
		}
		return null;
	}
	/// <summary>
	/// 対応するインデックスにおけるタップ情報が指す全てのGameObjectを取得
	/// </summary>
	public GameObject[] GetTappedGameObjects(int index = 0, float maxDistance = Mathf.Infinity)
	{
		Ray ray;
#if UNITY_EDITOR
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID
		if(Input.touchCount <= index){
			if(index != 0)
				Debug.Log("[InputManager.GetTappedGameObject] Invalid Touch Index : "+index);
			return null;
		}
		ray = Camera.main.ScreenPointToRay(Input.GetTouch(index).position);
#else
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif
		RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
		GameObject[] retobjes = new GameObject[hits.Length];
		for(int i = 0; i < hits.Length; i++)
			retobjes[i] = hits[i].collider.gameObject;
		return retobjes;
	}
	/// <summary>
	/// スワイプ情報を返す
	/// </summary>
	public Vector2 GetSwipeDirction(int index = 0, bool swipeMode = SWIPEMODE_TIME_LIMIT)
	{
		Vector2 touchPos;
#if UNITY_EDITOR
		touchPos = Input.mousePosition;
#elif UNITY_IOS || UNITY_ANDROID
		if(Input.touchCount <= index){
			if(index != 0)
				Debug.Log("[InputManager.GetSwipeDirction] Invalid Touch Index : "+index);
			return Vector2.zero;
		}
		touchPos = Input.GetTouch(index).position;
#else
		touchPos = Input.mousePosition;
#endif
		if(GetTapDown(index)){
			swipeInfo.Start(touchPos);
		}else if(GetTapUp(index)){
			if(swipeInfo.validSwipe == false)
				return Vector2.zero;
			Vector2 res = touchPos - swipeInfo.touchPos;
			res = new Vector2(res.x/Screen.width, res.y/Screen.height);
			return res;
		}
		
		if(swipeMode == SWIPEMODE_TIME_LIMIT){
			if(swipeInfo.swipeTime > SwipeValidTime){
				swipeInfo.SetInvalid();
			}else if(swipeInfo.validSwipe){
				swipeInfo.swipeTime += Time.deltaTime;
			}
		}
				
		return Vector2.zero;
	}
}
