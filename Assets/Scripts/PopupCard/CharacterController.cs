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

    private bool _IsTopOfWall = false;
    public bool IsTopOfWall
    {
        get { return _IsTopOfWall; }
        set { _IsTopOfWall = value; }
    }
    private bool _MoveX = true;
    private bool _OverFoldLine = false;

    void Update()
    {
        //アニメーション中はキャラクターを動かさない
        if(StageCreater.I.IsPlayingAnimation == false)
        {
            //入力を取得
            float deltaHol = Time.deltaTime * _Speed * Input.GetAxis("Horizontal");
            float deltaVer = Time.deltaTime * _Speed * Input.GetAxis("Vertical");
            float deltaDrop = Time.deltaTime * _DropSpeed;
    
            if (!StageManager.I.CanUseLadder(_DummyCharacter.transform.position))
            {
                deltaVer = -deltaDrop;
            }
            //キャラクターの当たり判定を5点で行う
            List<Vector2> charaPosList = new List<Vector2>(4);
            charaPosList.Add(CharaParam.BottomRight);
            charaPosList.Add(CharaParam.Bottom);
            charaPosList.Add(CharaParam.BottomLeft);
            charaPosList.Add(CharaParam.TopRight);
            charaPosList.Add(CharaParam.TopLeft);
    
            Vector2 moveDir = StageManager.I.CalcAmountOfMovement(new Vector2(deltaHol, deltaVer));
    
            UpdateCharacterXZPosition(moveDir);
            if(Input.GetKeyDown(KeyCode.DownArrow) && _IsTopOfWall)
            {
                _DummyCharacter.transform.position -= 0.02f*Vector3.up;
                _CharacterX.transform.position -= 0.02f*Vector3.up;
                _CharacterZ.transform.position -= 0.02f*Vector3.up;
            }
            UpdateCharacterState(moveDir);
        }
    }
    /// <summary>
    /// 移動量を計算し、キャラの位置を更新
    /// </summary>
    private void UpdateCharacterXZPosition(Vector2 moveDir)
    {
        _DummyCharacter.transform.position += new Vector3(moveDir.x, moveDir.y, 0f);
        _CharacterX.transform.position += new Vector3(moveDir.x, moveDir.y, 0f);
        _CharacterZ.transform.position += new Vector3(0f, moveDir.y, -moveDir.x);
        
        if(_IsTopOfWall)
            _IsTopOfWall = StageManager.I.OnTopOfWall();
        
        _MoveX = CalcCurrentMoveDirection();

        if (Mathf.Abs(moveDir.x) > 0f)
        {
            float delta = Mathf.Sign(moveDir.x) * _DummyCharacter.transform.lossyScale.x / 2;
            float foldlineDist = StageManager.I.CalcFoldLineDistance(_DummyCharacter.transform.position, delta);
            if (Mathf.Abs(foldlineDist) < Mathf.Abs(delta))
            {
                if (_OverFoldLine == false)
                {
                    if (_MoveX)
                    {
                        Vector3 zCharaPos;
                        zCharaPos.x = _CharacterX.transform.position.x + foldlineDist;
                        zCharaPos.y = _DummyCharacter.transform.position.y;
                        zCharaPos.z = _CharacterX.transform.position.z + foldlineDist;
                        _CharacterZ.transform.position = zCharaPos;
                    }
                    else
                    {
                        Vector3 xCharaPos;
                        xCharaPos.x = _CharacterZ.transform.position.x - foldlineDist;
                        xCharaPos.y = _DummyCharacter.transform.position.y;
                        xCharaPos.z = _CharacterZ.transform.position.z - foldlineDist;
                        _CharacterX.transform.position = xCharaPos;
                    }

                    _OverFoldLine = true;
                }
            }
            else if (_OverFoldLine == true)
            {
                _OverFoldLine = false;
            }
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
        //キャラクター部分透過
        UpdateSubTransparent(moveDir);
    }

    /// <summary>
    /// キャラクターの部分透過を設定
    /// </summary>
    private void UpdateSubTransparent(Vector2 moveDir)
    {
        int r = 0;
        float delta = _DummyCharacter.transform.lossyScale.x;
        float foldlineDist = StageManager.I.CalcFoldLineDistance(_DummyCharacter.transform.position - delta / 2 * Vector3.right, delta);
        foreach (float x in StageManager.I.GetSortXCoordList(_DummyCharacter.transform.position.y))
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
    /// 現在の移動方向を計算する
    /// </summary>
    private bool CalcCurrentMoveDirection()
    {
        bool moveX = true;
        Vector3 charaPos = _DummyCharacter.transform.position;
        foreach (float x in StageManager.I.GetSortXCoordList(charaPos.y))
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
        private static readonly float ASPECT_RATE = 682f/423f;
        public static Vector3 Bottom
        {
            get { return CharacterController.I.DummyCharacter.transform.position; }
        }
        public static Vector3 BottomLeft
        {
            get { return CharacterController.I.DummyCharacter.transform.position
                         + new Vector3(-CharacterController.I.DummyCharacter.transform.lossyScale.x/2,
                                       0f,
                                       0f); }
        }
        public static Vector3 BottomRight
        {
            get { return CharacterController.I.DummyCharacter.transform.position
                         + new Vector3( CharacterController.I.DummyCharacter.transform.lossyScale.x/2,
                                        0f,
                                        0f); }
        }
        public static Vector3 Center
        {
            get { return CharacterController.I.DummyCharacter.transform.position
                         + new Vector3(0f,
                                       CharacterController.I.DummyCharacter.transform.lossyScale.y*ASPECT_RATE/2,
                                       0f); }
        }
        public static Vector3 Left
        {
            get { return CharacterController.I.DummyCharacter.transform.position
                         + new Vector3(-CharacterController.I.DummyCharacter.transform.lossyScale.x/2,
                                       CharacterController.I.DummyCharacter.transform.lossyScale.y*ASPECT_RATE/2,
                                       0f); }
        }
        public static Vector3 Right
        {
            get { return CharacterController.I.DummyCharacter.transform.position
                         + new Vector3( CharacterController.I.DummyCharacter.transform.lossyScale.x/2,
                                       CharacterController.I.DummyCharacter.transform.lossyScale.y*ASPECT_RATE/2,
                                       0f); }
        }
        public static Vector3 Top
        {
            get { return CharacterController.I.DummyCharacter.transform.position
                         + new Vector3(0f,
                                       CharacterController.I.DummyCharacter.transform.lossyScale.y*ASPECT_RATE,
                                       0f); }
        }
        public static Vector3 TopLeft
        {
            get { return CharacterController.I.DummyCharacter.transform.position
                         + new Vector3(-CharacterController.I.DummyCharacter.transform.lossyScale.x/2,
                                       CharacterController.I.DummyCharacter.transform.lossyScale.y*ASPECT_RATE,
                                       0f); }
        }
        public static Vector3 TopRight
        {
            get { return CharacterController.I.DummyCharacter.transform.position
                         + new Vector3( CharacterController.I.DummyCharacter.transform.lossyScale.x/2,
                                       CharacterController.I.DummyCharacter.transform.lossyScale.y*ASPECT_RATE,
                                       0f); }
        }
    }
}
