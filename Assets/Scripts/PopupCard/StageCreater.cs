using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageCreater : Singlton<StageCreater>
{
    public static readonly float OFFSET = 0.02f;
    public readonly float THICKNESS = 0.1f;
    public readonly float ANIMATION_TIME = 0.5f;
    
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
        
        InstantiatePaper();
        InstantiateBackground();
        InstantiateDecoration();
        if(existCharacter)
        {
            InstantiateCharacter();
            //HACK:キャラの向きや透過処理をさせたい
            CharacterController.I.UpdateCharacterState(Vector2.right);
        }
        
        if(existStage)
            CloseStage(ANIMATION_TIME, true, true);
        CloseStage(0f, false);
        OpenStage(ANIMATION_TIME, existStage);
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
    }
    /// <summary>
    /// 背景を生成する
    /// </summary>
    private void InstantiateBackground()
    {
        float thickness = _Paper.transform.localScale.z;
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
                    }
                    else
                    {
                        paper.transform.position = new Vector3(xOffset + _XOffset + thickness / 2, (y - prevY) / 2 + yOffset, -(xCoord.x - prevX) / 2 + zOffset);
                        paper.transform.forward = Vector3.right;
                        paper.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                        zOffset -= xCoord.x - prevX;
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
        }
        else
        {
            newDeco.transform.eulerAngles += new Vector3(0f, 90f, 0f);
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
            }
        }
    }
    /// <summary>
    /// ステージを開く
    /// </summary>
    public void OpenStage(float time, bool existStage)
    {
        IsPlayingAnimation = true;
        bool openleft = !TmpParameter.CloseDirctionLeft;
        //ステージがないときは本開く
        if(existStage == false)
            StartCoroutine(OpenObjectAnimation(_Book.transform.GetChild(0), _Book.transform.GetChild(0).position, true, openleft, time, true));
        foreach (Transform stageObj in _Root.transform)
        {
            TmpParameter tmpParam = stageObj.GetComponent<TmpParameter>();
            Vector3 anchorPos = tmpParam.AnimationAnchor;
            bool dirX = tmpParam.DirectionX;
            StartCoroutine(OpenObjectAnimation(stageObj, anchorPos, dirX, openleft, time, false));
        }
    }

    private IEnumerator OpenObjectAnimation(Transform obj, Vector3 anchor, bool dirX, bool openleft, float time, bool isBook)
    {
        int frameNum = 60;
        if (time == 0f)
        {
            frameNum = 1;
        }
        for (int i = 0; i < frameNum; i++)
        {
            Quaternion currentRotation = obj.rotation;
            if (openleft)
                obj.RotateAround(anchor, Vector3.up, 90f / frameNum);
            else
                obj.RotateAround(anchor, Vector3.up, -90f / frameNum);
            if (openleft && !dirX || !openleft && dirX)
                obj.rotation = currentRotation;
            yield return new WaitForSeconds(time / frameNum);
        }
        IsPlayingAnimation = false;
        Destroy(obj.GetComponent<TmpParameter>());
        if(_PreviousRoot != null)
            Destroy(_PreviousRoot);
        if(isBook)
            obj.position += new Vector3(-THICKNESS,0f,THICKNESS);
    }
    /// <summary>
    /// ステージを閉じる
    /// </summary>
    public void CloseStage(float time, bool closeleft, bool previous = false)
    {
        IsPlayingAnimation = true;
        TmpParameter.CloseDirctionLeft = closeleft;
        GameObject _AnimationRoot = previous ? _PreviousRoot : _Root;
        if(previous)
            foreach (Transform stageObj in _AnimationRoot.transform)
                stageObj.position += new Vector3(-THICKNESS,0f,THICKNESS);
        foreach (Transform stageObj in _AnimationRoot.transform)
        {
            Vector3 anchorPos;
            if (closeleft)
            {
                anchorPos = new Vector3(stageObj.position.x, 0f, _ZOffset);
                if(previous)
                    anchorPos += new Vector3(0f, 0f, THICKNESS);
            }
            else
            {
                anchorPos = new Vector3(_XOffset, 0f, stageObj.position.z);
                if(previous)
                    anchorPos += new Vector3(-THICKNESS, 0f, 0f);
            }
            bool dirX = Mathf.Abs(stageObj.eulerAngles.y) < 45f;
            TmpParameter tmpParam = stageObj.gameObject.AddComponent<TmpParameter>();
            tmpParam.AnimationAnchor = anchorPos;
            tmpParam.DirectionX = dirX;
            StartCoroutine(CloseObjectAnimation(stageObj, anchorPos, dirX, closeleft, time));
        }
    }

    private IEnumerator CloseObjectAnimation(Transform obj, Vector3 anchor, bool dirX, bool closeleft, float time)
    {
        int frameNum = 60;
        if (time <= 0.1f)
            frameNum = 1;
        for (int i = 0; i < frameNum; i++)
        {
            Quaternion currentRotation = obj.rotation;
            if (closeleft)
                obj.RotateAround(anchor, Vector3.up, 90f / frameNum);
            else
                obj.RotateAround(anchor, Vector3.up, -90f / frameNum);
            if (closeleft && dirX || !closeleft && !dirX)
                obj.rotation = currentRotation;
            yield return new WaitForSeconds(time / frameNum);
        }
        
    }
    
    public void Clear()
    {
        StopAllCoroutines();
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
