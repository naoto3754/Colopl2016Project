using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    private GameObject _DestinationCharacter;
    public GameObject DestinationCharacter
    {
        set { _DestinationCharacter = value; }
    }
    private GameObject _DummyCharacter;
    public GameObject DummyCharacter
    {
        get { return _DummyCharacter; }
        set { _DummyCharacter = value; }
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
    public bool ClearStage
    {
        get; set;
    }
    void FixedUpdate()
    {
        //アニメーション中はキャラクターを動かさない
        if (StageCreater.I.IsPlayingAnimation == false)
        {
            //入力を取得
            float deltaHol = Time.deltaTime * _Speed * Input.GetAxis("Horizontal");
            float deltaVer = Time.deltaTime * _Speed * Input.GetAxis("Vertical");
            Vector2 touchPos = InputManager.I.GetTapPos();            
            if(InputManager.I.GetAnyTap())
            {
                if(touchPos.x < 0.3f)
                    deltaHol = Time.deltaTime * _Speed * -1f;
                else if(touchPos.x > 0.7f)
                    deltaHol = Time.deltaTime * _Speed * 1f;
                if(touchPos.y < 0.3f)
                    deltaVer = Time.deltaTime * _Speed * -1f;
                else if(touchPos.y > 0.7f)
                    deltaVer = Time.deltaTime * _Speed * 1f;
                    
            } 
            float deltaDrop = Time.deltaTime * _DropSpeed;

            if (!CanUseLadder)
            {
                deltaVer = -deltaDrop;
            }else if(deltaVer > Ladder.MovementLimit)
            {
                deltaVer = Ladder.MovementLimit+0.01f;
            }

            Vector2 moveDir = StageManager.I.CalcAmountOfMovement(new Vector2(deltaHol, deltaVer));

            UpdateDummyCharacterPosition(moveDir);
            if (Input.GetKeyDown(KeyCode.DownArrow) && IsTopOfWall ||
                touchPos.y > 0.1f && touchPos.y < 0.3f && IsTopOfWall)
            {
                _DummyCharacter.transform.position -= 0.05f * Vector3.up;
            }
            UpdateCharacterState(moveDir);
            //ゴール判定
            if(ClearStage)
            {
                StageManager.I.InstantiateStage(StageManager.I.CurrentChapter, StageManager.I.CurrentStageIndex+1);
            }
        }
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
        // ダミーキャラの位置を実際のキャラ反映させる
        UpdateXZCharacterPosition(moveDir, foldXList);
        UpdateDestCharacterPosition(moveDir, foldXList);
        //キャラクター部分透過
        UpdateSubTransparent(moveDir, foldXList);
    }
    /// <summary>
    /// ダミーキャラの位置を実際のキャラ反映させる
    /// </summary>
    private void UpdateXZCharacterPosition(Vector2 moveDir, IEnumerable foldXList)
    {
        int r = 0;
        Vector3 charaPos = _DummyCharacter.transform.position;
        float delta = _DummyCharacter.transform.lossyScale.x;
        float foldlineDist = StageManager.I.CalcFoldLineDistance(_DummyCharacter.transform.position - delta / 2 * Vector3.right, delta);
        float prevX = -StageCreater.I.StageWidth/2;
        float xOffset = StageCreater.I.XOffset-StageCreater.I.StageWidth/2;
        float zOffset = StageCreater.I.ZOffset;
        float charaAnchor = _DummyCharacter.transform.position.x - delta / 2;   
        foreach (float x in foldXList)
        {
            if (prevX < charaAnchor && charaAnchor < x)
            {
                if (r == 0) //x方向移動
                {
                    if (foldlineDist == delta + 1f)
                    {
                        _CharacterX.transform.position = new Vector3(xOffset+charaPos.x-prevX-0.01f, charaPos.y, zOffset-0.01f);
                        return;
                    }
                    else
                    {
                        _CharacterX.transform.position = new Vector3(xOffset+charaPos.x-prevX-0.01f, charaPos.y, zOffset-0.01f);
                        _CharacterZ.transform.position = new Vector3(xOffset+x-prevX-0.01f, charaPos.y, zOffset-delta/2+foldlineDist-0.01f);
                        return;
                    }
                }
                else //z方向移動
                {
                    if (foldlineDist == delta + 1f)
                    {
                        _CharacterZ.transform.position = new Vector3(xOffset-0.01f, charaPos.y, zOffset-charaPos.x+prevX-0.01f);
                        return;
                    }
                    else
                    {
                        _CharacterX.transform.position = new Vector3(xOffset+delta/2-foldlineDist-0.01f, charaPos.y, zOffset-x+prevX-0.01f);
                        _CharacterZ.transform.position = new Vector3(xOffset-0.01f, charaPos.y, zOffset-charaPos.x+prevX-0.01f);
                        return;
                    }
                }
            }
            else
            {
                if(r == 0)
                    xOffset += x - prevX;
                else
                    zOffset -= x - prevX;
            }
            prevX = x;
            r = (int)Mathf.Repeat(r + 1, 2);
        }
    }
    /// <summary>
    /// 折り返し位置にキャラを表示
    /// </summary>
    private void UpdateDestCharacterPosition(Vector2 moveDir, IEnumerable foldXList)
    {
        List<Vector2> rangeList = new List<Vector2>();
        int r = 0;
        Vector3 charaPos = _DummyCharacter.transform.position;
        float delta = _DummyCharacter.transform.lossyScale.x;
        float foldlineDist = StageManager.I.CalcFoldLineDistance(_DummyCharacter.transform.position - delta / 2 * Vector3.right, delta);
        float prevX = -StageCreater.I.StageWidth/2;
        float searchX = 0f;
        float searchOffset = -StageCreater.I.StageWidth/2;
        float xOffset = StageCreater.I.XOffset-StageCreater.I.StageWidth/2;
        float zOffset = StageCreater.I.ZOffset;
        float charaAnchor = _DummyCharacter.transform.position.x - delta / 2;
        bool findOnXSide = false;
        
        foreach (float x in foldXList)
        {
            if (prevX < charaPos.x && charaPos.x < x && findOnXSide == false)
            {
                if (r == 0) //x方向移動
                {
                    searchX = searchOffset + charaPos.x - prevX;
                    findOnXSide = true;
                    xOffset += x - prevX;
                    searchOffset += x - prevX;
                }
                else //z方向移動
                {
                    searchX = searchOffset - charaPos.x + prevX;
                    for(int i = rangeList.Count-1; i >= 0; i--)
                    {
                        if(rangeList.Count%2 == i%2)
                        {
                            zOffset += rangeList[i].y - rangeList[i].x;
                        }
                        else
                        {
                            if(rangeList[i].x < searchX && searchX < rangeList[i].y)
                            {
                                _DestinationCharacter.transform.position = new Vector3(xOffset-rangeList[i].y+searchX-0.01f, charaPos.y, zOffset-0.01f);
                                _DestinationCharacter.transform.forward = Vector3.back;
                                return;
                            }
                            xOffset -= rangeList[i].y - rangeList[i].x;
                        } 
                    }
                }
            }
            else
            {   
                if(r == 0)
                {
                    xOffset += x - prevX;
                    rangeList.Add(new Vector2(searchOffset, searchOffset + x - prevX));
                    searchOffset += x - prevX;
                }
                else
                {
                    if(findOnXSide && (searchOffset-x+prevX < searchX && searchX < searchOffset))
                    {
                        _DestinationCharacter.transform.position = new Vector3(xOffset-0.01f, charaPos.y, zOffset-searchOffset+searchX-0.01f);
                        _DestinationCharacter.transform.forward = Vector3.right;
                        return;
                    }
                    zOffset -= x - prevX;
                    rangeList.Add(new Vector2(searchOffset - x + prevX, searchOffset));
                    searchOffset -= x - prevX;
                }
            }
            prevX = x;
            r = (int)Mathf.Repeat(r + 1, 2);
        }
    }

    /// <summary>
    /// キャラクターの部分透過を設定
    /// </summary>
    private void UpdateSubTransparent(Vector2 moveDir, IEnumerable foldXList)
    {
        int r = 0;
        float delta = _DummyCharacter.transform.lossyScale.x;
        float foldlineDist = StageManager.I.CalcFoldLineDistance(_DummyCharacter.transform.position - delta / 2 * Vector3.right, delta);
        foreach (float x in foldXList)
        {
            if (_DummyCharacter.transform.position.x - delta / 2 < x)
            {
                if (r == 0) //x方向移動
                {
                    if (foldlineDist == delta + 1f)
                    {
                        SetCharacterTransparent(1f, 0f, 0f, 1f);
                        return;
                    }
                    if (moveDir.x > 0f)
                    {
                        SetCharacterTransparent(foldlineDist / delta, 0f, 1f, foldlineDist / delta);
                        return;
                    }
                    else if (moveDir.x < 0f)
                    {
                        SetCharacterTransparent(1f, 1f - foldlineDist / delta, 1f - foldlineDist / delta, 0f);
                        return;
                    }
                }
                else //z方向移動
                {
                    if (foldlineDist == delta + 1f)
                    {
                        SetCharacterTransparent(0f, 1f, 1f, 0f);
                        return;
                    }
                    if (moveDir.x > 0f)
                    {
                        SetCharacterTransparent(1f, foldlineDist / delta, foldlineDist / delta, 0f);
                        return;
                    }
                    else if (moveDir.x < 0f)
                    {
                        SetCharacterTransparent(1f - foldlineDist / delta, 0f, 1f, 1f - foldlineDist / delta);
                        return;
                    }
                }
            }
            r = (int)Mathf.Repeat(r + 1, 2);
        }
        if (foldlineDist == delta + 1f)
        {
            SetCharacterTransparent(0f, 1f, 1f, 0f);
            return;
        }
        if (moveDir.x > 0f)
        {
            SetCharacterTransparent(1f, foldlineDist / delta, foldlineDist / delta, 0f);
            return;
        }
        else if (moveDir.x < 0f)
        {
            SetCharacterTransparent(1f - foldlineDist / delta, 1f, 0f, 1f - foldlineDist / delta);
            return;
        }
    }
    //キャラクター透過用関数
    private void SetCharacterTransparent(float xForward, float xBack, float zForward, float zBack)
    {
        _CharacterX.transform.GetChild(0).position = _CharacterX.transform.GetChild(1).position + new Vector3(-0.001f, 0f, 0f);
        _CharacterZ.transform.GetChild(0).position = _CharacterZ.transform.GetChild(1).position + new Vector3(0f, 0f, -0.001f);
        foreach (Material material in _CharacterX.GetComponentsInChildren<Renderer>().Select(x => x.material))
        {
            material.SetFloat("_ForwardThreshold", xForward);
            material.SetFloat("_BackThreshold", xBack);
        }
        foreach (Material material in _CharacterZ.GetComponentsInChildren<Renderer>().Select(x => x.material))
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
        }
        else
        {
            _CharacterX.GetComponent<Animator>().Play("idle");
            _CharacterZ.GetComponent<Animator>().Play("idle");
        }
        //キャラクター向き
        if (moveDir.x > 0f)
        {
            _CharacterX.transform.forward = Vector3.forward;
            _CharacterZ.transform.forward = Vector3.right;
        }
        else if (moveDir.x < 0f)
        {
            _CharacterX.transform.forward = Vector3.back;
            _CharacterZ.transform.forward = Vector3.left;
        }
    }
    
    /// <summary>
    /// 現在の移動方向を計算する
    /// </summary>
    private bool CalcCurrentMoveDirection(IEnumerable foldXList)
    {
        bool moveX = true;
        Vector3 charaPos = _DummyCharacter.transform.position;
        foreach (float x in foldXList)
        {
            if (charaPos.x < x)
                break;
            moveX = !moveX;
        }
        return moveX;
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
