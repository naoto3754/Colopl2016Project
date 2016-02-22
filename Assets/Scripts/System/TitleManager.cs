using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using TMPro;

/*
 * タイトル画面時の挙動を制御
 */
public class TitleManager : Singleton<TitleManager> 
{
	private readonly float ANIM_TIME = 2.5f;
	private readonly float FADE_TIME = 1.5f;
	private readonly float MARGIN = 20;
	private readonly float ROOT2 = Mathf.Sqrt(2);

	[SerializeField]
	private Image _LineBase;
	private Transform _LineRoot;
	private float _Width, _Height, _LineWidth;
	bool _IsTapped;
	bool _LineIsCreated;

	/// <summary>
	/// 初期化
	/// </summary>
	void Awake()
	{
		_LineRoot = _LineBase.transform.parent;
		_Width = Screen.width;
		_Height = Screen.height;
		_LineWidth = Screen.height / 960f;

		foreach(var image in GetComponentsInChildren<Image>())
		{
			image.DOColorA (1f, 0f);
			if (image.material != null && image.material.HasProperty("_MainColor")) {
				image.material.DOMainColorA (1f, 0f);
			}
		}
		foreach (var text in GetComponentsInChildren<TextMeshProUGUI>()) {
			text.DOColorA (1f, 0f);
		}
	}

	void Start()
	{
		AudioManager.I.PlayBGM (BGMConfig.Tag.TITLE);
	}
	/// <summary>
	/// 枠線を描画、アニメーションを登録する
	/// </summary>
	void CreateLine()
	{
		CreateOuter (ANIM_TIME, ANIM_TIME);
		CreateHolLong (ANIM_TIME, 0f);
		CreateVerLong (ANIM_TIME, 0.5f);
		CreateInner (ANIM_TIME, ANIM_TIME/2);
		CreateDiagonal ();

		_LineBase.gameObject.SetActive (false);
	}
	/// <summary>
	/// 一番外側の線
	/// </summary>
	void CreateOuter (float time, float delay)
	{
		float width = _Width-_LineWidth*MARGIN/3;
		float height = _Height-_LineWidth*MARGIN/3;
		Vector2[] path = new Vector2[] {
			new Vector2(0, height/2),
			new Vector2(width/2, height/2),
			new Vector2(width/2, -height/2),
			new Vector2(-width/2, -height/2),
			new Vector2(-width/2, height/2),
		};
		CreateLineFromPath (path, time, delay);
	}
	void CreateHolLong (float time, float delay)
	{
		float width = _Width-_LineWidth*MARGIN;
		float height = _Height-_LineWidth*MARGIN*3;
		Vector2[] path = new Vector2[] {
			new Vector2(0, height/2),
			new Vector2(-width/2, height/2),
			new Vector2(-width/2, -height/2),
			new Vector2(width/2, -height/2),
			new Vector2(width/2, height/2),
		};
		CreateLineFromPath (path, time, delay);
	}
	void CreateVerLong (float time, float delay)
	{
		float width = _Width-_LineWidth*MARGIN*3;
		float height = _Height-_LineWidth*MARGIN;
		Vector2[] path = new Vector2[] {
			new Vector2(0, height/2),
			new Vector2(width/2, height/2),
			new Vector2(width/2, -height/2),
			new Vector2(-width/2, -height/2),
			new Vector2(-width/2, height/2),
		};
		CreateLineFromPath (path, time, delay);
	}
	void CreateInner (float time, float delay)
	{
		float size = _LineWidth * MARGIN;
		float width = _Width-size*4;
		float height = _Height-size*4;
		Vector2[] path = new Vector2[] {
			new Vector2(width/2, height/2),
			new Vector2(width/2, height/2-size),
			new Vector2(width/2-size*2, height/2-size),
			new Vector2(width/2-size*3, height/2),
			new Vector2(-width/2+size*3, height/2),
			new Vector2(-width/2+size*2, height/2-size),
			new Vector2(-width/2, height/2-size),

			new Vector2(-width/2, height/2),
			new Vector2(-width/2+size, height/2),
			new Vector2(-width/2+size, height/2-size*2),
			new Vector2(-width/2, height/2-size*3),
			new Vector2(-width/2, -height/2+size*3),
			new Vector2(-width/2+size, -height/2+size*2),
			new Vector2(-width/2+size, -height/2),
			new Vector2(-width/2, -height/2),

			new Vector2(-width/2, -height/2+size),
			new Vector2(-width/2+size*2, -height/2+size),
			new Vector2(-width/2+size*3, -height/2),
			new Vector2(width/2-size*3, -height/2),
			new Vector2(width/2-size*2, -height/2+size),
			new Vector2(width/2, -height/2+size),
			new Vector2(width/2, -height/2),

			new Vector2(width/2-size, -height/2),
			new Vector2(width/2-size, -height/2+size*2),
			new Vector2(width/2, -height/2+size*3),
			new Vector2(width/2, height/2-size*3),
			new Vector2(width/2-size, height/2-size*2),
			new Vector2(width/2-size, height/2),
		};
		CreateLineFromPath (path, time, delay);
	}
	void CreateDiagonal ()
	{
		float size = _LineWidth * MARGIN;
		float width = _Width - size * 13;
		float height = _Height - size * 6.5f;
		Ease ease = Ease.OutExpo;

		CustomInstantiate (0, height / 2 + size / 3, 0, 0).DOWidth (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, height / 2 + size / 3, 180, 0).DOWidth (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, height / 2 - size / 3, 0, 0).DOWidth (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, height / 2 - size / 3, 180, 0).DOWidth (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, height / 2 - size / 3, 90, size * 2 / 3).DOLocalMoveX (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, height / 2 - size / 3, 90, size * 2 / 3).DOLocalMoveX (-width / 2, ANIM_TIME * 2).SetEase(ease);

		CustomInstantiate (0, -height / 2 + size / 3, 0, 0).DOWidth (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, -height / 2 + size / 3, 180, 0).DOWidth (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, -height / 2 - size / 3, 0, 0).DOWidth (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, -height / 2 - size / 3, 180, 0).DOWidth (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, -height / 2 - size / 3, 90, size * 2 / 3).DOLocalMoveX (width / 2, ANIM_TIME * 2).SetEase(ease);
		CustomInstantiate (0, -height / 2 - size / 3, 90, size * 2 / 3).DOLocalMoveX (-width / 2, ANIM_TIME * 2).SetEase(ease);

		CustomInstantiate (0, height / 2 - size, 0, 0).DOWidth (width / 2, ANIM_TIME).SetDelay(ANIM_TIME);
		CustomInstantiate (0, height / 2 - size, 180, 0).DOWidth (width / 2, ANIM_TIME).SetDelay (ANIM_TIME);

		CustomInstantiate (0, -height / 2 + size, 0, 0).DOWidth (width / 2, ANIM_TIME).SetDelay (ANIM_TIME);
		CustomInstantiate (0, -height / 2 + size, 180, 0).DOWidth (width / 2, ANIM_TIME).SetDelay (ANIM_TIME);

		int count = (int)(width / 2 / (size * 2 / 3));
		for(int n = 0; n < count; n++)
		{
			float length = size * ROOT2 * 2 / 3;
			float time = ANIM_TIME / count;
			float delay = ANIM_TIME * (1 + Mathf.Pow((float)n/(count-1), 1.5f));
			CustomInstantiate (size * 2 / 3 * (n + 1), height / 2 + size / 3, 225, 0).DOWidth (length, time).SetDelay (delay);
			CustomInstantiate (-size * 2 / 3 * n, height / 2 + size / 3, 225, 0).DOWidth (length, time).SetDelay (delay);

			CustomInstantiate (size * 2 / 3 * (n + 1), -height / 2 + size / 3, 225, 0).DOWidth (length, time).SetDelay (delay);
			CustomInstantiate (-size * 2 / 3 * n, -height / 2 + size / 3, 225, 0).DOWidth (length, time).SetDelay (delay);
		}
	}

