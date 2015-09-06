using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {
	[SerializeField]
	private GameObject _LadderSprite;

	void Awake () {
		Vector3 anchorPos = transform.position - new Vector3(transform.localScale.x/2,transform.localScale.y/2,0f);
		for(int tilingX = 0; tilingX < transform.localScale.x; tilingX++)
		{
			for(int tilingY = 0; tilingY < transform.localScale.y; tilingY++)
			{
				GameObject sprite = Instantiate(_LadderSprite, anchorPos+new Vector3(tilingX,tilingY,0), Quaternion.identity) as GameObject;
				sprite.transform.SetParent(transform);
				sprite.transform.localScale = Vector3.one;
				//TODO:色決める
				sprite.GetComponent<SpriteRenderer>().color = Color.black;
			}
		}
	}
	
	void Update () {
	
	}
}
