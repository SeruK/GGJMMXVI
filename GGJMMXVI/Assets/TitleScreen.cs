using UnityEngine;
using UE = UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreen : MonoBehaviour {
	[SerializeField]
	private Text labelTitle;
	[SerializeField]
	private Text labelSubtitle;

	private void OnEnable() {
		Color c = labelTitle.color;
		c.a = 0.0f;
		labelTitle.color = labelSubtitle.color = c;
		StartCoroutine( DoFading() );
	}

	private IEnumerator DoFading() {
		yield return StartCoroutine( FadeInLabel( labelTitle, 3.0f ) );
		yield return StartCoroutine( FadeInLabel( labelSubtitle, 3.0f ) );
		float timer = 5.0f;
		while( !Input.GetMouseButtonUp( 0 ) ) {
			yield return null;
			timer -= Time.deltaTime;
			if( timer < 0.0f ) {
				break;
			}
		}
		StartCoroutine( FadeOutLabel( labelSubtitle, 3.0f ) );
		yield return StartCoroutine( FadeOutLabel( labelTitle, 3.0f ) );
		yield return new WaitForSeconds( 2.0f );
		Application.LoadLevel( "Rituals" );
	}

	private IEnumerator FadeInLabel( Text label, float duration ) {
		float timer = duration;
		while( timer > 0.0f ) {
			yield return null;
			timer -= Time.deltaTime;
			Color c = label.color;

			if( Input.GetMouseButtonUp( 0 ) ) {
				c.a = 1.0f;
				label.color = c;
				break;
			} else {
				c.a = 1.0f - ( timer / duration );
				label.color = c;
			}
		}
	}

	private IEnumerator FadeOutLabel( Text label, float duration ) {
		float timer = duration;
		while( timer > 0.0f ) {
			yield return null;
			timer -= Time.deltaTime;
			Color c = label.color;
			c.a = timer / duration;
			label.color = c;
		}
	}
}
