using UnityEngine;
using UE = UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	[SerializeField]
	private Button buttonPrefab;
	[SerializeField]
	private Text labelKcal;
	[SerializeField]
	private Text labelMain;
	[SerializeField]
	private Transform buttonHolder;

	private Button[] buttons;
	private int[] sceneSelection;

	private int kcal;
	private int selectedButton;

	private void OnEnable() {
		selectedButton = -1;
		kcal = 0;
		sceneSelection = null;
		buttons = new Button[ 3 ];
		for( int i = 0; i < buttons.Length; ++i ) {
			Button button = Instantiate( buttonPrefab );
			button.transform.SetParent( buttonHolder );
			buttons[ i ] = button;
			button.gameObject.SetActive( false );
		}
		for( int i = 0; i < buttons.Length; ++i ) {
			buttons[ i ].onClick.AddListener( () => {
				selectedButton = i;
			} );
		}
		StartCoroutine( Run( scene: 0 ) );
	}

	private void OnDisable() {
		StopAllCoroutines();
		for( int i = 0; i < buttons.Length; ++i ) {
			Destroy( buttons[ i ].gameObject );
		}
	}

	private IEnumerator Run( int scene ) {
		string text = "";
		int nextScene = scene + 1;
		switch( scene ) {
			case 0: {
				Set( "Awoken by the noon light, you wake up in your room. It's saturday.",
					"Get up" );
				break;
			}

			case 1: {
				Set( "It's pretty chilly. The fuzz on your arms stand on edge. You consider shaving it.",
					"Pick up clothes" );
				break;
			}

			default: {
				UE.Debug.Log( "Game over" );
				yield break;
			}
		}

		yield return StartCoroutine( WaitForButton() );
		if( sceneSelection != null ) {
		
		}
		selectedButton = -1;
		StartCoroutine( Run( nextScene ) );
	}

	private void Set( string mainText, params string[] buttons ) {
		Set( mainText, kcal: 0, buttons: buttons );
	}

	private void Set( string mainText, int kcal, params string[] buttons ) {
		StartCoroutine( UpdateText( mainText ) );
		StartCoroutine( UpdateKcal( kcal ) );
		StartCoroutine( UpdateButtons( buttons ) );
	}

	private IEnumerator UpdateButtons( params string[] args ) {
		for( int i = 0; i < buttons.Length; ++i ) {
			string arg = args.Length <= i ? "" : args[ i ];
			buttons[ i ].gameObject.SetActive( !string.IsNullOrEmpty( arg ) );
			if( !string.IsNullOrEmpty( arg ) ) {
				buttons[ i ].GetComponentInChildren<Text>().text = arg;
			}
		}
		yield break;
	}

	private IEnumerator UpdateKcal( int value ) {
		if( kcal == 0 ) {
			yield break;
		}
		kcal += value;
		labelKcal.text = value.ToString();
		yield break;
	}

	private IEnumerator UpdateText( string value ) {
		labelMain.text = value;
		yield break;
	}

	private IEnumerator WaitForButton() {
		while( selectedButton == -1 ) {
			yield return null;
		}
	}
}
