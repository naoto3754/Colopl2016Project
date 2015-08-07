using UnityEngine;
using System.Collections;

public class BombEffect : MonoBehaviour {

	void Start () {
		StartCoroutine(Animation());
		transform.localScale = Vector3.zero;
	}
	
	private IEnumerator Animation()
	{
		int length = 50;
		for (int i = 0; i < length; i++)
		{
			GetComponent<Renderer>().material.SetFloat("_Lerp_t", (float)i/length);
			GetComponent<Renderer>().material.SetFloat("_Remap", (float)i/length);
			GetComponent<Renderer>().material.SetFloat("_opa", 1f-0.5f*(float)i/length);
			if (i < 20)
			{
				float inc = i*0.01f;
				transform.localScale += new Vector3(inc, inc, inc);
			}else
			{
				float dec = -0.03f;
				transform.localScale -= new Vector3(dec, dec, dec);
				GetComponent<Renderer>().material.SetFloat("_color", Mathf.Max(0.8f-2f*(float)i/length, 0f));
			}
			transform.Translate(0f,0.02f,0f);
			yield return new WaitForFixedUpdate();	
		}
		for (int i = length; i < 2*length; i++)
		{
			float dec = -0.03f;
			transform.localScale -= new Vector3(dec, dec, dec);
			GetComponent<Renderer>().material.SetFloat("_Lerp_t", (float)i/length);
			GetComponent<Renderer>().material.SetFloat("_Remap", (float)i/length);
			GetComponent<Renderer>().material.SetFloat("_opa", 1f-0.5f*(float)i/length);
			transform.Translate(0f,0.02f,0f);
			yield return new WaitForFixedUpdate();
			
		}
		
		Destroy(gameObject);
	}
}
