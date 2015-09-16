using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BookManager : MonoBehaviour {

	[SerializeField]
	private List<GameObject> _Books;
	private List<Vector3> _BookPosList;
	private List<Vector3> _BookRotList;
	private bool _FinishAnimation;
	void Start () {
		_BookPosList = new List<Vector3>();
		_BookRotList = new List<Vector3>();
		foreach(GameObject book in _Books)
		{
			_BookPosList.Add(book.transform.position);
			_BookRotList.Add(book.transform.eulerAngles);
		}
	}
	
	void Update () {
		if(InputManager.I.GetAnyTapDown())
		{
			GameObject tappedObj = InputManager.I.GetTappedGameObject();
			if(tappedObj != null && _FinishAnimation == false)
			{
				int idx = _Books.IndexOf(tappedObj);
				for(int i = 0; i < _Books.Count; i++)
				{
					if(i < idx){
						_Books[i].transform.DOMove(_Books[i].transform.position + 5*Vector3.left, 0.5f);
					}else if(i == idx){
						_Books[i].transform.DOMove(0.5f*Vector3.forward, 0.5f).SetDelay(0.25f);
						_Books[i].transform.DORotate(180*Vector3.up, 0.5f).SetEase(Ease.OutSine).SetDelay(0.25f).OnComplete(() => { _FinishAnimation = true; });
					}else{
						_Books[i].transform.DOMove(_Books[i].transform.position + 5*Vector3.right, 0.5f);
					}
				}
			}
			if(_FinishAnimation)
			{
				_FinishAnimation = false;
				for(int i = 0; i < _Books.Count; i++)
				{
					_Books[i].transform.position = _BookPosList[i];
					_Books[i].transform.eulerAngles = _BookRotList[i];
				}
			}
		}
	}
}
