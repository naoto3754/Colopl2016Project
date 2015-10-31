using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class CharacterController : Singlton<CharacterController>
{
    private GameObject _CharacterX;
    public GameObject CharacterX
    {
        set { _CharacterX = value; }
    }
    private GameObject _CharacterZ;
    public GameObject CharacterZ
    {
        set { _CharacterZ = value; }
    }
    private GameObject _DestCharacterX;
    public GameObject DestCharacterX
    {
        set { _DestCharacterX = value; }
    }
    private GameObject _DestCharacterZ;
    public GameObject DestCharacterZ
    {
        set { _DestCharacterZ = value; }
    }
    private GameObject _DummyCharacter;
    public GameObject DummyCharacter
    {
        get { return _DummyCharacter; }
        set { _DummyCharacter = value; }
    }

    public Vector2 InitPosition
    {
        get; set;
    }
	public Vector3 GoalPos
	{
		get; set;
	}

    [SerializeField]
    private float _Speed;
    [SerializeField]
    private float _DropSpeed;
    [SerializeField]
    public ColorData color;

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

    void Update()
    {
        //アニメーション中はキャラクターを動かさない
        if (StageCreater.I.IsPlayingAnimation)
			return;

		if (ClearStage)
			return;

		if (Input.GetKeyDown (KeyCode.C) || ( StageManager.I.CurrentInfo.GoalObj != null &&
			StageManager.I.CurrentInfo.GoalObj.Rect.Contains(_DummyCharacter.transform.position) ))
			ClearAction ();

        //入力を取得
        float deltaHol = Time.deltaTime * _Speed * Input.GetAxis("Horizontal");
        float deltaVer = Time.deltaTime * _Speed * Input.GetAxis("Vertical");
        Vector2 touchPos = InputManager.I.GetTapPos();

        if (InputManager.I.GetTapDown(0) || InputManager.I.GetTap(0))
        {
            Vector2 inputDir = InputManager.I.GetDistanceFromInitPos(0);
            inputDir.x = Mathf.Clamp(inputDir.x * 10, -1, 1);
            inputDir.x = Mathf.Abs(inputDir.x) < 0.5f ? 0f : inputDir.x;
            inputDir.y = Mathf.Clamp(inputDir.y * 15, -1, 1);
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
        if (InputManager.I.GetTapDown(0) || InputManager.I.GetTap(0))
        {
            Vector2 inputDir = InputManager.I.GetDistanceFromInitPos(0);
            inputDir.y = Mathf.Clamp(inputDir.y * 15, -1, 1);
            inputDir.y = Mathf.Abs(inputDir.y) < 0.5f ? 0f : inputDir.y;
            if (Input.GetKeyDown(KeyCode.DownArrow) && IsTopOfWall ||
                inputDir.y < -0.8f && IsTopOfWall)
            {
                _DummyCharacter.transform.position -= 0.05f * Vector3.up;
            }
        }
        UpdateCharacterState(moveDir);
        //ゴール判定
    }
    /// <summary>
    /// 移動量を計算し、キャラの位置を更新
    /// </summary>
    private void UpdateDummyCharacterPosition(Vector2 moveDir)
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
    }
    /// <summary>
    /// ダミーキャラの位置を実際のキャラ反映させる
    /// </summary>
    private void UpdateXZCharacterPosition(Vector3 charaPos, float delta, Transform xTrans, Transform zTrans, Vector2 moveDir, IEnumerable foldXList)
    {
        int r = 0;
        float foldlineDist = StageManager.I.CalcFoldLineDistance(charaPos - delta / 2 * Vector3.right, delta);
        float prevX = -StageCreater.I.StageWidth / 2;
        float xOffset = StageCreater.I.XOffset - StageCreater.I.StageWidth / 2;
        float zOffset = StageCreater.I.ZOffset;
        float charaAnchor = charaPos.x - delta / 2;
        foreach (float x in foldXList)
        {
            if (prevX < charaAnchor && charaAnchor < x)
            {
                if (r == 0) //x方向移動
                {
                    if (foldlineDist == delta + 1f)
                    {
                        xTrans.position = new Vector3(xOffset + charaPos.x - prevX - 0.01f, charaPos.y, zOffset - 0.01f);
                        return;
                    }
                    else
                    {
                        xTrans.position = new Vector3(xOffset + charaPos.x - prevX - 0.01f, charaPos.y, zOffset - 0.01f);
                        zTrans.position = new Vector3(xOffset + x - prevX - 0.01f, charaPos.y, zOffset - delta / 2 + foldlineDist - 0.01f);
                        return;
                    }
                }
                else //z方向移動
                {
                    if (foldlineDist == delta + 1f)
                    {
                        zTrans.position = new Vector3(xOffset - 0.01f, charaPos.y, zOffset - charaPos.x + prevX - 0.01f);
                        return;
                    }
                    else
                    {
                        xTrans.position = new Vector3(xOffset + delta / 2 - foldlineDist - 0.01f, charaPos.y, zOffset - x + prevX - 0.01f);
                        zTrans.position = new Vector3(xOffset - 0.01f, charaPos.y, zOffset - charaPos.x + prevX - 0.01f);
                        return;
                    }
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
            _CharacterX.GetComponent<Animator>().Play("walk");
            _CharacterZ.GetComponent<Animator>().Play("walk");
            _DestCharacterX.GetComponent<Animator>().Play("walk");
            _DestCharacterZ.GetComponent<Animator>().Play("walk");
        }
        else
        {
            _CharacterX.GetComponent<Animator>().Play("idle");
            _CharacterZ.GetComponent<Animator>().Play("idle");
            _DestCharacterX.GetComponent<Animator>().Play("idle");
            _DestCharacterZ.GetComponent<Animator>().Play("idle");
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

    public void SetInitPos()
    {
        _DummyCharacter.transform.position = new Vector3(InitPosition.x, InitPosition.y, _DummyCharacter.transform.position.z);
    }

	private void ClearAction()
	{
		ClearStage = true;
		Sequence sequence = DOTween.Sequence ();
		sequence.Join ( _CharacterX.transform.DOMove(GoalPos-Vector3.up, 1f) );
		sequence.Join ( _CharacterZ.transform.DOMove(GoalPos-Vector3.up, 1f) );
		foreach (Material mat in _CharacterX.GetComponentsInChildren<Renderer>().Select(x => x.material))
			sequence.Join ( mat.DOMainColor (new Color(1f,1f,1f,0f), 1f) );
		foreach (Material mat in _CharacterZ.GetComponentsInChildren<Renderer>().Select(x => x.material))
			sequence.Join ( mat.DOMainColor (new Color(1f,1f,1f,0f), 1f) );
		foreach (Material mat in _DestCharacterX.GetComponentsInChildren<Renderer>().Select(x => x.material))
			sequence.Join ( mat.DOMainColor (new Color(1f,1f,1f,0f), 1f) );
		foreach (Material mat in _DestCharacterZ.GetComponentsInChildren<Renderer>().Select(x => x.material))
			sequence.Join ( mat.DOMainColor (new Color(1f,1f,1f,0f), 1f) );

		sequence.OnComplete (() => {
			int chapter = StageManager.I.CurrentChapter;
			int bookID = StageManager.I.CurrentBookID;
			int index = StageManager.I.CurrentStageIndex;
			int[] indexInfo = StageManager.CalcStageIndexInfo (StageManager.CalcStageListIndex (chapter, bookID, index) + 1);
			StageManager.I.InstantiateStage (indexInfo [0], indexInfo [1], indexInfo [2]);
		});
		sequence.Play ();
	}

    //キャラクターの位置パラメータ
    public class CharaParam
    {
        private static readonly float ASPECT_RATE = 682f / 423f;
        public static Vector3 Bottom
        {
            get { return CharacterController.I.DummyCharacter.transform.position; }
        }
        public static Vector3 BottomLeft
        {
            get
            {
                return CharacterController.I.DummyCharacter.transform.position
                       + new Vector3(-CharacterController.I.DummyCharacter.transform.lossyScale.x / 2,
                                     0f,
                                     0f);
            }
        }
        public static Vector3 BottomRight
        {
            get
            {
                return CharacterController.I.DummyCharacter.transform.position
                       + new Vector3(CharacterController.I.DummyCharacter.transform.lossyScale.x / 2,
                                      0f,
                                      0f);
            }
        }
        public static Vector3 Center
        {
            get
            {
                return CharacterController.I.DummyCharacter.transform.position
                       + new Vector3(0f,
                                     CharacterController.I.DummyCharacter.transform.lossyScale.y * ASPECT_RATE / 2,
                                     0f);
            }
        }
        public static Vector3 Left
        {
            get
            {
                return CharacterController.I.DummyCharacter.transform.position
                       + new Vector3(-CharacterController.I.DummyCharacter.transform.lossyScale.x / 2,
                                     CharacterController.I.DummyCharacter.transform.lossyScale.y * ASPECT_RATE / 2,
                                     0f);
            }
        }
        public static Vector3 Right
        {
            get
            {
                return CharacterController.I.DummyCharacter.transform.position
                       + new Vector3(CharacterController.I.DummyCharacter.transform.lossyScale.x / 2,
                                     CharacterController.I.DummyCharacter.transform.lossyScale.y * ASPECT_RATE / 2,
                                     0f);
            }
        }
        public static Vector3 Top
        {
            get
            {
                return CharacterController.I.DummyCharacter.transform.position
                       + new Vector3(0f,
                                     CharacterController.I.DummyCharacter.transform.lossyScale.y * ASPECT_RATE,
                                     0f);
            }
        }
        public static Vector3 TopLeft
        {
            get
            {
                return CharacterController.I.DummyCharacter.transform.position
                       + new Vector3(-CharacterController.I.DummyCharacter.transform.lossyScale.x / 2,
                                     CharacterController.I.DummyCharacter.transform.lossyScale.y * ASPECT_RATE,
                                     0f);
            }
        }
        public static Vector3 TopRight
        {
            get
            {
                return CharacterController.I.DummyCharacter.transform.position
                       + new Vector3(CharacterController.I.DummyCharacter.transform.lossyScale.x / 2,
                                     CharacterController.I.DummyCharacter.transform.lossyScale.y * ASPECT_RATE,
                                     0f);
            }
        }
    }
}
