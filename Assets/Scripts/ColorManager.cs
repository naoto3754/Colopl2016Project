using UnityEngine;
using System.Collections;

public class ColorManager{
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
		return Color.white;
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
