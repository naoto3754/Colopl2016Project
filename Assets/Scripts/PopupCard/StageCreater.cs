﻿using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageCreater : Singlton<StageCreater>
{
    public static readonly float OFFSET = 0.01f;
    private readonly float ANIMATION_TIME = 0.5f;
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
    [SerializeField]
    private GameObject _Paper;
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

    public void CreateNewStage(float xOffset = 0f, float zOffset = -50f)
    {
        bool existStage = _Root != null;
        if(existStage)
            _PreviousRoot = _Root;    
        _Root = new GameObject("StageRoot");
        _XOffset = xOffset;
        _ZOffset = zOffset;
        InstantiateCharacter();
        InstantiatePaper();
        InstantiateDecoration();
        //HACK:キャラの向きや透過処理をさせたい
        CharacterController.I.UpdateCharacterState(Vector2.right);
        
        if(existStage)
        {
            CloseStage(ANIMATION_TIME, true, true);
            CloseStage(0f, false);
            Invoke("OpenForInvoke", 0.02f);

        }
        else
        {
            CloseStage(0f, false);
            OpenStage(ANIMATION_TIME);
        }
    }
    private void OpenForInvoke()
    {
        OpenStage(ANIMATION_TIME);
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
    /// ステージのカード部分をを生成する
    /// </summary>
    private void InstantiatePaper()
    {
        float thickness = _Paper.transform.localScale.z;
        //背景の生成
        //x方向
        GameObject background = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
        background.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        background.GetComponent<Renderer>().material.color = StageManager.I.CurrentInfo.BackgroundColor;
        background.transform.SetParent(_Root.transform);
        background.transform.position = new Vector3(-StageWidth/4 + thickness/2 + _XOffset, StageHeight/2, thickness/2 + _ZOffset);
        background.transform.localScale = new Vector3(StageWidth/2, StageHeight, thickness/2);
        //z方向
        background = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
        background.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        background.GetComponent<Renderer>().material.color = StageManager.I.CurrentInfo.BackgroundColor;
        background.transform.SetParent(_Root.transform);
        background.transform.position = new Vector3(thickness/2 + _XOffset, StageHeight/2, -StageWidth/4 + thickness/2 + _ZOffset);
        background.transform.localScale = new Vector3(StageWidth/2, StageHeight, thickness/2);
        background.transform.forward = Vector3.right;
        //ステージの紙オブジェクト生成
        IEnumerable<float> yCoordList = StageManager.I.GetSortYCoordList();
        float prevY = yCoordList.First();
        float yOffset = 0f;
        foreach (float y in yCoordList)
        {
            if (y == yCoordList.First())
                continue;
            bool setX = true;
            float prevX = -StageWidth / 2;
            float xOffset = -StageWidth / 2, zOffset = _ZOffset;
            IEnumerable<float> xCoordList = StageManager.I.GetSortXCoordList((prevY + y) / 2);
            foreach (float x in xCoordList)
            {
                GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
                paper.transform.SetParent(_Root.transform);
                if (setX)
                {
                    paper.transform.position = new Vector3((x - prevX) / 2 + xOffset + _XOffset, (y - prevY) / 2 + yOffset, zOffset + thickness / 2);
                    xOffset += x - prevX;
                }
                else
                {
                    paper.transform.position = new Vector3(xOffset + _XOffset + thickness / 2, (y - prevY) / 2 + yOffset, -(x - prevX) / 2 + zOffset);
                    paper.transform.forward = Vector3.right;
                    paper.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                    zOffset -= x - prevX;
                }
                paper.transform.localScale = new Vector3(x - prevX, y - prevY, _Paper.transform.localScale.z);
                if (x == xCoordList.First())
                    paper.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;

                setX = !setX;
                prevX = x;
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

        foreach (float x in StageManager.I.GetSortXCoordList(decoPos.y + decoScale.y / 2 * anchorHeightScale))
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
        float foldlineDist = StageManager.I.CalcFoldLineDistance(decoAnchorPos, delta);
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

    public void OpenStage(float time)
    {
        IsPlayingAnimation = true;
        bool openleft = !TmpParameter.CloseDirctionLeft;
        foreach (Transform stageObj in _Root.transform)
        {
            TmpParameter tmpParam = stageObj.GetComponent<TmpParameter>();
            Vector3 anchorPos = tmpParam.AnimationAnchor;
            bool dirX = tmpParam.DirectionX;
            StartCoroutine(OpenObjectAnimation(stageObj, anchorPos, dirX, openleft, time));
        }
    }

    private IEnumerator OpenObjectAnimation(Transform obj, Vector3 anchor, bool dirX, bool openleft, float time)
    {
        int frameNum = 120;
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
    }

    public void CloseStage(float time, bool closeleft, bool previous = false)
    {
        IsPlayingAnimation = true;
        TmpParameter.CloseDirctionLeft = closeleft;
        GameObject _AnimationRoot = previous ? _PreviousRoot : _Root;
        foreach (Transform stageObj in _AnimationRoot.transform)
        {
            Vector3 anchorPos;
            if (closeleft)
            {
                anchorPos = new Vector3(stageObj.position.x, 0f, _ZOffset);
            }
            else
            {
                anchorPos = new Vector3(_XOffset, 0f, stageObj.position.z);
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
        int frameNum = 120;
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
}
