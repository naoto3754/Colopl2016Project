using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class StageSelectManager : Singlton<StageSelectManager> {

	[SerializeField]
	private List<GameObject> _Books;
	public List<GameObject> Books
	{
		get { return _Books; }
	}
	private List<Vector3> _BookPosList;
	private List<Vector3> _BookRotList;
	private List<Vector3> _BookScaleList;
	private bool _FinishAnimation;
	
	public override void OnInitialize () {
		_BookPosList = new List<Vector3>();
		_BookRotList = new List<Vector3>();
		_BookScaleList=new List<Vector3>();
		foreach(GameObject book in _Books)
		{
			_BookPosList.Add(book.transform.position);
			_BookRotList.Add(book.transform.eulerAngles);
			_BookScaleList.Add(book.transform.localScale);
		}
	}
	
	public void Init()
	{
		if(_BookPosList == null)
			OnInitialize();
		for(int i = 0; i < _Books.Count; i++)
		{
			_Books[i].SetActive(true);
			_Books[i].transform.position = _BookPosList[i] + 100*Vector3.up;
			_Books[i].transform.localScale *= 0.5f;
			_Books[i].transform.DOMove(_BookPosList[i], 1f).SetEase(Ease.OutBounce).SetDelay(i*0.2f);
		}
	}
	
	void FixedUpdate () {
		UpdateBookAnimation();
	}
	
	void UpdateBookAnimation()
	{
		if(InputManager.I.GetAnyTapDown())
		{
			//アニメーション戻す
			if(_FinishAnimation)
			{
				_FinishAnimation = false;
				for(int i = 0; i < _Books.Count; i++)
				{
					_Books[i].transform.position = _BookPosList[i];
					_Books[i].transform.eulerAngles = _BookRotList[i];
					_Books[i].transform.localScale = _BookScaleList[i] * 0.5f;
				}
				return;
			}
			//本に触ったらアニメーションさせる
			GameObject tappedObj = InputManager.I.GetTappedGameObject();
			if(tappedObj == null)
				return;
			if(tappedObj != null && _FinishAnimation == false)
			{
				int idx = _Books.IndexOf(tappedObj);
				for(int i = 0; i < _Books.Count; i++)
				{
					if(i < idx){
						_Books[i].transform.DOMove(_Books[i].transform.position + new Vector3(-50, 0, 50), 0.5f);
					}else if(i == idx){
						_Books[i].transform.DOMove(_Books[i].transform.parent.position, 0.5f).SetDelay(0.25f);
						_Books[i].transform.DORotate(315*Vector3.up, 0.5f).SetEase(Ease.OutSine).SetDelay(0.25f);
						_Books[i].transform.GetChild(0).DOLocalRotate(-45*Vector3.up, 0.5f).SetEase(Ease.OutSine).SetDelay(0.25f);
						_Books[i].transform.GetChild(1).DOLocalRotate(-45*Vector3.up, 0.5f).SetEase(Ease.OutSine).SetDelay(0.25f);
						_Books[i].transform.DOScale(_Books[i].transform.localScale*2, 0.5f).SetDelay(0.25f)
							.OnComplete(() => {
								_FinishAnimation = true;
								StateManager.I.BookForStageCreater = _Books[idx];
								StateManager.I.GoState(StateManager.State.INGAME);
							});
					}else{
						_Books[i].transform.DOMove(_Books[i].transform.position + new Vector3(50, 0, -50), 0.5f);
					}
				}
			}
		}
	}
}
