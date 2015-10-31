using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    public CardRect Rect {
		get;
		private set;
	}
	
    void Awake()
    {
        //当たり判定作成
        Rect = new CardRect(transform.position, transform.lossyScale.x, transform.localScale.y);
    }
}
