using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class VolumeIconChanger : MonoBehaviour {

	[SerializeField]
	Sprite _Mute;
	[SerializeField]
	Sprite _VolumeOn;

	Button _Button;

	void Awake(){
		_Button = GetComponent<Button> ();
	}

	public void OnPushButton()
	{
		AudioManager.I.isMute = !AudioManager.I.isMute;

		_Button.image.sprite = AudioManager.I.isMute ? _Mute : _VolumeOn;
	}
}
