using UnityEngine;
using System.Collections;

[RequireComponent(typeof(StageObjectParameter))]
public class Ladder : MonoBehaviour
{
    public static float MovementLimit;
    
//    [SerializeField]
//    private GameObject _LadderSprite;
    private CardRect _Rect;
    private bool _PreviousBool;

    void Awake()
    {
        //当たり判定作成
        var param = GetComponent<StageObjectParameter>();
        _Rect = new CardRect(transform.position, transform.lossyScale.x, transform.localScale.y, param.Color);
        //はしごのサイズに合わせてスプライトを生成        
//        Vector3 anchorPos = transform.position - new Vector3(0f, transform.localScale.y / 2, 0f);
//        for (int tilingY = 0; tilingY < transform.localScale.y; tilingY++)
//        {
//            GameObject sprite = Instantiate(_LadderSprite, anchorPos + new Vector3(0, tilingY, 0), Quaternion.identity) as GameObject;
//            sprite.transform.SetParent(transform);
//            sprite.transform.localScale = new Vector3(transform.localScale.x, 1f, 1f);
//            //TODO:色決める
//            sprite.GetComponent<SpriteRenderer>().color = Color.black;
//        }
    }
    
    void FixedUpdate()
    {
        if(_Rect.Contains(CharacterController.I.DummyCharacter.transform.position))
        {
            _PreviousBool = true;
            CharacterController.I.CanUseLadder = true;
            MovementLimit = _Rect.up - CharacterController.I.DummyCharacter.transform.position.y;
        }
        else
        {
            if(_PreviousBool == true)
                CharacterController.I.CanUseLadder = false;
            else
                CharacterController.I.CanUseLadder = CharacterController.I.CanUseLadder || false;
                
            _PreviousBool = false;
        }
    }
}
