using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CustomBlur : MonoBehaviour
{
	[Range(0,10)]
	public float blurSize;
	[SerializeField]
	int _BlurIteration = 2;
	[SerializeField]
	private Material _Material;
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		_Material.SetFloat ("_BlurSize", blurSize);

		RenderTexture tmp = RenderTexture.GetTemporary (source.width, source.height, 0, source.format);

		Graphics.Blit(source, tmp, _Material);
		for (int i = 0; i < _BlurIteration; i++) {
			RenderTexture tmp2 = RenderTexture.GetTemporary (source.width, source.height, 0, source.format);
			Graphics.Blit(tmp, tmp2, _Material);
			Graphics.Blit(tmp2, tmp, _Material);
			RenderTexture.ReleaseTemporary (tmp2);
		}
		Graphics.Blit(tmp, destination, _Material);

		RenderTexture.ReleaseTemporary (tmp);
	}
}