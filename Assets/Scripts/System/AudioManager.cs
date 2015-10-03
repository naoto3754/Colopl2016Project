using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	protected static AudioManager instance;

	public static AudioManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = (AudioManager) FindObjectOfType(typeof(AudioManager));
				if(instance == null)
				{
					Debug.LogError("SoundManager Instance Error");
				}
			}

			return instance;
		}

	}

	public List<AudioContents> BGMList;
	public List<AudioContents> SEList;

	// === AudioSource ===
	// BGM
	private AudioSource BGMSource;
	// SE
	private AudioSource[] SESources = new AudioSource[16];

	void Awake (){

		// 全てのAudioSourceコンポーネントを追加する

		// BGM AudioSource
		BGMSource = gameObject.AddComponent<AudioSource>();
		// BGMはループを有効にする
		BGMSource.loop = true;

		// SE AudioSource
		for(int i = 0 ; i < SESources.Length ; i++ ){
			SESources[i] = gameObject.AddComponent<AudioSource>();
		}
	}

	// *****  BGM再生 *****
	// BGM再生
	public void PlayBGM(AudioContents.AudioTitle Title){
		foreach (AudioContents contents in BGMList) {
			if (contents.Title == Title) {
				BGMSource.Stop();
				BGMSource.clip = contents.Clip;
				BGMSource.volume = contents.Volume;
				BGMSource.Play ();
				return;
			}
		}
		Debug.LogError("BGM not found");
	}

	// BGM停止
	public void StopBGM(){
		BGMSource.Stop ();
		BGMSource.clip = null;
	}


	// *****  SE再生 *****
	public void PlaySE(AudioContents.AudioTitle Title){
		foreach (AudioContents contents in SEList) {
			if (contents.Title == Title) {
				SESources[0].Stop ();
				SESources[0].clip = contents.Clip;
				SESources[0].volume = contents.Volume;
				SESources[0].Play ();
				return;
			}
		}
		Debug.LogError("SE not found");
	}

	// SE停止
	public void StopSE(){
		// 全てのSE用のAudioSouceを停止する
		foreach(AudioSource source in SESources){
			source.Stop();
			source.clip = null;
		}  
	}
}

// 音量クラス
[Serializable]
public class AudioVolume{
	public float BGM = 1.0f;
	public float Voice = 1.0f;
	public float SE = 1.0f;
	public bool Mute = false;

	public void Init(){
		BGM = 1.0f;
		Voice = 1.0f;
		SE = 1.0f;
		Mute = false;
	}
}

[Serializable]
public class AudioContents{
	public enum AudioTitle{
		TEST
	}
	public AudioClip Clip;
	public AudioTitle Title;
	[Range(0,1)]
	public float Volume = 1.0f;

	public void Init(){
		Clip = null;
		Title = AudioTitle.TEST;
		Volume = 1.0f;
	}
}
