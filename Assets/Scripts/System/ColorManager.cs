using UnityEngine;
using System.Collections;
using TMPro;

public class ColorManager
{
    private static readonly float SHADOW_SCALE = 0.9f;

     /// <summary>
    /// ColorDaraに対応する色を取得する
    /// </summary>
    public static Color GetColorWithColorData(ColorData colordata)
    {
        //FIXME:色決めは適当
        switch (colordata)
        {
            case ColorData.NONE:
                return Color.white;
            case ColorData.RED:
                return Color.red;
            case ColorData.ORANGE:
                return new Color(1f, 1f, 0f);
            case ColorData.GREEN:
                return new Color(0.5f, 1f, 0.5f);
            case ColorData.PINK:
                return new Color(1f, 0.5f, 0.5f);
			case ColorData.BLUE:
				return new Color(0.5f, 0.5f, 1f);
        }
        //デフォルト色
        return Color.white;
    }

    /// <summary>
    /// オブジェクトに影色を乗算する
    /// </summary>
    public static void MultiplyShadowColor(GameObject obj)
    {
        if (obj.GetComponent<SpriteRenderer>() != null)
        {
			var mat = obj.GetComponent<Renderer> ().material;
			if (mat.HasProperty ("_Color")) {
				Color shadowColor = mat.GetColor ("_Color");
				shadowColor.r *= SHADOW_SCALE;
				shadowColor.g *= SHADOW_SCALE;
				shadowColor.b *= SHADOW_SCALE;
				mat.SetColor ("_Color", shadowColor);
			} else {
				Color shadowColor = obj.GetComponent<SpriteRenderer>().color;
				shadowColor.r *= SHADOW_SCALE;
				shadowColor.g *= SHADOW_SCALE;
				shadowColor.b *= SHADOW_SCALE;
				obj.GetComponent<SpriteRenderer>().color = shadowColor;
			}
            return;
        }
        if (obj.GetComponent<LineRenderer>())
        {
            return;
        }
		if (obj.GetComponent<TextMeshPro>())
		{
			var mat = obj.GetComponent<Renderer> ().material;
			Color shadowColor = mat.GetColor ("_FaceColor");
			shadowColor.r *= SHADOW_SCALE;
			shadowColor.g *= SHADOW_SCALE;
			shadowColor.b *= SHADOW_SCALE;
			mat.SetColor ("_FaceColor", shadowColor);
			return;
		}
        if (obj.GetComponent<Renderer>())
        {
			var mat = obj.GetComponent<Renderer> ().material;
			if (mat.HasProperty ("_Color")) {
				Color shadowColor = mat.GetColor ("_Color");
				shadowColor.r *= SHADOW_SCALE;
				shadowColor.g *= SHADOW_SCALE;
				shadowColor.b *= SHADOW_SCALE;
				mat.SetColor ("_Color", shadowColor);
			} else {
				Color shadowColor = mat.color;
				shadowColor.r *= SHADOW_SCALE;
				shadowColor.g *= SHADOW_SCALE;
				shadowColor.b *= SHADOW_SCALE;
				mat.color = shadowColor;
			}
            return;
        }
    }
}

public enum ColorData
{
    NONE,
    RED,
    ORANGE,
    YELLOW,
    GREEN,
    BLUE,
    PURPLE,
    PINK
}
