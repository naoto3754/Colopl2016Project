using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class CustomCharaController : MonoBehaviour
{
    private GameObject _CharacterX;
    private GameObject _CharacterZ;
    private GameObject _DestCharacterX;
    private GameObject _DestCharacterZ;
	private GameObject _FlatCharacter;

    public Vector2 InitPosition
    {
        get; set;
    }

    private float _Speed = 6;
    private float _DropSpeed = 4;
	public ColorData color {
		get;
		set;
	}

    //キャラクターの状態を表すプロパティ
    public bool IsTopOfWall
    {
        get; set;
    }
	public bool IsTopOfWall_Dest
	{
		get; set;
	}
    public bool CanUseLadder
    {
        get; set;
    }
	public bool ClearStage {
		get; set;
	}
	public bool GetCollection {
		get;
		set;
	}

	public void Init()
	{
		StageManager.I.CurrentController = this;
		_FlatCharacter = this.gameObject;
		color = ColorData.COLOR1;
		InitPosition = FlatTrans.position;

		_CharacterX = CreateCharacter (StageManager.I.CurrentInfo.InitialCharacterColor, true, true);
		_CharacterZ = CreateCharacter (StageManager.I.CurrentInfo.InitialCharacterColor, false, true);
		_DestCharacterX = CreateCharacter (Color.black, true, false);
		_DestCharacterZ = CreateCharacter (Color.black, false, false);

		UpdateDummyCharacterPosition(0.01f*Vector2.right);
	}
	private GameObject CreateCharacter(Color initColor,bool xDir,bool main)
	{
		GameObject character = Instantiate(_FlatCharacter, Vector3.zero, Quaternion.identity) as GameObject;
		if(xDir == false)
			character.transform.Rotate(0f, 90f, 0f);
		character.transform.SetParent(StageManager.I.DecoRoot.transform);
		character.tag = xDir ? StageCreater.X_TAG_NAME : StageCreater.Z_TAG_NAME;
		//TODO:色を決める
		var face = character.transform.GetChild (0).GetComponent<SpriteRenderer> ();
		var body = character.transform.GetChild (1).GetComponent<SpriteRenderer> ();
		face.material.SetColor("_MainColor", Color.white);
		body.material.SetColor("_MainColor", initColor);
		if (main == false) {
			face.material.SetFloat("_MaskWeight", 1f);
			body.material.SetFloat("_MaskWeight", 1f);	
			face.transform.Rotate (0,180,0);
			body.transform.Rotate (0,180,0);
		}
		if(xDir)
			ColorManager.MultiplyShadowColor(character.transform.GetChild(1).gameObject);
		Destroy (character.GetComponent<CustomCharaController> ());
		return character;
	}

    void Update()
	{
		if (ClearStage)
			return;
		
		if (InputManager.I.GetDoubleTap ()) {
			StageAnimator.I.Reverse ();
		}

        //アニメーション中はキャラクターを動かさない
		if (StageAnimator.I.IsPlayingAnimation)
			return;

		if (Input.GetKeyDown (KeyCode.C) || (StageManager.I.CurrentInfo.GoalObj != null &&
		    StageManager.I.CurrentInfo.GoalObj.Rect.Contains (Bottom))) {
			ClearAction ();
		}
			
		Vector2 moveDir = StageManager.I.CalcAmountOfMovement(CalcInputDir());

        UpdateDummyCharacterPosition(moveDir);
		JumppingOff ();
        UpdateCharacterState(moveDir);
    }

	private Vector2 CalcInputDir(){
		//入力を取得
		float deltaHol = Time.deltaTime * _Speed * Input.GetAxis("Horizontal");
		float deltaVer = Time.deltaTime * _Speed * Input.GetAxis("Vertical");

		if (InputManager.I.GetTapDown(0) || InputManager.I.GetTap(0))
		{
			Vector2 inputDir = InputManager.I.GetDistanceFromInitPos(0);
			inputDir.x = Mathf.Clamp(inputDir.x * 5, -1, 1);
			inputDir.x = Mathf.Abs(inputDir.x) < 0.5f ? 0f : inputDir.x;
			inputDir.y = Mathf.Clamp(inputDir.y * 7.5f, -1, 1);
			inputDir.y = Mathf.Abs(inputDir.y) < 0.5f ? 0f : inputDir.y;


			deltaHol = Time.deltaTime * _Speed * inputDir.x;
			deltaVer = Time.deltaTime * _Speed * inputDir.y;
		}

		float deltaDrop = Time.deltaTime * _DropSpeed;

		if (!CanUseLadder){
			deltaVer = -deltaDrop;
		}else if (deltaVer > Ladder.MovementLimit){
			deltaVer = Ladder.MovementLimit + 0.01f;
		}

		return new Vector2 (deltaHol, deltaVer);
	}

	private void JumppingOff()
	{
		bool down = Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S);
		if (InputManager.I.GetTapDown(0) || InputManager.I.GetTap(0) || down)
		{
			Vector2 inputDir = InputManager.I.GetDistanceFromInitPos(0);
			inputDir.y = Mathf.Clamp(inputDir.y * 15, -1, 1);
			inputDir.y = Mathf.Abs(inputDir.y) < 0.5f ? 0f : inputDir.y;
			bool inputDown = down || inputDir.y < -0.8f;
			bool canFall = (IsTopOfWall && StageManager.I.CanFall ()) || StageManager.I.OnJumpOffLine ();
			if ( inputDown && canFall )
			{
				FlatTrans.position -= 0.05f * Vector3.up;
			}
		}
	}

    /// <summary>
    /// 移動量を計算し、キャラの位置を更新
    /// </summary>
    public void UpdateDummyCharacterPosition(Vector2 moveDir)
    {
        FlatTrans.position += new Vector3(moveDir.x, moveDir.y, 0f);
        //飛び出ている部分の上に乗っているか判定
        if (IsTopOfWall)
            IsTopOfWall = StageManager.I.OnTopOfWall(BottomLeft, BottomRight);
		IsTopOfWall_Dest = IsTopOfWall && StageManager.I.OnTopOfWall(DestBottomLeft, DestBottomRight);
		IEnumerable foldXList = StageManager.I.GetFoldXCoordList(Bottom.y, IsTopOfWall);
		IEnumerable foldXList_Dest = StageManager.I.GetFoldXCoordList(Bottom.y, IsTopOfWall_Dest);

        // ダミーキャラの位置を実際のキャラ反映させる
        UpdateXZCharacterPosition(Bottom, Scale.x,
                                  _CharacterX.transform, _CharacterZ.transform,
                                  moveDir, foldXList, IsTopOfWall);
        UpdateXZCharacterPosition(DestBottom, Scale.x,
                                  _DestCharacterX.transform, _DestCharacterZ.transform,
								  moveDir, foldXList_Dest, IsTopOfWall_Dest);

        //キャラクター部分透過
        UpdateSubTransparent(Bottom, FlatTrans.lossyScale.x,
							 _CharacterX, _CharacterZ, moveDir, foldXList, IsTopOfWall, true);
        UpdateSubTransparent(DestBottom, FlatTrans.lossyScale.x,
							 _DestCharacterX, _DestCharacterZ, moveDir, foldXList, IsTopOfWall_Dest, false);
		
		float alpha = StageManager.I.IsOnObstacle () ? 0.2f : 0.8f;
		foreach (var destChara in DestCharacters) {
			foreach (Material material in destChara.GetComponentsInChildren<Renderer>().Select(x => x.material)) {
				Color c = material.GetColor ("_MainColor");
				c.a = alpha;
				material.SetColor ("_MainColor", c);
			}
		}
    }
    /// <summary>
    /// ダミーキャラの位置を実際のキャラ反映させる
    /// </summary>
	private void UpdateXZCharacterPosition(Vector3 charaPos, float delta, Transform xTrans, Transform zTrans, Vector2 moveDir, IEnumerable foldXList, bool isTop)
    {
        int r = 0;
        float foldlineDist = StageManager.I.CalcFoldLineDistance(charaPos - delta / 2 * Vector3.right, delta, isTop);
        float prevX = -StageManager.I.CurrentInfo.StageWidth / 2;
		float xOffset = StageManager.I.Offset.x - StageManager.I.CurrentInfo.StageWidth / 2;
		float zOffset = StageManager.I.Offset.z;
        float charaAnchor = charaPos.x - delta / 2;
		if (charaAnchor < prevX) 
		{
			xTrans.position = new Vector3(StageManager.I.Offset.x + charaPos.x - 0.01f, FlatTrans.position.y, zOffset - 0.01f);
			zTrans.position = xTrans.position + new Vector3(1,0,1) * (foldlineDist-delta/2);
			return;
		}
        foreach (float x in foldXList)
        {
            if (prevX < charaAnchor && charaAnchor < x)
            {
                if (r == 0) //x方向移動
                {
					xTrans.position = new Vector3(xOffset + charaPos.x - prevX - 0.01f, FlatTrans.position.y, zOffset - 0.01f);
					zTrans.position = xTrans.position + new Vector3(1,0,1) * (foldlineDist-delta/2);
					return;
                }
                else //z方向移動
                {
					zTrans.position = new Vector3(xOffset - 0.01f, FlatTrans.position.y, zOffset - charaPos.x + prevX - 0.01f);
					xTrans.position = zTrans.position + new Vector3(-1,0,-1) * (foldlineDist-delta/2);
					return;
                }
            }
			else
			{
                if(r==0) xOffset += x - prevX;
                else     zOffset -= x - prevX;
            }
            prevX = x;
            r = (int)Mathf.Repeat(r + 1, 2);
        }
    }

    /// <summary>
    /// キャラクターの部分透過を設定
    /// </summary>
	private void UpdateSubTransparent(Vector3 charaPos, float delta, GameObject xChara, GameObject zChara, Vector2 moveDir, IEnumerable foldXList, bool isTop, bool main)
    {
        int r = 0;
		float foldlineDist = StageManager.I.CalcFoldLineDistance(charaPos - delta / 2 * Vector3.right, delta, isTop);
        foreach (float x in foldXList)
        {
            if (charaPos.x - delta / 2 < x)
            {
				if (CalcTransparentParam (xChara, zChara, foldlineDist, delta, moveDir, r == 0 /*r=0ならx方向*/, main))
					return;
            }
            r = (int)Mathf.Repeat(r + 1, 2);
        }
		CalcTransparentParam (xChara, zChara, foldlineDist, delta, moveDir, false/*z方向*/, main);
    }
	/// <summary>
	/// キャラクター透過用パラメータの決定
	/// </summary>
	private bool CalcTransparentParam(GameObject xChara, GameObject zChara, float foldlineDist, float delta, Vector2 moveDir, bool xDir, bool main)
	{
		float rate = foldlineDist / delta;
		Vector4 p = Vector4.zero;
		if (foldlineDist == delta + 1f) {
			p = xDir ? new Vector4 (1, 0, 0, 1) : new Vector4 (0, 1, 1, 0);
		} else if (moveDir.x > 0f) {
			p = xDir ? new Vector4 (rate, 0, 1, rate) : new Vector4 (1, rate, rate, 0);
		} else if (moveDir.x < 0f) {
			p = xDir ? new Vector4 (1, 1f - rate, 1f - rate, 0) : new Vector4 (1f - rate, 0, 1, 1f - rate);
		} else {
			return false;
		}
		SetCharacterTransparent(xChara, zChara, p.x, p.y, p.z, p.w, main);
		return true;
	}
	/// <summary>
	/// キャラクター透過用関数
	/// </summary>
	private void SetCharacterTransparent(GameObject xChara, GameObject zChara, float xForward, float xBack, float zForward, float zBack, bool main)
	{ 
		float xforward = main ? xForward : 1-xBack;
		float xback = main ? xBack : 1-xForward;
		float zforward = main ? zForward : 1-zBack;
		float zback = main ? zBack : 1 - zForward;
        foreach (Material material in xChara.GetComponentsInChildren<Renderer>().Select(x => x.material))
        {
            material.SetFloat("_ForwardThreshold", xforward);
            material.SetFloat("_BackThreshold", xback);
        }
        foreach (Material material in zChara.GetComponentsInChildren<Renderer>().Select(x => x.material))
        {
            material.SetFloat("_ForwardThreshold", zforward);
            material.SetFloat("_BackThreshold", zback);
        }
    }
    /// <summary>
    /// 移動方向からキャラクターの向きやアニメーションを決定する
    /// </summary>
    public void UpdateCharacterState(Vector2 moveDir)
    {
		UpdateCharacterAnimation (moveDir);
        //キャラクター向き
        if (moveDir.x > 0f)
        {
            _CharacterX.transform.forward = Vector3.forward;
            _CharacterZ.transform.forward = Vector3.right;
            _DestCharacterX.transform.forward = Vector3.forward;
            _DestCharacterZ.transform.forward = Vector3.right;
        }
        else if (moveDir.x < 0f)
        {
            _CharacterX.transform.forward = Vector3.back;
            _CharacterZ.transform.forward = Vector3.left;
            _DestCharacterX.transform.forward = Vector3.back;
            _DestCharacterZ.transform.forward = Vector3.left;
        }
    }

	private void UpdateCharacterAnimation(Vector2 moveDir)
	{
		if (CanUseLadder) {
			foreach (var anim in AllCharacters.Select(x => x.GetComponent<Animator>())) {
				var stateInfo = anim.GetCurrentAnimatorStateInfo (0);
				if (stateInfo.IsName ("Ladder")) {
					if (moveDir.magnitude > 0.01f) {
						anim.speed = 1;
					} else {
						anim.speed = 0;
					}
				} else {
					anim.Play ("Ladder");
				}

			}
		} else if (moveDir.y < -0.05f) {
			foreach (var anim in AllCharacters.Select(x => x.GetComponent<Animator>())) {
				var stateInfo = anim.GetCurrentAnimatorStateInfo (0);
				if (!stateInfo.IsName ("Fall")) {
					anim.Play ("Fall");
				}
			}
		}
		else if (Mathf.Abs(moveDir.x) > 0.01f){			
			foreach (var anim in AllCharacters.Select(x => x.GetComponent<Animator>())) {
				var stateInfo = anim.GetCurrentAnimatorStateInfo (0);
				if (stateInfo.IsName ("Fall")) {
					anim.Play ("Walk");
				} else if (stateInfo.IsName ("FallOUT")) {
					anim.Play ("WalkIN", -1, 1 - stateInfo.normalizedTime);
				} else if (stateInfo.IsName ("Wait")) {
					anim.Play ("WalkIN");
				} else if (stateInfo.IsName ("WalkOUT")) {
					anim.Play ("WalkIN", -1, 1 - stateInfo.normalizedTime);
				} else if (stateInfo.IsName ("WalkIN")) {
					if (stateInfo.normalizedTime > 0.9f) {
						anim.Play ("Walk");
					}
				} else if (stateInfo.IsName ("Ladder")) {
					anim.Play ("Wait");
				}
			}
			if (AudioManager.I.IsPlayingSE (AudioContents.AudioTitle.WALK) == false) {
				AudioManager.I.PlaySE (AudioContents.AudioTitle.WALK);
			}
		}else{
			foreach (var anim in AllCharacters.Select(x => x.GetComponent<Animator>())) {
				var stateInfo = anim.GetCurrentAnimatorStateInfo (0);
				if (stateInfo.IsName ("Fall")) {
					anim.Play ("FallOUT");
				} else if (stateInfo.IsName ("FallOUT")) {
					if (stateInfo.normalizedTime > 0.9f) {
						anim.Play ("Wait");
					}
				}else if (stateInfo.IsName ("Walk")) {
					anim.Play ("WalkOUT");
				} else if(stateInfo.IsName ("WalkIN")){
					anim.Play ("WalkOUT", -1, 1 - stateInfo.normalizedTime);
				} else if(stateInfo.IsName ("WalkOUT")){
					if (stateInfo.normalizedTime > 0.9f) {
						anim.Play ("Wait");
					}
				} else if (stateInfo.IsName ("Ladder")) {
					anim.Play ("Wait");
				}
			}
			AudioManager.I.StopSE (AudioContents.AudioTitle.WALK);
		}
	}

	public void SwapCharacter()
	{
		UnityUtility.SwapGameObject (_CharacterX, _DestCharacterX);
		UnityUtility.SwapGameObject (_CharacterZ, _DestCharacterZ);
	}

	public void ChangeColor(ColorData cd, Color c)
	{
		color = cd;
		foreach (var body in Characters.Select(x => x.transform.GetChild (1).GetComponent<SpriteRenderer> ())) {
			body.material.SetColor ("_MainColor", c);
		}
	}
	/// <summary>
	/// クリア時の処理
	/// </summary>
	private void ClearAction()
	{
		int chapter = StageManager.I.CurrentChapter;
		int bookID = StageManager.I.CurrentBookID;
		int stageIndex = StageManager.I.CurrentStageIndex;
		int index = StageManager.CalcStageListIndex (chapter, bookID, stageIndex);

		Vector3 goalPos = StageManager.I.CurrentInfo.GoalObj.GetComponent<StageObjectParameter> ().ObjectsOnStage [0].transform.position;
		ClearStage = true;
		Sequence sequence = DOTween.Sequence ();
		sequence.Join ( _CharacterX.transform.DOMove(goalPos-Vector3.up*(1f+Scale.y*ASPECT_RATE/6f), 1f) );
		sequence.Join ( _CharacterZ.transform.DOMove(goalPos-Vector3.up*(1f+Scale.y*ASPECT_RATE/6f), 1f) );
		foreach(var chara in AllCharacters){
			foreach (Material mat in chara.GetComponentsInChildren<Renderer>().Select(x => x.material))
				sequence.Join ( mat.DOMainColor (new Color(1f,1f,1f,0f), 1f) );
		}
			
		//コレクション
		if(GetCollection){
			CollectionManager.I [chapter, bookID, stageIndex] = CollectionManager.State.COLLECTED;
		}

		StageClearManager.I.ClearStage(index);

		sequence.OnComplete (() => {
			if(stageIndex == 2){
				InGameManager.I.OnReturnHome();
			}else{
				int[] indexInfo = StageManager.CalcStageIndexInfo (index + 1);
				StageManager.I.InstantiateStage (indexInfo [0], indexInfo [1], indexInfo [2]);
			}
		});
		sequence.Play ();
	}
		
	public void SetInitParam()
	{
		IsTopOfWall = false;
		IsTopOfWall_Dest = false;
		FlatTrans.position = new Vector3(InitPosition.x, InitPosition.y, FlatTrans.position.z);
		ChangeColor (ColorData.COLOR1, StageManager.I.CurrentInfo.InitialCharacterColor);
	}

	/// <summary>
	/// キャラクターを非表示にする
	/// </summary>
	public void HideCharacter()
	{
		foreach (var chara in AllCharacters) {
			foreach (Material material in chara.GetComponentsInChildren<Renderer>().Select(x => x.material)) {
				material.SetFloat ("_ForwardThreshold", 0);
			}
		}
	}
	/// <summary>
	/// 平面上のキャラの位置をセットする
	/// </summary>
	public void SetPosition(Vector3 pos)
	{
		FlatTrans.position = pos;
	}
	/// <summary>
	/// 全キャラクターを取得
	/// </summary>
	private IEnumerable<GameObject> AllCharacters
	{
		get{ return new GameObject[] { _CharacterX, _CharacterZ, _DestCharacterX, _DestCharacterZ }; }
	}
	private IEnumerable<GameObject> Characters
	{
		get{ return new GameObject[] { _CharacterX, _CharacterZ }; }
	}
	private IEnumerable<GameObject> DestCharacters
	{
		get{ return new GameObject[] { _DestCharacterX, _DestCharacterZ }; }
	}

    private readonly float ASPECT_RATE = 500f / 800f;
	private readonly float RateX = 1f/4.0f;
    public Vector3 Bottom
    {
		get { return FlatTrans.position+new Vector3(0f,Scale.y*ASPECT_RATE/6f,0f); }
		set { FlatTrans.position = value-new Vector3(0f,Scale.y*ASPECT_RATE/6f,0f); }
    }
    public Vector3 BottomLeft
    {
		get { return Bottom + new Vector3(-Scale.x*RateX/2, 0f, 0f); }
    }
    public Vector3 BottomRight
    {
		get { return Bottom + new Vector3(Scale.x*RateX/2, 0f, 0f); }
    }
    public Vector3 Center
    {
        get { return FlatTrans.position + new Vector3(0f,Scale.y*ASPECT_RATE/2,0f); }
    }
    public Vector3 Left
    {
		get { return Center + new Vector3(-Scale.x*RateX/2, 0f, 0f); }
    }
    public Vector3 Right
    {
		get { return Center + new Vector3(Scale.x*RateX/2, 0f, 0f); }
    }
    public Vector3 Top
    {
        get { return FlatTrans.position + new Vector3(0f, Scale.y*ASPECT_RATE*5f/6f, 0f); }
    }
    public Vector3 TopLeft
    {
		get { return Top + new Vector3(-Scale.x*RateX/2, 0f, 0f); }
    }
    public Vector3 TopRight
    {
		get { return Top + new Vector3(Scale.x*RateX/2, 0f, 0f); }
    }
	public  Vector3 DestBottom
	{
		get { return GetDest(Bottom); }
	}
	public  Vector3 DestBottomLeft
	{
		get { return GetDest(BottomRight); }
	}
	public Vector3 DestBottomRight
	{
		get { return GetDest(BottomLeft); }
	}
	public Vector3 DestCenter
	{
		get { return GetDest(Center); }
	}
	public Vector3 DestLeft
	{
		get { return GetDest(Right); }
	}
	public Vector3 DestRight
	{
		get { return GetDest(Left); }
	}
	public Vector3 DestTop
	{
		get { return GetDest(Top); }
	}
	public Vector3 DestTopLeft
	{
		get { return GetDest(TopRight); }
	}
	public Vector3 DestTopRight
	{
		get { return GetDest(TopLeft); }
	}
	public Rectangle CharaRect
	{
		get { return new Rectangle(Center, Scale.x*RateX, Scale.y*ASPECT_RATE*2f/3f, color); }
	}
	public Rectangle DummyCharaRect
	{
		get { return new Rectangle(DestCenter, Scale.x*RateX, Scale.y*ASPECT_RATE*2f/3f, color); }
	}

	private Transform FlatTrans
	{
		get { return _FlatCharacter.transform; }
	}
	private Vector3 Scale
	{
		get { return FlatTrans.lossyScale; }
	}

	private Vector3 GetDest(Vector3 pos){
		pos.x *= -1;
		return pos;
	}
}
