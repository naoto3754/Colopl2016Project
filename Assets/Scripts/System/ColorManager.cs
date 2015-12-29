using UnityEngine;
using System.Collections;
using TMPro;

public class ColorManager
{
    private static readonly float SHADOW_SCALE = 0.9f;

    /// <summary>
    /// オブジェクトに影色を乗算する
    /// </summary>
    public static void MultiplyShadowColor(GameObject obj)
    {
        if (obj.GetComponent<SpriteRenderer>() != null)
        {
			var mat = obj.GetComponent<Renderer> ().material;
			SetShadowColor (mat, "_Color");
            return;
        }
        if (obj.GetComponent<LineRenderer>())
        {
            return;
        }
		if (obj.GetComponent<TextMeshPro>())
		{
//			var mat = obj.GetComponent<Renderer> ().material;
//			SetShadowColor (mat, "_FaceColor");
			return;
		}
        if (obj.GetComponent<Renderer>())
        {
			var mat = obj.GetComponent<Renderer> ().material;
			SetShadowColor (mat, "_Color");
            return;
        }
    }

	private static void SetShadowColor(Material mat, string key)
	{
		if (mat.HasProperty (key)) {
			Color shadowColor = mat.GetColor (key);
			shadowColor.r *= SHADOW_SCALE;
			shadowColor.g *= SHADOW_SCALE;
			shadowColor.b *= SHADOW_SCALE;
			mat.SetColor (key, shadowColor);
		}
	}
}

public enum ColorData
{
    NONE,
    COLOR1,
	COLOR2,
	COLOR3,
	COLOR4,
	COLOR5,
}
