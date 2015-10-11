using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageInfomation : MonoBehaviour
{
    //Inspector上で決定する項目
    [SerializeField]
    private GameObject _Character;
    [SerializeField]
    private Transform _StageSize;
    [SerializeField]
    private GameObject FoldLines;
    [SerializeField]
    private GameObject Holes;
    [SerializeField]
    private GameObject StageComponent;
    [SerializeField]
    private ColorData _InitialCharacterColor;
    [SerializeField]
    private Color _BackgroundColor;
    [SerializeField]
    private Texture _BackgroundTexture;
    [SerializeField]
    private Texture _LiningTexture;
    
    //Property
    public float StageWidth
    {
        get { return _StageSize.lossyScale.x; }
    }
    public float StageHeight
    {
        get { return _StageSize.lossyScale.y; }
    }
    public ColorData InitialCharacterColor
    {
        get { return _InitialCharacterColor; }
    }
    public Color BackgroundColor
    {
        get { return _BackgroundColor; }
    }
    public Texture BackgroundTexture
    {
        get { return _BackgroundTexture; }
    }
    public Texture LiningTexture
    {
        get { return _LiningTexture; }
    }
    //ステージオブジェクトリスト
    private List<Line> _FoldLine;
    public List<Line> FoldLine
    {
        get { return _FoldLine; }
    }
    private List<Line> _HoleLine;
    public List<Line> HoleLine
    {
        get { return _HoleLine; }
    }
    private List<Line> _GroundLine;
    public List<Line> GroundLine
    {
        get { return _GroundLine; }
    }
    private List<Line> _Slope;
    public List<Line> Slope
    {
        get { return _Slope; }
    }
    private List<Line> _Wall;
    public List<Line> Wall
    {
        get { return _Wall; }
    }
    private List<GameObject> _Decoration;
    public List<GameObject> Decoration
    {
        get { return _Decoration; }
    }

    void Awake()
    {
        //ステージ構成物のリストを作成
        InitList();
        //システム周りにパラメータを渡す
        InitSystemInfo();   
    }
    
    public void InitList()
    {
        _FoldLine = new List<Line>();
        _HoleLine = new List<Line>();
        _GroundLine = new List<Line>();
        _Slope = new List<Line>();
        _Wall = new List<Line>();
        _Decoration = new List<GameObject>();
        //折れ線リスト作成
        foreach (LineRenderer line in FoldLines.GetComponentsInChildren<LineRenderer>())
            _FoldLine.Add(new Line(line.transform.position, line.transform.position + line.transform.lossyScale, null));
        foreach (LineRenderer line in Holes.GetComponentsInChildren<LineRenderer>())
        {
            if (line.transform.lossyScale.x == 0f)
                _HoleLine.Add(new Line(line.transform.position, line.transform.position + line.transform.lossyScale, null));
        }
        //その他の線のリスト（地面　＿、坂　/、壁　｜）
        foreach (LineRenderer renderer in StageComponent.GetComponentsInChildren<LineRenderer>())
        {
            Vector3 linePos = renderer.transform.position;
            Vector3 lineScale = renderer.transform.lossyScale;
            
            StageObjectParameter param = null;
            if (renderer.GetComponent<StageObjectParameter>() != null)
            {
                param = renderer.GetComponent<StageObjectParameter>();
                if(renderer.GetComponent<StageObjectParameter>().UseAsDecoration)
                    _Decoration.Add(renderer.gameObject);
            }
            
            if (lineScale.y == 0f)
                _GroundLine.Add(new Line(linePos, linePos + lineScale, param));
            else if (lineScale.x == 0f)
                _Wall.Add(new Line(linePos, linePos + lineScale, param));
            else
                _Slope.Add(new Line(linePos, linePos + lineScale, param));
        }
        //ステージ表示物
        foreach (SpriteRenderer renderer in StageComponent.GetComponentsInChildren<SpriteRenderer>())
        {            
            _Decoration.Add(renderer.gameObject);
        }
    }
    /// <summary>
    /// ステージを始める上で初期化しておくべき情報をセットする
    /// </summary>
    private void InitSystemInfo()
    {
        if(StageManager.I.CurrentInfo != null)
            Destroy(StageManager.I.CurrentInfo.gameObject);
        if(_Character != null)
        {
            CharacterController.I.ClearStage = false;
            CharacterController.I.DummyCharacter = _Character;
        }
        StageManager.I.CurrentInfo = this;
        StageCreater.I.CreateNewStage(_Character != null);
    }
}
