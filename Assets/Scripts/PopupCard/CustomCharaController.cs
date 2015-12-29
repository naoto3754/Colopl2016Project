using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class CustomCharaController : MonoBehaviour
{
	private GameObject _InitAnchor;
	private GameObject _InitDestAnchor;
    private GameObject _CharacterX;
    private GameObject _CharacterZ;
    private GameObject _DestCharacterX;
    private GameObject _DestCharacterZ;
    private GameObject _DummyCharacter;

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
    public bool CanUseLadder
    {
        get; set;
    }
	public bool ClearStage {
		get;
		set;
	}

	public void Init()
	{
		StageManager.I.CurrentController = this;
		_DummyCharacter = this.gameObject;
		color = ColorData.COLOR1;
		InitPosition = _DummyCharacter.transform.position;

		_CharacterX = CreateCharacter (StageManager.I.CurrentInfo.InitialCharacterColor, true);
		_CharacterZ = CreateCharacter (StageManager.I.CurrentInfo.InitialCharacterColor, false);
		_DestCharacterX = CreateCharacter (new Color(0,0,0,0.5f), true);
		_DestCharacterZ = CreateCharacter (new Color(0,0,0,0.5f), false);

		UpdateDummyCharacterPosition(0.01f*Vector2.right);
		SetInitPosAnchor ();
	}
	private GameObject CreateCharacter(Color initColor,bool xDir)
	{
		GameObject character = Instantiate(_DummyCharacter, Vector3.zero, Quaternion.identity) as GameObject;
		if(xDir == false)
			character.transform.Rotate(0f, 90f, 0f);
		character.transform.SetParent(StageManager.I.DecoRoot.transform);
		character.tag = xDir ? StageCreater.X_TAG_NAME : StageCreater.Z_TAG_NAME;
		//TODO:色を決める
		var face = character.transform.GetChild (0).GetComponent<SpriteRenderer> ();
		var body = character.transform.GetChild (1).GetComponent<SpriteRenderer> ();
		face.material.SetColor("_MainColor", Color.white);
		face.sortingOrder = 101;
		body.material.SetColor("_MainColor", initColor);
		body.sortingOrder = 100;
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

		if (Input.GetKeyDown (KeyCode.C) || ( StageManager.I.CurrentInfo.GoalObj != null &&
			StageManager.I.CurrentInfo.GoalObj.Rect.Contains(_DummyCharacter.transform.position) ))
			ClearAction ();

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

        if (!CanUseLadder)
        {
            deltaVer = -deltaDrop;
        }
        else if (deltaVer > Ladder.MovementLimit)
        {
            deltaVer = Ladder.MovementLimit + 0.01f;
        }

        Vector2 moveDir = StageManager.I.CalcAmountOfMovement(new Vector2(deltaHol, deltaVer));

        UpdateDummyCharacterPosition(moveDir);
		JumppingOff ();
        UpdateCharacterState(moveDir);
    }

	private void JumppingOff()
	{
		if (InputManager.I.GetTapDown(0) || InputManager.I.GetTap(0) || Input.GetKeyDown (KeyCode.DownArrow))
		{
			Vector2 inputDir = InputManager.I.GetDistanceFromInitPos(0);
			inputDir.y = Mathf.Clamp(inputDir.y * 15, -1, 1);
			inputDir.y = Mathf.Abs(inputDir.y) < 0.5f ? 0f : inputDir.y;
			bool inputDown = Input.GetKeyDown (KeyCode.DownArrow) || inputDir.y < -0.8f;
			bool canFall = (IsTopOfWall && StageManager.I.CanFall ()) || StageManager.I.OnJumpOffLine ();
			if ( inputDown && canFall )
			{
				_DummyCharacter.transform.position -= 0.05f * Vector3.up;
			}
		}
	}

    /// <summary>
    /// 移動量を計算し、キャラの位置を更新
    /// </summary>
    public void UpdateDummyCharacterPosition(Vector2 moveDir)
    {
        _DummyCharacter.transform.position += new Vector3(moveDir.x, moveDir.y, 0f);
        IEnumerable foldXList = StageManager.I.GetFoldXCoordList(_DummyCharacter.transform.position.y);
        //飛び出ている部分の上に乗っているか判定
        if (IsTopOfWall)
            IsTopOfWall = StageManager.I.OnTopOfWall();

        Vector3 destPos = _DummyCharacter.transform.position;
        destPos.x *= -1;
        // ダミーキャラの位置を実際のキャラ反映させる
        UpdateXZCharacterPosition(_DummyCharacter.transform.position, _DummyCharacter.transform.lossyScale.x,
                                  _CharacterX.transform, _CharacterZ.transform,
                                  moveDir, foldXList);
        UpdateXZCharacterPosition(destPos, _DummyCharacter.transform.lossyScale.x,
                                  _DestCharacterX.transform, _DestCharacterZ.transform,
                                  moveDir, foldXList);

        //キャラクター部分透過
        UpdateSubTransparent(_DummyCharacter.transform.position, _DummyCharacter.transform.lossyScale.x,
                             _CharacterX, _CharacterZ, moveDir, foldXList);
        UpdateSubTransparent(destPos, _DummyCharacter.transform.lossyScale.x,
                             _DestCharacterX, _DestCharacterZ, moveDir, foldXList);
		if (StageManager.I.IsOnObstacle ()) {
			foreach (Material material in _DestCharacterX.GetComponentsInChildren<Renderer>().Select(x => x.material)) {
				Color c = material.GetColor("_MainColor");
				c.a = 0.1f;
				material.SetColor("_MainColor", c);
			}
			foreach (Material material in _DestCharacterZ.GetComponentsInChildren<Renderer>().Select(x => x.material)) {
				Color c = material.GetColor("_MainColor");
				c.a = 0.1f;
				material.SetColor("_MainColor", c);
			}
		} else {
			foreach (Material material in _DestCharacterX.GetComponentsInChildren<Renderer>().Select(x => x.material)) {
				Color c = material.GetColor("_MainColor");
				c.a = 0.5f;
				material.SetColor("_MainColor", c);
			}
			foreach (Material material in _DestCharacterZ.GetComponentsInChildren<Renderer>().Select(x => x.material)) {
				Color c = material.GetColor("_MainColor");
				c.a = 0.5f;
				material.SetColor("_MainColor", c);
			}
		}
    }
    /// <summary>
    /// ダミーキャラの位置を実際のキャラ反映させる
    /// </summary>
    private void UpdateXZCharacterPosition(Vector3 charaPos, float delta, Transform xTrans, Transform zTrans, Vector2 moveDir, IEnumerable foldXList)
    {
        int r = 0;
        float foldlineDist = StageManager.I.CalcFoldLineDistance(charaPos - delta / 2 * Vector3.right, delta);
        float prevX = -StageManager.I.CurrentInfo.StageWidth / 2;
		float xOffset = StageManager.I.Offset.x - StageManager.I.CurrentInfo.StageWidth / 2;
		float zOffset = StageManager.I.Offset.z;
        float charaAnchor = charaPos.x - delta / 2;
        foreach (float x in foldXList)
        {
            if (prevX < charaAnchor && charaAnchor < x)
            {
				
                if (r == 0) //x方向移動
                {
					xTrans.position = new Vector3(xOffset + charaPos.x - prevX - 0.01f, charaPos.y, zOffset - 0.01f);
					zTrans.position = xTrans.position + new Vector3(1,0,1) * (foldlineDist-delta/2);
					return;
                }
                else //z方向移動
                {
					zTrans.position = new Vector3(xOffset - 0.01f, charaPos.y, zOffset - charaPos.x + prevX - 0.01f);
					xTrans.position = zTrans.position + new Vector3(-1,0,-1) * (foldlineDist-delta/2);
					return;
                }
            }
            else
            {
                if (r == 0)
                    xOffset += x - prevX;
                else
                    zOffset -= x - prevX;
            }
            prevX = x;
            r = (int)Mathf.Repeat(r + 1, 2);
        }
    }

    /// <summary>
    /// キャラクターの部分透過を設定
    /// </summary>
    private void UpdateSubTransparent(Vector3 charaPos, float delta, GameObject xChara, GameObject zChara, Vector2 moveDir, IEnumerable foldXList)
    {
        int r = 0;
        float foldlineDist = StageManager.I.CalcFoldLineDistance(charaPos - delta / 2 * Vector3.right, delta);
        foreach (float x in foldXList)
        {
            if (charaPos.x - delta / 2 < x)
            {
                if (r == 0) //x方向移動
                {
                    if (foldlineDist == delta + 1f)
                    {
                        SetCharacterTransparent(xChara, zChara, 1f, 0f, 0f, 1f);
                        return;
                    }
                    if (moveDir.x > 0f)
                    {
                        SetCharacterTransparent(xChara, zChara, foldlineDist / delta, 0f, 1f, foldlineDist / delta);
                        return;
                    }
                    else if (moveDir.x < 0f)
                    {
                        SetCharacterTransparent(xChara, zChara, 1f, 1f - foldlineDist / delta, 1f - foldlineDist / delta, 0f);
                        return;
                    }
                }
                else //z方向移動
                {
                    if (foldlineDist == delta + 1f)
                    {
                        SetCharacterTransparent(xChara, zChara, 0f, 1f, 1f, 0f);
                        return;
                    }
                    if (moveDir.x > 0f)
                    {
                        SetCharacterTransparent(xChara, zChara, 1f, foldlineDist / delta, foldlineDist / delta, 0f);
                        return;
                    }
                    else if (moveDir.x < 0f)
                    {
                        SetCharacterTransparent(xChara, zChara, 1f - foldlineDist / delta, 0f, 1f, 1f - foldlineDist / delta);
                        return;
                    }
                }
            }
            r = (int)Mathf.Repeat(r + 1, 2);
        }
        if (foldlineDist == delta + 1f)
        {
            SetCharacterTransparent(xChara, zChara, 0f, 1f, 1f, 0f);
            return;
        }
        if (moveDir.x > 0f)
        {
            SetCharacterTransparent(xChara, zChara, 1f, foldlineDist / delta, foldlineDist / delta, 0f);
            return;
        }
        else if (moveDir.x < 0f)
        {
            SetCharacterTransparent(xChara, zChara, 1f - foldlineDist / delta, 1f, 0f, 1f - foldlineDist / delta);
            return;
        }
    }
    //キャラクター透過用関数
    private void SetCharacterTransparent(GameObject xChara, GameObject zChara, float xForward, float xBack, float zForward, float zBack)
    {
        xChara.transform.GetChild(0).position = xChara.transform.GetChild(1).position + new Vector3(-0.001f, 0f, 0f);
        zChara.transform.GetChild(0).position = zChara.transform.GetChild(1).position + new Vector3(0f, 0f, -0.001f);
        foreach (Material material in xChara.GetComponentsInChildren<Renderer>().Select(x => x.material))
        {
            material.SetFloat("_ForwardThreshold", xForward);
            material.SetFloat("_BackThreshold", xBack);
        }
        foreach (Material material in zChara.GetComponentsInChildren<Renderer>().Select(x => x.material))
        {
            material.SetFloat("_ForwardThreshold", zForward);
            material.SetFloat("_BackThreshold", zBack);
        }
    }
    /// <summary>
    /// 移動方向からキャラクターの向きやアニメーションを決定する
    /// </summary>
    public void UpdateCharacterState(Vector2 moveDir)
    {
        //アニメーション
        if (Mathf.Abs(moveDir.x) > 0.01f)
        {
			foreach (var anim in GetAllCharacter().Select(x => x.GetComponent<Animator>())) {
				anim.Play ("walk");
			}
			if (AudioManager.I.IsPlayingSE (AudioContents.AudioTitle.WALK) == false) {
				AudioManager.I.PlaySE (AudioContents.AudioTitle.WALK);
			}
        }
        else
        {
			foreach (var anim in GetAllCharacter().Select(x => x.GetComponent<Animator>())) {
				anim.Play ("idle");
			}
			AudioManager.I.StopSE (AudioContents.AudioTitle.WALK);
        }
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

	public void SwapCharacter()
	{
		UnityUtility.SwapGameObject(_CharacterX, _DestCharacterX);
		UnityUtility.SwapGameObject(_CharacterZ, _DestCharacterZ);
	}

    public void SetInitPos()
    {
        _DummyCharacter.transform.position = new Vector3(InitPosition.x, InitPosition.y, _DummyCharacter.transform.position.z);
		_CharacterX.transform.position = _InitAnchor.transform.position;
		_CharacterZ.transform.position = _InitAnchor.transform.position;
		_DestCharacterX.transform.position = _InitDestAnchor.transform.position;
		_DestCharacterZ.transform.position = _InitDestAnchor.transform.position;
    }

	private void SetInitPosAnchor ()
	{
		_InitAnchor = new GameObject ("initAnchor");
		_InitAnchor.transform.SetParent(StageManager.I.PaperRoot.transform);
		_InitAnchor.transform.position = _CharacterX.transform.position;
		_InitAnchor.tag = StageCreater.X_TAG_NAME;

		_InitDestAnchor = new GameObject ("initDestAnchor");
		_InitDestAnchor.transform.SetParent(StageManager.I.PaperRoot.transform);
		_InitDestAnchor.transform.position = _DestCharacterZ.transform.position;
		_InitDestAnchor.tag = StageCreater.Z_TAG_NAME;
	}

	public void ChangeColor(ColorData cd, Color c)
	{
		color = cd;
		var bodyX = _CharacterX.transform.GetChild (1).GetComponent<SpriteRenderer> ();
		bodyX.material.SetColor("_MainColor", c);
		var bodyZ = _CharacterZ.transform.GetChild (1).GetComponent<SpriteRenderer> ();
		bodyZ.material.SetColor("_MainColor", c);
		ColorManager.MultiplyShadowColor(bodyZ.gameObject);
	}

	private void ClearAction()
	{
		int chapter = StageManager.I.CurrentChapter;
		int bookID = StageManager.I.CurrentBookID;
		int stageIndex = StageManager.I.CurrentStageIndex;
		int index = StageManager.CalcStageListIndex (chapter, bookID, stageIndex);

		Vector3 goalPos = StageManager.I.CurrentInfo.GoalObj.GetComponent<StageObjectParameter> ().ObjectsOnStage [0].transform.position;
		ClearStage = true;
		Sequence sequence = DOTween.Sequence ();
		sequence.Join ( _CharacterX.transform.DOMove(goalPos-Vector3.up, 1f) );
		sequence.Join ( _CharacterZ.transform.DOMove(goalPos-Vector3.up, 1f) );
		foreach(var chara in GetAllCharacter()){
			foreach (Material mat in chara.GetComponentsInChildren<Renderer>().Select(x => x.material))
				sequence.Join ( mat.DOMainColor (new Color(1f,1f,1f,0f), 1f) );
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
		
	public void SetPosition(Vector3 pos)
	{
		_DummyCharacter.transform.position = pos;
	}

	private IEnumerable<GameObject> GetAllCharacter()
	{
		GameObject[] retList = new GameObject[]{
			_CharacterX,
			_CharacterZ,
			_DestCharacterX,
			_DestCharacterZ,
		};
		return retList;
	}

    private readonly float ASPECT_RATE = 682f / 423f;
    public  Vector3 Bottom
    {
        get { return _DummyCharacter.transform.position; }
    }
    public  Vector3 BottomLeft
    {
        get
        {
            return _DummyCharacter.transform.position
                   + new Vector3(-_DummyCharacter.transform.lossyScale.x / 2, 0f, 0f);
        }
    }
    public Vector3 BottomRight
    {
        get
        {
            return _DummyCharacter.transform.position
                   + new Vector3(_DummyCharacter.transform.lossyScale.x / 2, 0f, 0f);
        }
    }
    public Vector3 Center
    {
        get
        {
            return _DummyCharacter.transform.position
                   + new Vector3(0f, _DummyCharacter.transform.lossyScale.y * ASPECT_RATE / 2, 0f);
        }
    }
    public Vector3 Left
    {
        get
        {
            return _DummyCharacter.transform.position
                   + new Vector3(-_DummyCharacter.transform.lossyScale.x / 2, _DummyCharacter.transform.lossyScale.y * ASPECT_RATE / 2, 0f);
        }
    }
    public Vector3 Right
    {
        get
        {
            return _DummyCharacter.transform.position
                   + new Vector3(_DummyCharacter.transform.lossyScale.x / 2, _DummyCharacter.transform.lossyScale.y * ASPECT_RATE / 2, 0f);
        }
    }
    public Vector3 Top
    {
        get
        {
            return _DummyCharacter.transform.position
                   + new Vector3(0f, _DummyCharacter.transform.lossyScale.y * ASPECT_RATE, 0f);
        }
    }
    public Vector3 TopLeft
    {
        get
        {
            return _DummyCharacter.transform.position
                   + new Vector3(-_DummyCharacter.transform.lossyScale.x / 2, _DummyCharacter.transform.lossyScale.y * ASPECT_RATE, 0f);
        }
    }
    public Vector3 TopRight
    {
        get
        {
            return _DummyCharacter.transform.position
                   + new Vector3(_DummyCharacter.transform.lossyScale.x / 2, _DummyCharacter.transform.lossyScale.y * ASPECT_RATE, 0f);
        }
    }
}
