using UnityEngine;
using System.Collections;

public class DecorationObjectParameter : MonoBehaviour 
{
	[Range(-1f, 1f)]
	public float leftHeightWithMaxWidth = 0f;
	[Range(-1f, 1f)]
	public float rightHeightWithMaxWidth = 0f;
}
