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
    private ColorData _InitialCharacterColor;
	public ColorData InitialCharacterColor
	{
		get { return _InitialCharacterColor; }
	}
    [SerializeField]
    private Color _BackgroundColor;
	public Color BackgroundColor
	{
		get { return _BackgroundColor; }
	}
    [SerializeField]
    private Sprite _BackgroundTexture;
	public Sprite BackgroundTexture
	{
		get { return _BackgroundTexture; }
	}
    [SerializeField]
    private Sprite _BackgroundNoShadowTexture;
	public Sprite BackgroundNoShadowTexture
	{
		get { return _BackgroundNoShadowTexture; }
	}
    [SerializeField]
    private Sprite _LiningTexture;
	public Sprite LiningTexture
	{
		get { return _LiningTexture; }
	}
	[SerializeField]
	private RuntimeAnimatorController[] _Animations;
	public RuntimeAnimatorController[] Animations
	{
		get { return _Animations; }
	}

    //Property
    public float StageWidth
    {
        get { return _StageSize.lossyScale.x; }
    }
    public float StageHeight
    {
        get { return _StageSize.lossyScale.y; }
    }
    
    //ステージオブジェクトリスト
	private Goal _Goal;
	public Goal GoalObj
	{
		get{ return _Goal; }
	}
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
	private List<Polygon> _Obstacle;
	public List<Polygon> Obstacle
	{
		get { return _Obstacle; }
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
		_Obstacle = new List<Polygon>();
		foreach (var param in GetComponentsInChildren<StageObjectParameter>()) {
			Vector3 pos = param.transform.position;
			Vector3 scale = param.transform.lossyScale;

			switch (param.Type) {
			case StageObjectType.LINE:
				var targetLineList = GetLineList (param.LineType);
				targetLineList.Add(new Line(pos, pos + scale, param));
				break;
			case StageObjectType.GOAL:
				_Goal = param.GetComponent<Goal>();
				break;
			case StageObjectType.RECTANGLE:
				_Obstacle.Add (new Rectangle (pos, scale.x, scale.y, param.Color));
				break;
			case StageObjectType.TRIANGLE:
				_Obstacle.Add (new Triangle(pos+new Vector3(-scale.x/2,scale.y/2,0), pos+new Vector3(scale.x/2, -scale.y/2, 0), pos+new Vector3(-scale.x/2, -scale.y/2, 0), param.Color));
				break;
			case StageObjectType.HOLE:
				foreach (LineRenderer line in param.GetComponentsInChildren<LineRenderer>()) {
					if (line.transform.lossyScale.x == 0f)
						_HoleLine.Add (new Line (line.transform.position, line.transform.position + line.transform.lossyScale, null));
				}
				break;
			case StageObjectType.LADDER:
				//Do nothing
				break;
			}

			if (param.UseAsDecoration) {
				_Decoration.Add (param.gameObject);
			}
		}
    }

	private List<Line> GetLineList(StageLineType linetype)
	{
		switch (linetype) {
		case StageLineType.FOLD:
			return _FoldLine;
		case StageLineType.GROUND:
			return _GroundLine;
		case StageLineType.SLOPE:
			return _Slope;
		case StageLineType.WALL:
			return _Wall;
		default:
			return null;
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
