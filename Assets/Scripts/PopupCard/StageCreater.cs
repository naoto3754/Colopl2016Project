using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageCreater : Singlton<StageCreater>
{
    public static readonly float OFFSET = 0.02f;
    private float _XOffset;
    private float _ZOffset;
    private GameObject _Root;

    [SerializeField]
    private GameObject _Paper;
    [SerializeField]
    private Transform _StageSize;
    private float StageWidth
    {
        get { return _StageSize.localScale.x; }
    }
    private float StageHeight
    {
        get { return _StageSize.localScale.y; }
    }

    void Start()
    {
        CreateStage();
    }

    public void CreateStage(float xOffset = 0f, float zOffset = -50f)
    {
        _XOffset = xOffset;
        _ZOffset = zOffset;
        _Root = new GameObject("StageRoot");
        InstantiateCharacter();
        InstantiateStage();
        InstantiateDecoration();
    }

    /// <summary>
    /// キャラクターを生成する
    /// </summary>
    private void InstantiateCharacter()
    {
        //X方向に動くキャラクター
        GameObject character = Instantiate(CharacterController.I.DummyCharacter,
                                           CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset + -OFFSET, 0f, _ZOffset),
                                           Quaternion.identity) as GameObject;
        character.transform.SetParent(_Root.transform);
        character.layer = 0;
        foreach (Transform child in character.transform)
            child.gameObject.layer = 0;
        //TODO:色を決める
        character.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_MainColor", new Color(0.9f, 0.45f, 0.45f));
        CharacterController.I.CharacterX = character;
        //Z方向に動くキャラクター
        character = Instantiate(CharacterController.I.DummyCharacter,
                                CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset + -OFFSET, 0f, _ZOffset),
                                Quaternion.identity) as GameObject;
        character.transform.Rotate(0f, 90f, 0f);
        character.transform.SetParent(_Root.transform);
        //TODO:色を決める
        character.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_MainColor", new Color(1f, 1f, 1f));
        character.transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_ForwardThreshold", 0f);
        character.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_MainColor", new Color(1f, 0.5f, 0.5f));
        character.transform.GetChild(1).GetComponent<Renderer>().material.SetFloat("_ForwardThreshold", 0f);
        character.layer = 0;
        foreach (Transform child in character.transform)
            child.gameObject.layer = 0;
        CharacterController.I.CharacterZ = character;
    }

    /// <summary>
    /// ステージのカード部分をを生成する
    /// </summary>
    private void InstantiateStage()
    {
        //ステージオブジェクト生成
        IEnumerable<float> yCoordList = DummyCard.I.GetSortYCoordList();
        float prevY = yCoordList.First();
        float yOffset = 0f;
        foreach (float y in yCoordList)
        {
            if (y == yCoordList.First())
                continue;
            bool setX = true;
            float prevX = -StageWidth / 2;
            float xOffset = -StageWidth / 2, zOffset = _ZOffset;
            foreach (float x in DummyCard.I.GetSortXCoordList((prevY + y) / 2))
            {
                GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
                paper.transform.SetParent(_Root.transform);
                if (setX)
                {
                    paper.transform.position = new Vector3((x - prevX) / 2 + xOffset + _XOffset, (y - prevY) / 2 + yOffset, zOffset);
                    xOffset += x - prevX;
                }
                else
                {
                    paper.transform.position = new Vector3(xOffset + _XOffset, (y - prevY) / 2 + yOffset, -(x - prevX) / 2 + zOffset);
                    paper.transform.forward = Vector3.right;
                    zOffset -= x - prevX;
                }
                paper.transform.localScale = new Vector3(x - prevX, y - prevY, 1f);
                setX = !setX;
                prevX = x;
            }
            GameObject lastPaper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
            lastPaper.transform.SetParent(_Root.transform);
            lastPaper.transform.position = new Vector3(xOffset + _XOffset, (y - prevY) / 2 + yOffset, -(StageWidth / 2 - prevX) / 2 + zOffset);
            lastPaper.transform.forward = Vector3.right;
            lastPaper.transform.localScale = new Vector3(StageWidth / 2 - prevX, y - prevY, 1f);
            yOffset += y - prevY;
            prevY = y;
        }
    }

    /// <summary>
    /// 見た目に必要なオブジェクトを生成する
    /// </summary>
    private void InstantiateDecoration()
    {
        foreach (GameObject decos in DummyCard.I.Decoration)
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
        Vector3 decoScale = deco.transform.localScale;
        Vector3 decoSetPos = new Vector3(-StageWidth / 2 - 0.01f + _XOffset, decoPos.y, _ZOffset - 0.01f);
        bool facingX = true;
        float prevX = -StageWidth / 2;

        foreach (float x in DummyCard.I.GetSortXCoordList(decoPos.y))
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
        float anchorHeightScale = 0f;
        if(deco.GetComponent<DecorationObjectParameter>() != null)
            anchorHeightScale = deco.GetComponent<DecorationObjectParameter>().leftHeightWithMaxWidth;
        
        Vector2 decoAnchorPos = new Vector2(decoPos.x - delta / 2,
                                            decoPos.y + decoScale.y / 2 * anchorHeightScale);
        float foldlineDist = DummyCard.I.CalcFoldLineDistance(decoAnchorPos, delta);
        if (Mathf.Abs(foldlineDist) < Mathf.Abs(delta))
        {
            if (facingX)
            {
                Vector3 newDecoPos;
                newDecoPos.x = newDeco.transform.position.x - decoScale.x/2 + foldlineDist;
                newDecoPos.y = newDeco.transform.position.y;
                newDecoPos.z = newDeco.transform.position.z - decoScale.x/2 + foldlineDist;
                GameObject newDeco2 = Instantiate(deco, newDecoPos, deco.transform.rotation) as GameObject;
                newDeco2.transform.eulerAngles += new Vector3(0f, 90f, 0f);
                newDeco.GetComponent<Renderer>().material.SetFloat("_ForwardThreshold", foldlineDist/delta);
                newDeco2.GetComponent<Renderer>().material.SetFloat("_BackThreshold", foldlineDist/delta);
            }
            else
            {
                Vector3 newDecoPos;
                newDecoPos.x = newDeco.transform.position.x + decoScale.x/2 - foldlineDist;
                newDecoPos.y = newDeco.transform.position.y;
                newDecoPos.z = newDeco.transform.position.z + decoScale.x/2 - foldlineDist;
                GameObject newDeco2 = Instantiate(deco, newDecoPos, deco.transform.rotation) as GameObject;
                ColorManager.MultiplyShadowColor(newDeco2);
                
                newDeco.GetComponent<Renderer>().material.SetFloat("_BackThreshold", foldlineDist/delta);
                newDeco2.GetComponent<Renderer>().material.SetFloat("_ForwardThreshold", foldlineDist/delta);
            }
            
        }
    }
}
