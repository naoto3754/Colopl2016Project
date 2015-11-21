using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class StageCreater : Singlton<StageCreater>
{
    public enum ReOpenType
    {
        FIRST_OPEN,
        TO_NEXT,
        RESTART_STAGE,
    }
    
    public readonly string X_TAG_NAME = "XSideComponent";
    public readonly string Z_TAG_NAME = "ZSideComponent";
    public readonly float START_ANGLE = 45f;
    public readonly float SHADOW_WEIGHT = 0.8f;
    
    private Ease OPEN_EASE = Ease.Linear;
    private Ease CLOSE_EASE = Ease.Linear;
    public static readonly float OFFSET = 0.02f;
    public readonly float THICKNESS = 0.1f;
    public readonly float ANIMATION_TIME = 1f;
    
    [SerializeField]
    private GameObject _Paper;
	[SerializeField]
	private Sprite _Fallback;
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
    private GameObject _BackgroundLeft;
    private GameObject _BackgroundRight;
    private GameObject _PreviousBackgroundLeft;
    private GameObject _PreviousBackgroundRight;
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

    public void Reverse(){
        if(IsPlayingAnimation == false)
        {
            _Sequence = DOTween.Sequence();
            ReOpenStageForReverse(1f, 1f, 0.3f);
            Vector3 pos = CharacterController.I.DummyCharacter.transform.position;
            pos.x *= -1;
            CharacterController.I.DummyCharacter.transform.position = pos; 
        }
    }
    
    public void RestartStage()
    {
        if(IsPlayingAnimation == false)
        {
            CharacterController.I.SetInitPos();
            _Sequence = DOTween.Sequence();
            ReOpenStage(45f, 0.5f, 0.5f, 0f, ReOpenType.RESTART_STAGE); 
        }
    }

    /// <summary>
    /// ステージを生成する。すでにステージがある場合、閉じた後に破棄する
    /// </summary>
    public void CreateNewStage(bool existCharacter = true, float xOffset = 50f, float zOffset = -50f)
    {
        
        bool existStage = _Root != null;
        if(existStage)
        {
            _PreviousRoot = _Root;
            _PreviousBackgroundLeft = _BackgroundLeft;
            _PreviousBackgroundRight = _BackgroundRight;
        }
        _Root = new GameObject("StageRoot");
        _BackgroundLeft = new GameObject("BackgroundLeft");
        _BackgroundRight = new GameObject("BackgroundRight");        
        _XOffset = xOffset;
        _ZOffset = zOffset;
        _Root.transform.position = new Vector3(_XOffset, 0f, _ZOffset);
        _BackgroundLeft.transform.position = new Vector3(_XOffset+THICKNESS*3/2, 0f, _ZOffset);
        _BackgroundRight.transform.position = new Vector3(_XOffset, 0f, _ZOffset+THICKNESS*3/2);
        
        InstantiatePaper();
        InstantiateDecoration();
        if(existCharacter)
        {
            InstantiateCharacter();
            //HACK:キャラの向きや透過処理をさせたい
			CharacterController.I.UpdateDummyCharacterPosition(0.01f*Vector2.right);
        }

        foreach(Renderer renderer in _Root.GetComponentsInChildren<Renderer>())
        {
            if(renderer.enabled == false)
                renderer.gameObject.SetActive(false);
            renderer.enabled = false;
        }
        foreach(Renderer renderer in _BackgroundLeft.GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
        foreach(Renderer renderer in _BackgroundRight.GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        _Sequence = DOTween.Sequence();
        _PrevSequence = DOTween.Sequence();
        if(existStage)
        {
            ClosePrevStage(90f, ANIMATION_TIME);
            ReOpenStage(0f, ANIMATION_TIME, 0.001f, 0f, ReOpenType.TO_NEXT);
        }
        else
        {
            ReOpenStage(START_ANGLE, ANIMATION_TIME, 0.001f, 0f, ReOpenType.FIRST_OPEN);
        }
    }
    
    /// <summary>
    /// キャラクターを生成する
    /// </summary>
    private void InstantiateCharacter()
    {   
        CharacterController.I.color = StageManager.I.CurrentInfo.InitialCharacterColor;
        CharacterController.I.InitPosition = CharacterController.I.DummyCharacter.transform.position;
        
		CharacterController.I.CharacterX = CreateCharacter (ColorManager.GetColorWithColorData(StageManager.I.CurrentInfo.InitialCharacterColor), true);
		CharacterController.I.CharacterZ = CreateCharacter (ColorManager.GetColorWithColorData(StageManager.I.CurrentInfo.InitialCharacterColor), false);
		CharacterController.I.DestCharacterX = CreateCharacter (new Color(0,0,0,0.5f), true);
		CharacterController.I.DestCharacterZ = CreateCharacter (new Color(0,0,0,0.5f), false);
        
    }

	private GameObject CreateCharacter(Color initColor,bool xDir)
	{
		GameObject character = Instantiate(CharacterController.I.DummyCharacter,
										   CharacterController.I.DummyCharacter.transform.position + new Vector3(_XOffset - OFFSET * 2, 0f, _ZOffset - OFFSET * 2),
										   Quaternion.identity) as GameObject;
		if(xDir == false)
			character.transform.Rotate(0f, 90f, 0f);
		character.transform.SetParent(_Root.transform);
		character.tag = xDir ? X_TAG_NAME : Z_TAG_NAME;
		//TODO:色を決める
		character.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_MainColor", Color.white);
		character.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_MainColor", initColor);
		if(xDir)
			ColorManager.MultiplyShadowColor(character.transform.GetChild(1).gameObject);
		return character;
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
                    if(xOffset != 0 && ( (setX && zOffset != _ZOffset) || (!setX && zOffset - (xCoord.x-prevX) != _ZOffset) ))
                        InstantiateBackground(xCoord.x, prevX, y, prevY, xOffset, yOffset, zOffset, thickness);
                    
                    GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
                    paper.transform.SetParent(_Root.transform);
					var sprite = paper.transform.GetChild (0);
					SetTexture (sprite.GetComponent<SpriteRenderer>(), 
								StageManager.I.CurrentInfo.BackgroundTexture, 
								new Vector2(prevX/StageWidth+0.5f, prevY/StageHeight), 
								new Vector2((xCoord.x-prevX)/StageWidth, (y-prevY)/StageHeight));
					foreach(var amim in StageManager.I.CurrentInfo.Animations)
					{
						GameObject layer = Instantiate (sprite.gameObject, sprite.position, sprite.rotation) as GameObject;
						layer.transform.SetParent (paper.transform);
						SetTexture (layer.GetComponent<SpriteRenderer>(), amim, 
									new Vector2(prevX/StageWidth+0.5f, prevY/StageHeight), 
									new Vector2((xCoord.x-prevX)/StageWidth, (y-prevY)/StageHeight));
					}
					
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
        GameObject paper = Instantiate(_Paper, Vector3.zero, Quaternion.identity) as GameObject;
		SetTexture (paper.transform.GetChild(0).GetComponent<SpriteRenderer>(), 
					StageManager.I.CurrentInfo.LiningTexture, 
					new Vector2(prevX/StageWidth+0.5f, prevY/StageHeight), 
					new Vector2((x-prevX)/StageWidth, (y-prevY)/StageHeight) );

        if(xOffset - (zOffset-_ZOffset) + (x - prevX)/2 < 0)
        {
            paper.transform.SetParent(_BackgroundLeft.transform);
            paper.transform.position = new Vector3(xOffset - (zOffset-_ZOffset) + (x - prevX)/2 + _XOffset+thickness/2, (y - prevY) / 2 + yOffset, _ZOffset + thickness*3/2);
        }   
        else
        {
            paper.transform.SetParent(_BackgroundRight.transform);
            paper.transform.position = new Vector3(_XOffset + thickness*3/2, (y - prevY) / 2 + yOffset, -( xOffset - (zOffset-_ZOffset) + (x - prevX)/2 ) + _ZOffset+thickness/2);
            paper.transform.forward = Vector3.right;
        }
        paper.transform.eulerAngles += 180*Vector3.forward;
        paper.transform.localScale = new Vector3(x - prevX + thickness - 0.001f, y - prevY, thickness);
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

	private void SetTexture(SpriteRenderer target, Sprite sprite, Vector2 offset, Vector2 scale)
	{
		target.sprite = sprite==null ? _Fallback : sprite;
		target.sortingOrder = 0;
		target.material.SetFloat ("_OffsetX", offset.x);
		target.material.SetFloat ("_OffsetY", offset.y);
		target.material.SetFloat ("_TilingX", scale.x);
		target.material.SetFloat ("_TilingY", scale.y);
	}
	private void SetTexture(SpriteRenderer target, RuntimeAnimatorController anim, Vector2 offset, Vector2 scale)
	{
		Animator animator = target.gameObject.AddComponent<Animator> ();
		animator.runtimeAnimatorController = anim;
		target.sortingOrder = 0;
		target.material.SetFloat ("_OffsetX", offset.x);
		target.material.SetFloat ("_OffsetY", offset.y);
		target.material.SetFloat ("_TilingX", scale.x);
		target.material.SetFloat ("_TilingY", scale.y);
	}

    /// <summary>
    /// 表示物をセット
    /// </summary>
    private void SetDecoration(GameObject deco)
	{
        //装飾オブジェククトの表示
        Vector3 decoPos = deco.transform.position;
        Vector3 decoScale = deco.transform.lossyScale;
        Vector3 decoSetPos = new Vector3(-StageWidth / 2 - OFFSET * 2 + _XOffset, decoPos.y, _ZOffset - OFFSET * 2);

        float anchorHeightScale = 0f;
		if (deco.GetComponent<StageObjectParameter>() != null)
            anchorHeightScale = deco.GetComponent<StageObjectParameter>().HeightWithMaxWidth;

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
		if (newDeco.GetComponent<Goal> () != null) {
			CharacterController.I.GoalPos = decoSetPos;
		}
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
    private Sequence _Sequence_Step1;
    private Sequence _Sequence_Step2;
    private Sequence _Sequence_Step3;
    private Sequence _Sequence_Step4;
    
    /// <summary>
    /// ステージを閉じて開く
    /// </summary>
    public void ClosePrevStage(float angle, float closetime)
    {
        _PreviousRoot.transform.position += new Vector3(-THICKNESS, 0, THICKNESS);
        _PreviousBackgroundLeft.transform.position += new Vector3(-THICKNESS, 0, THICKNESS);
        _PreviousBackgroundRight.transform.position += new Vector3(-THICKNESS, 0, THICKNESS);
        _PreviousRoot.transform.position += new Vector3(-THICKNESS, 0, THICKNESS);
        _PrevSequence.Append( _PreviousRoot.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        _PrevSequence.Join( _PreviousBackgroundLeft.transform.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        _PrevSequence.Join( _PreviousBackgroundRight.transform.DORotate(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        
        PushCloseStage(closetime, true);
        _PrevSequence.OnComplete(() => { 
            Destroy(_PreviousRoot);
            Destroy(_PreviousBackgroundLeft);
            Destroy(_PreviousBackgroundRight);
            IsPlayingAnimation = false; 
        });
        _PrevSequence.Play();
    }
    /// <summary>
    /// ステージを閉じて開く
    /// </summary>
    public void ReOpenStage(float angle, float opentime, float closetime, float waittime, ReOpenType type)
    {
        _Sequence.OnStart(() => {
             foreach(Renderer renderer in _Root.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;
            foreach(Renderer renderer in _BackgroundLeft.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;
            foreach(Renderer renderer in _BackgroundRight.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;
        });
        _Sequence.Append( _Root.transform.DOBlendableRotateBy(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        _Sequence.Join( _BackgroundLeft.transform.DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        _Sequence.Join( _BackgroundRight.transform.DORotate(angle*Vector3.up, closetime).SetEase(CLOSE_EASE) );
        switch(type)
        {    
        case ReOpenType.FIRST_OPEN:
        case ReOpenType.RESTART_STAGE:
            _Sequence.Join( _Book.transform.GetChild(0).DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
            _Sequence.Join( _Book.transform.GetChild(1).DORotate((angle-90)*Vector3.up, closetime).SetEase(CLOSE_EASE) );
            break;
        }
        PushCloseStage(closetime);
        _Sequence.Append( _Root.transform.DOBlendableRotateBy(-angle*Vector3.up, opentime).SetEase(OPEN_EASE).SetDelay(waittime));
        _Sequence.Join( _BackgroundLeft.transform.DORotate(0*Vector3.up, opentime).SetEase(OPEN_EASE) );
        _Sequence.Join( _BackgroundRight.transform.DORotate(0*Vector3.up, opentime).SetEase(OPEN_EASE) );
        //はじめは本を開く処理もする
        switch(type)
        {    
        case ReOpenType.FIRST_OPEN:
        case ReOpenType.RESTART_STAGE:
            _Sequence.Join( _Book.transform.GetChild(0).DORotate(0*Vector3.up, opentime).SetEase(OPEN_EASE) );
            _Sequence.Join( _Book.transform.GetChild(1).DORotate(-90*Vector3.up, opentime).SetEase(OPEN_EASE) );
            break;
        }
        PushOpenStage(opentime, type);
        _Sequence.Play();
    }
    /// <summary>
    /// 凹凸を押し出しながらステージを開く
    /// </summary>
    public void PushOpenStage(float time, ReOpenType type)
    {
        IsPlayingAnimation = true;
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
            List<Transform> rootChildren = new List<Transform>(_Root.transform.childCount);
            foreach (Transform child in _Root.transform)
                rootChildren.Add(child);
            foreach (Transform tmp in rootChildren)
            {
                tmp.GetChild(0).SetParent(_Root.transform);
                Destroy(tmp.gameObject);
            }
             switch(type)
            {    
            case ReOpenType.FIRST_OPEN:
            case ReOpenType.TO_NEXT:
                InGameManager.I.DisplayDictionary();
                break;
            }
            IsPlayingAnimation = false;
        });
    }

    /// <summary>
    /// 凹凸を押し出しながらステージを閉じる
    /// </summary>
    public void PushCloseStage(float time, bool previous = false)
    {
        IsPlayingAnimation = true;
        
        Sequence targetSequence = previous ? _PrevSequence : _Sequence;
        GameObject _AnimationRoot = previous ? _PreviousRoot : _Root;
        
        List<Transform> rootChildren = new List<Transform>(_AnimationRoot.transform.childCount);
        foreach (Transform child in _AnimationRoot.transform)
            rootChildren.Add(child);
        foreach (Transform stageObj in rootChildren)
        {
            Vector3 anchorPos;
            bool dirX = stageObj.tag != "ZSideComponent";
            
            anchorPos = new Vector3(_XOffset, 0f, stageObj.position.z);
                    
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
    
    /// <summary>
    /// ステージを閉じて開く
    /// </summary>
    public void ReOpenStageForReverse(float opentime, float closetime, float waittime)
    {
        IsPlayingAnimation = true;
        //ステップ1(180度開く)
        _Sequence_Step1 = DOTween.Sequence();
        _Sequence_Step1.Append( _Root.transform.DOBlendableRotateBy(-45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
        _Sequence_Step1.Join( _BackgroundLeft.transform.DORotate(45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
        _Sequence_Step1.Join( _BackgroundRight.transform.DORotate(-45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
        _Sequence_Step1.Join( _Book.transform.GetChild(0).DORotate(45*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
        _Sequence_Step1.Join( _Book.transform.GetChild(1).DORotate(-135*Vector3.up, closetime*1/3).SetEase(CLOSE_EASE) );
        ReverseAnimationStep1(closetime*1/3);
        _Sequence_Step1.OnComplete(() => {
            _Sequence_Step2 = DOTween.Sequence();
            _Sequence_Step2.Append( transform.DOMove(transform.position, 0f) );
            _Sequence_Step2.Join( _BackgroundLeft.transform.DORotate(-45*Vector3.up, closetime*2/3).SetEase(CLOSE_EASE) );
            _Sequence_Step2.Join( _BackgroundRight.transform.DORotate(45*Vector3.up, closetime*2/3).SetEase(CLOSE_EASE) );
            _Sequence_Step2.Join( _Book.transform.GetChild(0).DORotate(-45*Vector3.up, closetime*2/3).SetEase(CLOSE_EASE) );
            _Sequence_Step2.Join( _Book.transform.GetChild(1).DORotate(-45*Vector3.up, closetime*2/3).SetEase(CLOSE_EASE) );
            ReverseAnimationStep2(closetime*2/3);
            _Sequence_Step2.OnComplete(() => {
                _Sequence_Step3 = DOTween.Sequence();
                _Sequence_Step3.Append( transform.DOMove(transform.position, opentime*2/3).SetEase(OPEN_EASE).SetDelay(waittime));
                _Sequence_Step3.Join( _BackgroundLeft.transform.DORotate(45*Vector3.up, opentime*2/3).SetEase(OPEN_EASE) );
                _Sequence_Step3.Join( _BackgroundRight.transform.DORotate(-45*Vector3.up, opentime*2/3).SetEase(OPEN_EASE) );
                _Sequence_Step3.Join( _Book.transform.GetChild(0).DORotate(45*Vector3.up, opentime*2/3).SetEase(OPEN_EASE) );
                _Sequence_Step3.Join( _Book.transform.GetChild(1).DORotate(-135*Vector3.up, opentime*2/3).SetEase(OPEN_EASE) );
                ReverseAnimationStep3(opentime*2/3);
                _Sequence_Step3.OnComplete(() => {
                    _Sequence_Step4 = DOTween.Sequence();
                    _Sequence_Step4.Append( _Root.transform.DOBlendableRotateBy(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
                    _Sequence_Step4.Join( _BackgroundLeft.transform.DORotate(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
                    _Sequence_Step4.Join( _BackgroundRight.transform.DORotate(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
                    _Sequence_Step4.Join( _Book.transform.GetChild(0).DORotate(0*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
                    _Sequence_Step4.Join( _Book.transform.GetChild(1).DORotate(-90*Vector3.up, opentime*1/3).SetEase(OPEN_EASE) );
                    ReverseAnimationStep4(opentime*1/3);
                    _Sequence_Step4.Play();
                });
                _Sequence_Step3.Play();
            });
            _Sequence_Step2.Play();
        });
        _Sequence_Step1.Play();
    }
    public void ReverseAnimationStep1(float time)
    {
        
        List<Transform> rootChildren = new List<Transform>(_Root.transform.childCount);
        foreach (Transform child in _Root.transform)
            rootChildren.Add(child);
        foreach (Transform stageObj in rootChildren)
        {
            Vector3 anchorPos;
            bool dirX = stageObj.tag != "ZSideComponent";
            
            anchorPos = new Vector3(_XOffset+THICKNESS/2, 0f, stageObj.position.z);
                    
            GameObject anchor = new GameObject("TmpAnchor");
            anchor.transform.SetParent(_Root.transform);
            anchor.transform.position = anchorPos;
            stageObj.SetParent(anchor.transform);
            if(!dirX)
            { 
                _Sequence_Step1.Join( anchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(CLOSE_EASE)
                .OnUpdate(() =>{
                    Vector3 angle = anchor.transform.GetChild(0).eulerAngles;
                    angle.y = _Root.transform.eulerAngles.y+90f;
                    anchor.transform.GetChild(0).eulerAngles = angle;
                }) );
            }
            else
            {
                _Sequence_Step1.Join( anchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(CLOSE_EASE) );
            }
            if(stageObj.GetComponent<Renderer>() != null && stageObj.GetComponent<Renderer>().material.name == "ThickPaper (Instance)")
            {
                Material mat = stageObj.GetComponent<Renderer>().material;
                _Sequence_Step1.Join( mat.DoShadowWeight(0f, time) );
            }
        }
    }
    public void ReverseAnimationStep2(float time)
    {
        List<Transform> rootChildren = new List<Transform>(_Root.transform.childCount);
        foreach (Transform child in _Root.transform)
            rootChildren.Add(child);
        foreach (Transform tmpAnchor in rootChildren)
        {            
            Transform stageObj = tmpAnchor.GetChild(0);
            Vector3 pos = stageObj.transform.position;
            float sign = Mathf.Sign( pos.x-(_XOffset-StageWidth/2) - (pos.z-_ZOffset) - StageWidth/2 );
            
            GameObject anchor = new GameObject("TmpAnchor");
            anchor.transform.position = new Vector3(_XOffset, 0f, _ZOffset);
            anchor.transform.SetParent(tmpAnchor);
            stageObj.SetParent(anchor.transform);
            if(sign < 0)
            {
                anchor.tag = X_TAG_NAME;
                _Sequence_Step2.Join( anchor.transform.DOBlendableRotateBy(-90*Vector3.up, time).SetEase(CLOSE_EASE) );
                if(stageObj.GetComponent<Renderer>() != null && stageObj.GetComponent<Renderer>().material.name == "ThickPaper (Instance)")
                {
                    Material mat = stageObj.GetComponent<Renderer>().material;
                    _Sequence_Step2.Join( mat.DOColor(mat.color*SHADOW_WEIGHT, time) );
                }
            }
            else
            {
                anchor.tag = Z_TAG_NAME;
                _Sequence_Step2.Join( anchor.transform.DOBlendableRotateBy(90*Vector3.up, time).SetEase(CLOSE_EASE) );
            }
        }
    }
    public void ReverseAnimationStep3(float time)
    {
        foreach (Transform tmpAnchor in _Root.transform)
        {         
            Transform stageObj = tmpAnchor.GetChild(0).GetChild(0);
            Transform anchor = tmpAnchor.GetChild(0);
            bool sign = anchor.tag != Z_TAG_NAME;  
            
            if(sign)
            {
                _Sequence_Step3.Join( anchor.transform.DOBlendableRotateBy(0*Vector3.up, time).SetEase(OPEN_EASE) );
                if(stageObj.GetComponent<Renderer>() != null && stageObj.GetComponent<Renderer>().material.name == "ThickPaper (Instance)")
                {
                    Material mat = stageObj.GetComponent<Renderer>().material;
                    _Sequence_Step3.Join( mat.DOColor(mat.color/SHADOW_WEIGHT, time) );
                }
            }
            else
            {                
                _Sequence_Step3.Join( anchor.transform.DOBlendableRotateBy(0*Vector3.up, time).SetEase(OPEN_EASE) );
            }            
        }
        
    }
    public void ReverseAnimationStep4(float time)
    {
        foreach (Transform tmpAnchor in _Root.transform)
        {
            Transform stageObj = tmpAnchor.GetChild(0).GetChild(0); 
            bool dirX = stageObj.tag != "ZSideComponent";
            if(dirX)
            {
                _Sequence_Step4.Join( tmpAnchor.transform.DOBlendableRotateBy(-45*Vector3.up, time).SetEase(OPEN_EASE) );
            }
            else
            { 
                _Sequence_Step4.Join( tmpAnchor.transform.DOBlendableRotateBy(-45*Vector3.up, time).SetEase(OPEN_EASE)
                .OnUpdate(() =>{
                    Vector3 angle = stageObj.eulerAngles;
                    angle.y = _Root.transform.eulerAngles.y+90f;
                    stageObj.eulerAngles = angle;
                }) );
            }
            if(stageObj.GetComponent<Renderer>() != null && stageObj.GetComponent<Renderer>().material.name == "ThickPaper (Instance)")
            {
                Material mat = stageObj.GetComponent<Renderer>().material;
                _Sequence_Step4.Join( mat.DoShadowWeight(1f, time) );
            }   
        }
        _Sequence_Step4.OnComplete(() => {
            List<Transform> tmpAnchors = new List<Transform>(_Root.transform.childCount);
            foreach (Transform tmpAnchor in _Root.transform)
                tmpAnchors.Add(tmpAnchor);
            foreach (Transform tmpAnchor in tmpAnchors)
            {
                Transform stageObj = tmpAnchor.GetChild(0).GetChild(0);
                stageObj.SetParent(_Root.transform);
                Destroy(tmpAnchor.gameObject);
            }
            IsPlayingAnimation = false;
        });
    }
    
    public void Clear()
    {
        _Sequence.Complete();
        DestroyObject( _Book );
        DestroyObject( _Root );
        DestroyObject( _PreviousRoot );
        DestroyObject( _BackgroundLeft );
        DestroyObject( _PreviousBackgroundLeft );
        DestroyObject( _BackgroundRight );
        DestroyObject( _PreviousBackgroundRight );
        
    }
    
    private void DestroyObject(GameObject obj)
    {
        if(obj != null)
            Destroy(obj);
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
