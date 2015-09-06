using UnityEngine;
using System.Collections;

public class ColorManager{
	private static readonly float SHADOW_SCALE = 0.9f;
	
	public static Color GetColorWithColorData(ColorData colordata)
	{
		//FIXME:色決めは適当
		switch(colordata)
		{
		case ColorData.NONE:
			return Color.white;
		case ColorData.RED:
			return Color.red;
		case ColorData.ORANGE:
			return new Color(1f,1f,0f);
		}
		//デフォルト色
		return Color.white;
	}
	
	public static void MultiplyShadowColor(GameObject obj)
	{
		if(obj.GetComponent<SpriteRenderer>() != null)
		{
			Color shadowColor = obj.GetComponent<SpriteRenderer>().color;
			shadowColor.r *= SHADOW_SCALE;
			shadowColor.g *= SHADOW_SCALE;
			shadowColor.b *= SHADOW_SCALE;
			obj.GetComponent<SpriteRenderer>().color = shadowColor;
			return;
		}
		if(obj.GetComponent<LineRenderer>())
		{
			return;
		}
		if(obj.GetComponent<Renderer>())
		{
			Color shadowColor = obj.GetComponent<Renderer>().material.color;
			shadowColor.r *= SHADOW_SCALE;
			shadowColor.g *= SHADOW_SCALE;
			shadowColor.b *= SHADOW_SCALE;
			obj.GetComponent<Renderer>().material.color = shadowColor;
			return;
		}
	}
}

public enum ColorData {
	NONE,
	RED,
	ORANGE,
	YELLOW,
	GREEN,
	BLUE,
	PURPLE,
	PINK
}
