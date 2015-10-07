using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class StageCreater : Singlton<StageCreater>
{
    
    private readonly string X_TAG_NAME = "XSideComponent";
    private readonly string Z_TAG_NAME = "ZSideComponent";
    public readonly float START_ANGLE = 45f;
    
    private Ease OPEN_EASE = Ease.Linear;
    private Ease CLOSE_EASE = Ease.Linear;
    public static readonly float OFFSET = 0.02f;
    public readonly float THICKNESS = 0.1f;
    public readonly float ANIMATION_TIME = 1f;
    
    [SerializeField]
    private GameObject _Paper;
    private GameObject _Book;
    public GameObject Book
    {
        get { return _Book; }
        set { _Book = value; }
    }
    private float _XOffset;
    public float XOffset
    {
        get { return _XOffset; }
    }
    private float _ZOffset;
    public float ZOffset
    {
        get { return _ZOffset; }
    }
    private GameObject _PreviousRoot;
    private GameObject _Root;
    public float StageWidth
    {
        get { return StageManager.I.CurrentInfo.StageWidth; }
    }
    public float StageHeight
    {
        get { return StageManager.I.CurrentInfo.StageHeight; }
    }
    public bool IsPlayingAnimation
    {
        get;
        set;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Ease[] array = Enum.GetValues(typeof(Ease)) as Ease[];
            int rand = UnityEngine.Random.Range(0, array.Length-2);
            OPEN_EASE = array[rand];
            CLOSE_EASE = array[rand];
            _Sequence = DOTween.Sequence();
            ReOpenStage(45f, ANIMATION_TIME, ANIMATION_TIME, 0.3f, false);
            Vector3 pos = CharacterController.I.DummyCharacter.transform.position;
            pos.x *= -1;
            CharacterController.I.DummyCharacter.transform.position = pos; 
        }
    }

    /// <summary>
    /// ステージを生成する。すでにステージがある場合、閉じた後に破棄する
    /// </summary>
    public void CreateNewStage(bool existCharacter = true, float xOffset = 50f, float zOffset = -50f)
    {
        
        bool existStage = _Root != null;
        if(existStage)
            _PreviousRoot = _Root;    
        _Root = new GameObject("StageRoot");
        _XOffset = xOffset;
        _ZOffset = zOffset;
        _Root.transform.position = new Vector3(_XOffset, 0f, _ZOffset);
        
        InstantiatePaper();
        InstantiateBackground();
        InstantiateDecoration();
        if(existCharacter)
        {
            
            InstantiateCharacter();
            //HACK:キャラの向きや透過処理をさせたい
            CharacterController.I.UpdateCharacterState(Vector2.right);
        }

        foreach(Renderer renderer in _Root.GetComponentsInChildren<Renderer>())
        {
            if(renderer.enabled == false)
                renderer.gameObject.SetActive(false);
            renderer.enabled = false;
        }

        _Sequence = DOTween.Sequence();
        _PrevSequence = DOTween.Sequence();
        Ease[] array = Enum.GetValues(typeof(Ease)) as Ease[];
        int rand = UnityEngine.Random.Range(0, array.Length-2);
        OPEN_EASE = array[rand];
        CLOSE_EASE = array[rand];
        if(existStage)
        {
            ClosePrevStage(90f, ANIMATION_TIME);
            ReOpenStage(0f, ANIMATION_TIME, 0f, 0f, existStage);
        }
        else
        {
            ReOpenStage(START_ANGLE, ANIMATION_TIME, 0f, 0f, existStage);
        }
    }
    
    /// <summary>
    /// キャラクターを生成する
    /// </summary>
    private void InstantiateCharacter()
    {        
        CharacterController.I.color = StageManager.I.CurrentInfo.InitialCharacterColor;
        //X方向に動くキャラクター
        GameObject character = Instantiate(CharacterController.I.DummyCharacter,
                                           CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset - OFFSET * 2, 0f, _ZOffset - OFFSET * 2),
                                           Quaternion.identity) as GameObject;
        character.transform.SetParent(_Root.transform);
        character.layer = 0;
        foreach (Transform child in character.transform)
            child.gameObject.layer = 0;
        //TODO:色を決める
        character.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_MainColor", ColorManager.GetColorWithColorData(StageManager.I.CurrentInfo.InitialCharacterColor));
        ColorManager.MultiplyShadowColor(character.transform.GetChild(1).gameObject);
        CharacterController.I.CharacterX = character;
        //Z方向に動くキャラクター
        character = Instantiate(CharacterController.I.DummyCharacter,
                                CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset - OFFSET * 2, 0f, _ZOffset - OFFSET * 2),
                                Quaternion.identity) as GameObject;
        character.transform.Rotate(0f, 90f, 0f);
        character.transform.SetParent(_Root.transform);
        character.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_MainColor", ColorManager.GetColorWithColorData(StageManager.I.CurrentInfo.InitialCharacterColor));
        character.layer = 0;
        foreach (Transform child in character.transform)
            child.gameObject.layer = 0;
        CharacterController.I.CharacterZ = character;
        //X方向に動くキャラクター
        character = Instantiate(CharacterController.I.DummyCharacter,
                                CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset - OFFSET * 2, 0f, _ZOffset - OFFSET * 2),
                                Quaternion.identity) as GameObject;
        character.transform.SetParent(_Root.transform);
        character.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_MainColor", new Color(0,0,0,0));
        character.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_MainColor", new Color(0,0,0,0.5f));
        character.layer = 0;
        foreach (Transform child in character.transform)
            child.gameObject.layer = 0;
        CharacterController.I.DestCharacterX = character;
        //Z方向に動くキャラクター
        character = Instantiate(CharacterController.I.DummyCharacter,
                                CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset - OFFSET * 2, 0f, _ZOffset - OFFSET * 2),
                                Quaternion.identity) as GameObject;
        character.transform.Rotate(0f, 90f, 0f);
        character.transform.SetParent(_Root.transform);
        character.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_MainColor", new Color(0,0,0,0));
        character.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_MainColor", new Color(0,0,0,0.5f));
        character.layer = 0;
        foreach (Transform child in character.transform)
            child.gameObject.layer = 0;
        CharacterController.I.DestCharacterZ = character;
    }
    /// <summary>
    /// 背景を生成する
    /// </summary>
    private void InstantiateBackground()
    {
        //背景の生成
        //x方向
        GameObject background = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
        background.GetComponent<Renderer>().material.mainTexture = StageManager.I.CurrentInfo.LiningTexture;
        background.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f,0f);
        background.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.5f, 1f);
        background.transform.SetParent(_Root.transform);
        background.transform.localScale = new Vector3(StageWidth/2, StageHeight, 0.01f);
        background.transform.position = new Vector3(-StageWidth/4 + _XOffset, StageHeight/2, _ZOffset + 0.01f);
        background.transform.eulerAngles += 180*Vector3.forward;
        background.tag = X_TAG_NAME;
        //z方向
        background = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
        background.GetComponent<Renderer>().material.mainTexture = StageManager.I.CurrentInfo.LiningTexture;
        background.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.5f,0f);
        background.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.5f, 1f);
        background.transform.SetParent(_Root.transform);
        background.transform.localScale = new Vector3(StageWidth/2, StageHeight, 0.01f);
        background.transform.position = new Vector3(_XOffset + 0.01f, StageHeight/2, -StageWidth/4 + _ZOffset);
        background.transform.forward = Vector3.right;
        background.transform.eulerAngles += 180*Vector3.forward;
        background.tag = Z_TAG_NAME;
    }
    /// <summary>
    /// ステージのカード部分をを生成する
    /// </summary>
    private void InstantiatePaper()
    {
        Vector3 scale = _Paper.transform.localScale;
        float thickness = scale.z;
        //ステージの紙オブジェクト生成
        IEnumerable<float> yCoordList = StageManager.I.GetSortYCoordList();
        float prevY = yCoordList.First();
        float yOffset = 0f;
        foreach (float y in yCoordList)
        {
            if (y == yCoordList.First())
                continue;
            bool setX = true;
            bool duringHole = false;
            float prevX = -StageWidth / 2;
            float xOffset = -StageWidth / 2, zOffset = _ZOffset;
            IEnumerable<XCoord> xCoordList = StageManager.I.GetXCoordList((prevY + y) / 2, true);
            foreach (XCoord xCoord in xCoordList)
            {
                //折れ線の場合
                if(duringHole == false){
                    GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
                    paper.transform.SetParent(_Root.transform);
                    paper.GetComponent<Renderer>().material.mainTexture = StageManager.I.CurrentInfo.BackgroundTexture;
                    paper.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(prevX/StageWidth+0.5f, prevY/StageHeight);
                    paper.GetComponent<Renderer>().material.mainTextureScale = new Vector2((xCoord.x-prevX)/StageWidth, (y-prevY)/StageHeight);
                    if (setX)
                    {
                        paper.transform.position = new Vector3((xCoord.x - prevX) / 2 + xOffset + _XOffset, (y - prevY) / 2 + yOffset, zOffset + thickness / 2);
                        xOffset += xCoord.x - prevX;
                        paper.tag = X_TAG_NAME;
                    }
                    else
                    {
                        paper.transform.position = new Vector3(xOffset + _XOffset + thickness / 2, (y - prevY) / 2 + yOffset, -(xCoord.x - prevX) / 2 + zOffset);
                        paper.transform.forward = Vector3.right;
                        paper.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                        zOffset -= xCoord.x - prevX;
                        paper.tag = Z_TAG_NAME;
                    }
                    paper.transform.eulerAngles += 180*Vector3.forward;
                    paper.transform.localScale = new Vector3(xCoord.x - prevX-0.001f, y - prevY, thickness);
                    if (xCoord.x == xCoordList.First().x)
                        paper.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                    
                    if(xCoord.fold)
                        setX = !setX;
                    else
                        duringHole = true;
                    prevX = xCoord.x;
                }
                //穴の場合
                else 
                {
                    if (setX)
                        xOffset += xCoord.x - prevX;
                    else
                        zOffset -= xCoord.x - prevX;
                    if(xCoord.fold)
                        setX = !setX;
                    else
                        duringHole = false;
                    prevX = xCoord.x;
                }
            }
            yOffset += y - prevY;
            prevY = y;
        }
    }
    /// <summary>
    /// 見た目に必要なオブジェクトを生成する
    /// </summary>
    private void InstantiateDecoration()
    {
        foreach (GameObject decos in StageManager.I.CurrentInfo.Decoration)
        {
            SetDecoration(decos);
            foreach (Transform child in decos.transform)
            {
                SetDecoration(child.gameObject);
            }
        }
    }

    /// <summary>
    /// 表示物をセット
    /// </summary>
    private void SetDecoration(GameObject deco)
    {
        //表示物がないオブジェクトなら処理をしない
        if (deco.GetComponent<Renderer>() == null &&
           deco.GetComponent<SpriteRenderer>() == null &&
           deco.GetComponent<LineRenderer>() == null)
            return;

        //装飾オブジェククトの表示
        Vector3 decoPos = deco.transform.position;
        Vector3 decoScale = deco.transform.lossyScale;
        Vector3 decoSetPos = new Vector3(-StageWidth / 2 - OFFSET * 2 + _XOffset, decoPos.y, _ZOffset - OFFSET * 2);

        float anchorHeightScale = 0f;
        if (deco.GetComponent<DecorationObjectParameter>() != null)
            anchorHeightScale = deco.GetComponent<DecorationObjectParameter>().leftHeightWithMaxWidth;

        bool facingX = true;
        float prevX = -StageWidth / 2;

        foreach (float x in StageManager.I.GetFoldXCoordList(decoPos.y + decoScale.y / 2 * anchorHeightScale, true))
        {
            if (decoPos.x - decoScale.x / 2 < x)
                break;

            if (facingX)
                decoSetPos.x += x - prevX;
            else
                decoSetPos.z -= x - prevX;
            facingX = !facingX;
            prevX = x;
        }
        if (facingX)
            decoSetPos.x += decoPos.x - prevX;
        else
            decoSetPos.z -= decoPos.x - prevX;
        GameObject newDeco = Instantiate(deco, decoSetPos, deco.transform.rotation) as GameObject;
        newDeco.transform.SetParent(_Root.transform);
        if (facingX)
        {
            ColorManager.MultiplyShadowColor(newDeco);
            newDeco.tag = X_TAG_NAME;
        }
        else
        {
            newDeco.transform.eulerAngles += new Vector3(0f, 90f, 0f);
            newDeco.tag = Z_TAG_NAME;
        }

        //折り目にまたがっている場合は2枚で表示
        float delta = decoScale.x;
        Vector2 decoAnchorPos = new Vector2(decoPos.x - delta / 2,
                                            decoPos.y + decoScale.y / 2 * anchorHeightScale);
        float foldlineDist = StageManager.I.CalcFoldLineDistance(decoAnchorPos, delta, true);
        if (Mathf.Abs(foldlineDist) < Mathf.Abs(delta))
        {
            if (facingX)
            {
                Vector3 newDecoPos;
                newDecoPos.x = newDeco.transform.position.x - decoScale.x / 2 + foldlineDist;
                newDecoPos.y = newDeco.transform.position.y;
                newDecoPos.z = newDeco.transform.position.z - decoScale.x / 2 + foldlineDist;
                GameObject newDeco2 = Instantiate(deco, newDecoPos, deco.transform.rotation) as GameObject;
                newDeco2.transform.SetParent(_Root.transform);
                newDeco2.transform.eulerAngles += new Vector3(0f, 90f, 0f);
                newDeco.GetComponent<Renderer>().material.SetFloat("_ForwardThreshold", foldlineDist / delta);
                newDeco2.GetComponent<Renderer>().material.SetFloat("_BackThreshold", foldlineDist / delta);
                newDeco2.tag = Z_TAG_NAME;
            }
            else
            {
                Vector3 newDecoPos;
                newDecoPos.x = newDeco.transform.position.x + decoScale.x / 2 - foldlineDist;
                newDecoPos.y = newDeco.transform.position.y;
                newDecoPos.z = newDeco.transform.position.z + decoScale.x / 2 - foldlineDist;
                GameObject newDeco2 = Instantiate(deco, newDecoPos, deco.transform.rotation) as GameObject;
                newDeco2.transform.SetParent(_Root.transform);
                ColorManager.MultiplyShadowColor(newDeco2);

                newDeco.GetComponent<Renderer>().material.SetFloat("_ForwardThreshold", foldlineDist / delta);
                newDeco2.GetComponent<Renderer>().material.SetFloat("_BackThreshold", foldlineDist / delta);
                newDeco2.tag = X_TAG_NAME;
            }
        }
    }
    
    private Sequence _PrevSequence;
    private Sequence _Sequence;
    
    /// <summary>
    /// ステージを閉じて開く
    /// </summary>
    public void ClosePrevStage(float angle, float closetime)
    {
        _PrevSequence.Append( _PreviousRoot.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        CloseStage(closetime, true);
        _PrevSequence.OnComplete(() => { 
            Destroy(_PreviousRoot);
            IsPlayingAnimation = false; 
        });
        _PrevSequence.Play();
    }
    /// <summary>
    /// ステージを閉じて開く
    /// </summary>
    public void ReOpenStage(float angle, float opentime, float closetime, float waittime, bool existStage)
    {
        _Sequence.OnStart(() => {
             foreach(Renderer renderer in _Root.GetComponentsInChildren<Renderer>())
             {
                renderer.enabled = true;
             } 
        });
        _Sequence.Append( _Root.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        if(existStage == false)
        {    
            _Sequence.Join( _Book.transform.GetChild(0).DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
            _Sequence.Join( _Book.transform.GetChild(1).DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        }
        CloseStage(closetime);
        _Sequence.Append( _Root.transform.DOBlendableRotateBy(-angle*Vector3.up, opentime).SetEase(OPEN_EASE).SetDelay(waittime) );
        //はじめは本を開く処理もする
        if(existStage == false)
        {    
            _Sequence.Join( _Book.transform.GetChild(0).DORotate(0*Vector3.up, opentime).SetEase(OPEN_EASE) );
            _Sequence.Join( _Book.transform.GetChild(1).DORotate(-90*Vector3.up, opentime).SetEase(OPEN_EASE) );
        }
        OpenStage(opentime);
        _Sequence.Play();
    }
    /// <summary>
    /// ステージを開く
    /// </summary>
    public void OpenStage(float time)
    {
        IsPlayingAnimation = true;        
        //  if(existStage == false)
        //      StartCoroutine(OpenObjectAnimation(_Book.transform.GetChild(0), _Book.transform.GetChild(0).position, openleft, time, true));
        foreach (Transform tmpAnchor in _Root.transform)
        {
            bool dirX = tmpAnchor.GetChild(0).tag != "ZSideComponent";
            if(dirX)
            {
                _Sequence.Join( tmpAnchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(OPEN_EASE) );
            }
            else
            {
                Transform child = tmpAnchor.transform.GetChild(0); 
                _Sequence.Join( tmpAnchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(OPEN_EASE)
                .OnUpdate(() =>{
                    Vector3 angle = child.eulerAngles;
                    angle.y = _Root.transform.eulerAngles.y+90f;
                    child.eulerAngles = angle;
                }) );
            }
        }
        _Sequence.OnComplete(() => {
            IsPlayingAnimation = false;
            List<Transform> rootChildren = new List<Transform>(_Root.transform.childCount);
            foreach (Transform child in _Root.transform)
                rootChildren.Add(child);
            foreach (Transform tmp in rootChildren)
            {
                tmp.GetChild(0).SetParent(_Root.transform);
                Destroy(tmp.gameObject);
            }
            
            InGameManager.I.DisplayDictionary();
        });
    }

    /// <summary>
    /// ステージを閉じる
    /// </summary>
    public void CloseStage(float time, bool previous = false)
    {
        IsPlayingAnimation = true;
        Sequence targetSequence = previous ? _PrevSequence : _Sequence;
        GameObject _AnimationRoot = previous ? _PreviousRoot : _Root;
        //  if(previous)
        //  {
        //      foreach (Transform stageObj in _AnimationRoot.transform)
        //          stageObj.position += new Vector3(-THICKNESS,0f,THICKNESS);
        //  }
        List<Transform> rootChildren = new List<Transform>(_AnimationRoot.transform.childCount);
        foreach (Transform child in _AnimationRoot.transform)
            rootChildren.Add(child);
        foreach (Transform stageObj in rootChildren)
        {
            Vector3 anchorPos;
            bool dirX = stageObj.tag != "ZSideComponent";
            
            anchorPos = new Vector3(_XOffset, 0f, stageObj.position.z);
            if(previous)
                anchorPos += new Vector3(-THICKNESS, 0f, 0f);
                    
            GameObject anchor = new GameObject("TmpAnchor");
            anchor.transform.SetParent(_AnimationRoot.transform);
            anchor.transform.position = anchorPos;
            stageObj.SetParent(anchor.transform);
            if(!dirX)
            { 
                targetSequence.Join( anchor.transform.DOBlendableRotateBy(-90*Vector3.up, time).SetEase(CLOSE_EASE)
                .OnUpdate(() =>{
                    Vector3 angle = anchor.transform.GetChild(0).eulerAngles;
                    angle.y = _AnimationRoot.transform.eulerAngles.y+90f;
                    anchor.transform.GetChild(0).eulerAngles = angle;                    
                }) );
            }
            else
            {
                targetSequence.Join( anchor.transform.DOBlendableRotateBy(-90*Vector3.up, time).SetEase(CLOSE_EASE) );
            }
        }
    }
    
    public void Clear()
    {
        _Sequence.Complete();
        if(_PreviousRoot != null)
            Destroy( _PreviousRoot );
        if(_Root != null)
            Destroy( _Root );
    }
}

//折れ線と穴開け用の線のどちらの線かを判断するために実装。
public struct XCoord
{
    public float x;
    public bool fold;
    
    public XCoord(float x, bool fold)
    {
        this.x = x;
        this.fold = fold;
    }
}
