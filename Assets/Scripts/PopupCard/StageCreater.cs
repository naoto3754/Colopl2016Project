using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageCreater : Singlton<StageCreater>
{
    public static readonly float OFFSET = 0.01f;
    private readonly float ANIMATION_INITIAL_WEIGHT = 0.02f;
    private float _XOffset;
    private float _ZOffset;
    
    private GameObject _Root;
    [SerializeField]
    private GameObject _Paper;
    [SerializeField]
    private Transform _StageSize;
    private float StageWidth
    {
        get { return _StageSize.lossyScale.x; }
    }
    private float StageHeight
    {
        get { return _StageSize.lossyScale.y; }
    }
    public bool IsPlayingAnimation
    {
        get;
        set;
    }

    void Start()
    {
        _Root = new GameObject("StageRoot");
        CreateStage();
        CloseStage(0f,false);
        OpenStage(1f);
    }

    public void CreateStage(float xOffset = 0f, float zOffset = -50f)
    {
        _XOffset = xOffset;
        _ZOffset = zOffset;
        InstantiateCharacter();
        InstantiatePaper();
        InstantiateDecoration();
    }

    /// <summary>
    /// キャラクターを生成する
    /// </summary>
    private void InstantiateCharacter()
    {
        //X方向に動くキャラクター
        GameObject character = Instantiate(CharacterController.I.DummyCharacter,
                                           CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset - OFFSET*2, 0f, _ZOffset - OFFSET*2),
                                           Quaternion.identity) as GameObject;
        character.transform.SetParent(_Root.transform);
        character.layer = 0;
        foreach (Transform child in character.transform)
            child.gameObject.layer = 0;
        //TODO:色を決める
        character.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_MainColor", ColorManager.GetColorWithColorData(StageManager.I.CurrentInfo.InitialCharacterColor));
        ColorManager.MultiplyShadowColor(character);
        CharacterController.I.CharacterX = character;
        //Z方向に動くキャラクター
        character = Instantiate(CharacterController.I.DummyCharacter,
                                CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset - OFFSET*2, 0f, _ZOffset - OFFSET*2),
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
        background.transform.position = new Vector3(-StageWidth/4+0.1f+_XOffset, StageHeight/2, 0.1f+_ZOffset);
        background.transform.localScale = new Vector3(StageWidth/2, StageHeight, thickness);
        //z方向
        background = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
        background.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        background.GetComponent<Renderer>().material.color = StageManager.I.CurrentInfo.BackgroundColor;
        background.transform.SetParent(_Root.transform);
        background.transform.position = new Vector3(0.1f+_XOffset, StageHeight/2, -StageWidth/4+0.1f+_ZOffset);
        background.transform.localScale = new Vector3(StageWidth/2, StageHeight, thickness);
        background.transform.forward = Vector3.right;
        //ステージの紙オブジェクト生成
        IEnumerable<float> yCoordList = StageManager.I.CurrentInfo.GetSortYCoordList();
        float prevY = yCoordList.First();
        float yOffset = 0f;
        foreach (float y in yCoordList)
        {
            if (y == yCoordList.First())
                continue;
            bool setX = true;
            float prevX = -StageWidth / 2;
            float xOffset = -StageWidth / 2, zOffset = _ZOffset;
            IEnumerable<float> xCoordList = StageManager.I.CurrentInfo.GetSortXCoordList((prevY + y) / 2);
            foreach (float x in xCoordList)
            {
                GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
                paper.transform.SetParent(_Root.transform);
                if (setX)
                {
                    paper.transform.position = new Vector3((x - prevX) / 2 + xOffset + _XOffset, (y - prevY) / 2 + yOffset, zOffset+thickness/2);
                    xOffset += x - prevX;
                }
                else
                {
                    paper.transform.position = new Vector3(xOffset + _XOffset + thickness/2, (y - prevY) / 2 + yOffset, -(x - prevX) / 2 + zOffset);
                    paper.transform.forward = Vector3.right;
                    paper.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                    zOffset -= x - prevX;
                }
                paper.transform.localScale = new Vector3(x - prevX, y - prevY, _Paper.transform.localScale.z);
                if(x == xCoordList.First())
                    paper.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                    
                setX = !setX;
                prevX = x;
            }
            GameObject lastPaper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
            lastPaper.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
            lastPaper.transform.SetParent(_Root.transform);
            lastPaper.transform.position = new Vector3(xOffset + _XOffset + thickness/2, (y - prevY) / 2 + yOffset, -(StageWidth / 2 - prevX) / 2 + zOffset);
            lastPaper.transform.forward = Vector3.right;
            lastPaper.transform.localScale = new Vector3(StageWidth / 2 - prevX, y - prevY, thickness);
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
        Vector3 decoSetPos = new Vector3(-StageWidth / 2 - OFFSET*2 + _XOffset, decoPos.y, _ZOffset - OFFSET*2);

        float anchorHeightScale = 0f;
        if (deco.GetComponent<DecorationObjectParameter>() != null)
            anchorHeightScale = deco.GetComponent<DecorationObjectParameter>().leftHeightWithMaxWidth;

        bool facingX = true;
        float prevX = -StageWidth / 2;

        foreach (float x in StageManager.I.CurrentInfo.GetSortXCoordList(decoPos.y + decoScale.y / 2 * anchorHeightScale))
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
        float foldlineDist = StageManager.I.CurrentInfo.CalcFoldLineDistance(decoAnchorPos, delta);
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
        foreach(Transform stageObj in _Root.transform)
        {
            TmpParameter tmpParam = stageObj.GetComponent<TmpParameter>();
            Vector3 anchorPos = tmpParam.AnimationAnchor;
            bool dirX = tmpParam.DirectionX;
            StartCoroutine(OpenObjectAnimation(stageObj, anchorPos, dirX, openleft, time));
            
        }
    }
    
    private IEnumerator OpenObjectAnimation(Transform obj, Vector3 anchor, bool dirX, bool openleft ,float time)
    {
        int frameNum = 60;
        float weight = ANIMATION_INITIAL_WEIGHT;
        if(time == 0f)
        {
            frameNum = 1;
            weight = 1f;
        } 
        for(int i = 0; i < frameNum; i++)
        {
            Quaternion currentRotation = obj.rotation;
            if(openleft)
                obj.RotateAround(anchor, Vector3.up,  90f/frameNum*weight);
            else
                obj.RotateAround(anchor, Vector3.up, -90f/frameNum*weight);
            if(openleft && !dirX || !openleft && dirX)
                obj.rotation = currentRotation;
            if(i < frameNum/2)
                weight += (1f-ANIMATION_INITIAL_WEIGHT)/frameNum*4;
            else
                weight -= (1f-ANIMATION_INITIAL_WEIGHT)/frameNum*4;
            yield return new WaitForSeconds(time/frameNum);
        }
        IsPlayingAnimation = false;
        Destroy(obj.GetComponent<TmpParameter>());
    }
    
    public void CloseStage(float time, bool closeleft)
    {
        IsPlayingAnimation = true;
        TmpParameter.CloseDirctionLeft = closeleft;
        foreach(Transform stageObj in _Root.transform)
        {
            Vector3 anchorPos;
            if(closeleft)
                anchorPos = new Vector3(stageObj.position.x, 0f, _ZOffset);
            else
                anchorPos = new Vector3(_XOffset, 0f, stageObj.position.z);
            bool dirX = stageObj.eulerAngles == Vector3.zero;
            TmpParameter tmpParam = stageObj.gameObject.AddComponent<TmpParameter>();
            tmpParam.AnimationAnchor = anchorPos;
            tmpParam.DirectionX = dirX;
            StartCoroutine(CloseObjectAnimation(stageObj, anchorPos, dirX, closeleft, time));
        }
    }
    
    private IEnumerator CloseObjectAnimation(Transform obj, Vector3 anchor, bool dirX, bool closeleft ,float time)
    {
        int frameNum = 60;
        float weight = ANIMATION_INITIAL_WEIGHT;
        if(time == 0f)
        {
            frameNum = 1;
            weight = 1f;
        }
        for(int i = 0; i < frameNum; i++)
        {
            Quaternion currentRotation = obj.rotation;
            if(closeleft)
                obj.RotateAround(anchor, Vector3.up,  90f/frameNum*weight);
            else
                obj.RotateAround(anchor, Vector3.up, -90f/frameNum*weight);
            if(closeleft && dirX || !closeleft && !dirX)
                obj.rotation = currentRotation;
            if(i < frameNum/2)
                weight += (1f-ANIMATION_INITIAL_WEIGHT)/frameNum*4;
            else
                weight -= (1f-ANIMATION_INITIAL_WEIGHT)/frameNum*4;
            yield return new WaitForSeconds(time/frameNum);
        }
    }
}
