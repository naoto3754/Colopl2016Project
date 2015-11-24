using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class StageCreater : Singleton<StageCreater>
{
    public enum ReOpenType
    {
        FIRST_OPEN,
        TO_NEXT,
        RESTART_STAGE,
    }
    
    public static readonly string X_TAG_NAME = "XSideComponent";
	public static readonly string Z_TAG_NAME = "ZSideComponent";
    public static readonly float OFFSET = 0.02f;
    public static readonly float THICKNESS = 0.1f;
    
    [SerializeField]
    private GameObject _Paper;
	[SerializeField]
	private Sprite _Fallback;

    /// <summary>
    /// ステージを生成する。すでにステージがある場合、閉じた後に破棄する
    /// </summary>
    public void CreateNewStage(float xOffset = 50f, float zOffset = -50f)
    {
        bool existStage = StageManager.I.Root != null;
        if(existStage)
        {
            StageManager.I.PreviousRoot = StageManager.I.Root;
            StageManager.I.PreviousBackgroundLeft = StageManager.I.BackgroundLeft;
            StageManager.I.PreviousBackgroundRight = StageManager.I.BackgroundRight;
        }
        StageManager.I.Root = new GameObject("StageRoot");
        StageManager.I.BackgroundLeft = new GameObject("BackgroundLeft");
        StageManager.I.BackgroundRight = new GameObject("BackgroundRight");        
		StageManager.I.Offset = new Vector3(xOffset, 0, zOffset);
        StageManager.I.Root.transform.position = new Vector3(xOffset, 0f, zOffset);
        StageManager.I.BackgroundLeft.transform.position = new Vector3(xOffset+THICKNESS*3/2, 0f, zOffset);
        StageManager.I.BackgroundRight.transform.position = new Vector3(xOffset, 0f, zOffset+THICKNESS*3/2);
        
        InstantiatePaper();
        InstantiateDecoration();
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
            float prevX = -StageManager.I.CurrentInfo.StageWidth / 2;
			float xOffset = -StageManager.I.CurrentInfo.StageWidth / 2, zOffset = StageManager.I.Offset.z;
            IEnumerable<XCoord> xCoordList = StageManager.I.GetXCoordList((prevY + y) / 2, true);
            foreach (XCoord xCoord in xCoordList)
            {
                //折れ線の場合
                if(duringHole == false){
					if(xOffset != 0 && ( (setX && zOffset != StageManager.I.Offset.z) || (!setX && zOffset - (xCoord.x-prevX) != StageManager.I.Offset.z) ))
                        InstantiateBackground(xCoord.x, prevX, y, prevY, xOffset, yOffset, zOffset, thickness);
                    
                    GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
                    paper.transform.SetParent(StageManager.I.Root.transform);
					var sprite = paper.transform.GetChild (0);
					SetTexture (sprite.GetComponent<SpriteRenderer>(), setX, false,
								StageManager.I.CurrentInfo.BackgroundTexture, 
								new Vector2(prevX/StageManager.I.CurrentInfo.StageWidth+0.5f, prevY/StageManager.I.CurrentInfo.StageHeight), 
								new Vector2((xCoord.x-prevX)/StageManager.I.CurrentInfo.StageWidth, (y-prevY)/StageManager.I.CurrentInfo.StageHeight));
					foreach(var amim in StageManager.I.CurrentInfo.Animations)
					{
						GameObject layer = Instantiate (sprite.gameObject, sprite.position, sprite.rotation) as GameObject;
						layer.transform.SetParent (paper.transform);
						SetTexture (layer.GetComponent<SpriteRenderer>(), amim, 
									new Vector2(prevX/StageManager.I.CurrentInfo.StageWidth+0.5f, prevY/StageManager.I.CurrentInfo.StageHeight), 
									new Vector2((xCoord.x-prevX)/StageManager.I.CurrentInfo.StageWidth, (y-prevY)/StageManager.I.CurrentInfo.StageHeight));
					}
					
                    if (setX)
                    {
						paper.transform.position = new Vector3((xCoord.x - prevX) / 2 + xOffset + StageManager.I.Offset.x, (y - prevY) / 2 + yOffset, zOffset + thickness / 2);
                        xOffset += xCoord.x - prevX;
                        paper.tag = X_TAG_NAME;
                    }
                    else
                    {
						paper.transform.position = new Vector3(xOffset + StageManager.I.Offset.x + thickness / 2, (y - prevY) / 2 + yOffset, -(xCoord.x - prevX) / 2 + zOffset);
                        paper.transform.forward = Vector3.right;
                        zOffset -= xCoord.x - prevX;
                        paper.tag = Z_TAG_NAME;
                    }
                    paper.transform.eulerAngles += 180*Vector3.forward;
                    paper.transform.localScale = new Vector3(xCoord.x - prevX-0.001f, y - prevY, thickness);
                    
                    switch(xCoord.type)
                    {
                    case XCoord.Type.FOLD:
                        setX = !setX;
                        break;
                    case XCoord.Type.HOLE:
                        duringHole = true;
                        break;
                    case XCoord.Type.NONE:
                        //Do Nothing 
                        break;
                    }
                    
                    prevX = xCoord.x;
                }
                //穴の場合
                else 
                {
                    InstantiateBackground(xCoord.x, prevX, y, prevY, xOffset, yOffset, zOffset, thickness);
                    
                    if (setX)
                        xOffset += xCoord.x - prevX;
                    else
                        zOffset -= xCoord.x - prevX;
                    switch(xCoord.type)
                    {
                    case XCoord.Type.FOLD:
                        setX = !setX;
                        break;
                    case XCoord.Type.HOLE:
                        duringHole = false;
                        break;
                    case XCoord.Type.NONE:
                        //Do Nothing 
                        break;
                    }
                    prevX = xCoord.x;
                }
            }
            yOffset += y - prevY;
            prevY = y;
        }
    }
    
    private void InstantiateBackground(float x, float prevX, float y, float prevY, float xOffset, float yOffset, float zOffset, float thickness)
    {
		bool setX = xOffset - (zOffset - StageManager.I.Offset.z) + (x - prevX) / 2 < 0;
        GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
		SetTexture (paper.transform.GetChild(0).GetComponent<SpriteRenderer>(), setX, true,
					StageManager.I.CurrentInfo.LiningTexture, 
					new Vector2(prevX/StageManager.I.CurrentInfo.StageWidth+0.5f, prevY/StageManager.I.CurrentInfo.StageHeight), 
					new Vector2((x-prevX)/StageManager.I.CurrentInfo.StageWidth, (y-prevY)/StageManager.I.CurrentInfo.StageHeight) );

        if(setX)
        {
            paper.transform.SetParent(StageManager.I.BackgroundLeft.transform);
			paper.transform.position = new Vector3(xOffset - (zOffset-StageManager.I.Offset.z) + (x - prevX)/2 + StageManager.I.Offset.x+thickness/2, (y - prevY) / 2 + yOffset, StageManager.I.Offset.z + thickness*3/2);
        }   
        else
        {
            paper.transform.SetParent(StageManager.I.BackgroundRight.transform);
			paper.transform.position = new Vector3(StageManager.I.Offset.x + thickness*3/2, (y - prevY) / 2 + yOffset, -( xOffset - (zOffset-StageManager.I.Offset.z) + (x - prevX)/2 ) + StageManager.I.Offset.z+thickness/2);
            paper.transform.forward = Vector3.right;
        }
        paper.transform.eulerAngles += 180*Vector3.forward;
        paper.transform.localScale = new Vector3(x - prevX + thickness - 0.001f, y - prevY, thickness);
    }

	private void SetTexture(SpriteRenderer target, bool setX, bool isBackground, Sprite sprite, Vector2 offset, Vector2 scale)
	{
		if (sprite == null) {
			target.sprite = _Fallback;
			if (isBackground)
				target.color = new Color(1f, 165f/255f, 200f/255f, 1f);
			if (setX)
				ColorManager.MultiplyShadowColor (target.gameObject);
		} else {
			target.sprite = sprite;
		}
		target.sortingOrder = isBackground ? -100 : -50;
		target.material.SetFloat ("_OffsetX", offset.x);
		target.material.SetFloat ("_OffsetY", offset.y);
		target.material.SetFloat ("_TilingX", scale.x);
		target.material.SetFloat ("_TilingY", scale.y);
	}
	private void SetTexture(SpriteRenderer target, RuntimeAnimatorController anim, Vector2 offset, Vector2 scale)
	{
		Animator animator = target.gameObject.AddComponent<Animator> ();
		animator.runtimeAnimatorController = anim;
		target.sortingOrder = -40;
		target.material.SetFloat ("_OffsetX", offset.x);
		target.material.SetFloat ("_OffsetY", offset.y);
		target.material.SetFloat ("_TilingX", scale.x);
		target.material.SetFloat ("_TilingY", scale.y);
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
				//表示物がないオブジェクトなら処理をしない
				if (child.GetComponent<Renderer> () == null &&
					child.GetComponent<SpriteRenderer> () == null &&
					child.GetComponent<LineRenderer> () == null)
					continue;

				SetDecoration(child.gameObject);
			}
		}
	}
    /// <summary>
    /// 表示物をセット
    /// </summary>
    private void SetDecoration(GameObject deco)
	{
        //装飾オブジェククトの表示
        Vector3 decoPos = deco.transform.position;
        Vector3 decoScale = deco.transform.lossyScale;
        Vector3 decoSetPos = new Vector3(-StageManager.I.CurrentInfo.StageWidth / 2 - OFFSET * 2 + StageManager.I.Offset.x, decoPos.y, StageManager.I.Offset.z - OFFSET * 2);

		var line = deco.GetComponent<LineRenderer> ();
		var param = deco.GetComponent<StageObjectParameter> ();
		float anchorHeightScale = param == null ? 0f : param.HeightWithMaxWidth;

        bool facingX = true;
        float prevX = -StageManager.I.CurrentInfo.StageWidth / 2;

        foreach (float x in StageManager.I.GetFoldXCoordList(decoPos.y + decoScale.y / 2 * anchorHeightScale, true))
        {
			float leftEnd = line==null ? decoPos.x - decoScale.x / 2: decoPos.x;
            if (leftEnd < x)
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
		Destroy (newDeco.GetComponent<StageObjectParameter> ());
		param.ObjectsOnStage.Add (newDeco);
        newDeco.transform.SetParent(StageManager.I.Root.transform);

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
		Vector2 decoAnchorPos = line == null ? 
			new Vector2(decoPos.x - delta / 2,decoPos.y + decoScale.y / 2 * anchorHeightScale) :
			new Vector2(decoPos.x, decoPos.y);
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
				Destroy (newDeco2.GetComponent<StageObjectParameter> ());
				param.ObjectsOnStage.Add (newDeco2);
                newDeco2.transform.SetParent(StageManager.I.Root.transform);
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
                newDeco2.transform.SetParent(StageManager.I.Root.transform);
                ColorManager.MultiplyShadowColor(newDeco2);

                newDeco.GetComponent<Renderer>().material.SetFloat("_ForwardThreshold", foldlineDist / delta);
                newDeco2.GetComponent<Renderer>().material.SetFloat("_BackThreshold", foldlineDist / delta);
                newDeco2.tag = X_TAG_NAME;
            }
        }
    }
}

//折れ線と穴開け用の線のどちらの線かを判断するために実装。
public class XCoord
{
    public float x;
    public Type type;
    
    public XCoord(float x, Type type)
    {
        this.x = x;
        this.type = type;
    }
    
    public enum Type
    {
        FOLD,
        HOLE,
        NONE, 
    }
}