	void CreateLineFromPath(Vector2[] path, float time, float delay)
	{
		Sequence seq = DOTween.Sequence ();
		int length = path.Length;
		float totalDist = 0;
		for (int i = 0; i < length; i++) {
			int nextI = (i+1)%length;
			Vector2 diff = path [nextI]-path [i];
			totalDist += diff.magnitude;
		}
		for(int i = 0; i < length; i++)
		{
			int nextI = (i+1)%length;
			Vector2 diff = path [nextI]-path [i];
			float distance = diff.magnitude;
			float angle = Mathf.Atan2(diff.y, diff.x)*Mathf.Rad2Deg;
			var rectTrans = CustomInstantiate (path[i].x, path[i].y, angle, 0);
			float d = i==0 ? delay : 0f;
			seq.Append (rectTrans.DOWidth (distance + _LineWidth, time*distance/totalDist).SetEase (Ease.Linear).SetDelay(d));
		}
		seq.Play ();
	}

	RectTransform CustomInstantiate(float x, float y, float angle, float length)
	{
		var line = Instantiate(_LineBase.gameObject) as GameObject;
		line.transform.SetParent(_LineRoot);
		var rectTrans = line.GetComponent<RectTransform> ();
		rectTrans.localPosition = new Vector3(x,y,0);
		rectTrans.localEulerAngles = angle*Vector3.forward;
		rectTrans.localScale = Vector3.one;
		rectTrans.sizeDelta = new Vector2 (length, _LineWidth+0.5f);
		return rectTrans;
	}

	void Update()
	{
		if (Application.isShowingSplashScreen)
			return;

		if (_LineIsCreated == false) {
			_LineIsCreated = true;
			CreateLine ();
		}

		if (_IsTapped)
			return;
		
		if(InputManager.I.GetAnyTapDown())
		{
			_IsTapped = true;
			Sequence seq = DOTween.Sequence ();
			foreach(var image in GetComponentsInChildren<Image>())
			{
				seq.Join( image.DOColorA (0,FADE_TIME) );
				if (image.material != null && image.material.HasProperty("_MainColor")) {
					seq.Join( image.material.DOMainColorA (0, FADE_TIME) );
				}
			}
			foreach (var text in GetComponentsInChildren<TextMeshProUGUI>()) {
				seq.Join( text.DOColorA (0, FADE_TIME) );
			}
			seq.OnComplete (() => {
				StateManager.I.GoState(GameState.STAGE_SELECT);	
			});
		}
	}
}
