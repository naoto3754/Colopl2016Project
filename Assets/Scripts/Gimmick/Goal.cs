using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    private CardRect _Rect;
	
    void Awake()
    {
        //当たり判定作成
        _Rect = new CardRect(transform.position, transform.lossyScale.x, transform.localScale.y);
    }
    
    void FixedUpdate()
    {
        //ゴールに入ったらステージクリアフラグを立てる
        if(_Rect.Contains(CharacterController.I.DummyCharacter.transform.position))
        {
			CharacterController.I.ClearStage = true;   
		}
    }
}
