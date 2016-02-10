using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	protected static AudioManager instance;

	public static AudioManager I
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

	public List<BGMConfig> BGMList;
	public List<SEConfig> SEList;

	// === AudioSource ===
	// BGM
	private AudioSource BGMSource;
	// SE
	private AudioSource[] SESources = new AudioSource[16];

	public bool isMute {
		get;
		set;
	}
		
	void Awake (){
		isMute = false;
		// BGM AudioSource
		BGMSource = gameObject.AddComponent<AudioSource>();

		// SE AudioSource
		for(int i = 0 ; i < SESources.Length ; i++ ){
			SESources[i] = gameObject.AddComponent<AudioSource>();
		}
	}

	// *****  BGM再生 *****
	// BGM再生
	public void PlayBGM(BGMConfig.Tag tag){
		if (isMute)
			return;
		foreach (BGMConfig contents in BGMList) {
			if (contents.tag == tag) {
				bool loopOnly = contents.clip == null;
				BGMSource.Stop();
				BGMSource.clip = loopOnly ? contents.clip_loop : contents.clip;
				BGMSource.loop = loopOnly;
				BGMSource.volume = contents.volume;
				BGMSource.Play ();
				if (loopOnly == false) {
					StartCoroutine (WaitForLoop (contents.clip_loop));
				}
				return;
			}
		}
		Debug.LogError("BGM not found");
	}

	private IEnumerator WaitForLoop(AudioClip clip)
	{
		while (BGMSource.isPlaying) {
			yield return new WaitForFixedUpdate ();
		}
		BGMSource.clip = clip;
		BGMSource.loop = true;
		BGMSource.Play ();
	}

	// BGM停止
	public void StopBGM(bool isFade = false){
		StopAllCoroutines ();
		float time = isFade ? 2f: 0f;
		StartCoroutine (FadeStopBGM (time));
	}

	private readonly int FRAME = 60;
	private IEnumerator FadeStopBGM(float time)
	{
		
		float delta = BGMSource.volume/FRAME;
		for(int i = 0; i < FRAME; i++)
		{
			BGMSource.volume -= delta;
			yield return new WaitForSeconds (time/FRAME);
		}
		BGMSource.Stop ();
		BGMSource.clip = null;
	}

	// *****  SE再生 *****
	public void PlaySE(SEConfig.Tag tag){
		if (isMute)
			return;
		
		foreach (SEConfig contents in SEList) {
			if (contents.tag == tag) {
				foreach (var source in SESources) {
					if (source.isPlaying)
						continue;

					source.clip = contents.clip;
					source.volume = contents.volume;
					source.Play ();
					return;
				}
				SESources[0].Stop ();
				SESources[0].clip = contents.clip;
				SESources[0].volume = contents.volume;
				SESources[0].Play ();

				return;
			}
		}
		Debug.LogError("SE not found");
	}

	public bool IsPlayingSE(SEConfig.Tag tag){
		foreach (SEConfig contents in SEList) {
			if (contents.tag == tag) {
				foreach (var source in SESources) {
					if (source.isPlaying && source.clip == contents.clip){
						return true;
					}
				}
				return false;
			}
		}
		return false;
	}

	public void StopSE(SEConfig.Tag tag){
		foreach (SEConfig contents in SEList) {
			if (contents.tag == tag) {
				foreach (var source in SESources) {
					if (source.isPlaying && source.clip == contents.clip){
						source.Stop ();
						source.clip = null;
					}
				}
			}
			return;
		}
	}

	// SE停止
	public void StopAllSE(){
		// 全てのSE用のAudioSouceを停止する
		foreach(AudioSource source in SESources){
			source.Stop();
			source.clip = null;
		}  
	}
}

[Serializable]
public class BGMConfig{
	public enum Tag{
		NONE,
		THEME,
	}
	public AudioClip clip;
	public AudioClip clip_loop;
	public Tag tag;
	[Range(0,1)]
	public float volume = 1.0f;

	public void Init(){
		clip = null;
		clip_loop = null;
		tag = Tag.NONE;
		volume = 1.0f;
	}
}

[Serializable]
public class SEConfig{
	public enum Tag{
		NONE,
		CLOSE,
		TURN_OVER,
		WALK,
		STAGE_CLEAR,
		GET,
	}
	public AudioClip clip;
	public Tag tag
	;
	[Range(0,1)]
	public float volume = 1.0f;

	public void Init(){
		clip = null;
		tag = Tag.NONE;
		volume = 1.0f;
	}
}
