using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    public Rectangle Rect {
		get;
		private set;
	}
	
    void Awake()
    {
        //当たり判定作成
        Rect = new Rectangle(transform.position, transform.lossyScale.x, transform.localScale.y);
    }
}
