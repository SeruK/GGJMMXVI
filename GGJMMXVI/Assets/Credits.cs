using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Credits : MonoBehaviour {
	[SerializeField]
	private Text labelTitle;
	[SerializeField]
	private Transform peopleRoot;

	private void OnEnable() {
		var labels = new List<Text>();
		labels.Add( labelTitle );
		foreach( Transform child in peopleRoot ) {
			labels.Add( child.GetComponent<Text>() );
		}
		foreach( Text label in labels ) {
			label.color = new Color( 1, 1, 1, 0 );
		}
		StartCoroutine( Run( labels ) );
	}

	private IEnumerator Run( List<Text> labels ) {
		yield return StartCoroutine( FadeInLabels( labels ) );
		float timer = 5.0f;
		while( !Input.GetMouseButtonUp( 0 ) ) {
			yield return null;
			timer -= Time.deltaTime;
			if( timer < 0.0f ) {
				break;
			}
		}
		foreach( Text label in labels ) {
			StartCoroutine( FadeOutLabel( label, 3.0f ) );
		}
		yield return new WaitForSeconds( 5.0f );
		Application.Quit();
	}

	private IEnumerator FadeInLabels( List<Text> labels ) {
		foreach( Text label in labels ) {
			yield return StartCoroutine( FadeInLabel( label, 2.0f ) );
			yield return StartCoroutine( WaitOrClick( 1.0f ) );
		}
	}

	private IEnumerator FadeInLabel( Text label, float duration ) {
		yield return StartCoroutine( WaitOrClick( duration, alpha => {
			Color c = label.color;
			c.a = alpha;
			label.color = c;
		} ) );
	}

	private IEnumerator FadeOutLabel( Text label, float duration ) {
		yield return StartCoroutine( WaitOrClick( duration, alpha => {
			Color c = label.color;
			c.a = 1.0f - alpha;
			label.color = c;
		} ) );
	}

	private IEnumerator WaitOrClick( float duration, Action<float> action = null ) {
		float timer = duration;
		while( timer > 0.0f ) {
			yield return null;
			timer -= Time.deltaTime;
			if( Input.GetMouseButtonUp( 0 ) ) {
				if( action != null ) {
					action( 1.0f );
				}
				break;
			} else {
				if( action != null ) {
					action( 1.0f - timer / duration );
				}
			}
		}
	}
}
