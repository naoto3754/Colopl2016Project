using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FadeManager : Singlton<FadeManager> { 
	readonly Color INIT_COLOR = new Color(0,0,0,0);
	
	Texture2D _FadeTexture;
	bool _IsFading;
	//  Color _TexColor;
	
	public override void OnInitialize()
	{
		_FadeTexture = new Texture2D(1,1,TextureFormat.RGBA32, false);
	}
	
	void Update()
	{
	}
	
	public void FadeIn(float duration)
	{
		_IsFading = true;
		_FadeTexture.DOPixelColor(0f, duration).OnComplete(() => {
			_IsFading = false;
		});
	}
	
	public void FadeOut(float duration)
	{
		_IsFading = true;
		_FadeTexture.SetPixel(0,0,INIT_COLOR);
		_FadeTexture.Apply();
		_FadeTexture.DOPixelColor(0.7f, duration);
	}
	
	void OnGUI()
	{
		if(_IsFading)
		{
			_FadeTexture.Apply();
			GUI.DrawTexture( new Rect( 0, 0, Screen.width, Screen.height ), _FadeTexture );
		}
	}
}
