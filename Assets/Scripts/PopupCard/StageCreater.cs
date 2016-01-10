using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;

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
    public static readonly float OFFSET = 0.1f;
    public static readonly float THICKNESS = 0.1f;
    
    [SerializeField]
    private GameObject _Paper;
	[SerializeField]
	private Texture _Fallback;

    /// <summary>
    /// ステージを生成する。すでにステージがある場合、閉じた後に破棄する
    /// </summary>
    public void CreateNewStage(float xOffset = 50f, float zOffset = -50f)
    {
        bool existStage = StageManager.I.PaperRoot != null;
        if(existStage)
        {
			StageManager.I.PrevStageRoot = StageManager.I.StageRoot;
            StageManager.I.PrevPaperRoot = StageManager.I.PaperRoot;
			StageManager.I.PrevDecoRoot = StageManager.I.DecoRoot;
            StageManager.I.PrevBackRootL = StageManager.I.BackRootL;
            StageManager.I.PrevBackRootR = StageManager.I.BackRootR;
        }
		StageManager.I.StageRoot = new GameObject("StageRoot");
        StageManager.I.PaperRoot = new GameObject("PaperRoot");
		StageManager.I.DecoRoot = new GameObject("DecoRoot");
        StageManager.I.BackRootL = new GameObject("BackgroundLeft");
        StageManager.I.BackRootR = new GameObject("BackgroundRight");
		StageManager.I.Offset = new Vector3(xOffset, 0, zOffset);
        StageManager.I.PaperRoot.transform.position = new Vector3(xOffset, 0f, zOffset);
		StageManager.I.PaperRoot.transform.SetParent (StageManager.I.StageRoot.transform);
		StageManager.I.DecoRoot.transform.position = new Vector3(xOffset-OFFSET, 0f, zOffset-OFFSET);
		StageManager.I.DecoRoot.transform.SetParent (StageManager.I.StageRoot.transform);
        StageManager.I.BackRootL.transform.position = new Vector3(xOffset+THICKNESS*3/2, 0f, zOffset);
		StageManager.I.BackRootL.transform.SetParent (StageManager.I.StageRoot.transform);
        StageManager.I.BackRootR.transform.position = new Vector3(xOffset, 0f, zOffset+THICKNESS*3/2);
		StageManager.I.BackRootR.transform.SetParent (StageManager.I.StageRoot.transform);
        
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
            IEnumerable<XCoord> xCoordList = StageManager.I.GetXCoordList((prevY + y) / 2, false);
            foreach (XCoord xCoord in xCoordList)
            {
                //折れ線の場合
                if(duringHole == false){
					if(xOffset != 0 && ( (setX && zOffset != StageManager.I.Offset.z) || (!setX && zOffset - (xCoord.x-prevX) != StageManager.I.Offset.z) ))
                        InstantiateBackground(xCoord.x, prevX, y, prevY, xOffset, yOffset, zOffset, thickness);
                    
                    GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
                    paper.transform.SetParent(StageManager.I.PaperRoot.transform);
					var sprite = paper.transform.GetChild (0);
					SetTexture (sprite.GetComponent<Renderer>(), setX, false,
								StageManager.I.CurrentInfo.BackgroundTexture,
								StageManager.I.CurrentInfo.ShadowTexture,
								new Vector2((prevX/StageManager.I.CurrentInfo.StageWidth+0.5f+StageManager.I.CurrentStageIndex)/3f, (prevY/StageManager.I.CurrentInfo.StageHeight+StageManager.I.CurrentBookID)/3f), 
								new Vector2((xCoord.x-prevX)/StageManager.I.CurrentInfo.StageWidth/3f, (y-prevY)/StageManager.I.CurrentInfo.StageHeight/3f));
					foreach(var amim in StageManager.I.CurrentInfo.Animations)
					{
						GameObject layer = Instantiate (sprite.gameObject, sprite.position, sprite.rotation) as GameObject;
						layer.transform.SetParent (paper.transform);
						SetTexture (layer.GetComponent<SpriteRenderer>(), amim, 
							new Vector2((prevX/StageManager.I.CurrentInfo.StageWidth+0.5f+StageManager.I.CurrentStageIndex)/3f, (prevY/StageManager.I.CurrentInfo.StageHeight+StageManager.I.CurrentBookID)/3f), 
									new Vector2((xCoord.x-prevX)/StageManager.I.CurrentInfo.StageWidth/3f, (y-prevY)/StageManager.I.CurrentInfo.StageHeight/3f));
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
		SetTexture (paper.transform.GetChild(0).GetComponent<Renderer>(), setX, true,
					StageManager.I.CurrentInfo.LiningTexture,
					StageManager.I.CurrentInfo.LiningTexture,
					new Vector2((prevX/StageManager.I.CurrentInfo.StageWidth+0.5f+StageManager.I.CurrentStageIndex)/3f, (prevY/StageManager.I.CurrentInfo.StageHeight+StageManager.I.CurrentBookID)/3f), 
					new Vector2((x-prevX)/StageManager.I.CurrentInfo.StageWidth/3, (y-prevY)/StageManager.I.CurrentInfo.StageHeight/3) );

        if(setX)
        {
            paper.transform.SetParent(StageManager.I.BackRootL.transform);
			paper.transform.position = new Vector3(xOffset - (zOffset-StageManager.I.Offset.z) + (x - prevX)/2 + StageManager.I.Offset.x+thickness/2, (y - prevY) / 2 + yOffset, StageManager.I.Offset.z + thickness*3/2);
        }   
        else
        {
            paper.transform.SetParent(StageManager.I.BackRootR.transform);
			paper.transform.position = new Vector3(StageManager.I.Offset.x + thickness*3/2, (y - prevY) / 2 + yOffset, -( xOffset - (zOffset-StageManager.I.Offset.z) + (x - prevX)/2 ) + StageManager.I.Offset.z+thickness/2);
            paper.transform.forward = Vector3.right;
        }
        paper.transform.eulerAngles += 180*Vector3.forward;
        paper.transform.localScale = new Vector3(x - prevX + thickness - 0.001f, y - prevY, thickness);
    }

	private void SetTexture(Renderer target, bool setX, bool isBackground, Texture texture, Texture shadow, Vector2 offset, Vector2 scale)
	{
		if (texture == null) {
			target.material.SetTexture ("_NoShadowTex", _Fallback);
			target.material.SetTexture ("_ShadowTex", _Fallback);
			if (isBackground)
				target.material.color = StageManager.I.CurrentInfo.BackgroundColor;
			if (setX)
				ColorManager.MultiplyShadowColor (target.gameObject);
		} else {
			target.material.SetTexture ("_NoShadowTex", texture);
			target.material.SetTexture ("_ShadowTex", shadow);
		}
		target.sortingOrder = isBackground ? -100 : -50;
		target.material.SetTextureOffset("_NoShadowTex", offset);
		target.material.SetTextureOffset("_ShadowTex", offset);
		target.material.SetTextureScale("_NoShadowTex", scale);
		target.material.SetTextureScale("_ShadowTex", scale);
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
			CreateDeco(decos);
			foreach (Transform child in decos.transform)
			{
				//表示物がないオブジェクトなら処理をしない
				if (child.GetComponent<Renderer> () == null &&
					child.GetComponent<SpriteRenderer> () == null &&
					child.GetComponent<LineRenderer> () == null)
					continue;

				CreateDeco(child.gameObject);
			}
		}
	}
    /// <summary>
    /// 表示物をセット
    /// </summary>
    private void CreateDeco(GameObject deco)
	{
        //装飾オブジェククトの表示
        Vector3 decoPos = deco.transform.position;
        Vector3 decoScale = deco.transform.lossyScale;
        Vector3 decoSetPos = new Vector3(-StageManager.I.CurrentInfo.StageWidth / 2 - OFFSET * 2 + StageManager.I.Offset.x, decoPos.y, StageManager.I.Offset.z - OFFSET * 2);

		var tmPro = deco.GetComponent<TextMeshPro>();
		if (tmPro != null) {
			decoScale = UnityUtility.MultipleEachElement(decoScale, tmPro.bounds.size);
		}

		var param = deco.GetComponent<StageObjectParameter> ();
		float anchorHeightScale = param == null ? 0f : param.HeightWithMaxWidth;

        bool facingX = true;
        float prevX = -StageManager.I.CurrentInfo.StageWidth / 2;

		float yCoord = decoPos.y + decoScale.y / 2 * anchorHeightScale;
		foreach (float x in StageManager.I.GetFoldXCoordList(yCoord, false))
        {
			float leftEnd = decoPos.x - decoScale.x / 2;
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
        GameObject newDeco = Instantiate(deco, decoSetPos, Quaternion.identity) as GameObject;
		Destroy (newDeco.GetComponent<StageObjectParameter> ());
		Destroy (newDeco.GetComponent<EventBase> ());
		param.ObjectsOnStage.Add (newDeco);
        newDeco.transform.SetParent(StageManager.I.DecoRoot.transform);

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
		CreateSecondDeco(deco, newDeco, decoPos, decoScale, anchorHeightScale, facingX, tmPro, param);
    }

	private void CreateSecondDeco(GameObject orig, GameObject deco, Vector3 pos, Vector3 scale, float anchorHeightScale, bool facingX, TextMeshPro tmPro, StageObjectParameter param)
	{
		float delta = scale.x;
		Vector2 decoAnchorPos = new Vector2(pos.x - delta / 2,pos.y + scale.y / 2 * anchorHeightScale);
		List<float> foldlineDists = StageManager.I.CalcFoldLineDistance(decoAnchorPos, delta, false);
		if (foldlineDists.Count != 0)
		{
			GameObject deco2;
			Vector3 newDecoPos = Vector3.up * deco.transform.position.y;
			if (facingX)
			{
				newDecoPos.x = deco.transform.position.x - scale.x / 2 + foldlineDists[0];
				newDecoPos.z = deco.transform.position.z - scale.x / 2 + foldlineDists[0];
				deco2 = Instantiate(orig, newDecoPos, Quaternion.identity) as GameObject;
				deco2.transform.eulerAngles += new Vector3(0f, 90f, 0f);
				deco2.tag = Z_TAG_NAME;
			}
			else
			{
				newDecoPos.x = deco.transform.position.x + scale.x / 2 - foldlineDists[0];
				newDecoPos.z = deco.transform.position.z + scale.x / 2 - foldlineDists[0];
				deco2 = Instantiate(orig, newDecoPos, Quaternion.identity) as GameObject;
				deco2.tag = X_TAG_NAME;
				ColorManager.MultiplyShadowColor(deco2);
			}
				
			Destroy (deco2.GetComponent<StageObjectParameter> ());
			Destroy (deco2.GetComponent<EventBase> ());
			param.ObjectsOnStage.Add (deco2);
			deco2.transform.SetParent (StageManager.I.DecoRoot.transform);

			if (tmPro == null) {
				deco.GetComponent<Renderer> ().material.SetFloat (Constants.M_FORWARD_THRES, foldlineDists[0] / delta);
				deco2.GetComponent<Renderer> ().material.SetFloat (Constants.M_BACK_THRES, foldlineDists[0] / delta);
			} else {
				float width = tmPro.bounds.size.x;
				deco.GetComponent<Renderer> ().material.SetFloat (Constants.M_FORWARD_THRES, width/2 - ((delta-foldlineDists[0]) / delta) * width);
				deco.GetComponent<Renderer> ().material.SetFloat (Constants.M_BACK_THRES, -255);
				deco2.GetComponent<Renderer> ().material.SetFloat (Constants.M_FORWARD_THRES, 255);
				deco2.GetComponent<Renderer> ().material.SetFloat (Constants.M_BACK_THRES, - width/2 + (foldlineDists[0] / delta) * width);
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
