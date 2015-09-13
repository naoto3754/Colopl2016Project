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
    private GameObject StageComponent;
    [SerializeField]
    private ColorData _InitialCharacterColor;
    [SerializeField]
    private Color _BackgroundColor;
    [SerializeField]
    private GameObject _NextStage;
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
    public GameObject NextStage
    {
        get { return _NextStage; }
    }
    //ステージオブジェクトリスト
    private CardRect _Goal;
    public CardRect Goal
    {
        get { return _Goal; }
    }
    private List<Line> _FoldLine;
    public List<Line> FoldLine
    {
        get { return _FoldLine; }
    }
    private List<Line> _GroundLine;
    public List<Line> GroundLine
    {
        get { return _GroundLine; }
    }
    private List<CardRect> _Ladder;
    public List<CardRect> Ladder
    {
        get { return _Ladder; }
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
        Initialize();   
    }
    
    public void Initialize()
    {
        _FoldLine = new List<Line>();
        _GroundLine = new List<Line>();
        _Ladder = new List<CardRect>();
        _Slope = new List<Line>();
        _Wall = new List<Line>();
        _Decoration = new List<GameObject>();

        foreach (LineRenderer line in FoldLines.GetComponentsInChildren<LineRenderer>())
            _FoldLine.Add(new Line(line.transform.position, line.transform.position + line.transform.lossyScale, null));
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
        foreach (Ladder ladder in StageComponent.GetComponentsInChildren<Ladder>())
        {
            _Ladder.Add(new CardRect(ladder.transform.position,
                                     ladder.transform.lossyScale.x,
                                     ladder.transform.lossyScale.y,
                                     ladder.GetComponent<StageObjectParameter>().color));
        }
        foreach (SpriteRenderer renderer in StageComponent.GetComponentsInChildren<SpriteRenderer>())
        {
            if(renderer.gameObject.CompareTag("Goal"))
            {
                _Goal = new CardRect(renderer.transform.position, renderer.transform.lossyScale.x, renderer.transform.lossyScale.y);
            }
            _Decoration.Add(renderer.gameObject);
        }
        //ステージマネージャーに自分を渡す
        CharacterController.I.DummyCharacter = _Character;
        StageManager.I.CurrentInfo = this;
        StageCreater.I.CreateNewStage();
    }
}