using UnityEngine;
using System;
using System.Collections;

public class Music : MonoBehaviour {
	[SerializeField]
	private AnimationCurve volumeCurve;

	[NonSerialized]
	public float targetVolume;
	private AudioSource source;

	void OnEnable() {
		source = GetComponent<AudioSource>();
	}

	void Update () {
		if( source != null ) {
			targetVolume = Mathf.Clamp01( targetVolume );
			float currentVolume = source.volume;
			float newVolume = Mathf.Lerp( currentVolume, targetVolume, Time.deltaTime );
			source.volume = newVolume;
		}
	}

	public void SetVolume( float volume ) {
		targetVolume = volume;
		source.volume = volume;
	}
}
